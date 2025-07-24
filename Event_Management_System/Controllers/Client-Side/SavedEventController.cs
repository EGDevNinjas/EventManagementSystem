using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventManagementSystem.API.Controllers.Client_Side
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SavedEventController : ControllerBase
    {
        private readonly IGenericRepository<SavedEvent> _savedEventRepository;
        private readonly IGenericRepository<Event> _eventRepository;

        public SavedEventController(
            IGenericRepository<SavedEvent> savedEventRepository,
            IGenericRepository<Event> eventRepository)
        {
            _savedEventRepository = savedEventRepository;
            _eventRepository = eventRepository;
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

        // POST: api/SavedEvent
        /// <summary>
        /// Saves an event for the current user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveEvent([FromBody] SaveEventInputDto input)
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

            // Check if event exists
            var eventExists = await _eventRepository.FindByCondition(e => e.Id == input.EventId).AnyAsync();
            if (!eventExists)
            {
                return NotFound("Event not found");
            }

            // Check if already saved
            var alreadySaved = await _savedEventRepository
                .FindByCondition(se => se.UserId == userId && se.EventId == input.EventId)
                .AnyAsync();

            if (alreadySaved)
            {
                return Conflict("Event already saved");
            }

            var newSavedEvent = new SavedEvent
            {
                UserId = userId.Value,
                EventId = input.EventId,
                CreatedAt = DateTime.UtcNow
            };

            await _savedEventRepository.AddAsync(newSavedEvent);

            return Ok(new { message = "Event saved successfully" });
        }

        // DELETE: api/SavedEvent/{eventId}
        /// <summary>
        /// Deletes a saved event for the current user
        /// </summary>
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteSavedEvent(int eventId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var savedEvent = await _savedEventRepository
                .FindByCondition(se => se.UserId == userId && se.EventId == eventId)
                .FirstOrDefaultAsync();

            if (savedEvent == null)
            {
                return NotFound("Saved event not found");
            }

            await _savedEventRepository.DeleteAsync(savedEvent);

            return NoContent();
        }

        // GET: api/SavedEvent
        /// <summary>
        /// Gets a paginated list of saved events for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSavedEvents(int pageNumber = 1, int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var savedEvents = await _savedEventRepository
                .FindByCondition(se => se.UserId == userId)
                .Include(se => se.Event)
                .OrderBy(se => se.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(se => new SavedEventDto
                {
                    EventId = se.Event.Id,
                    Title = se.Event.Title,
                    Date = se.Event.Date,
                    Location = se.Event.Location,
                    SavedAt = se.CreatedAt
                })
                .ToListAsync();

            return Ok(savedEvents);
        }
    }
}