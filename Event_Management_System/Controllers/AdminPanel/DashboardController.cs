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
    public class DashboardController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Event> _eventRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IGenericRepository<User> userRepository,
            IGenericRepository<Event> eventRepository,
            IGenericRepository<Payment> paymentRepository,
            IMemoryCache cache,
            ILogger<DashboardController> logger)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _paymentRepository = paymentRepository;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stopwatch = Stopwatch.StartNew();
            const string cacheKey = "admin_dashboard_stats";

            if (!_cache.TryGetValue(cacheKey, out object cachedStats))
            {
                _logger.LogInformation("⛔ Cache is empty. Loading dashboard stats from database...");

                try
                {
                    var userCount = await _userRepository.CountAsync();
                    var eventCount = await _eventRepository.CountAsync();
                    var payments = await _paymentRepository.GetAll().ToListAsync();
                    var totalRevenue = payments.Sum(p => p.Amount);

                    cachedStats = new
                    {
                        UserCount = userCount,
                        EventCount = eventCount,
                        TotalRevenue = totalRevenue,
                        PendingEmails = payments.Count(p => p.PaymentStatus == "Pending")
                    };

                    _cache.Set(cacheKey, cachedStats, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

                    _logger.LogInformation("✅ Dashboard stats have been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving dashboard stats: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving dashboard stats");
                }
            }
            else
            {
                _logger.LogInformation("✅ Dashboard stats retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetDashboardStats took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedStats);
        }
    }
}