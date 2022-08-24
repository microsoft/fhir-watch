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
using Newtonsoft.Json.Linq;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientList
    {
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        IFhirService FhirService { get; set; }

        [Inject]
        DataverseService DataverseService { get; set; }
        protected bool ShowSearch { get; set; } = false;
        protected bool ShowComparison { get; set; } = false;
        public IDictionary<string, Tuple<Patient, JToken>> PatientsToCompare { get; set; } = new Dictionary<string, Tuple<Patient, JToken>>();
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
            var fhirId = "d001a1ee-19b9-a072-8ecd-91725f30e09d"; // Adan632 Brekke496
            var dvPatient = await DataverseService.GetPatientByFhirIdAsync(fhirId);
            var fhirPatient = Patients.FirstOrDefault(p => p.Id == fhirId);
            var value = new Tuple<Patient, JToken>(fhirPatient, dvPatient);
            PatientsToCompare.Add(fhirId, value);
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
