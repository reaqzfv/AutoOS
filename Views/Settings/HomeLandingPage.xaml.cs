using AutoOS.Views.Installer.Actions;
using CommunityToolkit.WinUI.Controls;
using Downloader;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Microsoft.Win32;
using System.Management;
using System.Net;
using System.Text;
using System.Text.Json;
using Windows.Storage;

namespace AutoOS.Views.Settings
{
    public sealed partial class HomeLandingPage : Page
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static readonly HttpClient httpClient = new();
        private readonly TextBlock StatusText = new()
        {
            Margin = new Thickness(0, 10, 0, 0),
            FontSize = 14,
            FontWeight = FontWeights.Medium
        };

        private readonly ProgressBar ProgressBar = new()
        {
            Margin = new Thickness(0, 15, 0, 0)
        };
        public HomeLandingPage()
        {
            InitializeComponent();
            Loaded += GetChangeLog;
        }

        private async void GetChangeLog(object sender, RoutedEventArgs e)
        {
            string storedVersion = localSettings.Values["Version"] as string;
            string currentVersion = ProcessInfoHelper.Version;

            if (storedVersion != currentVersion)
            {
                try
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AutoOS");

                    using var doc = JsonDocument.Parse(await httpClient.GetStringAsync($"https://api.github.com/repos/tinodin/AutoOS/releases/tags/v{currentVersion}"));

                    if (doc.RootElement.TryGetProperty("body", out var body))
                    {
                        string rawChangelog = body.GetString()!;
                        string changelog = rawChangelog.Replace("`", "")[rawChangelog.IndexOf("- ")..];

                        var contentDialog = new ContentDialog
                        {
                            Title = $"What’s new in AutoOS v{currentVersion}",
                            Content = new ScrollViewer
                            {
                                Content = new MarkdownTextBlock
                                {
                                    Text = changelog,
                                    Config = new MarkdownConfig()
                                },
                                Padding = new Thickness(0, 0, 36, 0)
                            },
                            CloseButtonText = "Close",
                            XamlRoot = XamlRoot
                        };

                        contentDialog.Resources["ContentDialogMaxWidth"] = 1000;
                        await contentDialog.ShowAsync();
                    }
                }
                catch
                {

                }

                await Update();
                StatusText.Text = "Update complete.";
                ProgressBar.Foreground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemFillColorSuccess"]);
                localSettings.Values["Version"] = currentVersion;
                await LogDiscordUser();
            }
        }

        private async Task Update()
        {
            var updater = new ContentDialog
            {
                Title = "Updating AutoOS",
                Content = new StackPanel
                {
                    Children =
                    {
                        StatusText,
                        ProgressBar
                    }
                },
                PrimaryButtonText = "Done",
                IsPrimaryButtonEnabled = false,
                Resources = new ResourceDictionary
                {
                    ["ContentDialogMinWidth"] = 500,
                    ["ContentDialogMaxWidth"] = 1000
                },
                XamlRoot = XamlRoot
            };

            _ = updater.ShowAsync();

            string previousTitle = string.Empty;

            bool AlwaysShowTrayIcons = (localSettings.Values["AlwaysShowTrayIcons"]?.ToString() == "1");
            string ScheduleMode = localSettings.Values["ScheduleMode"]?.ToString();
            string LightTime = localSettings.Values["LightTime"]?.ToString();
            string DarkTime = localSettings.Values["DarkTime"]?.ToString();

            string scheduleMode = ScheduleMode switch
            {
                "Sunset to sunrise" => "LocationService",
                "Custom hours" => "CustomHours",
                _ => ScheduleMode
            };

            var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
            {
                // stop windhawk
                ("Stopping Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc stop Windhawk"), null),
                ("Stopping Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c taskkill /f /im Windhawk.exe"), null),
                ("Stopping Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c rmdir /s /q ""C:\Program Files\Windhawk"""), null),
                ("Stopping Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c rmdir /s /q ""C:\ProgramData\Windhawk"""), null),
                ("Stopping Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Windhawk"" /f"), null),
                ("Stopping Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Windhawk /f"), null),

                // download windhawk
                ("Downloading Windhawk", async () => await RunDownload("https://www.dl.dropboxusercontent.com/scl/fi/omk2gg29v8yguskw4jhng/Windhawk.zip?rlkey=tljvtfus2tq57d3y5mzdt8ges&st=5h7z80ir&dl=0", Path.GetTempPath(), "Windhawk.zip"), null),
        
                // update windhawk
                ("Updating Windhawk", async () => await ProcessActions.RunExtract(Path.Combine(Path.GetTempPath(), "Windhawk.zip"), @"C:\Program Files\Windhawk"), null),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c robocopy ""C:\Program Files\Windhawk\Windhawk"" ""C:\ProgramData\Windhawk"" /MOVE /E"), null),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("CurrentUser", $"cmd /c reg import \"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Scripts", "windhawk.reg")}\""), null),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", $@"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Windhawk\Engine\Mods\auto-theme-switcher"" /v Disabled /t REG_DWORD /d 1 /f"), () => ScheduleMode == "Always Light" || ScheduleMode == "Always Dark"),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", $@"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Windhawk\Engine\Mods\auto-theme-switcher\Settings"" /v ScheduleMode /t REG_SZ /d {scheduleMode} /f"), null),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", $@"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Windhawk\Engine\Mods\auto-theme-switcher\Settings"" /v CustomLight /t REG_SZ /d {LightTime} /f"), null),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", $@"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Windhawk\Engine\Mods\auto-theme-switcher\Settings"" /v CustomDark /t REG_SZ /d {DarkTime} /f"), null),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Windhawk\Engine\Mods\taskbar-notification-icons-show-all"" /v Disabled /t REG_DWORD /d 1 /f"), () => AlwaysShowTrayIcons == false),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc create Windhawk binPath= ""\""C:\Program Files\Windhawk\windhawk.exe\"" -service"" start= auto"), null),
                ("Updating Windhawk", async () => await ProcessActions.RunPowerShell(@"$s=New-Object -ComObject WScript.Shell;$sc=$s.CreateShortcut([IO.Path]::Combine($env:APPDATA,'Microsoft\Windows\Start Menu\Programs\Windhawk.lnk'));$sc.TargetPath='C:\Program Files\Windhawk\windhawk.exe';$sc.Save()"), null),
                ("Updating Windhawk", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc start Windhawk"), null),
            };

            var filteredActions = actions.Where(a => a.Condition == null || a.Condition.Invoke()).ToList();
            int groupedTitleCount = 0;

            List<Func<Task>> currentGroup = [];

            for (int i = 0; i < filteredActions.Count; i++)
            {
                if (i == 0 || filteredActions[i].Title != filteredActions[i - 1].Title)
                {
                    groupedTitleCount++;
                }
            }

            double incrementPerTitle = groupedTitleCount > 0 ? 100 / (double)groupedTitleCount : 0;

            foreach (var (title, action, condition) in filteredActions)
            {
                if (previousTitle != string.Empty && previousTitle != title && currentGroup.Count > 0)
                {
                    foreach (var groupedAction in currentGroup)
                    {
                        try
                        {
                            await groupedAction();
                        }
                        catch (Exception ex)
                        {
                            StatusText.Text = ex.Message;
                            ProgressBar.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                        }
                    }

                    ProgressBar.Value += incrementPerTitle;
                    await Task.Delay(150);
                    currentGroup.Clear();
                }

                StatusText.Text = title + "...";
                currentGroup.Add(action);
                previousTitle = title;
            }

            if (currentGroup.Count > 0)
            {
                foreach (var groupedAction in currentGroup)
                {
                    try
                    {
                        await groupedAction();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = ex.Message;
                        ProgressBar.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                    }
                }
                ProgressBar.Value += incrementPerTitle;
            }

            updater.IsPrimaryButtonEnabled = true;
        }

        public async Task RunDownload(string url, string path, string file = null)
        {
            string title = StatusText.Text;

            var uiContext = SynchronizationContext.Current;

            DownloadBuilder downloadBuilder;

            if (url.Contains("raw.githubusercontent.com", StringComparison.OrdinalIgnoreCase))
            {
                using var client = new HttpClient();
                await File.WriteAllTextAsync(string.IsNullOrWhiteSpace(file) ? path : Path.Combine(path, file), await client.GetStringAsync(url), Encoding.UTF8);
                return;
            }
            else if (url.Contains("drivers.amd.com", StringComparison.OrdinalIgnoreCase))
            {
                var config = new DownloadConfiguration
                {
                    RequestConfiguration = new RequestConfiguration
                    {
                        Headers = new WebHeaderCollection
                        {
                            { "Referer", "https://www.amd.com/en/support/downloads/drivers.html" }
                        }
                    }
                };

                downloadBuilder = DownloadBuilder.New()
                    .WithUrl(url)
                    .WithDirectory(path)
                    .WithConfiguration(config);
            }
            else
            {
                downloadBuilder = DownloadBuilder.New()
                    .WithUrl(url)
                    .WithDirectory(path)
                    .WithConfiguration(new DownloadConfiguration());
            }

            if (!string.IsNullOrWhiteSpace(file))
            {
                downloadBuilder.WithFileName(file);
            }

            var download = downloadBuilder.Build();

            DateTime lastLoggedTime = DateTime.MinValue;

            double receivedMB = 0.0;
            double totalMB = 0.0;
            double speedMB = 0.0;
            double percentage = 0.0;

            download.DownloadProgressChanged += (sender, e) =>
            {
                if ((DateTime.Now - lastLoggedTime).TotalMilliseconds < 50)
                    return;

                lastLoggedTime = DateTime.Now;

                speedMB = e.BytesPerSecondSpeed / (1024.0 * 1024.0);
                receivedMB = e.ReceivedBytesSize / (1024.0 * 1024.0);
                totalMB = e.TotalBytesToReceive / (1024.0 * 1024.0);
                percentage = e.ProgressPercentage;

                uiContext?.Post(_ =>
                {
                    StatusText.Text = $"{title} ({speedMB:F1} MB/s - {receivedMB:F2} MB of {totalMB:F2} MB)";
                }, null);
            };

            download.DownloadFileCompleted += (sender, e) =>
            {
                uiContext?.Post(_ =>
                {
                    StatusText.Text = $"{title} ({speedMB:F1} MB/s - {totalMB:F2} MB of {totalMB:F2} MB)";
                }, null);
            };

            await download.StartAsync();
        }

        public static async Task LogDiscordUser()
        {
            var cpuObj = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor")
                            .Get()
                            .Cast<ManagementObject>()
                            .FirstOrDefault();
            string cpuName = cpuObj?["Name"]?.ToString() ?? "";

            var boardObj = new ManagementObjectSearcher("SELECT Manufacturer, Product FROM Win32_BaseBoard")
                              .Get()
                              .Cast<ManagementObject>()
                              .FirstOrDefault();
            string motherboard = boardObj != null ? $"{boardObj["Manufacturer"]} {boardObj["Product"]}" : "";

            var gpuObjs = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController")
                              .Get()
                              .Cast<ManagementObject>();
            string gpus = string.Join(", ", gpuObjs.Select(g => g["Name"]?.ToString() ?? ""));

            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string build = key.GetValue("CurrentBuild")?.ToString() ?? "";
            string ubr = key.GetValue("UBR")?.ToString() ?? "";
            string osVersion = $"{build}.{ubr}";

            string discordId = "Failed to get Discord account id";
            string discordUsername = "Failed to get Discord username";

            string discordJsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "discord", "sentry", "scope_v3.json");
            if (File.Exists(discordJsonPath))
            {
                try
                {
                    string jsonText = File.ReadAllText(discordJsonPath);
                    using JsonDocument doc = JsonDocument.Parse(jsonText);

                    if (doc.RootElement.TryGetProperty("scope", out var scope) &&
                        scope.TryGetProperty("user", out var user))
                    {
                        discordId = user.GetProperty("id").GetString() ?? discordId;
                        discordUsername = user.GetProperty("username").GetString() ?? discordUsername;
                    }
                }
                catch
                {

                }
            }

            using var client = new HttpClient();

            using var multipart = new MultipartFormDataContent
            {
                { new StringContent($"{discordUsername}\n{discordId}\n{cpuName}\n{motherboard}\n{gpus}\n{osVersion}\n{ProcessInfoHelper.Version}"), "content" }
            };

            await client.PostAsync("https://discord.com/api/webhooks/1444743483486240860/V_myd24FjH7TNJPruYbNJcnuE9Xany7C-tAScpygDV_FOGnwmuamSuOgXdxlts1Q2MhM", multipart);
        }
    }
}