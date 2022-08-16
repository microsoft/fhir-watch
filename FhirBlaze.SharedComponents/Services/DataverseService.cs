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

        public async Task<string> GetSampleContactDataAsync()
        {
            return await http.GetStringAsync("GetContacts");
        }

        public async Task<string> GetContactByFhirIdAsync(string fhirId)
        {
            return await http.GetStringAsync($"GetContact/{fhirId}");
        }
    }
}
