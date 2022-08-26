using FhirBlaze.PatientModule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientCompare
    {
        [Parameter]
        public IList<PatientCompareModel> Patients { get; set; }
    }
}
