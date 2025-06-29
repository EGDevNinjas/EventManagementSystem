using AutoMapper;
using EventManagementSystem.BLL.Services;
using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace EventManagementSystem.API.Controllers.Boking_Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        // Book / View bookings

        IGenericRepository<Booking> _repository;
        IGenericRepository<Ticket> _ticketrepository;
        IMapper _mapper;
        public BookingController(IGenericRepository<Booking> repository, IMapper mapper, IGenericRepository<Ticket> ticketrepository)
        {
            _repository = repository;
            _mapper = mapper;
            _ticketrepository = ticketrepository;

        }


        //  Retrieves a specific booking by its ID (includes QR code
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid booking ID");

                var booking = await _repository.GetByIdAsync(id);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {id} not found");
                }

                var bookingDTO = _mapper.Map<BookingDTO>(booking);

                return Ok(bookingDTO);
            }
            catch (Exception ex)
            {
                // logger logic
                return StatusCode(500, "Internal server error");
            }
        }


        // Retrieves all bookings for a specific user
        [HttpGet("/user/{userId}")]
        public async Task<IActionResult> GetAllByUserIdAsync(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID");

            var bookingsQuery = _repository.FindByCondition(p => p.UserId == userId);
            if (bookingsQuery == null) return NotFound("No bookings for this user");
            try
            {
                var bookings = await bookingsQuery.ToListAsync();
                if (!bookings.Any())
                    return NotFound("No bookings found for this user");

                var bookingsDTO = _mapper.Map<List<BookingDTO>>(bookings);
                return Ok(bookingsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        private string GenerateQRCode(int bookingId)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(bookingId.ToString(), QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);
                    return Convert.ToBase64String(qrCodeBytes);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO createBookingDTO)
        {
            try
            {
                // Check From the user's presence
                int userId = JWTReader.GetUserId(User);

                var ticket = await _ticketrepository.GetByIdAsync(createBookingDTO.TicketId);
                if (ticket == null)
                    return NotFound($"Ticket with ID {createBookingDTO.TicketId} not found.");

                // ✅ تحقق من التوفر لو فيه عدد معين للتذاكر
                //if (createBookingDTO.Quantity > ticket.RemainingQuantity)
                //    return BadRequest("Not enough tickets available.");

                // Create new booking
                var booking = new Booking
                {
                    UserId = 1, // مؤقتا لحد ما نضيف Login & Register
                    TicketId = createBookingDTO.TicketId,
                    Quantity = createBookingDTO.Quantity,
                    TotalPrice = createBookingDTO.TotalPrice,
                    QRCode = "", // Will be generated below
                    IsCheckedIn = false,
                    CheckInTime = null,
                    CheckedInByStaffId = null,
                    CreatedAt = DateTime.Now
                };

                // Save booking to get the ID
                await _repository.AddAsync(booking);

                // Generate QR code
                var qrCode = GenerateQRCode(booking.Id);
                if (string.IsNullOrEmpty(qrCode))
                    return StatusCode(500, "QR code generation failed");
                booking.QRCode = qrCode;

                // Update booking with QR code
                await _repository.UpdateAsync(booking);

                // Map to DTO for response
                var bookingDTO = _mapper.Map<BookingDTO>(booking);

                return CreatedAtAction(nameof(GetById), new { id = booking.Id }, bookingDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _repository.GetByIdAsync(id);
            if (booking == null) return BadRequest($"no Booking with id : {id}");

            await _repository.DeleteAsync(booking);
            return NoContent();
        }


    }
}
