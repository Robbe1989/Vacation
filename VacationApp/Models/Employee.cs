using System;

namespace VacationApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Department { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Today;

        // Neu: Urlaubstage pro Mitarbeiter (Ganzzahl)
        public int VacationDays { get; set; } = 20;
    }
}