using EMS.Domain.Common;

namespace EMS.Domain.Entities
{
    public class Employee : AuditableEntity
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required string Position { get; set; }
        public DateTime JoiningDate { get; set; }
        public  int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public bool IsActive { get; set; } = true;

        private SoftDelete softDelete = new();
        public SoftDelete SoftDelete { get => softDelete; set => softDelete = value; }

        public ICollection<PerformanceReview> PerformanceReviews { get; set; } = [];
    }
}
