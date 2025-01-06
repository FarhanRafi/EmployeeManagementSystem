using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Common
{
    public class SoftDelete
    {
        public bool IsDeleted { get; set; } = false;
    }
}
