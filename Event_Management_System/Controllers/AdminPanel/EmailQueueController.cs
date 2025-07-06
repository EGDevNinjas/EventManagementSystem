using EventManagementSystem.Core.DTO_Validators;
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
    public class EmailQueueController : ControllerBase
    {
        private readonly IGenericRepository<EmailQueue> _repository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<EmailQueueController> _logger;

        public EmailQueueController(
            IGenericRepository<EmailQueue> repository,
            IMemoryCache cache,
            ILogger<EmailQueueController> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmailQueues()
        {
            var stopwatch = Stopwatch.StartNew();
            const string cacheKey = "all_email_queues";

            if (!_cache.TryGetValue(cacheKey, out List<EmailQueue> cachedQueues))
            {
                _logger.LogInformation("⛔ Cache is empty. Loading email queues from database...");

                try
                {
                    cachedQueues = await _repository.GetAll().ToListAsync();

                    _cache.Set(cacheKey, cachedQueues, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));

                    _logger.LogInformation("✅ Email queues have been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving email queues: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving email queues");
                }
            }
            else
            {
                _logger.LogInformation("✅ Email queues retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetAllEmailQueues took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedQueues);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmailQueueById(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            string cacheKey = $"email_queue_{id}";

            if (!_cache.TryGetValue(cacheKey, out EmailQueue cachedQueue))
            {
                _logger.LogInformation($"⛔ Cache miss for email queue ID {id}");

                try
                {
                    cachedQueue = await _repository.GetByIdAsync(id);

                    if (cachedQueue == null)
                    {
                        _logger.LogWarning($"⚠️ Email queue with ID {id} not found");
                        return NotFound();
                    }

                    _cache.Set(cacheKey, cachedQueue, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(2)));

                    _logger.LogInformation($"✅ Email queue ID {id} has been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving email queue: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving the email queue");
                }
            }
            else
            {
                _logger.LogInformation($"✅ Email queue ID {id} retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetEmailQueueById took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(cachedQueue);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmailQueue([FromBody] EmailQueueDTO emailQueueDto)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("⏳ Starting email queue creation...");

            try
            {
                var validator = new EmailQueueValidator();
                var validationResult = await validator.ValidateAsync(emailQueueDto);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("⚠️ Validation errors: {@Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var emailQueue = new EmailQueue
                {
                    ToEmail = emailQueueDto.ToEmail,
                    Subject = emailQueueDto.Subject,
                    Body = emailQueueDto.Body,
                    IsSent = emailQueueDto.IsSent,
                    CreatedAt = DateTime.Now
                };

                await _repository.AddAsync(emailQueue);

                // Invalidate cache
                _cache.Remove("all_email_queues");

                _logger.LogInformation($"✅ Email queue to {emailQueue.ToEmail} created successfully.");

                stopwatch.Stop();
                _logger.LogInformation($"⏱ CreateEmailQueue took {stopwatch.ElapsedMilliseconds}ms");

                return CreatedAtAction(nameof(GetEmailQueueById), new { id = emailQueue.Id }, emailQueue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error creating email queue: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the email queue");
            }
        }


        [HttpPut("{id}/send")]
        public async Task<IActionResult> MarkEmailAsSent(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation($"⏳ Starting mark as sent for email queue ID {id}...");

            try
            {
                var emailQueue = await _repository.GetByIdAsync(id);
                if (emailQueue == null)
                {
                    _logger.LogWarning($"⚠️ Email queue with ID {id} not found");
                    return NotFound();
                }

                emailQueue.IsSent = true;
                emailQueue.SentAt = DateTime.Now;
                await _repository.UpdateAsync(emailQueue);

                // Invalidate relevant cache entries
                _cache.Remove("all_email_queues");
                _cache.Remove($"email_queue_{id}");

                _logger.LogInformation($"✅ Email queue ID {id} marked as sent.");

                stopwatch.Stop();
                _logger.LogInformation($"⏱ MarkEmailAsSent took {stopwatch.ElapsedMilliseconds}ms");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error marking email as sent: {ex.Message}");
                return StatusCode(500, "An error occurred while marking email as sent");
            }
        }
    }
}