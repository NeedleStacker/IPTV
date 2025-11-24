using IPTVPlayer.Avalonia.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace IPTVPlayer.Avalonia.Services
{
    public class SettingsService
    {
        private readonly string _settingsFilePath;

        public SettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "IPTVPlayer.Avalonia");
            Directory.CreateDirectory(appFolder);
            _settingsFilePath = Path.Combine(appFolder, "settings.json");
        }

        public AppSettings LoadSettings()
        {
            if (!File.Exists(_settingsFilePath))
            {
                return new AppSettings { Volume = 50, IsAutoLoadEnabled = false };
            }

            var json = File.ReadAllText(_settingsFilePath);
            return JsonConvert.DeserializeObject<AppSettings>(json);
        }

        public void SaveSettings(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_settingsFilePath, json);
        }
    }
}
