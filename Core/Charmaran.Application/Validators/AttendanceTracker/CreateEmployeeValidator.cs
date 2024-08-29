using System.Threading;
using System.Threading.Tasks;
using Charmaran.Domain.Entities.AttendanceTracker;
using Charmaran.Persistence.Contracts.AttendanceTracker;
using Charmaran.Shared.Extensions;
using FluentValidation;

namespace Charmaran.Application.Validators.AttendanceTracker
{
    public class CreateEmployeeValidator : AbstractValidator<Employee>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public CreateEmployeeValidator(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;

            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull().WithMessage("{PropertyName} must not be null")
                .MaximumLength(100).WithMessage("{PropertyName} must be 100 characters or less")
                .Must((name) =>
                {
                    return name.HasFirstAndLastName() && name.ContainsLettersOnly();
                }).WithMessage("{PropertyName} must have a first and last name and only contain letters");

            RuleFor(e => e)
                .MustAsync(NameIsUnique).WithMessage("Name must be unique");
        }

        private async Task<bool> NameIsUnique(Employee employee, CancellationToken cancellationtoken)
        {
            return await this._employeeRepository.IsEmployeeNameUnique(employee.Name);
        }
    }
}