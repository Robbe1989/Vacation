using System;
using System.Drawing;

namespace VacationApp.Models
{
    public class VacationType
    {
        public int Id { get; set; }
        public string Abbreviation { get; set; } = ""; // z.B. "U" für Urlaub, "K" für Krank
        public string Name { get; set; } = "";          // z.B. "Urlaub", "Krankheit"
        public string ColorHex { get; set; } = "#FFA500"; // Farbe als Hex-String (default: Orange)

        public Color GetColor()
        {
            try
            {
                return ColorTranslator.FromHtml(ColorHex);
            }
            catch
            {
                return Color.Orange; // Fallback
            }
        }
    }
}