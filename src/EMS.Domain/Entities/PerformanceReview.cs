using EMS.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace EMS.Domain.Entities
{
    public class PerformanceReview : AuditableEntity
    {
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime ReviewDate { get; set; }
        [Range(1, 10, ErrorMessage = "Review score should be in between 1 to 10")]
        public int ReviewScore { get; set; }
        public required string ReviewNotes { get; set; }
        public SoftDelete SoftDelete { get; set; } = new();
    }
}
