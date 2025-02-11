using Microsoft.AspNetCore.Mvc;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.Dto;
using RedMango_API.Services;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace RedMango_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICloudinaryService _cloudinaryService;
        private ApiResponse _response;
        public MenuItemController(ApplicationDbContext db, ICloudinaryService cloudinaryService)
        {
            _db = db;
            _cloudinaryService = cloudinaryService;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            _response.Result = _db.MenuItems;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetMenuItems")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            MenuItem _menuItem = _db.MenuItems.FirstOrDefault(x => x.Id == id);
            if (_menuItem == null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = _menuItem;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
                    {
                        return BadRequest();
                    }
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(menuItemCreateDTO.File.FileName);
                    var uploadResult = await _cloudinaryService.UploadImageAsync(menuItemCreateDTO.File, "Images");

                    MenuItem menuItemToCreate = new MenuItem
                    {
                        Name = menuItemCreateDTO.Name,
                        Description = menuItemCreateDTO.Description,
                        SpecialTag = menuItemCreateDTO.SpecialTag,
                        Category = menuItemCreateDTO.Category,
                        Price = menuItemCreateDTO.Price,
                        Image = uploadResult.SecureUrl.ToString()
                    };
                    _db.MenuItems.Add(menuItemToCreate);
                    await _db.SaveChangesAsync();
                    _response.Result = menuItemToCreate;
                    _response.StatusCode = System.Net.HttpStatusCode.Created;
                    return CreatedAtRoute("GetMenuItems", new { id = menuItemToCreate.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.ToString());
                return BadRequest(_response);
            }
        }

    [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemUpdateDTO == null || menuItemUpdateDTO.Id == 0)
                    {
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    MenuItem menuItemFromDb = await _db.MenuItems.FindAsync(id);
                    if (menuItemFromDb == null)
                    {
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    menuItemFromDb.Name = menuItemUpdateDTO.Name;
                    menuItemFromDb.Description = menuItemUpdateDTO.Description;
                    menuItemFromDb.SpecialTag = menuItemUpdateDTO.SpecialTag;
                    menuItemFromDb.Category = menuItemUpdateDTO.Category;
                    menuItemFromDb.Price = menuItemUpdateDTO.Price;

                    string publicId = _cloudinaryService.GetPublicId(menuItemFromDb.Image);
                    if (menuItemUpdateDTO.File != null || menuItemUpdateDTO.File.Length > 0)
                    {
                        var uploadResult = await _cloudinaryService.UploadImageAsync(menuItemUpdateDTO.File, "Images");
                        await _cloudinaryService.DeleteImageAsync(publicId);
                        menuItemFromDb.Image = uploadResult.SecureUrl.ToString();
                    }

                    _db.MenuItems.Update(menuItemFromDb);
                    await _db.SaveChangesAsync();
                    _response.StatusCode = System.Net.HttpStatusCode.Created;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.ToString());
                return BadRequest(_response);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMenuItem(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                MenuItem menuItemFromDb = await _db.MenuItems.FindAsync(id);
                if (menuItemFromDb == null)
                {
                    return BadRequest();
                }
                string publicId = _cloudinaryService.GetPublicId(menuItemFromDb.Image);
                await _cloudinaryService.DeleteImageAsync(publicId);
                _db.MenuItems.Remove(menuItemFromDb);
                await _db.SaveChangesAsync();
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.ToString());
                return BadRequest(_response);
            }
        }
    }
}
