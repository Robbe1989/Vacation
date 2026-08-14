public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public double Fte { get; set; } = 1.0;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public bool UseFte { get; set; } = true; // neu/prüfen
}