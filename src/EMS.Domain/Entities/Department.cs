using EMS.Domain.Common;

namespace EMS.Domain.Entities
{
    public class Department : BaseEntity
    {
        public required string Name { get; set; }
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public decimal Budget { get; set; }

        public ICollection<Employee> Employees { get; set; } = [];
    }
}
