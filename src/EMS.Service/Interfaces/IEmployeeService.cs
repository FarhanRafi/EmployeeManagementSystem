using Azure;
using EMS.Domain.Common;
using EMS.Domain.Entities;
using EMS.Service.DTOs;

namespace EMS.Service.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> CreateEmployeeAsync(EmployeeCreationDto employeeDto);
        Task<PaginatedResult<EmployeeDto>> GetEmployeeAsync(int page, int pageSize, string? name, string? position, int departmentId, int minScore, int maxScore);
        Task<EmployeeCreationDto> GetEmployeeByIdAsync(int id);
        Task<Employee> UpdateEmployeeAsync(int id, EmployeeCreationDto employeeDto);
        Task<Employee> DeleteEmployeeAsync(int id);
        
    }
}
