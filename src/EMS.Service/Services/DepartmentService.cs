using EMS.Infrastructure.Data;
using EMS.Service.DTOs;
using EMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace EMS.Service.Services
{
    public class DepartmentService(AppDbContext context) : IDepartmentService
    {
        private readonly AppDbContext _dbContext = context;

        public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync()
        {
            return await _dbContext.Departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name
            }).ToListAsync();
        }
    }
}
