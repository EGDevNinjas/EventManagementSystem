using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventManagementSystem.API.Controllers.Client_Side
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IGenericRepository<EventRating> _ratingRepository;

        public RatingController(IGenericRepository<EventRating> ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        // POST: api/Rating
        /// <summary>
        /// Submits or updates a rating for an event
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SubmitRating([FromBody] RatingInputDto input)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRating = await _ratingRepository
                .FindByCondition(r => r.UserId == userId && r.EventId == input.EventId)
                .FirstOrDefaultAsync();

            if (existingRating != null)
            {
                existingRating.Rating = input.Rating;
                existingRating.Comment = input.Comment;
                await _ratingRepository.UpdateAsync(existingRating);
            }
            else
            {
                var newRating = new EventRating
                {
                    UserId = userId.Value,
                    EventId = input.EventId,
                    Rating = input.Rating,
                    Comment = input.Comment,
                    CreatedAt = DateTime.UtcNow
                };
                await _ratingRepository.AddAsync(newRating);
            }

            return Ok(new { message = "Rating submitted successfully" });
        }

        // GET: api/Rating/event/{eventId}/summary
        /// <summary>
        /// Gets a summary of ratings for an event (average and count)
        /// </summary>
        [HttpGet("event/{eventId}/summary")]
        public async Task<IActionResult> GetRatingSummary(int eventId)
        {
            var ratings = await _ratingRepository
                .FindByCondition(r => r.EventId == eventId)
                .ToListAsync();

            if (ratings.Any())
            {
                var average = ratings.Average(r => r.Rating);
                var count = ratings.Count;
                return Ok(new RatingSummaryDto { AverageRating = average, RatingCount = count });
            }
            else
            {
                return Ok(new RatingSummaryDto { AverageRating = 0, RatingCount = 0 });
            }
        }

        // GET: api/Rating/event/{eventId}
        /// <summary>
        /// Gets a paginated list of ratings for an event
        /// </summary>
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetRatings(int eventId, int pageNumber = 1, int pageSize = 10)
        {
            var ratings = await _ratingRepository
                .FindByCondition(r => r.EventId == eventId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RatingDto
                {
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(ratings);
        }

        // GET: api/Rating/user/event/{eventId}
        /// <summary>
        /// Gets the current user's rating for a specific event
        /// </summary>
        [HttpGet("user/event/{eventId}")]
        public async Task<IActionResult> GetUserRating(int eventId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var rating = await _ratingRepository
                .FindByCondition(r => r.UserId == userId && r.EventId == eventId)
                .Select(r => new RatingDto
                {
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (rating != null)
            {
                return Ok(rating);
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE: api/Rating/user/event/{eventId}
        /// <summary>
        /// Deletes the current user's rating for a specific event
        /// </summary>
        [HttpDelete("user/event/{eventId}")]
        public async Task<IActionResult> DeleteUserRating(int eventId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var rating = await _ratingRepository
                .FindByCondition(r => r.UserId == userId && r.EventId == eventId)
                .FirstOrDefaultAsync();

            if (rating != null)
            {
                await _ratingRepository.DeleteAsync(rating);
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}