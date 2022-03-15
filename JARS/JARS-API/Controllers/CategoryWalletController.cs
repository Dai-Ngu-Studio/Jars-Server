using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace JARS_API.Controllers
{
    [Route("api/v1/category-wallets")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult> AddCategoryWallet(CategoryWallet categoryWallet)
        {   
            if (categoryWallet.ParentCategoryId.Value == 0 || categoryWallet.ParentCategoryId is null)
            {               
            }
            else {
                CategoryWallet parentCate = await repository.GetCategoryWallet(categoryWallet.ParentCategoryId.Value);
                if (parentCate == null)
                {
                    return BadRequest("Id of parentCategory that you have just inputed does not exist");
                }
            }

            CategoryWallet _categoryWallet = new CategoryWallet
            {
                ParentCategoryId = categoryWallet.ParentCategoryId,
                Name = categoryWallet.Name,
                CurrentCategoryLevel = categoryWallet.CurrentCategoryLevel,
            };
            await repository.AddCategoryWallet(_categoryWallet);
            return Ok(200);
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
