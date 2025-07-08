using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace EventManagementSystem.API.Controllers.Client_Side
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventBrowseController : ControllerBase
    {
        private readonly IGenericRepository<Event> _genericRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<EventBrowseController> _logger;

        public EventBrowseController(
            IGenericRepository<Event> genericRepository,
            IMemoryCache cache,
            ILogger<EventBrowseController> logger)
        {
            _genericRepository = genericRepository;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("GetAllEvents")]

        public async Task<IActionResult> GetAllEvents()
        {
            var stopwatch = Stopwatch.StartNew();  // ⏱ start stopwatch to measure execution time

            string cacheKey = "all_events";

            if (!_cache.TryGetValue(cacheKey, out List<Event> cachedEvents))
            {
                _logger.LogInformation("⛔ Cache is empty. Loading events from the database...");

                try
                {
                    var events = await _genericRepository.GetAll().ToListAsync();

                    cachedEvents = events;

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                    _cache.Set(cacheKey, cachedEvents, cacheEntryOptions);

                    _logger.LogInformation("✅ Events have been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error while retrieving events: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving the events: " + ex.Message);
                }
            }
            else
            {
                _logger.LogInformation("✅ Events retrieved from cache.");
            }

            stopwatch.Stop(); // ⏹ stop the stopwatch and log the execution time

            _logger.LogInformation($"⏱ GetAllEvents endpoint took {stopwatch.ElapsedMilliseconds} ms to execute.");

            return Ok(cachedEvents);
        }
    }
}