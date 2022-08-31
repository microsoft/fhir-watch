using FhirBlaze.PatientModule.Models;
using FhirBlaze.SharedComponents.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientList
    {
        [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        IFhirService FhirService { get; set; }
        [Inject]
        DataverseService DataverseService { get; set; }
        [Inject]
        IJSRuntime JsRuntime { get; set; }

        protected bool ShowSearch { get; set; } = false;
        protected bool ShowComparison { get; set; } = false;
        protected bool Loading { get; set; } = true;
        protected bool ProcessingSearch { get; set; } = false;
        protected DateTime FilterDate { get; set; } = DateTime.UtcNow.AddDays(-7);
        protected SimplePatient DraftPatient { get; set; } = new SimplePatient();
        protected List<PatientCompareModel> SelectedPatients => Patients.Where(p => p.IsSelected).ToList();

        public List<PatientCompareModel> Patients { get; set; } = new List<PatientCompareModel>();

        protected override async Task OnInitializedAsync()
        {
            // Try to fetch previously selected filterDate
            string dateStr = String.Empty;
            try
            {
                dateStr = await JsRuntime.InvokeAsync<string>("stateManager.load", nameof(FilterDate));
            }
            catch (InvalidOperationException)
            {
                // do nothing
            }
            if (!string.IsNullOrEmpty(dateStr))
            {
                FilterDate = DateTime.Parse(dateStr);
            }

            await FetchData();
            ShouldRender();
        }

        protected async Task FetchData()
        {
            Loading = true;
            Patients.Clear();
            var fhirPatients = await FhirService.GetPatientsAsync(FilterDate);
            var dvPatients = await DataverseService.GetPatients(FilterDate);
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

            Loading = false;
        }

        public async Task SearchPatient(Patient patient)
        {
            //ResetSelectedPatient();
            //try
            //{
            //    Patients = await FhirService.SearchPatient(patient); //change to patient
            //    ProcessingSearch = true;

            //    ProcessingSearch = false;
            //    ToggleSearch();
            //    ShouldRender();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Exception");
            //    Console.WriteLine(e.Message); //manage the cancel search
            //}
        }

        private void ToggleComparison()
        {
            ShowComparison = !ShowComparison;
        }

        private void ToggleSearch()
        {
            ShowSearch = !ShowSearch;

            if (ShowSearch)
            {
                DraftPatient = new SimplePatient();
            }
        }

        private void ClearSelection()
        {
            Patients.ForEach(p => p.IsSelected = false);
        }

        private async Task FilterDateChanged(DateTime newDateTime)
        {
            FilterDate = newDateTime;

            // persist selection to localstorage
            await JsRuntime.InvokeAsync<object>(
                "stateManager.save", nameof(FilterDate), FilterDate.ToShortDateString());

            await FetchData();
            ShouldRender();
        }

        private void SelectAll(object isChecked)
        {
            Patients.ForEach(p => p.IsSelected = (bool)isChecked);
        }

        private void PatientSelected(object isChecked, PatientCompareModel newPatient)
        {
            newPatient.IsSelected = (bool)isChecked && !SelectedPatients.Contains(newPatient);

            StateHasChanged();
        }

        private void NavigateToPatientDetail(EventArgs e, string id)
        {
            NavigationManager.NavigateTo($"patient/{id}");
        }
    }
}
