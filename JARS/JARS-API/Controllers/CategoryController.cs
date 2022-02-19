using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JARS_API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/categories")]
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
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _repository.GetCategoryByCategoryIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategory(int id, Category category)
        {
            var parentCategory = await _repository.GetCategoryByCategoryIdAsync(category.ParentCategoryId);
            var result = await _repository.GetCategoryByCategoryIdAsync(id);

            if (result == null)
                return NotFound();
            try
            {
                Category _category = new Category
                {
                    Id = id,
                    Name = category.Name == null ? result.Name : category.Name,
                    CurrentCategoryLevel = category.ParentCategoryId == null ? result.CurrentCategoryLevel : category.ParentCategoryId,
                    ParentCategoryId = parentCategory != null ? category.ParentCategoryId : id,
                };
                await _repository.UpdateCategoryAsync(_category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_repository.GetCategoryByCategoryIdAsync(category.Id) == null)
                {
                    return NotFound();
                }
                else { throw; }
            }
            return Ok(category);
        }
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            try
            {
                Category _category = new Category
                {
                    Name = category.Name,
                    CurrentCategoryLevel = category.CurrentCategoryLevel
                };
                await _repository.CreateCategoryAsync(_category);

                var categoryFound = await _repository.GetCategoryByCategoryIdAsync(category.ParentCategoryId);
                Category addParentCategoryId = new Category
                {
                    Id = _category.Id,
                    Name = _category.Name,
                    CurrentCategoryLevel = _category.CurrentCategoryLevel,
                    ParentCategoryId = categoryFound != null ? category.ParentCategoryId : _category.Id,
                };
                await _repository.UpdateCategoryAsync(addParentCategoryId);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category category = new Category
            {
                Id = id
            };
            try
            {
                await _repository.DeleteCategoryAsync(category);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(category);
        }


    }
}
