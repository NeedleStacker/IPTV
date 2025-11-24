using IPTVPlayer.Avalonia.Models;
using System.Threading.Tasks;

namespace IPTVPlayer.Avalonia.Services
{
    public class EpgService
    {
        // This is a placeholder for the actual EPG service.
        // When the `mojtv ID kanala.txt` file is available, this service can be implemented
        // to fetch and parse EPG data.
        public async Task<string> GetCurrentProgram(Channel channel)
        {
            // Placeholder: returns a default message.
            await Task.Delay(10); // Simulate async operation
            return "EPG Podaci nisu dostupni";
        }
    }
}
