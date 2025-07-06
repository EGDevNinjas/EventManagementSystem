using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using FluentValidation;

namespace EventManagementSystem.Core.DTO_Validators
{
    public class RegisterUserValidator: AbstractValidator<RegisterUserDto>
    {

        public RegisterUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("first name cannot be empty")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters")
                .MinimumLength(2).WithMessage("Name cannot be less than 2 characters");
                
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("last name cannot be empty")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters")
                .MinimumLength(2).WithMessage("Name cannot be less than 2 characters");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email Cannot be Empty")
                .EmailAddress().WithMessage("Invalid Email Format");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is Required")
                .Matches(@"^01[0-2,5]{1}[0-9]{8}$").WithMessage("Invalid Egyptian phone number");
            
        }

    }
}
