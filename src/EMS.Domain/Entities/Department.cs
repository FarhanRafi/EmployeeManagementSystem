using EMS.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Domain.Entities
{
    public class Department : BaseEntity
    {
        public required string Name { get; set; }
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; }

        public ICollection<Employee> Employees { get; set; } = [];
    }
}
