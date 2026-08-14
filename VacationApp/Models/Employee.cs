namespace VacationApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Department { get; set; } = "";
        public double Fte { get; set; } = 1.0;
        public DateTime StartDate { get; set; } = DateTime.Today;

        // Neu: ob dieses Employee das FTE-Feld nutzt
        public bool UseFte { get; set; } = true;
    }
}