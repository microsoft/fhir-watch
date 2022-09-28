﻿using FhirWatch.PatientModule.Models;
using FhirWatch.SharedComponents;
using FhirWatch.SharedComponents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace FhirWatch.PatientModule
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

            try
            {
                // todo: instead of saving nulls to local storage, remove entry
                Filters.FirstName = await JsRuntime.InvokeAsync<string>("stateManager.load", nameof(Filters.FirstName));
                Filters.FirstName = Filters.FirstName == "null" ? null : Filters.FirstName;
            }
            catch (InvalidOperationException) { /* do nothing */ }
            catch (JSException) { /* do nothing */ }

            try
            {
                Filters.LastName = await JsRuntime.InvokeAsync<string>("stateManager.load", nameof(Filters.LastName));
                Filters.LastName = Filters.LastName == "null" ? null : Filters.LastName;
            }
            catch (InvalidOperationException) { /* do nothing */ }
            catch (JSException) { /* do nothing */ }

            try
            {
                Filters.FhirId = await JsRuntime.InvokeAsync<string>("stateManager.load", nameof(Filters.FhirId));
                Filters.FhirId = Filters.FhirId == "null" ? null : Filters.FhirId;
            }
            catch (InvalidOperationException) { /* do nothing */ }
            catch (JSException) { /* do nothing */ }

            // some issues with awaiting a task
            await FetchData();

            ShouldRender();
        }

        // todo: move count logic server side
        protected async Task FetchData()
        {
            Loading = true;
            Patients.Clear();

            var fhirPatients = await FhirService.GetPatientsAsync(Filters);
            var dvPatients = await DataverseService.GetPatientsAsync(Filters);
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

            FhirCount = fhirList.Count;
            DVCount = dvList.Count;

            Loading = false;
        }

        private void NavigateToPatientDetail(EventArgs e, string id)
        {
            NavigationManager.NavigateTo($"patient/{id}");
        }

        private void NavigateToPatientDashboard(EventArgs e)
        {
            NavigationManager.NavigateTo("patient-dashboard");
        }
    }
}
