using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JARS_API.Dtos;
namespace JARS_API.Controllers
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletReposiotry repository;
        public WalletController(IWalletReposiotry repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        public IEnumerable<WalletDto> Getwallets()
        {
            var wallets = repository.GetAllWallets().Select(wallet =>wallet.AsDto());
            return wallets;
        }
        [HttpGet("{id}")]
        public ActionResult<WalletDto> Getwallet(int id)
        {
            var wallet = repository.GetWallet(id);
            if (wallet is null)
            {
                return NotFound();
            }
            return wallet.AsDto();
        }
        //POST /wallets/
        [HttpPost]
        public ActionResult<Wallet> AddWallet(CreateWalletDto createWallet)
        {
            
            try
            {   Wallet wallet = new Wallet
                {
                  Name = createWallet.Name,   
                  Percentage = createWallet.Percentage,
                  StartDate = createWallet.StartDate,
                  WalletAmount = createWallet.WalletAmount,  
                  AccountId = createWallet.AccountId,
                  
               };
                repository.AddWallet(wallet);
                return CreatedAtAction(nameof(Getwallet), new { id = wallet.Id }, wallet.AsDto());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
           
        }

        //PUT /wallets/{id}
        [HttpPut("{id}")]
        public ActionResult Updatewallet(int id,CreateWalletDto createWalletDto)
        {
            Wallet existedWallet = repository.GetWallet(id);
            if(existedWallet is null)
            {
                return NotFound();
            }
            else
            {
                Wallet updateWallet = new Wallet
                {   Id = id,
                    Name = createWalletDto.Name,
                    Percentage = createWalletDto.Percentage,
                    StartDate = createWalletDto.StartDate,
                    WalletAmount = createWalletDto.WalletAmount,
                    AccountId = createWalletDto.AccountId,
                };
                repository.UpdateWallet(updateWallet);
                return NoContent();
            }
            
        }
        //Delete /wallets/{id}
        [HttpDelete]
        public ActionResult Deletewallet(int id)
        {
           repository.DeleteWallet(id);
            return NoContent();

        }

    }
}
