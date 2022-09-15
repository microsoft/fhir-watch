using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FhirBlaze.SharedComponents.Services
{
    public class DataverseService
    {
        private readonly HttpClient http;

        public DataverseService(HttpClient http)
        {
            this.http = http;
        }

        /// <summary>
        /// Get Patient by FHIR Id.
        /// </summary>
        /// <param name="fhirId"></param>
        /// <returns></returns>
        public async Task<JObject> GetPatientByFhirIdAsync(string fhirId)
        {
            var result = await http.GetStringAsync($"patients/{fhirId}");

            if (string.IsNullOrWhiteSpace(result))
                return null;

            return JObject.Parse(result);
        }

        /// <summary>
        /// Get count of all Patients.
        /// </summary>
        /// <returns>int</returns>
        public async Task<int> GetPatientCount()
        {
            var jsonResult = await http.GetStringAsync("patientcount");

            if (string.IsNullOrWhiteSpace(jsonResult))
                return 0;

            var result = JObject.Parse(jsonResult);
            return result.Value<int>("count");
        }

        /// <summary>
        /// Get a list of all Patients.
        /// </summary>
        /// <returns></returns>
        public async Task<JArray> GetPatientsAsync()
        {
            var results = await http.GetStringAsync($"patients");

            if (string.IsNullOrWhiteSpace(results))
                return null;

            return JArray.Parse(results);
        }

        /// <summary>
        /// Get a list of all Patients where lastModified date is between startLastModified and endLastModified.
        /// </summary>
        /// <param name="startLastModified"></param>
        /// <param name="endLastModified"></param>
        /// <returns></returns>
        public async Task<JArray> GetPatientsAsync(DateTime startLastModified, DateTime endLastModified)
        {
            return await GetPatientsAsync(new PatientFilters { StartDate = startLastModified, EndDate = endLastModified });
        }

        /// <summary>
        /// Get a list of all Patients matching <see cref="PatientFilters"/> criteria.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<JArray> GetPatientsAsync(PatientFilters filters)
        {
            var queryStr = "patients";

            var qryStringFilters = new List<string>();

            if (!string.IsNullOrEmpty(filters.FhirId)) // todo: fix
                qryStringFilters.Add($"fhirId={filters.FhirId}");
            if (!string.IsNullOrEmpty(filters.LastName))
                qryStringFilters.Add($"lastName={filters.LastName}");
            if (!string.IsNullOrEmpty(filters.FirstName)) // todo: fix
                qryStringFilters.Add($"firstName={filters.FirstName}");

            if (!qryStringFilters.Any())
                qryStringFilters.AddRange(new[] {
                $"startDate={filters.StartDate:yyyy-MM-dd}",
                $"endDate={filters.EndDate:yyyy-MM-dd}" });

            if (qryStringFilters.Any())
                queryStr = queryStr + "?" + string.Join('&', qryStringFilters.ToArray());

            var results = await http.GetStringAsync(queryStr);

            if (string.IsNullOrWhiteSpace(results))
                return null;

            return JArray.Parse(results);
        }
    }
}
