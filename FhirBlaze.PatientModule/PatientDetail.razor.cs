using FhirBlaze.SharedComponents.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph;
using System.Collections.Generic;
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
        [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Loading = true;
            await base.OnInitializedAsync();
            var patients = await FhirService.GetPatientsAsync();
            Patient = patients.First(p => p.Id == Id);
            Loading = false;

            MyFolder.Add(new MailItem { Id = 1, FolderName = "Inbox", HasSubFolder = true, Expanded = true, ParentId = "" });
            MyFolder.Add(new MailItem { Id = 2, FolderName = "Category", ParentId = "1", HasSubFolder = true, Expanded = true });
            MyFolder.Add(new MailItem { Id = 3, FolderName = "Primary", ParentId = "2", HasSubFolder = false, Expanded = true });
            MyFolder.Add(new MailItem { Id = 4, FolderName = "Social", ParentId = "6", HasSubFolder = false, Expanded = true });
            MyFolder.Add(new MailItem { Id = 5, FolderName = "Promotion", ParentId = "6", HasSubFolder = false, Expanded = true });
            MyFolder.Add(new MailItem { Id = 6, FolderName = "Demo", ParentId = "2", HasSubFolder = true, Expanded = true });

            ShouldRender();
        }
        private void NavigateToPatientList()
        {
            navigationManager.NavigateTo("patient");
        }


        List<MailItem> MyFolder = new List<MailItem>();
        protected class MailItem
        {
            public int Id { get; set; }
            public string ParentId { get; set; }
            public bool HasSubFolder { get; set; }
            public string FolderName { get; set; }
            public bool Expanded { get; set; }
        }
    }
}
