using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<string> Get()
        {
            ClaimsPrincipal user = HttpContext.User as ClaimsPrincipal;
            string? value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"Authorized user with UID: {value}");
            return value;
        }
    }
}
