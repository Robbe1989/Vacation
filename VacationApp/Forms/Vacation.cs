using System;

namespace VacationApp.Models
{
    public class Vacation
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
        public int VacationTypeId { get; set; } = 1;  // Standard Urlaubstyp
        public string Comment { get; set; } = "";

        public int Days => (EndDate - StartDate).Days + 1;
    }
}