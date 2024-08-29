using System;
using System.Threading;
using System.Threading.Tasks;
using Charmaran.Domain.Entities.AttendanceTracker;
using Charmaran.Persistence.Contracts.AttendanceTracker;
using FluentValidation;

namespace Charmaran.Application.Validators.AttendanceTracker
{
    public class CreateAttendanceEntryValidator : AbstractValidator<AttendanceEntry>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public CreateAttendanceEntryValidator(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;

            RuleFor(a => a.EmployeeId)
                .MustAsync(EmployeeExists).WithMessage("Employee must exist");

            RuleFor(a => a.Amount)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0")
                .LessThanOrEqualTo(2).WithMessage("{PropertyName} must be less than or equal to 2");

            RuleFor(a => a.Category)
                .IsInEnum().WithMessage("{PropertyName} must exist");

            RuleFor(a => a.InputDate)
                .Must(date => date != default(DateTime)).WithMessage("Must have a input date")
                .Must(DateIsNotFuture).WithMessage("{PropertyName} must not be a future date");

            RuleFor(a => a.Notes)
                .Must(notes => (notes?.Length ?? 0) < 501).WithMessage("{PropertyName} may not contain more than 500 characters");
        }

        private async Task<bool> EmployeeExists(int employeeId, CancellationToken cancellationtoken)
        {
            return await this._employeeRepository.EmployeeExists(employeeId);
        }

        private bool DateIsNotFuture(DateTime date)
        {
            return date < DateTime.Now;
        }
    }
}