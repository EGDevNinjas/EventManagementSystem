using AutoMapper;
using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.API.Controllers.Booking_Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IMapper _mapper;

        public PaymentController(
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<Booking> bookingRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        //  Payment Processing

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment
            ([FromBody] ProcessPaymentDTO processPaymentDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var booking = await _bookingRepository.GetByIdAsync(processPaymentDTO.BookingId);
                if (booking == null)
                    return NotFound($"Booking with ID {processPaymentDTO.BookingId} not found");

                var existingPayment = await _paymentRepository
                    .FindByCondition(p => p.BookingId == processPaymentDTO.BookingId)
                    .FirstOrDefaultAsync();

                if (existingPayment != null)
                    return BadRequest($"Payment already exists for booking {processPaymentDTO.BookingId}");

                if (processPaymentDTO.Amount != booking.TotalPrice)
                    return BadRequest($"Payment amount ({processPaymentDTO.Amount}) does not match booking total ({booking.TotalPrice})");

                var payment = new Payment
                {
                    BookingId = processPaymentDTO.BookingId,
                    PaymentMethod = processPaymentDTO.PaymentMethod,
                    PaymentStatus = "Pending", // Initial status
                    TransactionId = processPaymentDTO.TransactionId,
                    Amount = processPaymentDTO.Amount,
                    PaidAt = null, // Will be set when payment is confirmed
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepository.AddAsync(payment);

                var paymentDTO = _mapper.Map<PaymentDTO>(payment);

                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, paymentDTO);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while processing the payment");
            }
        }

        [HttpPost("confirm/{paymentId}")]
        public async Task<IActionResult> ConfirmPayment(int paymentId)
        {
            try
            {
                if (paymentId <= 0)
                    return BadRequest("Invalid payment ID");

                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                    return NotFound($"Payment with ID {paymentId} not found");

                if (payment.PaymentStatus == "Completed")
                    return BadRequest($"Payment {paymentId} is already confirmed");

                if (payment.PaymentStatus != "Pending")
                    return BadRequest($"Payment {paymentId} cannot be confirmed. Current status: {payment.PaymentStatus}");

                payment.PaymentStatus = "Completed";
                payment.PaidAt = DateTime.UtcNow;

                await _paymentRepository.UpdateAsync(payment);

                var paymentDTO = _mapper.Map<PaymentDTO>(payment);

                var confirmationResponse = new PaymentConfirmationDTO
                {
                    Message = $"Payment {paymentId} has been confirmed successfully",
                    Payment = paymentDTO,
                    ConfirmedAt = DateTime.UtcNow
                };

                return Ok(confirmationResponse);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while confirming the payment");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid payment ID");

                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null)
                    return NotFound($"Payment with ID {id} not found");

                var paymentDTO = _mapper.Map<PaymentDTO>(payment);
                return Ok(paymentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the payment");
            }
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetPaymentByBookingId(int bookingId)
        {
            try
            {
                if (bookingId <= 0)
                    return BadRequest("Invalid booking ID");

                var payment = await _paymentRepository
                    .FindByCondition(p => p.BookingId == bookingId)
                    .FirstOrDefaultAsync();

                if (payment == null)
                    return NotFound($"No payment found for booking {bookingId}");

                var paymentDTO = _mapper.Map<PaymentDTO>(payment);
                return Ok(paymentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the payment");
            }
        }



        // Payment History & Management
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetPaymentsByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return BadRequest("Payment status is required");

                var payments = await _paymentRepository
                    .FindByCondition(p => p.PaymentStatus.ToLower() == status.ToLower())
                    .ToListAsync();

                if (!payments.Any())
                    return NotFound($"No payments found with status: {status}");

                var paymentDTOs = _mapper.Map<List<PaymentDTO>>(payments);
                return Ok(paymentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving payments");
            }
        }





    }
}
