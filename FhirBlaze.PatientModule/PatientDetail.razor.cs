using FhirBlaze.SharedComponents.Services;
using Hl7.Fhir.ElementModel.Types;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        protected string PatientDV { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Loading = true;
            await base.OnInitializedAsync();
            var patients = await FhirService.GetPatientsAsync();
            Patient = patients.First(p => p.Id == Id);
            
            //string fileName = "Adan632_Brekke496.json";
            //string jsonString = File.ReadAllText(fileName);

            string jsonString = "{\r\n  \"@odata.etag\": \"W/\\\"9042015\\\"\",\r\n  \"customertypecode@OData.Community.Display.V1.FormattedValue\": \"Default Value\",\r\n  \"customertypecode\": 1,\r\n  \"address1_latitude@OData.Community.Display.V1.FormattedValue\": \"32.94799\",\r\n  \"address1_latitude\": 32.947991129353447,\r\n  \"birthdate@OData.Community.Display.V1.FormattedValue\": \"12/31/2014\",\r\n  \"birthdate\": \"2014-12-31\",\r\n  \"merged@OData.Community.Display.V1.FormattedValue\": \"No\",\r\n  \"merged\": false,\r\n  \"gendercode@OData.Community.Display.V1.FormattedValue\": \"Male\",\r\n  \"gendercode\": 1,}";
            PatientDV = jsonString;
            Loading = false;
            ShouldRender();
        }
        private void NavigateToPatientList()
        {
            navigationManager.NavigateTo("patient");
        }
    }
}
