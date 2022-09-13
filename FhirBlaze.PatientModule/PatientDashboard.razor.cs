using FhirBlaze.PatientModule.Models;
using FhirBlaze.SharedComponents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // todo: make form model class?
        protected string FilterId { get; set; } = null;
        protected DateTime FilterStartDate { get; set; } = DateTime.UtcNow.AddDays(-7);
        protected DateTime FilterEndDate { get; set; } = DateTime.UtcNow;
        protected string FilterFirstName { get; set; } = null;
        protected string FilterLastName { get; set; } = null;

        public List<PatientCompareModel> Patients { get; set; } = new List<PatientCompareModel>();
        protected int FhirOrphanCount { get; set; }
        protected int DataverseOrphanCount { get; set; }
        protected int TotalRecordCount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                FilterStartDate = await JsRuntime.InvokeAsync<DateTime>("stateManager.load", nameof(FilterStartDate));
            }
            catch (InvalidOperationException) { /* do nothing */ }

            try
            {
                FilterEndDate = await JsRuntime.InvokeAsync<DateTime>("stateManager.load", nameof(FilterEndDate));
            }
            catch (InvalidOperationException) { /* do nothing */ }

            await FetchData();

            ShouldRender();
        }

        // todo: move count logic server side
        protected async Task FetchData()
        {
            Loading = true;
            Patients.Clear();

            var fhirPatients = await FhirService.GetPatientsAsync(FilterStartDate, FilterEndDate);
            var dvPatients = await DataverseService.GetPatients(FilterStartDate, FilterEndDate);
            var fhirList = fhirPatients.Select(f => new PatientViewModel(f)).ToList();
            var dvList = dvPatients.Select(d => new PatientViewModel(d)).ToList();

            foreach (var fhirPatient in fhirList)
            {
                var matchingDvPatient = dvList.FirstOrDefault(d => d.Id == fhirPatient.Id);
                Patients.Add(new PatientCompareModel(fhirPatient.Id, fhirPatient, matchingDvPatient));
            }

            foreach (var dvPatient in dvList)
            {
                if (dvPatient.Id == null || !Patients.Any(p => p.Id == dvPatient.Id))
                {
                    Patients.Add(new PatientCompareModel(dvPatient.Id, null, dvPatient));
                }
            }

            FhirOrphanCount = Patients.Count(p => p.Item1 != null && p.Item2 == null);
            DataverseOrphanCount = Patients.Count(p => p.Item2 != null && p.Item1 == null);
            TotalRecordCount = Patients.Count();

            Loading = false;
        }

        private async Task Search()
        {
            // save filters to local storage
            await JsRuntime.InvokeAsync<object>("stateManager.save", nameof(FilterId), FilterId);
            await JsRuntime.InvokeAsync<object>("stateManager.save", nameof(FilterFirstName), FilterFirstName);
            await JsRuntime.InvokeAsync<object>("stateManager.save", nameof(FilterLastName), FilterLastName);

            // navigate to compare page
            NavigationManager.NavigateTo("patients");
        }

        private void ClearFilters()
        {
            FilterId = null;
            FilterFirstName = null;
            FilterLastName = null;
        }

        private async Task FilterDateChanged(DateTime newDateTime)
        {
            FilterDate = newDateTime;

            // persist selection to localstorage
            await JsRuntime.InvokeAsync<object>(
                "stateManager.save", nameof(FilterDate), FilterDate.ToShortDateString());
        }
    }
}
