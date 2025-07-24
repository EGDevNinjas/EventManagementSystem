using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.API.Controllers.Client_Side
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDashboardController : ControllerBase
    {
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IGenericRepository<SavedEvent> _savedEventRepository;
        private readonly IGenericRepository<Notification> _notificationRepository;

        public ClientDashboardController(
            IGenericRepository<Booking> bookingRepository,
            IGenericRepository<SavedEvent> savedEventRepository,
            IGenericRepository<Notification> notificationRepository)
        {
            _bookingRepository = bookingRepository;
            _savedEventRepository = savedEventRepository;
            _notificationRepository = notificationRepository;
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

        [HttpGet("attendance")]
        public async Task<IActionResult> GetAttendance(int pageNumber = 1, int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var now = DateTime.UtcNow;

            var attendance = await _bookingRepository
                .FindByCondition(b => b.UserId == userId && b.IsCheckedIn && b.Ticket.Event.Date < now)
                .Include(b => b.Ticket)
                .ThenInclude(t => t.Event)
                .OrderByDescending(b => b.Ticket.Event.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new AttendanceDto
                {
                    BookingId = b.Id,
                    EventTitle = b.Ticket.Event.Title,
                    EventDate = b.Ticket.Event.Date,
                    CheckInTime = b.CheckInTime.Value
                })
                .ToListAsync();

            return Ok(attendance);
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings(int pageNumber = 1, int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var now = DateTime.UtcNow;

            var bookings = await _bookingRepository
                .FindByCondition(b => b.UserId == userId && b.Ticket.Event.Date >= now)
                .Include(b => b.Ticket)
                .ThenInclude(t => t.Event)
                .OrderBy(b => b.Ticket.Event.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new UpcomingBookingDto
                {
                    BookingId = b.Id,
                    EventTitle = b.Ticket.Event.Title,
                    EventDate = b.Ticket.Event.Date,
                    Quantity = b.Quantity,
                    TotalPrice = b.TotalPrice
                })
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpGet("saved-events")]
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
                    Location = se.Event.Location
                })
                .ToListAsync();

            return Ok(savedEvents);
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications(int pageNumber = 1, int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var notifications = await _notificationRepository
                .FindByCondition(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }
    }

    
}