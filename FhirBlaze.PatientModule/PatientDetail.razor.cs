using FhirBlaze.SharedComponents;
using FhirBlaze.SharedComponents.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientDetail
    {
        [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; }
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        IFhirService FhirService { get; set; }
        [Inject]
        DataverseService DataverseService { get; set; }
        
        protected bool Loading { get; set; } = true;
        protected Patient Patient { get; set; } = new Patient();
        protected JToken PatientDV { get; set; }
        protected Branch Trunk1 { get; set; }
        protected Branch Trunk2 { get; set; }
        protected bool Raw { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            Loading = true;
            await base.OnInitializedAsync();
            Patient = await FhirService.GetResourceByIdAsync<Patient>(Id);
            PatientDV = await DataverseService.GetPatientByFhirIdAsync(Id);

            Loading = false;
            Trunk1 = new Branch(Patient, "Patient", 1);
            Trunk2 = new Branch(PatientDV, "Patient", 1);

            ShouldRender();
        }
        private void NavigateToPatientList()
        {
            navigationManager.NavigateTo("patient");
        }
    }
}
