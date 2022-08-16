using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Collections;
using System.Collections.Generic;

namespace FhirWatch.Api
{
    public class GetContacts
    {
        private readonly ServiceClient _client;
        private readonly IList<string> columnNames;

        public GetContacts(ServiceClient client)
        {
            _client = client;

            columnNames = new[]
            {
                "mobilephone",
                "emailaddress1",
                "telephone1",
                "msemr_generalpractioner",
                "msemr_contacttype",
                "firstname",
                "lastname",
                "msemr_azurefhirid",
                "telephone2",
                "gendercode",
                "birthdate",
                "address1_city",
                "address1_line1",
                "address1_postalcode",
                "address1_stateorprovince",
                "address1_country",
                "familystatuscode",
                "msemr_azurefhirversion",
                "msemr_azurefhirlastupdatedon",
                "accountid"
            };
        }

        [FunctionName("GetContacts")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var json = await File.ReadAllTextAsync(context.FunctionAppDirectory + "/Adan632_Brekke496.json");

            JObject data = JObject.Parse(json);

            return new OkObjectResult(data);
        }

        [FunctionName("GetContactsById")]
        public async Task<IActionResult> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetContact/{fhirId}")] HttpRequest req,
            string fhirId,
            ILogger log)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = "contact",
                ColumnSet = new ColumnSet(columnNames.ToArray()),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "msemr_azurefhirid",
                            Operator = ConditionOperator.Equal,
                            Values = { fhirId }
                        }
                    }
                }
            };

            var resp = await _client.RetrieveMultipleAsync(query);
            return new OkObjectResult(resp.Entities);
        }
    }
}
