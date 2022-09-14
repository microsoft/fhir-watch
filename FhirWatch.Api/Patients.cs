using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FhirWatch.Api
{
    public class Patients
    {
        private readonly ServiceClient _client;
        private readonly IList<string> columnNames;

        public Patients(ServiceClient client)
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

        [FunctionName("AllPatients")]
        public async Task<IActionResult> GetPatients(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patients")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var startLastModifiedDT = string.IsNullOrEmpty(req.Query["startDate"]) ?
                    DateTime.MinValue : DateTime.Parse(req.Query["startDate"]);
                var endLastModifiedDT = string.IsNullOrEmpty(req.Query["endDate"]) ?
                    DateTime.UtcNow : DateTime.Parse(req.Query["endDate"]);

                var fhirId = req.Query["fhirId"].ToString();
                var firstName = req.Query["firstName"].ToString();
                var lastName = req.Query["lastName"].ToString();

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
                            Values = { 935000000 } 
                            /*
                             * Patient - 935000000
                             * Practitioner - 935000001
                             * Related Person - 935000002
                             */
                        }
                    }
                    }
                };

                if (!string.IsNullOrEmpty(fhirId))
                {
                    query.Criteria.AddCondition(new ConditionExpression
                    {
                        AttributeName = "msemr_azurefhirid",
                        Operator = ConditionOperator.Equal,
                        Values = { fhirId }
                    });
                }

                if (!string.IsNullOrEmpty(firstName))
                {
                    query.Criteria.AddCondition(new ConditionExpression
                    {
                        AttributeName = "firstname",
                        Operator = ConditionOperator.Like,
                        Values = { firstName }
                    });
                }

                if (!string.IsNullOrEmpty(lastName))
                {
                    query.Criteria.AddCondition(new ConditionExpression
                    {
                        AttributeName = "lastname",
                        Operator = ConditionOperator.Like,
                        Values = { lastName }
                    });
                }

                if (query?.Criteria?.Conditions?.Count <= 1 || startLastModifiedDT > DateTime.MinValue)
                {
                    query.Criteria.AddCondition(new ConditionExpression
                    {
                        AttributeName = "msemr_azurefhirlastupdatedon",
                        Operator = ConditionOperator.OnOrAfter,
                        Values = { startLastModifiedDT }
                    });

                    if (endLastModifiedDT > DateTime.UtcNow)
                    {
                        query.Criteria.AddCondition(new ConditionExpression
                        {
                            AttributeName = "msemr_azurefhirlastupdatedon",
                            Operator = ConditionOperator.OnOrBefore,
                            Values = { endLastModifiedDT }
                        });
                    }
                }

                var resp = await _client.RetrieveMultipleAsync(query);

                return new OkObjectResult(resp.Entities);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FunctionName("PatientById")]
        public async Task<IActionResult> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patients/{fhirId}")] HttpRequest req,
            string fhirId,
            ILogger log)
        {
            var startLastModifiedDT = string.IsNullOrEmpty(req.Query["startLastModified"]) ?
                DateTime.MinValue : DateTime.Parse(req.Query["startLastModified"]);
            var endLastModifiedDT = string.IsNullOrEmpty(req.Query["endLastModified"]) ?
                DateTime.UtcNow : DateTime.Parse(req.Query["endLastModified"]);

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

            if (startLastModifiedDT > DateTime.MinValue)
            {
                query.Criteria.AddCondition(new ConditionExpression
                {
                    AttributeName = "msemr_azurefhirlastupdatedon",
                    Operator = ConditionOperator.OnOrAfter,
                    Values = { startLastModifiedDT }
                });
            }

            if (endLastModifiedDT > DateTime.UtcNow)
            {
                query.Criteria.AddCondition(new ConditionExpression
                {
                    AttributeName = "msemr_azurefhirlastupdatedon",
                    Operator = ConditionOperator.OnOrBefore,
                    Values = { endLastModifiedDT }
                });
            }

            var resp = await _client.RetrieveMultipleAsync(query);

            return new OkObjectResult(resp.Entities.FirstOrDefault());
        }
    }
}
