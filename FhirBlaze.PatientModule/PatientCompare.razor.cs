using FhirBlaze.PatientModule.Models;
using FhirBlaze.SharedComponents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientCompare
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        IFhirService FhirService { get; set; }
        [Inject]
        DataverseService DataverseService { get; set; }
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        [Parameter]
        public IList<PatientCompareModel> Patients { get; set; } = new List<PatientCompareModel>();
        protected bool Loading { get; set; } = true;
        protected DateTime FilterStartDate { get; set; } = DateTime.UtcNow.AddDays(-7);
        protected DateTime FilterEndDate { get; set; } = DateTime.UtcNow;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                var str = await JsRuntime.InvokeAsync<string>("stateManager.load", nameof(FilterStartDate));
                if (DateTime.TryParse(str, out DateTime date))
                    FilterStartDate = date;
            }
            catch (InvalidOperationException) { /* do nothing */ }
            catch (JSException) { /* do nothing */ }

            try
            {
                var str = await JsRuntime.InvokeAsync<string>("stateManager.load", nameof(FilterEndDate));
                if (DateTime.TryParse(str, out DateTime date))
                    FilterEndDate = date;
            }
            catch (InvalidOperationException) { /* do nothing */ }
            catch (JSException) { /* do nothing */ }

            // some issues with awaiting a task
            await FetchData(FilterStartDate, FilterEndDate);

            ShouldRender();
        }

        // todo: move count logic server side
        protected async Task FetchData(DateTime startFilterDate, DateTime endFilterDate)
        {
            Loading = true;
            Patients.Clear();

            var fhirPatients = await FhirService.GetPatientsAsync(startFilterDate, endFilterDate);
            var dvPatients = await DataverseService.GetPatients(startFilterDate, endFilterDate);
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

        private void NavigateToPatientDetail(EventArgs e, string id)
        {
            NavigationManager.NavigateTo($"patient/{id}");
        }
    }
}
