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
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IMapper _mapper;

        public PaymentController(
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<Booking> bookingRepository,
            IMapper mapper,
            IGenericRepository<User> userRepository)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // 1) Payment Processing

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


        // 2) Payment History & Management
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

        // Get all payments for a user
        [HttpGet("user/{userId}/payments")]
        public async Task<IActionResult> GetAllPaymentsforUser(int userId)
        {
            try
            {
                var userExists = await _userRepository
                    .FindByCondition(b => b.Id == userId)
                    .AnyAsync();

                if (!userExists)
                    return NotFound($"No bookings found for user with ID {userId}");

                var payments = await _paymentRepository
                    .FindByCondition(p => p.Booking.UserId == userId)
                    .Include(p => p.Booking)
                    .ToListAsync();

                var paymentDTOs = _mapper.Map<List<PaymentDTO>>(payments);
                return Ok(paymentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving payments for the user");
            }
        }

        // Get all payments for an organizer's events
        [HttpGet("organizer/{organizerId}/")]
        public async Task<IActionResult> GetPaymentsByOrganizerId(int organizerId)
        {
            try
            {
                var organizerExists = await _userRepository
                    .FindByCondition
                    (u => u.Id == organizerId &&
                    u.UserRoles.Any(ur => ur.Role.Name == "Organizer"))
                    .AnyAsync(); // check if organizer exists && is an organizer ?

                if (!organizerExists)
                    return NotFound($"Organizer with ID {organizerId} does not exist.");

                var payments = await _paymentRepository
                    .FindByCondition(p => p.Booking.UserId == organizerId)
                    .Include(p => p.Booking)
                    .ToListAsync();
                if (payments == null || payments.Count == 0)
                    return NotFound($"No payments found for organizer with ID {organizerId}");
                var paymentDTOs = _mapper.Map<List<PaymentDTO>>(payments);
                return Ok(paymentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving payments for the organizer");
            }
        }

        // 3) Refund Management

        // Refund a payment
        [HttpPost("{paymentId}/refund")]
        public async Task<IActionResult> ProcessRefund(int paymentId)
        {
            if (paymentId <= 0)
                return BadRequest("Invalid payment ID.");

            try
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment is null)
                    return NotFound($"No payment found with ID {paymentId}.");
                if (payment.PaymentStatus == "Refunded")
                    return BadRequest($"Payment {paymentId} is already refunded.");

                if (!string.Equals(payment.PaymentStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                    return BadRequest($"Payment {paymentId} cannot be refunded. Current status: '{payment.PaymentStatus}'.");

                // TODO: Integrate actual payment provider refund logic here

                payment.PaymentStatus = "Refunded";
                await _paymentRepository.UpdateAsync(payment);

                var paymentDTO = _mapper.Map<PaymentDTO>(payment);

                return Ok(new
                {
                    Message = $"Payment {paymentId} has been refunded successfully.",
                    Payment = paymentDTO
                });
            }
            catch (Exception ex)
            {
                // TODO: Inject and use a logger to log the exception
                // _logger.LogError(ex, "Error while processing refund for payment ID {PaymentId}", paymentId);
                return StatusCode
                    (500, "An internal error occurred while processing the refund.");
            }
        }

        // Get refund history for a payment
        [HttpGet("{paymentId}/refund-history")]
        public async Task<IActionResult> GetRefundHistory(int paymentId)
        {
            try
            {
                if (paymentId <= 0)
                    return BadRequest("Invalid payment ID.");
                var payment = 
                    await _paymentRepository.GetByIdAsync(paymentId);

                if (payment == null)
                    return NotFound($"Payment with ID {paymentId} not found.");

                var refundHistory = new
                {
                    PaymentId = payment.Id,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionId = payment.TransactionId,
                    Status = payment.PaymentStatus,
                    PaidAt = payment.PaidAt?.ToString("o") ?? "Not paid yet",
                    CreatedAt = payment.CreatedAt.ToString("o")
                };
                return Ok(refundHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the refund history");
            }
        }


        // 4) Payment Status Updates
        
        [HttpPut("{paymentId}/status")]
        public async Task<IActionResult> UpdatePayment(int paymentId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                return BadRequest("New status is required.");

            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment is null)
                return NotFound($"there is no payments with id {paymentId}");

            var allowedStatuses = new[] { "Pending", "Completed", "Failed", "Refunded" };
            if (!allowedStatuses.Contains(newStatus, StringComparer.OrdinalIgnoreCase))
                return BadRequest("Invalid status value.");

            if (string.Equals(payment.PaymentStatus,
                newStatus, StringComparison.OrdinalIgnoreCase))
                return BadRequest($"Payment is already in '{payment.PaymentStatus}' status.");

            payment.PaymentStatus = newStatus;

            try
            {
                await _paymentRepository.UpdateAsync(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "internal Server Error");
            }
            var paymentDTO = _mapper.Map<PaymentDTO>(payment);
            return Ok(paymentDTO);
        }

        
        [HttpPost("{paymentId}/cancel")]
        public async Task<IActionResult> CancelPayment(int paymentId)
        {
            if (paymentId <= 0)
                return BadRequest("Invalid payment ID.");

            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment is null)
                return NotFound($"No payment found with ID {paymentId}.");

            if (!string.Equals(payment.PaymentStatus, "Pending", StringComparison.OrdinalIgnoreCase))
                return BadRequest($"Only pending payments can be canceled. Current status: {payment.PaymentStatus}.");

            payment.PaymentStatus = "Canceled"; 

            try
            {
                await _paymentRepository.UpdateAsync(payment);
                var paymentDTO = _mapper.Map<PaymentDTO>(payment);
                return Ok(new { Message = $"Payment {paymentId} has been canceled successfully.", Payment = paymentDTO });
            }
            catch (Exception ex)
            {
                // TODO: log error
                return StatusCode(500, "An error occurred while canceling the payment.");
            }
        }






    }
}
