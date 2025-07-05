using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        var stopwatch = Stopwatch.StartNew();  // ⏱️ start stopwatch to measure execution time

        string cacheKey = "all_events";

        if (!_cache.TryGetValue(cacheKey, out List<Event> cachedEvents))
        {
            _logger.LogInformation("⛔ Cache is empty. Loading events from the database...");

            try
            {
                var events = await _genericRepository.GetAll().ToListAsync();

                cachedEvents = events;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

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

        stopwatch.Stop(); // ⏹️ stop the stopwatch and log the execution time

            _logger.LogInformation($"⏱️ GetAllEvents endpoint took {stopwatch.ElapsedMilliseconds} ms to execute.");

        return Ok(cachedEvents);
    }

    [HttpGet("GetEventById/{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var eventItem = await _genericRepository.GetByIdAsync(id);
                if (eventItem == null)
                {
                    return NotFound("Event not found");
                }
                return Ok(eventItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the event: " + ex.Message);
            }
        }
        //example: /api/EventBrowse/SearchEvents/Tech Conference
        //example: /api/EventBrowse/SearchEvents/2023-10-01
        // Search events by title or description 
        // but what is the searchTerm?
        // It can be a keyword, a date, or any string that matches the title or description of the event.
        [HttpGet("SearchEvents/{searchTerm}")]
        public async Task<IActionResult> SearchEvents(string searchTerm)
        {
            try
            {
                var events = await _genericRepository
                    .FindByCondition(e => e.Title.Contains(searchTerm) || e.Description.Contains(searchTerm))
                    .ToListAsync();

                if (!events.Any())
                {
                    return NotFound("No events found matching the search term");
                }

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while searching for events: " + ex.Message);
            }
        }
        

        [HttpGet("FilterEventsByCategory/{categoryId}")]
        public async Task<IActionResult> FilterEventsByCategory(int categoryId)
        {
            try
            {
                var events = await _genericRepository
                    .FindByCondition(e => e.CategoryId == categoryId)
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while filtering events: " + ex.Message);
            }
        
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
        {
            var events = _genericRepository
                .GetPaged(e => string.IsNullOrEmpty(keyword) || e.Title.Contains(keyword), page, pageSize);

            return Ok(await events.ToListAsync());
        }


    }
}
