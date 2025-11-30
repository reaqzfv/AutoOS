using AutoOS.Views.Installer.Actions;
using CommunityToolkit.WinUI.Controls;
using Downloader;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Text;
using System.Text.Json;
using Windows.Storage;
using AutoOS.Views.Settings.Scheduling.Services;

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
                await LogDiscordUser();
                localSettings.Values["Version"] = currentVersion;
                StatusText.Text = "Update complete.";
                await Task.Delay(1000);
                ProgressBar.Foreground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemFillColorSuccess"]);
                StatusText.Text = "Restarting in 3...";
                await Task.Delay(1000);
                StatusText.Text = "Restarting in 2...";
                await Task.Delay(1000);
                StatusText.Text = "Restarting in 1...";
                await Task.Delay(1000);
                StatusText.Text = "Restarting...";
                await Task.Delay(750);
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c shutdown /r /t 0",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                Process.Start(processStartInfo);
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
            bool NVIDIA = false;
            bool ServicesEnabled = false;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            {
                foreach (var obj in searcher.Get())
                {
                    string name = obj["Name"]?.ToString();
                    string version = obj["DriverVersion"]?.ToString();

                    if (name != null)
                    {
                        if (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase))
                        {
                            NVIDIA = true;
                        }
                    }
                }
            }

            using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Beep"))
            {
                ServicesEnabled = (int)(key?.GetValue("Start", 0) ?? 0) == 1;
            }

            var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
            {
                // disable show frequently used folders
                ("Disabling show frequently used folders", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ShowFrequent /t REG_DWORD /d 0 /f"), null),

                // remove custom priorities
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\SearchIndexer.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\ctfmon.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\fontdrvhost.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\winlogon.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\lsass.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\sihost.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\sppsvc.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\dwm.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\svchost.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\WmiPrvSE.exe\PerfOptions"" /f"), null),
                ("Removing custom priorities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\csrss.exe\PerfOptions"" /f"), null),

                // remove unnecessary process mitigations
                ("Remove unnecessary process mitigations", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c for %a in (fontdrvhost.exe dwm.exe lsass.exe svchost.exe WmiPrvSE.exe winlogon.exe csrss.exe audiodg.exe ntoskrnl.exe services.exe) do reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\%a"" /v ""MitigationOptions"" /f && reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\%a"" /v ""MitigationAuditOptions"" /f"), null),

                // enable eventlog
                ("Enabling EventLog", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog"" /v Start /t REG_DWORD /d 4 /f & sc stop EventLog"), () => ServicesEnabled == true),
                
                // enable eventsystem
                ("Enabling EventSystem", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventSystem"" /v Start /t REG_DWORD /d 4 /f & sc stop EventSystem"), () => ServicesEnabled == true),

                // update vencord plugins
                ("Update Vencord Plugins", async () => await Task.Run(() => File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Scripts", "settings.json"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vencord", "settings", "settings.json"), true)), null),

                // import the optimized nvidia profile
                ("Importing the optimized NVIDIA profile", async () => await ProcessActions.ImportProfile("BaseProfile.nip"), () => NVIDIA == true),

                // Disable the nvidia tray icon
                ("Disabling the NVIDIA tray icon", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Global\NVTweak"" /v ""HideXGpuTrayIcon"" /t REG_DWORD /d 1 /f"), () => NVIDIA == true),
                ("Disabling the NVIDIA tray icon", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\Global\CoProcManager"" /v ""ShowTrayIcon"" /t REG_DWORD /d 0 /f"), () => NVIDIA == true),

                // disable the dlss indicator
                ("Disabling the DLSS Indicator", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\Global\NGXCore"" /v ""ShowDlssIndicator"" /t REG_DWORD /d 0 /f"), () => NVIDIA == true),

                // disable automatic updates
                ("Disabling automatic updates", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\Global\CoProcManager"" /v AutoDownload /t REG_DWORD /d 0 /f"), () => NVIDIA == true),

                // disable telemetry
                ("Disabling telemetry", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\Global\FTS"" /v EnableRID44231 /t REG_DWORD /d 0 /f"), () => NVIDIA == true),
                ("Disabling telemetry", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\Global\FTS"" /v EnableRID64640 /t REG_DWORD /d 0 /f"), () => NVIDIA == true),
                ("Disabling telemetry", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\Global\FTS"" /v EnableRID66610 /t REG_DWORD /d 0 /f"), () => NVIDIA == true),
                ("Disabling telemetry", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c cd /d ""C:\Windows\System32\DriverStore\FileRepository\"" & dir NvTelemetry64.dll /a /b /s & del NvTelemetry64.dll /a /s"), () => NVIDIA == true),

                // disable logging
                ("Disabling logging", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Parameters"" /v LogDisableMasks /t REG_BINARY /d ""00ffff0f01ffff0f02ffff0f03ffff0f04ffff0f05ffff0f06ffff0f07ffff0f08ffff0f09ffff0f0affff0f0bffff0f0cffff0f0dffff0f0effff0f0fffff0f10ffff0f11ffff0f12ffff0f13ffff0f14ffff0f15ffff0f16ffff0f00ffff1f01ffff1f02ffff1f03ffff1f04ffff1f05ffff1f06ffff1f07ffff1f08ffff1f09ffff1f0affff1f0bffff1f0cffff1f0dffff1f0effff1f0fffff1f00ffff2f01ffff2f02ffff2f03ffff2f04ffff2f05ffff2f06ffff2f07ffff2f08ffff2f09ffff2f0affff2f0bffff2f0cffff2f0dffff2f0effff2f0fffff2f00ffff3f01ffff3f02ffff3f03ffff3f04ffff3f05ffff3f06ffff3f07ffff3f"" /f"), () => NVIDIA == true),
                ("Disabling logging", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Parameters"" /v LogWarningEntries /t REG_DWORD /d 0 /f"), () => NVIDIA == true),
                ("Disabling logging", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Parameters"" /v LogPagingEntries /t REG_DWORD /d 0 /f"), () => NVIDIA == true),
                ("Disabling logging", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Parameters"" /v LogEventEntries /t REG_DWORD /d 0 /f"), () => NVIDIA == true),
                ("Disabling logging", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Parameters"" /v LogErrorEntries /t REG_DWORD /d 0 /f"), () => NVIDIA == true),

                // disable error code correction (ecc)
                ("Disabling error code correction (ECC)", async () => await ProcessActions.RunPowerShellScript("ecc.ps1", ""), () => NVIDIA == true),

                // configure miscellaneous nvidia settings
                ("Configuring miscellaneous NVIDIA settings", async () => await ProcessActions.RunPowerShellScript("nvidiamisc.ps1", ""), () => NVIDIA == true),

                // disable display power savings
                ("Disabling Display Power Savings", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Global\NVTweak"" /v ""DisplayPowerSaving"" /t REG_DWORD /d 0 /f"), () => NVIDIA == true),
                ("Disabling Display Power Savings", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\Software\NVIDIA Corporation\Global\NVTweak"" /v ""DisplayPowerSaving"" /t REG_DWORD /d 0 /f"), () => NVIDIA == true),

                // disable hd audio power savings
                ("Disabling HD Audio Power Savings", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm"" /v ""EnableHDAudioD3Cold"" /t REG_DWORD /d 0 /f"), () => NVIDIA == true),

                // disable standby states
                ("Disabling standy states", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 238c9fa8-0aad-41ed-83f4-97be242c8f20 abfc2519-3608-4c2a-94ea-171b0ed546ab 0"), null),
                
                // disable hybrid sleep
                ("Disabling hybrid sleep", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 238c9fa8-0aad-41ed-83f4-97be242c8f20 94ac6d29-73ce-41a6-809f-6363ba21b47e 0"), null),

                // disable drive powersaving features
                ("Disabling drive powersaving features", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c for %a in (EnhancedPowerManagementEnabled AllowIdleIrpInD3 EnableSelectiveSuspend DeviceSelectiveSuspended SelectiveSuspendEnabled SelectiveSuspendOn EnumerationRetryCount ExtPropDescSemaphore WaitWakeEnabled D3ColdSupported WdfDirectedPowerTransitionEnable EnableIdlePowerManagement IdleInWorkingState IdleTimeoutInMS MinimumIdleTimeoutInMS WakeEnabled) do for /f ""delims="" %b in ('reg query ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum"" /s /f ""%a"" ^| findstr ""HKEY""') do reg add ""%b"" /v ""%a"" /t REG_DWORD /d 0 /f"), null),
                ("Disabling drive powersaving features", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c for %a in (DisableIdlePowerManagement DisableRuntimePowerManagement) do for /f ""delims="" %b in ('reg query ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum"" /s /f ""%a"" ^| findstr ""HKEY""') do reg add ""%b"" /v ""%a"" /t REG_DWORD /d 1 /f"), null),

                // disable sleep study
                ("Disabling sleep study", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Power"" /v ""SleepStudyDisabled"" /t REG_DWORD /d 1 /f"), null),
                ("Disabling sleep study", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Power"" /v ""SleepStudyDeviceAccountingLevel"" /t REG_DWORD /d 0 /f"), null),
                ("Disabling sleep study", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Power"" /v ""SleepstudyAccountingEnabled"" /t REG_DWORD /d ""0"" /f"), null),

                // remove autogpuaffinity
                ("Removing AutoGpuAffinity", async () => { var p = Path.Combine(PathHelper.GetAppDataFolderPath(), "AutoGpuAffinity"); if (Directory.Exists(p)) Directory.Delete(p, true); }, null),

                // optimize affinities
                ("Optimizing Affinities", async () => await ProcessActions.Sleep(1000), null),
                ("Optimizing Affinities", async () => await AutoAffinityService.ApplyAutoAffinities(), null),
                ("Optimizing Affinities", async () => await ProcessActions.Sleep(2000), null),
                ("Optimizing Affinities", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\kernel"" /v ReservedCpuSets /f"), null)

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

            //updater.IsPrimaryButtonEnabled = true;
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