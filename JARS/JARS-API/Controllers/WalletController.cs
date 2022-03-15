using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JARS_API.Controllers
{
    [Route("api/v1/wallets")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletReposiotry repository;
        public WalletController(IWalletReposiotry repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        [Authorize]
        /// <summary>
        /// This method will return list of wallets upon accountId
        /// </summary>
        public async Task<IEnumerable<Wallet>> GetWallets()
        {
            ClaimsPrincipal httpUser = HttpContext.User as ClaimsPrincipal;           
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid != null)
            {
                if (await repository.GetAllWallets(uid) != null)
                {
                    return await repository.GetAllWallets(uid);
                }
            }
            return null;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Wallet>> GetWallet(int id)
        {
            return await repository.GetWallet(id);
        }
        //POST /wallets/
        [HttpPost]
        public async Task AddWallet(Wallet wallet)
        {
            Wallet _wallet = new Wallet
            {
                AccountId = wallet.AccountId,
                Name = wallet.Name,
                StartDate = wallet.StartDate,
                WalletAmount = wallet.WalletAmount,
                Percentage = wallet.Percentage,
            };
                  await repository.AddWallet(_wallet);
        }

        //PUT /wallets/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWallet(int id,Wallet wallet)
        {

            if (id != wallet.Id)
            {
                return BadRequest();
            }
            try
            {
                await repository.UpdateWallet(wallet);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (repository.GetWallet(wallet.Id) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(wallet);

        }
        //Delete /wallets/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWallet(int id)
        {
           repository.DeleteWallet(id);
            return Ok();

        }

    }
}
