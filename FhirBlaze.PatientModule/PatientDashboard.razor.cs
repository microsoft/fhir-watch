using FhirBlaze.SharedComponents;
using FhirBlaze.SharedComponents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientDashboard
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; }
        [Inject]
        IFhirService FhirService { get; set; }
        [Inject]
        DataverseService DataverseService { get; set; }
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        protected bool Loading { get; set; } = true;

        protected PatientFilters Filters { get; set; } = new PatientFilters();

        protected int FhirCount { get; set; }
        protected int DVCount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var str = await JsRuntime.InvokeAsync<string>("stateManager.load", nameof(Filters.StartDate));
                if (DateTime.TryParse(str, out DateTime date))
                    Filters.StartDate = date;
            }
            catch (InvalidOperationException) { /* do nothing */ }
            catch (JSException) { /* do nothing */ }
            
            try
            {
                var str = await JsRuntime.InvokeAsync<string>("stateManager.load", nameof(Filters.EndDate));
                if (DateTime.TryParse(str, out DateTime date))
                    Filters.EndDate = date;
            }
            catch (InvalidOperationException) { /* do nothing */ }
            catch (JSException) { /* do nothing */ }

            await FetchData();

            ShouldRender();
        }

        // todo: move count logic server side
        protected async Task FetchData()
        {
            Loading = true;
            
            FhirCount = await FhirService.GetResourceCountAsync<Hl7.Fhir.Model.Patient>();
            DVCount = await DataverseService.GetPatientCount();

            Loading = false;
        }

        private async Task Search()
        {
            // save filters to local storage
            await JsRuntime.InvokeAsync<object>("stateManager.save", nameof(Filters.FhirId), Filters.FhirId);
            await JsRuntime.InvokeAsync<object>("stateManager.save", nameof(Filters.FirstName), Filters.FirstName);
            await JsRuntime.InvokeAsync<object>("stateManager.save", nameof(Filters.LastName), Filters.LastName);

            // navigate to compare page
            NavigationManager.NavigateTo("patients");
        }

        private void ClearFilters()
        {
            Filters = new PatientFilters();
        }

        private async Task StartFilterDateChanged(DateTime newDateTime)
        {
            Filters.StartDate = newDateTime;

            // persist selection to localstorage
            await JsRuntime.InvokeAsync<object>(
                "stateManager.save", nameof(Filters.StartDate), Filters.StartDate.ToShortDateString());
        }

        private async Task EndFilterDateChanged(DateTime newDateTime)
        {
            Filters.EndDate = newDateTime;

            // persist selection to localstorage
            await JsRuntime.InvokeAsync<object>(
                "stateManager.save", nameof(Filters.EndDate), Filters.EndDate.ToShortDateString());
        }

        private bool DisableDates()
        {
            return !string.IsNullOrWhiteSpace(Filters.FirstName)
                || !string.IsNullOrWhiteSpace(Filters.LastName)
                || !string.IsNullOrWhiteSpace(Filters.FhirId);
        }
    }
}
