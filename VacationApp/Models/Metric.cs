using System;

namespace VacationApp.Models
{
    public class Metric
    {
        public int Id { get; set; }
        // internal key, e.g. "fte"
        public string Key { get; set; } = "";
        // display name, e.g. "VZÄ"
        public string DisplayName { get; set; } = "";
        // whether this metric is used globally
        public bool Use { get; set; } = true;
    }
}