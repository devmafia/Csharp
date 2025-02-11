using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedMango_API.Utility;

namespace RedMango_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> GetSomething()
        {
            return "You are authorized to see this";
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = SD.Role_Admin)]
        public ActionResult<string> GetSomething(int someIntValue)
        {
            return "You are authorized with Role of Admin";
        }
    }
}
