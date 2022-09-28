using Hl7.Fhir.Model;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace FhirWatch.PatientModule.Models
{
    public class PatientViewModel
    {
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
        // 
        // todo: eventually map based on configuration provided from sync agent which has json path for fhir objects saved

        public PatientViewModel(Patient patient)
        {
            Id = patient.Id;
            var officialName = patient.Name.FirstOrDefault(n => n.Use == HumanName.NameUse.Official);
            FirstName = officialName?.Given?.FirstOrDefault();
            LastName = officialName?.Family;
            DateOfBirth = patient.BirthDate;
            Address1Line1 = patient.Address.FirstOrDefault()?.Line?.FirstOrDefault();
            Address1City = patient.Address.FirstOrDefault()?.City;
            Address1Country = patient.Address.FirstOrDefault()?.Country;
            Address1StateProvince = patient.Address.FirstOrDefault()?.State;
            GenderCode = patient.Gender.ToString();
            FamilyStatusCode = patient.MaritalStatus?.Text;
            LastUpdatedOn = patient.Meta.LastUpdated.ToString();
            FhirVersion = patient.VersionId.ToString();
            Telephone2 = patient.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone && t.Use == ContactPoint.ContactPointUse.Home)?.Value;

            // doesn't seem to be mapped?
            //ContactType = patient.Contact
        }

        public PatientViewModel(JToken patientJson)
        {
            var jObject = JObject.Parse(patientJson.ToString());

            Id = jObject.SelectToken(".attributes[?(@.key == 'msemr_azurefhirid')].value")?.ToString();
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
            EmailAddress = jObject.SelectToken(".attributes[?(@.key == 'emailaddress1')].value")?.ToString();
            MobilePhone = jObject.SelectToken(".attributes[?(@.key == 'mobilephone')].value")?.ToString();
            Telephone1 = jObject.SelectToken(".attributes[?(@.key == 'telephone1')].value")?.ToString();
            GeneralPractioner = jObject.SelectToken(".attributes[?(@.key == 'msemr_generalpractioner')].value")?.ToString();
            Address1PostalCode = jObject.SelectToken(".attributes[?(@.key == 'address1_postalcode')].value")?.ToString();
        }

    }

    public class PatientCompareModel
    {
        public string Id { get; set; }
        public PatientViewModel Item1 { get; set; }
        public PatientViewModel Item2 { get; set; }
        public bool IsSelected { get; set; } = false;

        public PatientCompareModel(string id, PatientViewModel item1, PatientViewModel item2)
        {
            Id = id;
            Item1 = item1;
            Item2 = item2;
        }

    }
}
