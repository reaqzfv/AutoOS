using Microsoft.Win32;
using System.Text.Json;

namespace AutoOS.Helpers
{
    public static class AmdHelper
    {
        private static readonly HttpClient httpClient = new();

        public static string GetCurrentVersion()
        {
            using var baseKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}");
            foreach (var subKeyName in baseKey.GetSubKeyNames())
            {
                using var subKey = baseKey.OpenSubKey(subKeyName);
                if (subKey.GetValue("ProviderName") is string provider && provider.Contains("Advanced Micro Devices"))
                {
                    return (string)subKey.GetValue("RadeonSoftwareVersion");
                }
            }

            return null;
        }
        public static async Task<(string currentVersion, string newestVersion, string newestDownloadUrl)> CheckUpdate()
        {
            string currentVersion = GetCurrentVersion();

            httpClient.DefaultRequestHeaders.Referrer = new Uri("http://support.amd.com");

            string json = await httpClient.GetStringAsync("https://drivers.amd.com/drivers/installer/json/DrvDldDetails_Consumer_WHQL_Win10.json");
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement[0];

            string newestVersion = root.GetProperty("externalbuildversion").GetString();
            string newestDownloadUrl = root.GetProperty("fullbuild").GetString().Replace("www2.ati.com", "drivers.amd.com").Replace("-combined", "");

            return (currentVersion, newestVersion, newestDownloadUrl);
        }
    }
}