using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Enums;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using Charmaran.UI.Components.Modals;
using Charmaran.UI.Contracts;
using Charmaran.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Charmaran.UI.Pages
{
    public partial class AttendanceTracker
    {
        [Inject] private IEmployeeService EmployeeService { get; set; } = null!;

        [Inject] private IAttendanceEntryService AttendanceEntryService { get; set; } = null!;

        [CascadingParameter] public IModalService Modal { get; set; } = null!;

        [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

        [Inject] private IToastService ToastService { get; set; } = null!;

        //---------------------------------------------------------------------------------------------------
        //Properties 
        //---------------------------------------------------------------------------------------------------
        private int EmployeeIdModel
        {
            get { return this.SelectedEmployeeId; }
            set
            {
                this.SelectedEmployeeId = value;
                _ = OnSelectedEmployeeChange(this.SelectedEmployeeId);
            }
        }

        private int SelectedEmployeeId { get; set; }

        private int YearModel
        {
            get { return this.SelectedYear; }
            set
            {
                this.SelectedYear = value;
                OnSelectedYearChange();
            }
        }

        private int SelectedYear { get; set; }

        private string? MonthModel
        {
            get { return this.SelectedMonth; }
            set
            {
                this.SelectedMonth = value;
                OnSelectedMonthChange();
            }
        }

        private string? SelectedMonth { get; set; }
        private EmployeeDetailed? SelectedEmployee { get; set; }
        private List<EmployeeDto>? Employees { get; set; }
        private List<int[][]> Grids { get; set; } = new List<int[][]>();
        private int[] Years { get; set; } = null!;

        private string[] Months { get; } = new string[]
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        //---------------------------------------------------------------------------------------------------
        //Methods
        //---------------------------------------------------------------------------------------------------
        protected override async Task OnInitializedAsync()
        {
            this.SelectedYear = DateTime.Now.Year;
            this.SelectedMonth = Months[DateTime.Now.Month - 1];

            SetupYears();
            this.Employees = await this.EmployeeService.GetEmployees(false);
            this.Grids.Add(CreateCalenderGrid());
        }

        private async Task ResetEmployeeSelect()
        {
            await this.JsRuntime.InvokeVoidAsync("changeToDefaultOption");
        }

        private void SetupYears()
        {
            List<int> years = new List<int>();
            int start = 2024;

            for (int i = start; i <= DateTime.UtcNow.Year + 30; i++)
            {
                years.Add(i);
            }

            this.Years = years.ToArray();
        }

        private int ConvertMonthNameToInt(string? month)
        {
            return Array.IndexOf(Months, month) + 1;
        }

        private int[][] CreateCalenderGrid()
        {
            int[][] grid = new int[5][];
            int startDay = 0;
            int day = 1;

            int month = this.SelectedMonth != null ? ConvertMonthNameToInt(this.SelectedMonth) : DateTime.Now.Month;


            int days = DateTime.DaysInMonth(this.SelectedYear, month);

            switch (new DateTime(this.SelectedYear, month, 1).DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    startDay = 0;
                    break;
                case DayOfWeek.Monday:
                    startDay = 1;
                    break;
                case DayOfWeek.Tuesday:
                    startDay = 2;
                    break;
                case DayOfWeek.Wednesday:
                    startDay = 3;
                    break;
                case DayOfWeek.Thursday:
                    startDay = 4;
                    break;
                case DayOfWeek.Friday:
                    startDay = 5;
                    break;
                case DayOfWeek.Saturday:
                    startDay = 6;
                    break;
            }

            for (int i = 0; i < 5; i++)
            {
                grid[i] = new int[7];
                for (int j = startDay; j < 7; j++)
                {
                    if (day <= days)
                    {
                        grid[i][j] = day;
                        day++;
                    }
                }

                startDay = 0;
            }

            return grid;
        }

        private async Task ShowAddEmployeeModal()
        {
            IModalReference formModal = Modal.Show<AddEmployeeModal>("Add Employee");
            ModalResult result = await formModal.Result;

            if (result.Cancelled)
            {
                Console.WriteLine("Modal was cancelled");
            }
            else
            {
                string name = (string)result.Data!;

                CreateEmployeeResponse response = await this.EmployeeService.AddEmployee(name);
                if (response.Success == false)
                {
                    string errors = string.Concat($"{Environment.NewLine}", response.ValidationErrors.ToArray());
                    if (response.Success == false && string.Equals(response.Message, "Employee Creation Invalid"))
                    {
                        this.ToastService.ShowWarning(errors);
                    }
                    else
                    {
                        this.ToastService.ShowError("Unexpected Error Occured");
                    }
                }
                else
                {
                    this.ToastService.ShowSuccess("Employee Added Successfully");
                    this.Employees = await this.EmployeeService.GetEmployees(false);

                    //Update selected employee to newly added employee
                    EmployeeDto tempEmployee = this.Employees.First(e => e.Id == response.EmployeeDto?.Id);
                    int index = this.Employees.IndexOf(tempEmployee) + 1;
                    await this.JsRuntime.InvokeVoidAsync("selectNewEmployee", index);

                    ReCreateCalender();
                }
            }
        }

        private async void On_RemoveEmployeeClick()
        {
            if (this.SelectedEmployee == null)
            {
                return;
            }

            ModalParameters parameters = new ModalParameters
            {
                { nameof(RemoveEmployeeModal.Name), this.SelectedEmployee.Name }
            };


            IModalReference formModal = this.Modal.Show<RemoveEmployeeModal>("Confirm Employee Deletion", parameters);
            ModalResult result = await formModal.Result;

            if (result.Cancelled)
            {
                Console.WriteLine("Modal was cancelled");
            }
            else
            {
                DeleteEmployeeResponse response =
                    await this.EmployeeService.DeleteEmployee(this.SelectedEmployee.Id);
                if (response.Success == false)
                {
                    if (response.Success == false && string.Equals(response.Message,
                            "Employee failed to delete, no such employee exists"))
                    {
                        this.ToastService.ShowWarning("Employee Not Found");
                        this.SelectedEmployee = null;
                        await ResetEmployeeSelect();
                    }
                    else
                    {
                        this.ToastService.ShowError("Unexpected Error");
                    }
                }
                else
                {
                    this.ToastService.ShowSuccess("Employee Successfully Deleted");
                    this.Employees = await this.EmployeeService.GetEmployees(false);
                    this.SelectedEmployee = null;

                    await ResetEmployeeSelect();
                    ReCreateCalender();
                }
            }
        }

        private async Task OnSelectedEmployeeChange(int newValue)
        {
            if (newValue > 0)
            {
                this.SelectedEmployee = await this.EmployeeService.GetEmployee(newValue);
                if (this.SelectedEmployee != null)
                {
                    this.SelectedEmployee.AttendanceEntries =
                        (List<AttendanceEntryDto>)await this.AttendanceEntryService.GetAttendanceEntries(
                            this.SelectedEmployee.Id, this.SelectedYear);
                }

                UpdateEmployeeStatistics();
            }
            else
            {
                this.SelectedEmployee = null;
            }

            ReCreateCalender();
        }

        private void UpdateEmployeeStatistics()
        {
            if (this.SelectedEmployee != null)
            {
                //30 Day values
                this.SelectedEmployee.LatePoints30 = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-30) &&
                                x.Category == AttendanceEntryCategory.Late).Select(x => x.Amount).Sum();
                this.SelectedEmployee.LeftEarlyPoints30 = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-30) &&
                                x.Category == AttendanceEntryCategory.LeftEarly).Select(x => x.Amount).Sum();
                this.SelectedEmployee.UnExcusedAbsencePoints30 = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-30) &&
                                x.Category == AttendanceEntryCategory.UnexcusedAbsence).Select(x => x.Amount).Sum();
                this.SelectedEmployee.NoCallNoShowPoints30 = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-30) &&
                                x.Category == AttendanceEntryCategory.NoCallNoShow).Select(x => x.Amount).Sum();
                this.SelectedEmployee.TotalPoints30Days = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-30)).Select(x => x.Amount).Sum();

                //180 Day values
                this.SelectedEmployee.LatePoints180 = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-180) &&
                                x.Category == AttendanceEntryCategory.Late).Select(x => x.Amount).Sum();
                this.SelectedEmployee.LeftEarlyPoints180 = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-180) &&
                                x.Category == AttendanceEntryCategory.LeftEarly).Select(x => x.Amount).Sum();
                this.SelectedEmployee.UnExcusedAbsencePoints180 = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-180) &&
                                x.Category == AttendanceEntryCategory.UnexcusedAbsence).Select(x => x.Amount).Sum();
                this.SelectedEmployee.NoCallNoShowPoints180 = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-180) &&
                                x.Category == AttendanceEntryCategory.NoCallNoShow).Select(x => x.Amount).Sum();
                this.SelectedEmployee.TotalPoints180Days = this.SelectedEmployee.AttendanceEntries
                    .Where(x => x.InputDate >= DateTime.Today.AddDays(-180)).Select(x => x.Amount).Sum();
            }
        }

        private void ReCreateCalender()
        {
            this.Grids.Clear();

            this.Grids.Add(CreateCalenderGrid());
            InvokeAsync(StateHasChanged);
        }

        private void OnSelectedYearChange()
        {
            ReCreateCalender();
        }

        private void OnSelectedMonthChange()
        {
            ReCreateCalender();
        }

        private async void CalenderDayClicked(CalenderItem date)
        {
            if (this.SelectedEmployee != null)
            {
                ModalParameters parameters = new ModalParameters();
                IEnumerable<AttendanceEntryDto> entries = this.SelectedEmployee.AttendanceEntries.Where(x =>
                    x.InputDate == new DateTime(date.Year, date.Month, date.Day));
                int id = -1;
                AttendanceEntryDto? existingEntry = null;

                IEnumerable<AttendanceEntryDto> attendanceEntryDtos = entries.ToList();
                if (attendanceEntryDtos.Any())
                {
                    existingEntry = attendanceEntryDtos.First();
                    id = existingEntry.Id;

                    parameters.Add(nameof(AttendanceEntryModal.ExistingEntry), new AttendanceEntryModel
                    {
                        Category = existingEntry.Category,
                        Amount = existingEntry.Amount,
                        InputDate = existingEntry.InputDate,
                        Notes = existingEntry.Notes
                    });
                }

                parameters.Add(nameof(AttendanceEntryModal.DateSelected),
                    new DateTime(date.Year, date.Month, date.Day));

                IModalReference formModal = Modal.Show<AttendanceEntryModal>("Add Entry", parameters);
                ModalResult result = await formModal.Result;

                if (result.Cancelled)
                {
                    Console.WriteLine("Modal was cancelled");
                }
                else
                {
                    AttendanceEntryModel modalEntry = (AttendanceEntryModel)result.Data!;

                    if (modalEntry.InputDate == DateTime.MinValue)
                    {
                        await this.AttendanceEntryService.DeleteAttendanceEntry(id);
                        this.SelectedEmployee.AttendanceEntries =
                            (List<AttendanceEntryDto>)await this.AttendanceEntryService.GetAttendanceEntries(
                                this.SelectedEmployee.Id, this.SelectedYear);
                    }
                    else if (existingEntry != null)
                    {
                        existingEntry.Notes = modalEntry.Notes;
                        existingEntry.Category = modalEntry.Category;
                        existingEntry.Amount = modalEntry.Amount;
                        await this.AttendanceEntryService.UpdateAttendanceEntry(existingEntry);
                    }
                    else
                    {
                        modalEntry.EmployeeId = this.SelectedEmployee.Id;
                        await this.AttendanceEntryService.AddAttendanceEntry(modalEntry);
                        this.SelectedEmployee.AttendanceEntries =
                            (List<AttendanceEntryDto>)await this.AttendanceEntryService.GetAttendanceEntries(
                                this.SelectedEmployee.Id, this.SelectedYear);
                    }

                    UpdateEmployeeStatistics();
                    ReCreateCalender();
                }
            }
        }
    }
}