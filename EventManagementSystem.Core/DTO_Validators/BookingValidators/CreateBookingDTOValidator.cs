using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManagementSystem.Core.DTOs;
using FluentValidation;

namespace EventManagementSystem.Core.DTO_Validators.BookingValidators
{
    public class CreateBookingDTOValidator:AbstractValidator<CreateBookingDTO>
    {
        public CreateBookingDTOValidator()
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

        }
    }
}
