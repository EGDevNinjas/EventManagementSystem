using EventManagementSystem.Core.DTOs;
using FluentValidation;

namespace EventManagementSystem.Core.DTO_Validators.BookingValidators
{
    public class BookingValidator : AbstractValidator<BookingDTO>
    {
        public BookingValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User ID must be greater than 0");

            RuleFor(x => x.TicketId)
                .GreaterThan(0)
                .WithMessage("Ticket ID must be greater than 0");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Quantity must be between 1 and 100");

            RuleFor(x => x.TotalPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total price cannot be negative");

            RuleFor(x => x.QRCode)
                .MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.QRCode))
                .WithMessage("QR Code cannot exceed 200 characters");

            RuleFor(x => x.CheckedInByStaffId)
                .GreaterThan(0)
                .When(x => x.CheckedInByStaffId.HasValue)
                .WithMessage("Staff ID must be greater than 0 when provided");

            RuleFor(x => x.CheckInTime)
                .GreaterThanOrEqualTo(x => x.CreatedAt)
                .When(x => x.CheckInTime.HasValue)
                .WithMessage("Check-in time cannot be before booking creation time");

            RuleFor(x => x.CreatedAt)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("Created date cannot be in the future");
        }
    
    }


}
