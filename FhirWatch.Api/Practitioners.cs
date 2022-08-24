using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FhirWatch.Api
{
    public class Practitioners
    {
        private readonly ServiceClient _client;
        private readonly IList<string> columnNames;

        public Practitioners(ServiceClient client)
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

        [FunctionName("AllPractitioners")]
        public async Task<IActionResult> GetPractitioners(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "practitioners")] HttpRequest req,
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
                            AttributeName = "msemr_contacttype",
                            Operator = ConditionOperator.Equal,
                            Values = { 935000001 } 
                            /*
                             * Patient - 935000000
                             * Practitioner - 935000001
                             * Related Person - 935000002
                             */
                        }
                    }
                }
            };

            var resp = await _client.RetrieveMultipleAsync(query);
            return new OkObjectResult(resp.Entities);
        }

        [FunctionName("PractitionerById")]
        public async Task<IActionResult> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "practitioners/{fhirId}")] HttpRequest req,
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
            return new OkObjectResult(resp.Entities.FirstOrDefault());
        }
    }
}
