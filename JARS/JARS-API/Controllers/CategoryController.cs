using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Mvc;

namespace JARS_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        public CategoryController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetAllCategories()
        {
            var result = await _repository.GetCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryl(int id)
        {
            var category = await _repository.GetCategoryByCategoryIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
    }
}
