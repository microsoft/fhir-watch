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

        public async Task<string> GetSamplePatientDataAsync()
        {
            return await http.GetStringAsync("GetContacts");
        }

        public async Task<string> GetPatientByFhirIdAsync(string fhirId)
        {
            return await http.GetStringAsync($"patients/{fhirId}");
        }

        public async Task<string> GetPatients()
        {
            return await http.GetStringAsync($"patients");
        }
    }
}
