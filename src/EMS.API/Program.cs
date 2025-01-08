using EMS.Domain.Entities;
using EMS.Infrastructure.Data;
using EMS.Service.DTOs;
using EMS.Service.Interfaces;
using EMS.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")  
              .AllowAnyHeader()                   
              .AllowAnyMethod();                  
    });
});

var app = builder.Build();

app.UseCors("AllowLocalhostFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Department APIs

app.MapGet("/api/departments", async (IDepartmentService departmentService) =>
{
    var departments = await departmentService.GetDepartmentsAsync();
    return Results.Ok(new { Data = departments });
});

#endregion

#region Employee APIs

app.MapPost("/api/employees", async (IEmployeeService employeeService, [FromBody] EmployeeCreationDto employeeDto) =>
{
    try
    {
        var employee = await employeeService.CreateEmployeeAsync(employeeDto);

        var location = $"/api/employees/{employee.Id}";
        return Results.Created(location, new { employee.Id, employee.Name, employee.Email });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (ApplicationException)
    {
        return Results.Problem("An error occurred while creating the employee. Please try again.");
    }
}).WithName("CreateEmployee");

app.MapGet("api/employees", async (
    IEmployeeService employeeService, 
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? name = null,
    [FromQuery] string? position = null,
    [FromQuery] int departmentId = 0,
    [FromQuery] int minScore = 0,
    [FromQuery] int maxScore = 0) =>
{
    var employees = await employeeService.GetEmployeeAsync(page, pageSize, name, position, departmentId, minScore, maxScore);
    return Results.Ok(new 
    { 
        recordsTotal = employees.TotalRecords, 
        recordsFiltered = employees.TotalFilteredRecords, 
        data = employees.Data
    });
});

#endregion


await app.RunAsync();

