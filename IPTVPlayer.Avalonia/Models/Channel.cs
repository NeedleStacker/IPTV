namespace IPTVPlayer.Avalonia.Models
{
    public class Channel
    {
        public string? TvgId { get; set; }
        public string? TvgName { get; set; }
        public string? TvgLogo { get; set; }
        public string? GroupTitle { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? CurrentProgram { get; set; } // For EPG
    }
}
