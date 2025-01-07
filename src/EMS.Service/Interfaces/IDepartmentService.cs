using EMS.Service.DTOs;

namespace EMS.Service.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(); // for id, name
    }
}
