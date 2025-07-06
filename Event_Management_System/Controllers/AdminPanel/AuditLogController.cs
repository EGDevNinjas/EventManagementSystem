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
    public class AuditLogController : ControllerBase
    {
        private readonly IGenericRepository<AuditLog> _repository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(
            IGenericRepository<AuditLog> repository,
            IMemoryCache cache,
            ILogger<AuditLogController> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuditLogs()
        {
            var stopwatch = Stopwatch.StartNew();
            const string cacheKey = "all_audit_logs";

            if (!_cache.TryGetValue(cacheKey, out List<AuditLog> cachedLogs))
            {
                _logger.LogInformation("⛔ Cache is empty. Loading audit logs from database...");

                try
                {
                    cachedLogs = await _repository.GetAll().ToListAsync();

                    _cache.Set(cacheKey, cachedLogs, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

                    _logger.LogInformation("✅ Audit logs have been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving audit logs: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving audit logs");
                }
            }
            else
            {
                _logger.LogInformation("✅ Audit logs retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetAllAuditLogs took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedLogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditLogById(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            string cacheKey = $"audit_log_{id}";

            if (!_cache.TryGetValue(cacheKey, out AuditLog cachedLog))
            {
                _logger.LogInformation($"⛔ Cache miss for audit log ID {id}");

                try
                {
                    cachedLog = await _repository.GetByIdAsync(id);

                    if (cachedLog == null)
                    {
                        _logger.LogWarning($"⚠️ Audit log with ID {id} not found");
                        return NotFound();
                    }

                    _cache.Set(cacheKey, cachedLog, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(2)));

                    _logger.LogInformation($"✅ Audit log ID {id} has been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving audit log: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving the audit log");
                }
            }
            else
            {
                _logger.LogInformation($"✅ Audit log ID {id} retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetAuditLogById took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedLog);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAuditLogsByUser(int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            string cacheKey = $"audit_logs_user_{userId}";

            if (!_cache.TryGetValue(cacheKey, out List<AuditLog> cachedLogs))
            {
                _logger.LogInformation($"⛔ Cache miss for user audit logs ID {userId}");

                try
                {
                    cachedLogs = await _repository.FindByCondition(a => a.UserId == userId).ToListAsync();

                    _cache.Set(cacheKey, cachedLogs, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));

                    _logger.LogInformation($"✅ User {userId} audit logs have been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving user audit logs: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving user audit logs");
                }
            }
            else
            {
                _logger.LogInformation($"✅ User {userId} audit logs retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetAuditLogsByUser took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedLogs);
        }

        [HttpGet("action/{action}")]
        public async Task<IActionResult> GetAuditLogsByAction(string action)
        {
            var stopwatch = Stopwatch.StartNew();
            string cacheKey = $"audit_logs_action_{action}";

            if (!_cache.TryGetValue(cacheKey, out List<AuditLog> cachedLogs))
            {
                _logger.LogInformation($"⛔ Cache miss for action audit logs: {action}");

                try
                {
                    cachedLogs = await _repository.FindByCondition(a => a.Action != null && a.Action.Contains(action)).ToListAsync();

                    if (cachedLogs == null || !cachedLogs.Any())
                    {
                        _logger.LogWarning($"⚠️ No audit logs found for action: {action}");
                        return NotFound($"No audit logs found for action: {action}");
                    }

                    _cache.Set(cacheKey, cachedLogs, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));

                    _logger.LogInformation($"✅ Action {action} audit logs have been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving action audit logs: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving action audit logs");
                }
            }
            else
            {
                _logger.LogInformation($"✅ Action {action} audit logs retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetAuditLogsByAction took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedLogs);
        }


        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetAuditLogsByDate(string date)
        {
            var stopwatch = Stopwatch.StartNew();
            string cacheKey = $"audit_logs_date_{date}";

            if (!_cache.TryGetValue(cacheKey, out List<AuditLog> cachedLogs))
            {
                _logger.LogInformation($"⛔ Cache miss for date audit logs: {date}");

                try
                {
                    if (!DateTime.TryParse(date, out var targetDate))
                    {
                        _logger.LogWarning($"⚠️ Invalid date format: {date}");
                        return BadRequest("Invalid date format. Use YYYY-MM-DD");
                    }

                    var nextDay = targetDate.AddDays(1);
                    cachedLogs = await _repository.FindByCondition(a =>
                        a.Timestamp >= targetDate && a.Timestamp < nextDay).ToListAsync();

                    _cache.Set(cacheKey, cachedLogs, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));

                    _logger.LogInformation($"✅ Date {date} audit logs have been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving date audit logs: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving date audit logs");
                }
            }
            else
            {
                _logger.LogInformation($"✅ Date {date} audit logs retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetAuditLogsByDate took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedLogs);
        }
    }
}