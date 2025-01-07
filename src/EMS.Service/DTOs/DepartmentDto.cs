using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
