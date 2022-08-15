using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Hosting.Internal;

namespace FhirWatch.Api
{
    public static class GetContacts
    {
        [FunctionName("GetContacts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var json = File.ReadAllText(context.FunctionAppDirectory + "/Adan632_Brekke496.json");

            JObject data = JObject.Parse(json);

            return new OkObjectResult(data);
        }
    }
}
