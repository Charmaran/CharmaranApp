using System;
using Charmaran.Domain.Entities.AttendanceTracker;
using FluentValidation;

namespace Charmaran.Application.Validators.AttendanceTracker
{
    public class CreateHolidayValidator : AbstractValidator<Holiday>
    {
        public CreateHolidayValidator()
        {

            RuleFor(h => h.Date)
                .Must(date => date != default(DateTime)).WithMessage("{PropertyName} is required");

            RuleFor(h => h.Name)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .MaximumLength(100).WithMessage("{PropertyName} must be 100 characters or less");
        }
    }
}