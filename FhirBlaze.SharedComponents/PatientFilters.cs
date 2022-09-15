using System;
using System.Reflection.Metadata.Ecma335;

namespace FhirBlaze.SharedComponents
{
    public class PatientFilters
    {
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-7);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public string FhirId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsSearchable => !string.IsNullOrEmpty(FhirId) || !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName);
    }
}
