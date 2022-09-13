using System;

namespace FhirBlaze.SharedComponents
{
    public class PatientFilters
    {
        public DateTime FilterStartDate { get; set; } = DateTime.UtcNow.AddDays(-7);
        public DateTime FilterEndDate { get; set; } = DateTime.UtcNow;
        public string FhirId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
