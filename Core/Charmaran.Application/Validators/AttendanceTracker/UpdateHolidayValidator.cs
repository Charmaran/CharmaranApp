using System;
using Charmaran.Domain.Entities.AttendanceTracker;
using FluentValidation;

namespace Charmaran.Application.Validators.AttendanceTracker
{
    public class UpdateHolidayValidator : AbstractValidator<Holiday>
    {
        public UpdateHolidayValidator()
        {
            RuleFor(p => p.Date)
                .Must(date => date != default(DateTime)).WithMessage("{PropertyName} is required");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} is required");
        }
    }
}