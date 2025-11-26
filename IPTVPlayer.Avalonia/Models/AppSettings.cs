using System.Collections.Generic;

namespace IPTVPlayer.Avalonia.Models
{
    public class AppSettings
    {
        public string? LastM3uPath { get; set; }
        public int Volume { get; set; }
        public bool IsAutoLoadEnabled { get; set; }
        public double ButtonScale { get; set; } = 1.0;
        public List<string> CategoryOrder { get; set; } = new List<string>();
    }
}
