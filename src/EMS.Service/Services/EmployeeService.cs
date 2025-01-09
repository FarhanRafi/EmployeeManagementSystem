using EMS.Domain.Common;
using EMS.Domain.Entities;
using EMS.Infrastructure.Data;
using EMS.Service.DTOs;
using EMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMS.Service.Services
{
    public partial class EmployeeService(AppDbContext context) : IEmployeeService
    {
        private readonly AppDbContext _dbContext = context;

        public async Task<Employee> CreateEmployeeAsync(EmployeeCreationDto employeeDto)
        {
            ValidateEmployeeDto(employeeDto);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var employee = new Employee
                {
                    Name = employeeDto.Name,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone,
                    Position = employeeDto.Position,
                    JoiningDate = employeeDto.JoiningDate.ToUniversalTime(),
                    DepartmentId = employeeDto.DepartmentId,
                    IsActive = employeeDto.IsActive,
                };

                await _dbContext.Employees.AddAsync(employee);

                var department = await _dbContext.Departments.FindAsync(employeeDto.DepartmentId) ?? throw new InvalidOperationException("Invalid department.");
                employee.Department = department;

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return employee; 
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating the employee.", ex);
            }
        }

        public async Task<PaginatedResult<EmployeeDto>> GetEmployeeAsync(int page, int pageSize, string? name, string? position, int departmentId, int minScore, int maxScore)
        {
            var query = _dbContext.Employees
                .Include(x => x.Department)
                .Include(x => x.PerformanceReviews)
                .AsQueryable();
            var totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(e => e.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));

                // query = query.Where(e => EF.Functions.FreeText(e.Name, name)); 
                // For full-text search: 
                // 1. Enable Full-Text Search in DB (catalog + index). 
                // 2. Ensure SQL Server supports it. Dev edition doesn't support 
                // 3. Use EF.Functions.FreeText/Contains in LINQ.
            }
            if (!string.IsNullOrWhiteSpace(position))
            {
                query = query.Where(e => e.Position.Contains(position));
            }
            if (departmentId > 0)
            {
                query = query.Where(e => e.DepartmentId == departmentId);
            }

            if (minScore > 0)
            {
                query = query.Where(e => e.PerformanceReviews.Count >= minScore);
            }
            if (maxScore > 0)
            {
                query = query.Where(e => e.PerformanceReviews.Count <= maxScore);
            }

            var filteredRecordCount = await query.CountAsync();

            var employeeRawData = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(e => new
            {
                e.Id,
                e.Name,
                e.Email,
                Department = e.Department.Name,
                Phone = e.Phone ?? e.Phone,
                e.JoiningDate,
                e.Position,
                e.PerformanceReviews,
                Status = e.IsActive
            }).ToListAsync();

            var employees = employeeRawData.Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                Department = e.Department,
                Phone = e.Phone,
                JoiningDate = e.JoiningDate.ToLocalTime().ToString("dd-MMM-yyyy"),
                Position = e.Position,
                Status = e.Status,
                PerformanceScore = e.PerformanceReviews.Count > 0 ? e.PerformanceReviews.Average(pr => pr.ReviewScore) : 0,
            }).ToList();

            return new PaginatedResult<EmployeeDto>
            {
                Data = employees,
                TotalRecords = totalCount,
                TotalFilteredRecords = filteredRecordCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<EmployeeCreationDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);

            return employee is null
                ? throw new ArgumentNullException(nameof(id), "Employee not found")
                : new EmployeeCreationDto
                {
                    Name = employee.Name,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    Position = employee.Position,
                    JoiningDate = employee.JoiningDate.ToLocalTime(),
                    DepartmentId = employee.DepartmentId,
                    IsActive = employee.IsActive
                };
        }

        public async Task<Employee> UpdateEmployeeAsync(int id, EmployeeCreationDto employeeDto)
        {
            ValidateEmployeeDto(employeeDto);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var employee = await _dbContext.Employees.FindAsync(id)
                    ?? throw new KeyNotFoundException($"Employee with ID {id} not found.");

                employee.Name = employeeDto.Name;
                employee.Email = employeeDto.Email;
                employee.Phone = employeeDto.Phone;
                employee.Position = employeeDto.Position;
                employee.JoiningDate = employeeDto.JoiningDate.ToUniversalTime();
                employee.DepartmentId = employeeDto.DepartmentId;
                employee.IsActive = employeeDto.IsActive;

                var department = await _dbContext.Departments.FindAsync(employeeDto.DepartmentId)
                    ?? throw new InvalidOperationException("Invalid department.");
                employee.Department = department;

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return employee;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating the employee.", ex);
            }
        }

        public async Task<Employee> DeleteEmployeeAsync(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id) ?? throw new ArgumentNullException(nameof(id), "Employee not found");

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();

            return employee;
        }

        private static void ValidateEmployeeDto(EmployeeCreationDto employeeDto)
        {
            if (string.IsNullOrWhiteSpace(employeeDto.Name))
                throw new ArgumentException("Employee name is required.");

            if (string.IsNullOrWhiteSpace(employeeDto.Email) || !IsValidEmail(employeeDto.Email))
                throw new ArgumentException("Invalid email address.");

            if (employeeDto.DepartmentId <= 0)
                throw new ArgumentException("Department ID is required and must be valid.");
        }
        private static bool IsValidEmail(string email)
        {
            var emailRegex = MyRegex();
            return emailRegex.IsMatch(email);
        }

        [System.Text.RegularExpressions.GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial System.Text.RegularExpressions.Regex MyRegex();
    }
}
