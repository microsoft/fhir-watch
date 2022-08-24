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

        public FhirPatientViewModel(string id, JToken token)
        {
            // hardcoded data for demo
            //Id = id;
            //FirstName = "MJ";
            //LastName = "Schanne";
            //DateOfBirth = DateTime.Now.ToString();

            var jValue = token as JValue;
            
            try
            {
                var obj = jValue.Children().FirstOrDefault();
                var obj2 = jValue.ToArray().FirstOrDefault();
                var obj3 = jValue.ToList().FirstOrDefault();

                var val = jValue.Value;

                var array = val as JArray;

                var valType = val.GetType();

                //var jObject = obj as JObject;

                //var attributes = jObject["attributes"];

                //DateOfBirth = obj?.SelectToken(".attributes[?(@.key == 'birthdate')]")?.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Id = id;
            //FirstName = null;
            //LastName = null;
            //try
            //{
            //    DateOfBirth = attributes.Select(i => i as JObject).Select(o => o?.Properties()?.FirstOrDefault(p => p.Name == "birthdate")).FirstOrDefault()?.Value.ToString();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
    }
}
