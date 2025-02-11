using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.Dto;
using RedMango_API.Services;
using RedMango_API.Utility;

namespace RedMango_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<ActionResult> GetOrders(int? userId)
        {
            try
            {
                var orderHeaders = _db.OrderHeaders.Include(x => x.OrderDetails)
                    .ThenInclude(u => u.MenuItem)
                    .OrderByDescending(x => x.OrderHeaderId);
                    
                if (userId != 0 && userId != null)
                {
                    _response.Result = orderHeaders.Where(x => x.UserId == userId);
                }
                else
                {
                    _response.Result = orderHeaders;
                }
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message };
            }
            return Ok(_response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetOrder(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var orderHeaders = _db.OrderHeaders.Include(x => x.OrderDetails)
                    .ThenInclude(u => u.MenuItem)
                    .Where(x => x.OrderHeaderId == id);
                if (orderHeaders == null)
                {
                    _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = orderHeaders;
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message };
            }
            return Ok(_response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            try
            {
                OrderHeader order = new OrderHeader()
                {
                    UserId = orderHeaderDTO.UserId,
                    PickupEmail = orderHeaderDTO.PickupEmail,
                    PickupName = orderHeaderDTO.PickupName,
                    PickupPhoneNumber = orderHeaderDTO.PickupPhoneNumber,
                    OrderTotal = orderHeaderDTO.OrderTotal,
                    OrderDate = DateTime.Now,
                    OrderID = orderHeaderDTO.OrderID,
                    TotalItems = orderHeaderDTO.TotalItems,
                    Status = String.IsNullOrEmpty(orderHeaderDTO.Status) ? SD.status_pending : orderHeaderDTO.Status
                };

                if (ModelState.IsValid)
                {
                    _db.OrderHeaders.Add(order);
                    _db.SaveChanges();
                    foreach (var orderDetailDTO in orderHeaderDTO.OrderDetailsDTO)
                    {
                        OrderDetails orderDetails = new()
                        {
                            OrderHeaderId = order.OrderHeaderId,
                            ItemName = orderDetailDTO.ItemName,
                            MenuItemId = orderDetailDTO.MenuItemId,
                            Price = orderDetailDTO.Price,
                            Quantity = orderDetailDTO.Quantity
                        };
                        _db.OrderDetails.Add(orderDetails);
                    }
                    await _db.SaveChangesAsync();
                    _response.Result = order;
                    _response.StatusCode = System.Net.HttpStatusCode.Created;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message };
            }
            return Ok(_response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrderHeader(int id, [FromBody] OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (orderHeaderUpdateDTO == null || orderHeaderUpdateDTO.OrderHeaderId == 0)
                    {
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    OrderHeader orderHeaderFromDb = await _db.OrderHeaders.FindAsync(id);
                    if (orderHeaderFromDb == null)
                    {
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    orderHeaderFromDb.PickupName = orderHeaderUpdateDTO.PickupName;
                    orderHeaderFromDb.PickupPhoneNumber = orderHeaderUpdateDTO.PickupPhoneNumber;
                    orderHeaderFromDb.PickupEmail = orderHeaderUpdateDTO.PickupEmail;
                    orderHeaderFromDb.OrderID = orderHeaderUpdateDTO.OrderID;
                    orderHeaderFromDb.Status = orderHeaderUpdateDTO.Status;
                    _db.OrderHeaders.Update(orderHeaderFromDb);
                    await _db.SaveChangesAsync();
                    _response.StatusCode = System.Net.HttpStatusCode.Created;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message };
            }
            return Ok(_response);
        }
    }
    }
