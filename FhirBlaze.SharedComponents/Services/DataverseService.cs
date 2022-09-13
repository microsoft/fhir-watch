using Newtonsoft.Json.Linq;
using System;
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
            var results = await http.GetStringAsync($"patients?startLastModified={startLastModified.ToShortDateString()}&endLastModified={endLastModified.ToShortDateString()}");

            if (string.IsNullOrWhiteSpace(results))
                return null;

            return JArray.Parse(results);
        }
    }
}
