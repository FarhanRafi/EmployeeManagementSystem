using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.DTOs
{
    public class EmployeeCreationDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required string Position { get; set; }
        public DateTime JoiningDate { get; set; }
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
