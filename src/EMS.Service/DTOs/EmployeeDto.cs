namespace EMS.Service.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required string Position { get; set; }
        public required string JoiningDate { get; set; }
        public required string Department { get; set; }
        public bool Status { get; set; }
        public double PerformanceScore { get; set; }
    }
}
