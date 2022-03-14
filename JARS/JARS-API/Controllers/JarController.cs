using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [Route("api/v1/sixJars")]
    public class JarController : Controller
    {
        private readonly IWalletReposiotry repository;
        public JarController(IWalletReposiotry repository)
        {
            this.repository = repository;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateSixJars([FromQuery] decimal totalAmount)
        {   
                ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;
                string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (uid != null)
                {
                    if (await repository.countWallets(uid) == 0)
                    {
                        if (await repository.GetAllWallets(uid) != null)
                        {
                            await repository.Add6DefaultJars(uid, totalAmount);
                        }
                        return Ok("Add success 6 Jars");
                    }
                    else return BadRequest("This account already have more than 1 wallet, cannot create 6 default jars");
                   
                }
                return BadRequest("Authorize problem or wrong input");
            

        }
    }
}
