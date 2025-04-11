using Microsoft.AspNetCore.Components;

namespace Charmaran.UI.Components
{
    public partial class DateButton
    {
        [Parameter]
        public int Day { get; set; }

        [Parameter]
        public string BackGroundColor { get; set; } = null!;

        [Parameter]
        public EventCallback<int> DayClicked { get; set; }


        private void CallBackCalenderClick()
        {
            DayClicked.InvokeAsync(this.Day);
        }
    }
}