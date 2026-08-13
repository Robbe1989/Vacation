using System;

namespace VacationApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public double Fte { get; set; } = 1.0;
    }
}
