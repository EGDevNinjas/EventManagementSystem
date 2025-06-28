using AutoMapper;
using EventManagementSystem.API.DTOs;
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
        IMapper _mapper;
        public BookingController(IGenericRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

                // Only generate QR code if it doesn't exist
                if (string.IsNullOrEmpty(booking.QRCode))
                {
                    var qrCode = GenerateQRCode(booking.Id);
                    booking.QRCode = qrCode;
                    await _repository.UpdateAsync(booking);
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




    }
}
