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
        public List<Tuple<FhirPatientViewModel, FhirPatientViewModel>> PatientPairs { get; set; } = new();

        protected override void OnParametersSet()
        {
            foreach (var entry in Patients)
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
        // 
        // todo: eventually map based on configuration provided from sync agent which has json path for fhir objects saved
        public FhirPatientViewModel(string id, Patient patient)
        {
            Id = id;
            var officialName = patient.Name.FirstOrDefault(n => n.Use == HumanName.NameUse.Official);
            FirstName = officialName?.Given?.FirstOrDefault();
            LastName = officialName?.Family;
            DateOfBirth = patient.BirthDate;

            Address1Line1 = patient.Address.FirstOrDefault()?.Line?.FirstOrDefault();
            Address1City = patient.Address.FirstOrDefault()?.City;
            Address1Country = patient.Address.FirstOrDefault()?.Country;
            Address1StateProvince = patient.Address.FirstOrDefault()?.State;

            GenderCode = patient.Gender.ToString();
            FamilyStatusCode = patient.MaritalStatus.Text;

            LastUpdatedOn = patient.Meta.LastUpdated.ToString();
            FhirVersion = patient.VersionId.ToString();
            Telephone2 = patient.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone && t.Use == ContactPoint.ContactPointUse.Home)?.Value;
            
            // doesn't seem to be mapped?
            //ContactType = patient.Contact
        }

        public FhirPatientViewModel(string id, JToken patientJson)
        {
            Id = id;

            var jObject = JObject.Parse(patientJson.ToString());

            FirstName = jObject.SelectToken(".attributes[?(@.key == 'firstname')].value")?.ToString();
            LastName = jObject.SelectToken(".attributes[?(@.key == 'lastname')].value")?.ToString();
            DateOfBirth = jObject.SelectToken(".attributes[?(@.key == 'birthdate')].value")?.ToString();
            
            Address1Line1 = jObject.SelectToken(".attributes[?(@.key == 'address1_line1')].value")?.ToString();
            Address1City = jObject.SelectToken(".attributes[?(@.key == 'address1_city')].value")?.ToString();
            Address1Country = jObject.SelectToken(".attributes[?(@.key == 'address1_country')].value")?.ToString();
            Address1StateProvince = jObject.SelectToken(".attributes[?(@.key == 'address1_stateorprovince')].value")?.ToString();

            GenderCode = jObject.SelectToken(".formattedValues[?(@.key == 'gendercode')].value")?.ToString();
            FamilyStatusCode = jObject.SelectToken(".formattedValues[?(@.key == 'familystatuscode')].value")?.ToString();

            LastUpdatedOn = jObject.SelectToken(".attributes[?(@.key == 'msemr_azurefhirlastupdatedon')].value")?.ToString();
            FhirVersion = jObject.SelectToken(".attributes[?(@.key == 'msemr_azurefhirversion')].value")?.ToString();
            Telephone2 = jObject.SelectToken(".attributes[?(@.key == 'telephone2')].value")?.ToString();
            ContactType = jObject.SelectToken(".formattedValues[?(@.key == 'msemr_contacttype')].value")?.ToString();

            // not getting pulled back in json at the moment but requested by the 
            //EmailAddress = jObject.SelectToken(".attributes[?(@.key == 'lastname')].value")?.ToString();
            //MobilePhone = jObject.SelectToken(".attributes[?(@.key == 'firstname')].value")?.ToString();
            //Telephone1 = jObject.SelectToken(".attributes[?(@.key == 'birthdate')].value")?.ToString();
            //GeneralPractioner = jObject.SelectToken(".attributes[?(@.key == 'firstname')].value")?.ToString();
            //Address1PostalCode = jObject.SelectToken(".attributes[?(@.key == 'firstname')].value")?.ToString();
        }

        //"mobilephone",
        //"emailaddress1",
        //"telephone1",
        //"msemr_generalpractioner",
        //"msemr_contacttype",
        //"firstname",
        //"lastname",
        //"msemr_azurefhirid",
        //"telephone2",
        //"gendercode",
        //"birthdate",
        //"address1_city",
        //"address1_line1",
        //"address1_postalcode",
        //"address1_stateorprovince",
        //"address1_country",
        //"familystatuscode",
        //"msemr_azurefhirversion",
        //"msemr_azurefhirlastupdatedon",
        //"accountid"
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string MobilePhone { get; set; }
        public string EmailAddress { get; set; }
        public string Telephone1 { get; set; }
        public string GeneralPractioner { get; set; }
        public string ContactType { get; set; }
        public string Telephone2 { get; set; }
        public string GenderCode { get; set; }
        public string Address1City { get; set; }
        public string Address1Line1 { get; set; }
        public string Address1PostalCode { get; set; }
        public string Address1StateProvince { get; set; }
        public string Address1Country { get; set; }
        public string FamilyStatusCode { get; set; }
        public string FhirVersion { get; set; }
        public string LastUpdatedOn { get; set; }

    }
}
