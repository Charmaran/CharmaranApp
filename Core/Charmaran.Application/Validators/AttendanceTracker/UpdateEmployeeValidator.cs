using System.Threading;
using System.Threading.Tasks;
using Charmaran.Domain.Entities.AttendanceTracker;
using Charmaran.Persistence.Contracts.AttendanceTracker;
using FluentValidation;

namespace Charmaran.Application.Validators.AttendanceTracker
{
    public class UpdateEmployeeValidator : AbstractValidator<Employee>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public UpdateEmployeeValidator(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;

            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull().WithMessage("{PropertyName} must not be null")
                .MaximumLength(100).WithMessage("{PropertyName} must be 100 characters or less");

            RuleFor(e => e)
                .MustAsync(NameIsUnique).WithMessage("Name must be unique");
        }

        private async Task<bool> NameIsUnique(Employee employee, CancellationToken cancellationtoken)
        {
            return await this._employeeRepository.IsEmployeeNameUnique(employee.Name);
        }
    }
}