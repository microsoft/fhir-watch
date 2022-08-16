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

        public async Task<string> GetSampleContactData()
        {
            return await http.GetStringAsync("GetContacts");
        }
    }
}
