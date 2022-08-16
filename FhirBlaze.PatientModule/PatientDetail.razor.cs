using FhirBlaze.SharedComponents;
using FhirBlaze.SharedComponents.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientDetail
    {
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        IFhirService FhirService { get; set; }
        [Parameter]
        public string Id { get; set; }
        protected bool Loading { get; set; } = true;
        protected Patient Patient { get; set; } = new Patient();
        protected Branch Trunk { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Loading = true;
            await base.OnInitializedAsync();
            var patients = await FhirService.GetPatientsAsync();
            Patient = patients.First(p => p.Id == Id);
            Loading = false;
            Trunk = new Branch(Patient, "Patient", 1);

            ShouldRender();
        }
        private void NavigateToPatientList()
        {
            navigationManager.NavigateTo("patient");
        }
    }
}
