using System.Collections.Generic;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Enums;
using Charmaran.UI.Models;
using Microsoft.AspNetCore.Components;

namespace Charmaran.UI.Components
{
    public partial class Calender
    {
        [Parameter]
        public int[][] CalenderGrid { get; set; } = null!;

        [Parameter]
        public IEnumerable<AttendanceEntryDto>? AttendanceEntries { get; set; }

        [Parameter]
        public int Year { get; set; }

        [Parameter]
        public int Month { get; set; }

        [Parameter]
        public EventCallback<CalenderItem> DayClicked { get; set; }

        private int Day { get; set; } = 1;

        protected override void OnAfterRender(bool firstRender)
        {
            this.Day = 1;
            base.OnAfterRender(firstRender);
        }

        private void CallBackCalenderClick(int day)
        {
            CalenderItem item = new CalenderItem
            {
                Year = this.Year,
                Month = this.Month,
                Day = day
            };

            DayClicked.InvokeAsync(item);
        }

        private string ConvertEntryTypeToColor(AttendanceEntryCategory category)
        {
            string colorCode;

            switch (category)
            {
                case AttendanceEntryCategory.Late:
                    colorCode = "#ffff00";
                    break;
                case AttendanceEntryCategory.Vacation:
                    colorCode = "#05f705";
                    break;
                case AttendanceEntryCategory.UnexcusedAbsence:
                    colorCode = "#ff0000";
                    break;
                case AttendanceEntryCategory.ExcusedAbsence:
                    colorCode = "#07e1ce";
                    break;
                case AttendanceEntryCategory.LeftEarly:
                    colorCode = "#ff8c00";
                    break;
                case AttendanceEntryCategory.NoCallNoShow:
                    colorCode = "#FC4923";
                    break;
                default:
                    colorCode = "#ffffff";
                    break;
            }

            return colorCode;
        }
    }
}