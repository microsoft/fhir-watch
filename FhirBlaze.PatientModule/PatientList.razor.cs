using FhirBlaze.PatientModule.Models;
using FhirBlaze.SharedComponents.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Task = System.Threading.Tasks.Task;
using Newtonsoft.Json.Linq;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientList
    {
        [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        IFhirService FhirService { get; set; }
        [Inject]
        DataverseService DataverseService { get; set; }

        protected bool ShowSearch { get; set; } = false;
        protected bool ShowComparison { get; set; } = false;
        protected bool Loading { get; set; } = true;
        protected bool ProcessingSearch { get; set; } = false;
        protected SimplePatient DraftPatient { get; set; } = new SimplePatient();
        //protected IList<PatientCompareModel> SelectedPatients { get; set; } = new List<PatientCompareModel>();
        protected List<PatientCompareModel> SelectedPatients => Patients.Where(p => p.IsSelected).ToList();

        public List<PatientCompareModel> Patients { get; set; } = new List<PatientCompareModel>();

        protected override async Task OnInitializedAsync()
        {
            Loading = true;
            await base.OnInitializedAsync();
            var fhirPatients = await FhirService.GetPatientsAsync();
            var dvPatients = await DataverseService.GetPatients();
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
            ShouldRender();
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
            if (ShowComparison) ResetSelectedPatient();
            ShowComparison = !ShowComparison;
        }

        private void ToggleSearch()
        {
            ShowSearch = !ShowSearch;
            ResetSelectedPatient();
            if (ShowSearch)
            {
                DraftPatient = new SimplePatient();
            }
        }

        private void ResetSelectedPatient()
        {
            Patients.ForEach(p => p.IsSelected = false);
            //SelectedPatients.Clear();
        }

        private void SelectAll(object isChecked)
        {
            if ((bool)isChecked)
            {
                Patients.ForEach(p => p.IsSelected = true);
            }
            else
            {
                Patients.ForEach(p => p.IsSelected = false);
            }
        }

        private void PatientSelected(object isChecked, PatientCompareModel newPatient)
        {
            if ((bool)isChecked && !SelectedPatients.Contains(newPatient))
            {
                newPatient.IsSelected = true;
                //SelectedPatients.Add(newPatient);
            }
            else
            {
                newPatient.IsSelected = false;
                //if (SelectedPatients.Contains(newPatient))
                //{
                //    SelectedPatients.Remove(newPatient);
                //}
            }
            StateHasChanged();
        }

        private void NavigateToPatientDetail(EventArgs e, string id)
        {
            navigationManager.NavigateTo($"patient/{id}");
        }
    }
}
