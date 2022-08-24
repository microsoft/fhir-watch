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
        public IDictionary<string, Tuple<Patient, JToken>> Patients { get; set; }
        public List<Tuple<FhirPatientViewModel, FhirPatientViewModel>> PatientPairs { get; set; } = new ();

        protected override void OnParametersSet()
        {
            foreach(var entry in Patients)
            {
                // fist object in tuple is patient from fhir, second is the one from dataverse
                var patient1 = new FhirPatientViewModel(entry.Key, entry.Value.Item1);
                var patient2 = new FhirPatientViewModel(entry.Key, entry.Value.Item2);
                var tuple = new Tuple<FhirPatientViewModel, FhirPatientViewModel>(patient1, patient2);
                PatientPairs.Add(tuple);
            }
        }

    }

    public class FhirPatientViewModel
    {
        public FhirPatientViewModel(string id, Patient patient)
        {
            Id = id;
            FirstName = patient.Name.FirstOrDefault()?.Given?.FirstOrDefault();
            LastName = patient.Name.FirstOrDefault()?.Family;
            DateOfBirth = patient.BirthDate;
        }

        public FhirPatientViewModel(string id, JToken patientJson)
        {
            Id = id;

            var jObject = JObject.Parse(patientJson.ToString());
            var attributes = jObject["attributes"];

            FirstName = jObject.SelectToken(".attributes[?(@.key == 'firstname')].value")?.ToString();
            LastName = jObject.SelectToken(".attributes[?(@.key == 'lastname')].value")?.ToString();
            DateOfBirth = jObject.SelectToken(".attributes[?(@.key == 'birthdate')].value")?.ToString();
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
    }
}
