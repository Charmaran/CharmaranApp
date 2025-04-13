using System;
using Blazored.Modal;
using Blazored.Modal.Services;
using Charmaran.Shared.AttendanceTracker.Enums;
using Charmaran.UI.Models;
using Charmaran.UI.Models.Enums;
using Microsoft.AspNetCore.Components;

namespace Charmaran.UI.Components.Modals
{
    public partial class AttendanceEntryModal
    {
        [CascadingParameter] BlazoredModalInstance ModalInstance { get; set; } = null!;

        [Parameter] public AttendanceEntryModel? ExistingEntry { get; set; }

        [Parameter] public DateTime DateSelected { get; set; }

        private string? Category { get; set; }

        private string? Amount { get; set; }

        private string? Notes { get; set; }

        private AttendanceEntryModel? NewEntry { get; set; }

        protected override void OnInitialized()
        {
            if (ExistingEntry != null)
            {
                int value = (int)ExistingEntry.Category + 1;
                this.Category = value.ToString();
                switch (ExistingEntry.Amount)
                {
                    case 0.5F:
                        this.Amount = "1";
                        break;
                    case 1:
                        this.Amount = "1";
                        break;
                }

                this.Notes = ExistingEntry.Notes;
            }
        }

        private void SubmitForm()
        {
            AttendanceEntryCategory category = AttendanceEntryCategory.Late;
            float amount = 1;
            bool isNewEntry = ExistingEntry == null;

            if (Category != null && Amount != null)
            {
                switch (Category)
                {
                    case "1":
                        category = AttendanceEntryCategory.Late;
                        break;
                    case "2":
                        category = AttendanceEntryCategory.LeftEarly;
                        break;
                    case "3":
                        category = AttendanceEntryCategory.UnexcusedAbsence;
                        break;
                    case "4":
                        category = AttendanceEntryCategory.ExcusedAbsence;
                        break;
                    case "5":
                        category = AttendanceEntryCategory.NoCallNoShow;
                        break;
                    case "6":
                        category = AttendanceEntryCategory.Vacation;
                        break;
                }

                switch (Amount)
                {
                    case "1":
                        amount = 0.5F;
                        break;
                    case "2":
                        amount = 1;
                        break;
                }

                NewEntry = new AttendanceEntryModel
                {
                    CustomModalResult = ExistingEntry == null ? CustomModalResult.Create : CustomModalResult.Update,
                    Category = category,
                    Amount = amount,
                    InputDate = DateSelected,
                    Notes = Notes,
                    IsNewEntry = isNewEntry
                };
                ModalInstance.CloseAsync(ModalResult.Ok(NewEntry));
            }
        }

        private void SubmitDelete()
        {
            ModalInstance.CloseAsync(ModalResult.Ok(new AttendanceEntryModel
            {
                CustomModalResult = CustomModalResult.Delete
            }));
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }
    }
}