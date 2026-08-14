using System.Collections.Generic;

namespace VacationApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool UseFte { get; set; } = true;
        public string FteOptionsRaw { get; set; } = "";

        public IEnumerable<(string Label, double Value)> GetFteOptions()
        {
            var list = new List<(string, double)>();
            if (string.IsNullOrWhiteSpace(FteOptionsRaw))
            {
                list.Add(("Vollzeit (100%)", 1.00));
                list.Add(("Halbtags (50%)", 0.50));
                list.Add(("Teilzeit 80% (80%)", 0.80));
                return list;
            }

            var lines = FteOptionsRaw.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2 && double.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val))
                {
                    list.Add((parts[0].Trim(), val));
                }
            }

            if (list.Count == 0)
                list.Add(("Vollzeit (100%)", 1.00));
            return list;
        }
    }
}