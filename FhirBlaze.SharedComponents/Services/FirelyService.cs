using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirBlaze.SharedComponents.Services
{
    public class FirelyService : IFhirService
    {
        private readonly int _defaultPageSize = 50;

        private FhirClient _fhirClient;

        public FirelyService(FhirClient client)
        {
            _fhirClient = client;
        }

        #region Patient
        public async Task<IList<Patient>> GetPatientsAsync()
        {
            var bundle = await _fhirClient.SearchAsync<Patient>(pageSize: 50);

            var result = new List<Patient>();

            while (bundle != null)
            {
                result.AddRange(bundle.Entry.Select(p => (Patient)p.Resource).ToList());
                bundle = await _fhirClient.ContinueAsync(bundle);
            }

            return result;
        }

        public async Task<IList<Patient>> GetPatientsAsync(DateTime startLastModified, DateTime endLastModified)
        {
            return await GetPatientsAsync(new PatientFilters { StartDate = startLastModified, EndDate = endLastModified });
        }

        public async Task<IList<Patient>> GetPatientsAsync(PatientFilters patientFilters)
        {
            Bundle bundle = new();
            var result = new List<Patient>();
            var qryStringFilters = new List<string>();

            if (!string.IsNullOrEmpty(patientFilters.FhirId)) // todo: fix
                qryStringFilters.Add($"_id={patientFilters.FhirId}");
            if (!string.IsNullOrEmpty(patientFilters.LastName))
                qryStringFilters.Add($"family:contains={patientFilters.LastName}");
            if(!string.IsNullOrEmpty(patientFilters.FirstName)) // todo: fix
                qryStringFilters.Add($"given:contains={patientFilters.FirstName}");

            if (!qryStringFilters.Any())
                qryStringFilters.AddRange(new[] {
                $"_lastUpdated=gt{patientFilters.StartDate:yyyy-MM-dd}",
                $"_lastUpdated=lt{patientFilters.EndDate:yyyy-MM-dd}" });

            bundle = await _fhirClient.SearchAsync<Patient>(criteria: qryStringFilters.ToArray(), pageSize: 50);

            while (bundle != null)
            {
                result.AddRange(bundle.Entry.Select(p => (Patient)p.Resource).ToList());
                bundle = await _fhirClient.ContinueAsync(bundle);
            }

            return result;
        }

        public async Task<int> GetPatientCountAsync()
        {
            var bundle = await _fhirClient.SearchAsync<Patient>(summary: SummaryType.Count);
            return bundle.Total ?? 0;
        }

        
        public async Task<IList<Patient>> SearchPatient(Patient Patient)
        {
            string givenName = ""; //The given name is not working on the mapping
            string familyName = Patient.Name[0].Family;
            string identifier = Patient.Identifier[0].Value;
            Bundle bundle;

            if (!string.IsNullOrEmpty(identifier))
            {
                bundle = await _fhirClient.SearchByIdAsync<Patient>(identifier);

                if (bundle != null)
                    return bundle.Entry.Select(p => (Patient)p.Resource).ToList();
            }

            if (!string.IsNullOrEmpty(familyName))
            {
                bundle = await _fhirClient.SearchAsync<Patient>(criteria: new[] { $"family:contains={familyName}" });

                if (bundle != null)
                    return bundle.Entry.Select(p => (Patient)p.Resource).ToList();
            }

            return await GetPatientsAsync();
        }

        public async Task<Patient> CreatePatientAsync(Patient patient)
        {
            return await _fhirClient.CreateAsync(patient);
        }

        public async Task<Patient> UpdatePatientAsync(string patientId, Patient patient)
        {
            if (patientId != patient.Id)
            {
                throw new System.Exception("Unknown patient ID");
            }

            return await _fhirClient.UpdateAsync(patient);
        }
        #endregion        

        #region Practitioners
        public async Task<TResource> GetResourceByIdAsync<TResource>(string resourceId) where TResource : Hl7.Fhir.Model.Resource, new()
        {
            var result = await _fhirClient.SearchByIdAsync<TResource>(resourceId, pageSize: _defaultPageSize);

            TResource r = result.Entry.Select(e => (TResource)e.Resource).First();

            return r;
        }

        public async Task<IList<Practitioner>> GetPractitionersAsync()
        {
            var bundle = await _fhirClient.SearchAsync<Practitioner>(pageSize: 50);
            var result = new List<Practitioner>();
            while (bundle != null)
            {
                result.AddRange(bundle.Entry.Select(p => (Practitioner)p.Resource).ToList());
                bundle = await _fhirClient.ContinueAsync(bundle);
            }

            return result;
        }

        public async Task<int> GetPractitionerCountAsync()
        {
            var bundle = await _fhirClient.SearchAsync<Practitioner>(summary: SummaryType.Count);
            return bundle.Total ?? 0;
        }

        public async Task<IList<Practitioner>> SearchPractitioner(IDictionary<string, string> searchParameters)
        {
            string identifier = searchParameters["identifier"];

            var searchResults = new List<Practitioner>();

            if (!string.IsNullOrEmpty(identifier))
            {
                Bundle bundle = await _fhirClient.SearchByIdAsync<Practitioner>(identifier);

                if (bundle != null)
                    searchResults = bundle.Entry.Select(p => (Practitioner)p.Resource).ToList();
            }
            else
            {
                IList<string> filterStrings = new List<string>();
                foreach (var parameter in searchParameters)
                {
                    if (!string.IsNullOrEmpty(parameter.Value))
                    {
                        filterStrings.Add($"{parameter.Key}:contains={parameter.Value}");
                    }
                }
                Bundle bundle = await _fhirClient.SearchAsync<Practitioner>(criteria: filterStrings.ToArray<string>());

                if (bundle != null)
                    searchResults = bundle.Entry.Select(p => (Practitioner)p.Resource).ToList();
            }

            return searchResults;
        }

        public async Task<Practitioner> CreatePractitionersAsync(Practitioner practitioner)
        {
            return await _fhirClient.CreateAsync(practitioner);
        }

        public async Task<Practitioner> UpdatePractitionerAsync(string practitionerId, Practitioner practitioner)
        {
            if (practitionerId != practitioner.Id)
            {
                throw new Exception("Unknown practitioner ID");
            }

            return await _fhirClient.UpdateAsync(practitioner);
        }
        #endregion
    }
}