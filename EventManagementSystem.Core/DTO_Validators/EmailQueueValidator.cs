using EventManagementSystem.Core.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.DTO_Validators
{
    public class EmailQueueValidator : AbstractValidator<EmailQueueDTO>
    {
        public EmailQueueValidator()
        {
            RuleFor(x => x.ToEmail)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required")
                .MaximumLength(200).WithMessage("Subject cannot exceed 200 characters");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body is required");

            RuleFor(x => x.IsSent)
                .NotNull().WithMessage("IsSent must have a value");
        }
    }
}
