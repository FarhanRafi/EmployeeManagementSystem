using EMS.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace EMS.Domain.Entities
{
    public class PerformanceReview : AuditableEntityWithSoftDeletation
    {
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime ReviewDate { get; set; }
        [Range(1, 10, ErrorMessage = "Review score must be between 1 and 10.")]
        public int ReviewScore { get; set; }
        public required string ReviewNotes { get; set; }
    }
}
