using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;

namespace RedMango_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly ApplicationDbContext _db;
        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetShoppingCart(int userId)
        {
            try
            {
                if (userId == 0)
                {
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string> { "User ID cannot be null or empty." };
                    return BadRequest(_response);
                }

                ShoppingCart shoppingCart = await _db.ShoppingCarts
                    .Include(x => x.CartItems)
                    .ThenInclude(u => u.MenuItem)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (shoppingCart == null)
                {
                    _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string> { "Shopping cart not found." };
                    return NotFound(_response);
                }

                if (shoppingCart.CartItems == null || !shoppingCart.CartItems.Any())
                {
                    shoppingCart.CartTotal = 0;
                }
                else
                {
                    shoppingCart.CartTotal = shoppingCart.CartItems.Sum(x => x.Quantity * x.MenuItem.Price);
                }

                _response.Result = shoppingCart;
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddOrUpdateItemInCart(int userId, int menuItemId, int updateQuantityBy)
        {
            MenuItem menuItem = await _db.MenuItems.FirstOrDefaultAsync(x => x.Id == menuItemId);
            if (menuItem == null)
            {
                return NotFound(new { Message = "MenuItem not found" });
            }

            ShoppingCart shoppingCart = _db.ShoppingCarts.Include(x => x.CartItems).FirstOrDefault(x => x.UserId == userId);
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { UserId = userId, CartItems = new List<CartItem>() };
                _db.ShoppingCarts.Add(shoppingCart);
                await _db.SaveChangesAsync();
            }

            CartItem cartItem = shoppingCart.CartItems.FirstOrDefault(x => x.MenuItemId == menuItemId);
            if (cartItem == null)
            {
                if (updateQuantityBy > 0)
                {
                    cartItem = new CartItem
                    {
                        MenuItemId = menuItemId,
                        Quantity = updateQuantityBy,
                        ShoppingCartId = shoppingCart.Id,
                    };
                    shoppingCart.CartItems.Add(cartItem);
                }
            }
            else
            {
                int newQuantity = cartItem.Quantity + updateQuantityBy;

                if (newQuantity <= 0)
                {
                    _db.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = newQuantity;
                }
            }

            if (!shoppingCart.CartItems.Any())
            {
                _db.ShoppingCarts.Remove(shoppingCart);
            }

            await _db.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.Result = shoppingCart;
            return Ok(_response);
        }

    }
}
