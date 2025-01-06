using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }
}
