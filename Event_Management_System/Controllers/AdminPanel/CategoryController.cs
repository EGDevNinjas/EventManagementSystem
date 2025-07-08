using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace EventManagementSystem.API.Controllers.AdminPanel
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGenericRepository<Category> _repository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            IGenericRepository<Category> repository,
            IMemoryCache cache,
            ILogger<CategoryController> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var stopwatch = Stopwatch.StartNew();
            const string cacheKey = "all_categories";

            if (!_cache.TryGetValue(cacheKey, out List<Category> cachedCategories))
            {
                _logger.LogInformation("⛔ Cache is empty. Loading categories from database...");

                try
                {
                    cachedCategories = await _repository.GetAll().ToListAsync();

                    _cache.Set(cacheKey, cachedCategories, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));

                    _logger.LogInformation("✅ Categories have been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving categories: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving categories");
                }
            }
            else
            {
                _logger.LogInformation("✅ Categories retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetAllCategories took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedCategories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            string cacheKey = $"category_{id}";

            if (!_cache.TryGetValue(cacheKey, out Category cachedCategory))
            {
                _logger.LogInformation($"⛔ Cache miss for category ID {id}");

                try
                {
                    cachedCategory = await _repository.GetByIdAsync(id);

                    if (cachedCategory == null)
                    {
                        _logger.LogWarning($"⚠️ Category with ID {id} not found");
                        return NotFound();
                    }

                    _cache.Set(cacheKey, cachedCategory, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

                    _logger.LogInformation($"✅ Category ID {id} has been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving category: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving the category");
                }
            }
            else
            {
                _logger.LogInformation($"✅ Category ID {id} retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetCategoryById took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedCategory);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("⏳ Starting category creation...");

            try
            {
                if (string.IsNullOrEmpty(categoryDto.Name))
                {
                    _logger.LogWarning("⚠️ Category name is required");
                    return BadRequest("Category name is required");
                }

                var category = new Category
                {
                    Name = categoryDto.Name
                };

                await _repository.AddAsync(category);

                // Invalidate cache
                _cache.Remove("all_categories");

                _logger.LogInformation($"✅ Category {category.Name} created successfully.");

                stopwatch.Stop();
                _logger.LogInformation($"⏱ CreateCategory took {stopwatch.ElapsedMilliseconds}ms");

                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error creating category: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the category");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation($"⏳ Starting update for category ID {id}...");

            try
            {
                var existingCategory = await _repository.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    _logger.LogWarning($"⚠️ Category with ID {id} not found");
                    return NotFound();
                }

                existingCategory.Name = categoryDto.Name;
                await _repository.UpdateAsync(existingCategory);

                // Invalidate relevant cache entries
                _cache.Remove("all_categories");
                _cache.Remove($"category_{id}");

                _logger.LogInformation($"✅ Category ID {id} updated successfully.");

                stopwatch.Stop();
                _logger.LogInformation($"⏱ UpdateCategory took {stopwatch.ElapsedMilliseconds}ms");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error updating category: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the category");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation($"⏳ Starting delete for category ID {id}...");

            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning($"⚠️ Category with ID {id} not found");
                    return NotFound();
                }

                await _repository.DeleteAsync(category);

                // Invalidate relevant cache entries
                _cache.Remove("all_categories");
                _cache.Remove($"category_{id}");

                _logger.LogInformation($"✅ Category ID {id} deleted successfully.");

                stopwatch.Stop();
                _logger.LogInformation($"⏱ DeleteCategory took {stopwatch.ElapsedMilliseconds}ms");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error deleting category: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the category");
            }
        }
    }
}