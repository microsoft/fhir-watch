using System;
using System.Reflection.Metadata.Ecma335;

namespace FhirWatch.SharedComponents
{
    public class PatientFilters
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FhirId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsSearchable => !string.IsNullOrEmpty(FhirId) || !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName);
    }
}
