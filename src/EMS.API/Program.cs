using EMS.Domain.Entities;
using EMS.Infrastructure.Data;
using EMS.Service.DTOs;
using EMS.Service.Interfaces;
using EMS.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Text.Json;

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

// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();

        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            context.Response.ContentType = "application/json";
            var response = new { Error = "Resource not found" };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
    catch (Exception ex)
    {
        // Log the exception
        Console.WriteLine($"Error: {ex.GetType().Name} - {ex.Message}");

        // standardized error response
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            AccessViolationException => StatusCodes.Status500InternalServerError,
            ArgumentNullException => StatusCodes.Status400BadRequest,
            ArgumentOutOfRangeException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            NullReferenceException => StatusCodes.Status500InternalServerError,
            FormatException => StatusCodes.Status400BadRequest,
            SqlException => StatusCodes.Status500InternalServerError,
            TimeoutException => StatusCodes.Status500InternalServerError,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            FileNotFoundException => StatusCodes.Status404NotFound,
            DirectoryNotFoundException => StatusCodes.Status404NotFound,
            OperationCanceledException => StatusCodes.Status408RequestTimeout,
            FileLoadException => StatusCodes.Status500InternalServerError,
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            JsonException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new
        {
            Error = ex switch
            {
                ApplicationException => ex.Message,
                AccessViolationException => "Access violation error occurred.",
                ArgumentNullException => "Required argument is missing.",
                ArgumentOutOfRangeException => "Argument out of valid range.",
                ArgumentException => "Invalid argument provided.",
                InvalidOperationException => "Invalid operation performed.",
                NullReferenceException => "An unexpected error occurred.",
                FormatException => "Input has an invalid format.",
                SqlException => "Database error occurred. Please try again later.",
                TimeoutException => "The operation timed out.",
                KeyNotFoundException => "Resource not found.",
                FileNotFoundException => "File not found.",
                DirectoryNotFoundException => "Directory not found.",
                OperationCanceledException => "The operation was canceled.",
                FileLoadException => "Unable to load the file.",
                BadHttpRequestException => "Malformed HTTP request.",
                NotImplementedException => "The requested feature is not implemented.",
                DbUpdateConcurrencyException => "Concurrency conflict occurred. Please try again.",
                JsonException => "Failed to parse JSON data.",
                _ => "An unexpected error occurred. Please try again later."
            }
        };

        await context.Response.WriteAsJsonAsync(response);
    }
});

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

app.MapGet("api/employees/{id:int}", async(IEmployeeService employeeService, int id) =>
{
    var employee = await employeeService.GetEmployeeByIdAsync(id);
    return Results.Ok(new { id, employee } );
});

app.MapPut("/api/employees/{id:int}", async (IEmployeeService employeeService, int id, [FromBody] EmployeeCreationDto employeeDto) =>
{
    try
    {
        var updatedEmployee = await employeeService.UpdateEmployeeAsync(id, employeeDto);
        return Results.Ok (updatedEmployee);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

#endregion


await app.RunAsync();

