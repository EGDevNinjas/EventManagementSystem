using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace EventManagementSystem.API.Controllers.StaffPanel
{
    [Route("api/staff/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IGenericRepository<Staff> _staffRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CheckInController> _logger;

        public CheckInController(
            IGenericRepository<Booking> bookingRepository,
            IGenericRepository<Staff> staffRepository,
            IMemoryCache cache,
            ILogger<CheckInController> logger)
        {
            _bookingRepository = bookingRepository;
            _staffRepository = staffRepository;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetCheckInStatus(int bookingId)
        {
            var stopwatch = Stopwatch.StartNew();
            string cacheKey = $"checkin_status_{bookingId}";

            if (!_cache.TryGetValue(cacheKey, out Booking cachedBooking))
            {
                _logger.LogInformation($"⛔ Cache miss for booking ID {bookingId}");

                try
                {
                    cachedBooking = await _bookingRepository.GetByIdAsync(bookingId);

                    if (cachedBooking == null)
                    {
                        _logger.LogWarning($"⚠️ Booking with ID {bookingId} not found");
                        return NotFound();
                    }

                    _cache.Set(cacheKey, cachedBooking, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(2)));

                    _logger.LogInformation($"✅ Booking ID {bookingId} has been cached.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error retrieving booking: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving the booking");
                }
            }
            else
            {
                _logger.LogInformation($"✅ Booking ID {bookingId} retrieved from cache.");
            }

            stopwatch.Stop();
            _logger.LogInformation($"⏱ GetCheckInStatus took {stopwatch.ElapsedMilliseconds}ms");

            return Ok(new
            {
                IsCheckedIn = cachedBooking.IsCheckedIn,
                CheckInTime = cachedBooking.CheckInTime
            });
        }

        [HttpPost]
        public async Task<IActionResult> CheckInAttendee([FromBody] CheckInDTO checkInDto)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("⏳ Starting attendee check-in...");

            try
            {
                var booking = await _bookingRepository.GetByIdAsync(checkInDto.BookingId);
                if (booking == null)
                {
                    _logger.LogWarning($"⚠️ Booking with ID {checkInDto.BookingId} not found");
                    return NotFound();
                }

                var staff = await _staffRepository.GetByIdAsync(checkInDto.StaffId);
                if (staff == null)
                {
                    _logger.LogWarning($"⚠️ Staff with ID {checkInDto.StaffId} not found");
                    return NotFound("Staff member not found");
                }

                booking.IsCheckedIn = true;
                booking.CheckInTime = DateTime.Now;
                booking.CheckedInByStaffId = checkInDto.StaffId;

                await _bookingRepository.UpdateAsync(booking);

                // Invalidate relevant cache entries
                _cache.Remove($"checkin_status_{checkInDto.BookingId}");

                _logger.LogInformation($"✅ Attendee with booking ID {checkInDto.BookingId} checked in successfully.");

                stopwatch.Stop();
                _logger.LogInformation($"⏱ CheckInAttendee took {stopwatch.ElapsedMilliseconds}ms");

                return Ok(new
                {
                    Message = "Check-in successful",
                    BookingId = booking.Id,
                    CheckInTime = booking.CheckInTime
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error during check-in: {ex.Message}");
                return StatusCode(500, "An error occurred during check-in");
            }
        }
    }
}