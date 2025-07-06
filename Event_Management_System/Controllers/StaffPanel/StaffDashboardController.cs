using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace EventManagementSystem.API.Controllers.StaffPanel
{
    [Route("api/staff/[controller]")]
    [ApiController]
    public class StaffDashboardController : ControllerBase
    {
        private readonly IGenericRepository<StaffAssignment> _assignmentRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<StaffDashboardController> _logger;

        public StaffDashboardController(
            IGenericRepository<StaffAssignment> assignmentRepository,
            IGenericRepository<Booking> bookingRepository,
            IMemoryCache cache,
            ILogger<StaffDashboardController> logger)
        {
            _assignmentRepository = assignmentRepository;
            _bookingRepository = bookingRepository;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var stopwatch = Stopwatch.StartNew();
            const string cacheKey = "staff_dashboard_data";

            if (!_cache.TryGetValue(cacheKey, out object cachedData))
            {
                _logger.LogInformation("⛔ Cache is empty. Loading staff dashboard data from database...");

                try
                {
                    // Example data - adjust based on your actual requirements
                    var assignments = await _assignmentRepository.GetAll().ToListAsync();
                    var checkIns = await _bookingRepository.FindByCondition(b => b.IsCheckedIn).ToListAsync();

                    cachedData = new
                    {
                        AssignedEvents = assignments.Count,
                        TodayCheckIns = checkIns.Count(c => c.CheckInTime?.Date == DateTime.Today),
                        UpcomingEvents = assignments.Select(a => a.Event).Distinct().Count()
                    };

                    _cache.Set(cacheKey, cachedData, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

                    _logger.LogInformation("✅ Staff dashboard data has been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving staff dashboard data: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving dashboard data");
                }
            }
            else
            {
                _logger.LogInformation("✅ Staff dashboard data retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetDashboardData took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedData);
        }
    }
}