using FhirBlaze.PatientModule.models;
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

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientList
    {
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        IFhirService FhirService { get; set; }        
        protected bool ShowSearch { get; set; } = false;
        protected bool Loading { get; set; } = true;
        
        protected bool ProcessingSearch { get; set; } = false;
        
        protected SimplePatient DraftPatient { get; set; } = new SimplePatient();
        
        protected Patient SelectedPatient { get; set; } = new Patient();
        [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; }
        public IList<Patient> Patients { get; set; } = new List<Patient>();

        protected override async Task OnInitializedAsync()
        {            
            Loading = true;
            await base.OnInitializedAsync();
            Patients = await FhirService.GetPatientsAsync();
            Loading = false;
            ShouldRender();
        }        

        public async Task SearchPatient(Patient patient)
        {
            ResetSelectedPatient();
            try
            {
                Patients = await FhirService.SearchPatient(patient); //change to patient
                ProcessingSearch = true;
                
                ProcessingSearch = false;
                ToggleSearch();
                ShouldRender();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception");
                Console.WriteLine(e.Message); //manage the cancel search
            }
        }
        
        public void ToggleSearch()
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
            SelectedPatient = null;
        }

        private void PatientSelected(EventArgs e, Patient newPatient)
        {
            SelectedPatient = newPatient;            
        }

        private void NavigateToPatientDetail(EventArgs e, string id)
        {
            navigationManager.NavigateTo($"patient/{id}");
        }
    }
}
