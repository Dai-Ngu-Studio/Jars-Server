using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JARS_API.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JARS_API.Controllers
{
    [Route("api/v1/category-wallets")]
    [ApiController]
    public class CategoryWalletController : ControllerBase
    {
        private readonly ICategoryWalletReposiotry repository;
        public CategoryWalletController(ICategoryWalletReposiotry repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        public async Task<IEnumerable<CategoryWallet>> GetCategoryWallets()
        {
            return await repository.GetAllCategoryWallets();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryWallet>> GetCategoryWallet(int id)
        {
            return await repository.GetCategoryWallet(id);
        }
        //POST /wallets/
        [HttpPost]
        public async Task AddCategoryWallet(CategoryWallet categoryWallet)
        {
            CategoryWallet _categoryWallet = new CategoryWallet
            {
                Name = categoryWallet.Name,
                CurrentCategoryLevel = categoryWallet.CurrentCategoryLevel,
                ParentCategoryId = categoryWallet.ParentCategoryId,
                WalletId = categoryWallet.WalletId,
            };
            await repository.AddCategoryWallet(categoryWallet);
        }

        //PUT /wallets/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategoryWallet(int id, CategoryWallet categoryWallet)
        {
            if (id != categoryWallet.Id)
            {
                return BadRequest();
            }
            try
            {
                await repository.UpdateCategoryWallet(categoryWallet);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (repository.GetCategoryWallet(categoryWallet.Id) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(categoryWallet);
        }
        //Delete /wallets/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryWallet(int id)
        {
            await repository.DeleteCategoryWallet(id);
            return Ok();

        }


    }
}
