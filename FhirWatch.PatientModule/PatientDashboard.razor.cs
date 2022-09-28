using FhirWatch.SharedComponents;
using FhirWatch.SharedComponents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace FhirWatch.PatientModule
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
            await SaveAsync(nameof(Filters.FhirId), Filters.FhirId);
            await SaveAsync(nameof(Filters.FirstName), Filters.FirstName);
            await SaveAsync(nameof(Filters.LastName), Filters.LastName);
            await SaveAsync(nameof(Filters.StartDate), Filters.StartDate);
            await SaveAsync(nameof(Filters.EndDate), Filters.EndDate);

            // navigate to compare page
            NavigationManager.NavigateTo("patients");
        }

        private async Task SaveAsync(string propertyName, object value)
        {
            try
            {
                await (value != null ?
                JsRuntime.InvokeVoidAsync("stateManager.save", propertyName, value) :
                JsRuntime.InvokeVoidAsync("localStorage.removeItem", propertyName));
            } catch (Exception exception)
            {
                throw exception;
            }
        }

        private async void ClearFilters()
        {
            Filters = new PatientFilters();

            await JsRuntime.InvokeVoidAsync("localStorage.clear");
        }

        private async Task StartFilterDateChanged(DateTime? newDateTime)
        {
            Filters.StartDate = newDateTime;

            // persist selection to localstorage
            await JsRuntime.InvokeAsync<object>(
                "stateManager.save", nameof(Filters.StartDate), Filters.StartDate?.ToShortDateString());
        }

        private async Task EndFilterDateChanged(DateTime? newDateTime)
        {
            Filters.EndDate = newDateTime;

            // persist selection to localstorage
            await JsRuntime.InvokeAsync<object>(
                "stateManager.save", nameof(Filters.EndDate), Filters.EndDate?.ToShortDateString());
        }

        private bool DisableDates()
        {
            return !string.IsNullOrWhiteSpace(Filters.FirstName)
                || !string.IsNullOrWhiteSpace(Filters.LastName)
                || !string.IsNullOrWhiteSpace(Filters.FhirId);
        }
    }
}
