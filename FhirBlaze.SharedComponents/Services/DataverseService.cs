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
            return JObject.Parse(result);
        }

        public async Task<JObject> GetPatientByFhirIdAsync(string fhirId, DateTime lastModified)
        {
            var result = await http.GetStringAsync($"patients/{fhirId}?lastModified={lastModified.ToShortDateString()}");
            return JObject.Parse(result);
        }

        public async Task<JArray> GetPatients()
        {
            var results = await http.GetStringAsync($"patients");
            return JArray.Parse(results);
        }

        public async Task<JArray> GetPatients(DateTime lastModified)
        {
            var results = await http.GetStringAsync($"patients?lastModified={lastModified.ToShortDateString()}");
            return JArray.Parse(results);
        }
    }
}
