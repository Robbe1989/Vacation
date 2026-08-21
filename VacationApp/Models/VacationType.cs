using System;

namespace VacationApp.Models
{
    public class VacationType
    {
        public int Id { get; set; }
        public string Abbreviation { get; set; } = ""; // z.B. "U" für Urlaub, "K" für Krank
        public string Name { get; set; } = "";          // z.B. "Urlaub", "Krankheit"
    }
}