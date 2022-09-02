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

        public async Task<JObject> GetPatientByFhirIdAsync(string fhirId)
        {
            var result = await http.GetStringAsync($"patients/{fhirId}");

            if (string.IsNullOrWhiteSpace(result))
                return null;

            return JObject.Parse(result);
        }

        public async Task<JObject> GetPatientByFhirIdAsync(string fhirId, DateTime lastModified)
        {
            var result = await http.GetStringAsync($"patients/{fhirId}?lastModified={lastModified.ToShortDateString()}");

            if (string.IsNullOrWhiteSpace(result))
                return null;

            return JObject.Parse(result);
        }

        public async Task<JArray> GetPatients()
        {
            var results = await http.GetStringAsync($"patients");

            if (string.IsNullOrWhiteSpace(results))
                return null;

            return JArray.Parse(results);
        }

        public async Task<JArray> GetPatients(DateTime lastModified)
        {
            var results = await http.GetStringAsync($"patients?lastModified={lastModified.ToShortDateString()}");

            if (string.IsNullOrWhiteSpace(results))
                return null;

            return JArray.Parse(results);
        }
    }
}
