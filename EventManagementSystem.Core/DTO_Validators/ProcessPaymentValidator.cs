using FluentValidation;
using EventManagementSystem.Core.DTOs;

namespace EventManagementSystem.API.DTO_Validators
{
    public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentDTO>
    {
        public ProcessPaymentValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("Booking ID must be greater than 0");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Payment method is required")
                .MaximumLength(50)
                .WithMessage("Payment method cannot exceed 50 characters");

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage("Transaction ID is required")
                .MaximumLength(100)
                .WithMessage("Transaction ID cannot exceed 100 characters");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0");
        }
    }
}