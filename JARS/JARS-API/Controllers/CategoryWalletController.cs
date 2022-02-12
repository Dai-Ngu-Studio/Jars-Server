using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JARS_API.Dtos;
namespace JARS_API.Controllers
{
    [Route("api/category-wallet")]
    [ApiController]
    public class CategoryWalletController : ControllerBase
    {
        private readonly ICategoryWalletReposiotry repository;
        public CategoryWalletController(ICategoryWalletReposiotry repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        public IEnumerable<CategoryWalletDto> GetCategoryWallets()
        {
            var wallets = repository.GetAllCategoryWallets().Select(categorywallet => categorywallet.AsCateWalletDto());
            return wallets;
        }
        [HttpGet("{id}")]
        public ActionResult<CategoryWalletDto> GetCategoryWallet(int id)
        {
            var categoryWallet = repository.GetCategoryWallet(id);
            if (categoryWallet is null)
            {
                return NotFound();
            }
            return categoryWallet.AsCateWalletDto();
        }
        //POST /wallets/
        [HttpPost]
        public ActionResult<CategoryWallet> AddCategoryWallet(CUCategoryWalletDto cUCategoryWalletDto)
        {

            try
            {
                CategoryWallet categoryWallet = new CategoryWallet
                {
                    Name = cUCategoryWalletDto.Name,
                    WalletId = cUCategoryWalletDto.WalletId,
                    CurrentCategoryLevel = cUCategoryWalletDto.CurrentCategoryLevel,
                    ParentCategoryId = cUCategoryWalletDto.ParentCategoryId,


                };
                repository.AddCategoryWallet(categoryWallet);
                return CreatedAtAction(nameof(GetCategoryWallet), new { id = categoryWallet.Id }, categoryWallet.AsCateWalletDto());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        //PUT /wallets/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCategoryWallet(int id, CUCategoryWalletDto cUCategoryWalletDto)
        {
            CategoryWallet existedCategoryWallet = repository.GetCategoryWallet(id);
            if (existedCategoryWallet is null)
            {
                return NotFound();
            }
            else
            {
                CategoryWallet updateCategoryWallet = new CategoryWallet
                {
                    Id = id,
                    Name = cUCategoryWalletDto.Name,
                    WalletId = cUCategoryWalletDto.WalletId,
                    CurrentCategoryLevel = cUCategoryWalletDto.CurrentCategoryLevel,
                    ParentCategoryId = cUCategoryWalletDto.ParentCategoryId,
                };
                repository.UpdateCategoryWallet(updateCategoryWallet);
                return NoContent();
            }

        }
        //Delete /wallets/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCategoryWallet(int id)
        {
            repository.DeleteCategoryWallet(id);
            return NoContent();
        }

    }
}
