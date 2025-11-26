using IPTVPlayer.Avalonia.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPTVPlayer.Avalonia.Services
{
    public class M3uService
    {
        public async Task<List<Channel>> ParseM3u(string filePath)
        {
            string content;
            if (filePath.StartsWith("http"))
            {
                using var httpClient = new HttpClient();
                content = await httpClient.GetStringAsync(filePath);
            }
            else
            {
                // check if file exists
                if (!File.Exists(filePath))
                {
                    return new List<Channel>();
                }
                content = await File.ReadAllTextAsync(filePath);
            }

            var channels = new List<Channel>();
            var lines = content.Split('\n');
            Channel? currentChannel = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("#EXTINF"))
                {
                    currentChannel = new Channel();
                    var match = Regex.Match(line, "tvg-id=\"(.*?)\"");
                    if (match.Success) currentChannel.TvgId = match.Groups[1].Value;

                    match = Regex.Match(line, "tvg-name=\"(.*?)\"");
                    if (match.Success) currentChannel.TvgName = match.Groups[1].Value;

                    match = Regex.Match(line, "tvg-logo=\"(.*?)\"");
                    if (match.Success) currentChannel.TvgLogo = match.Groups[1].Value;

                    match = Regex.Match(line, "group-title=\"(.*?)\"");
                    if (match.Success) currentChannel.GroupTitle = match.Groups[1].Value;

                    var nameMatch = Regex.Match(line, ",(.+)$");
                    if (nameMatch.Success) currentChannel.Name = nameMatch.Groups[1].Value.Trim();
                }
                else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#") && currentChannel != null)
                {
                    currentChannel.Url = line.Trim();
                    if(!string.IsNullOrWhiteSpace(currentChannel.Url))
                    {
                        channels.Add(currentChannel);
                    }
                    currentChannel = null;
                }
            }

            return channels;
        }
    }
}
