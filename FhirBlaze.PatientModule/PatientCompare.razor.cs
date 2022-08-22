using FhirBlaze.SharedComponents;
using FhirBlaze.SharedComponents.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace FhirBlaze.PatientModule
{
    [Authorize]
    public partial class PatientCompare
    {
        [Parameter]
        public IDictionary<string, (Patient, JToken)> Patients { get; set; }

    }

    public class FhirPatientViewModel
    {
        public FhirPatientViewModel(Patient patient)
        {
            FirstName = patient.Name.FirstOrDefault()?.Given?.FirstOrDefault();
            LastName = patient.Name.FirstOrDefault()?.Family;
            DateOfBirth = patient.BirthDate;
        }

        public FhirPatientViewModel(JToken token)
        {
            FirstName = "MJ";
            LastName = "Schanne";
            DateOfBirth = "01/01/2000";

        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }

    }
}
