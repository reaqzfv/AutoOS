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
using AutoOS.Views.Installer.Actions;
using System.Diagnostics;

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
        private readonly string list = Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "lists.ini");
        private readonly string nsudoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "NSudo", "NSudoLC.exe");

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

            bool servicesState = (int)(Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Beep")?.GetValue("Start", 0) ?? 0) == 1;
            bool wifiState = false;
            bool bluetoothState = false;
            bool laptopState = false;

            string previousTitle = string.Empty;

            var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
            {
                // disable large mtu
                ("Disabling large MTU", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"netsh int ip loopbacklargemtu=disabled"), null),

                // disable failure actions
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure AudioEndpointBuilder reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag AudioEndpointBuilder 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure Appinfo reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag Appinfo 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure AppXSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag AppXSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure CaptureService reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag CaptureService 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure cbdhsvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag cbdhsvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure ClipSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag ClipSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure CryptSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag CryptSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure DevicesFlowUserSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag DevicesFlowUserSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure Dhcp reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag Dhcp 0"), null),
                //("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure DispBrokerDesktopSvc reset=0 actions=//"), null),
                //("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag DispBrokerDesktopSvc 0"), null),
                //("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure Dnscache reset=0 actions=//"), null),
                //("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag Dnscache 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure DoSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag DoSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure DsmSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag DsmSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure gpsvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag gpsvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure InstallService reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag InstallService 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure KeyIso reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag KeyIso 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure lfsvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag lfsvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure msiserver reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag msiserver 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure NcbService reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag NcbService 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure netprofm reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag netprofm 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure NgcSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag NgcSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure NgcCtnrSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag NgcCtnrSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure nsi reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag nsi 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure ProfSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag ProfSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure sppsvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag sppsvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure StateRepository reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag StateRepository 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure TextInputManagementService reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag TextInputManagementService 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure TrustedInstaller reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag TrustedInstaller 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure UdkUserSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failu^^reflag UdkUserSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure WFDSConMgrSvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag WFDSConMgrSvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure WinHttpAutoProxySvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag WinHttpAutoProxySvc 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure Winmgmt reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag Winmgmt 0"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failure Wcmsvc reset=0 actions=//"), null),
                ("Disabling failure actions", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"sc failureflag Wcmsvc 0"), null),

                // save services state
                ("Saving Services & Drivers state ", async () => await Task.Run(() => servicesState = (int)(Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Beep")?.GetValue("Start", 0) ?? 0) == 1), null),
                ("Saving Services & Drivers state ", async () => await Task.Run(() => wifiState = new[] { "WlanSvc", "Dhcp", "Netman", "NetSetupSvc", "NlaSvc", "Wcmsvc", "WinHttpAutoProxySvc" }.All(service => File.ReadAllLines(list).Any(line => line.Trim() == service)) && new[] { "# tdx", "# vwififlt", "# Netwtw10", "# Netwtw14" }.All(driver => File.ReadAllLines(list).Any(line => line.Trim() == driver))), null),
                ("Saving Services & Drivers state ", async () => await Task.Run(() => bluetoothState = new[] { "BluetoothUserService", "BTAGService", "BthAvctpSvc", "bthserv", "DeviceAssociationService", "DevicesFlowUserSvc", "DsmSvc", "NcbService", "WFDSConMgrSvc" }.All(service => File.ReadAllLines(list).Any(line => line.Trim() == service)) && new[] { "# BthA2dp", "# BthEnum", "# BthHFAud", "# BthHFEnum", "# BthLEEnum", "# BthMini", "# BTHMODEM", "# BthPan", "# BTHPORT", "# BTHUSB", "# HidBth", "# ibtusb", "# Microsoft_Bluetooth_AvrcpTransport", "# RFCOMM" }.All(driver => File.ReadAllLines(list).Any(line => line.Trim() == driver))), null),
                ("Saving Services & Drivers state ", async () => await Task.Run(() => laptopState = new[] { "# msisadrv" }.All(driver => File.ReadAllLines(list).Any(line => line.Trim() == driver))), null),

                // enable services & drivers
                ("Enabling Services & Drivers", async () => await Process.Start(new ProcessStartInfo { FileName = nsudoPath, Arguments = $"-U:T -P:E -Wait -ShowWindowMode:Hide \"{Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "build", Directory.GetDirectories(Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "build")).OrderByDescending(d => Directory.GetLastWriteTime(d)).FirstOrDefault()?.Split('\\').Last(), "Services-Enable.bat")}\"", CreateNoWindow = true }).WaitForExitAsync(), () => servicesState == false),

                // update lists.ini
                ("Updating lists.ini", async () => await Task.Run(() => File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "Service-list-builder", "lists.ini"), Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "lists.ini"), true)), null),
                ("Updating lists.ini", async () => await File.WriteAllLinesAsync(list, [.. (await File.ReadAllLinesAsync(list)).Select(line => new[] { "WlanSvc", "Dhcp", "Netman", "NetSetupSvc", "NlaSvc", "Wcmsvc", "WinHttpAutoProxySvc" }.Contains(line.Trim().TrimStart('#', ' ')) ? line.TrimStart('#', ' ') : new[] { "tdx", "vwififlt", "Netwtw10", "Netwtw14" }.Contains(line.Trim().TrimStart('#', ' ')) ? "# " + line.TrimStart('#') : line)]), () => wifiState == true),
                ("Updating lists.ini", async () => await File.WriteAllLinesAsync(list, [.. (await File.ReadAllLinesAsync(list)).Select(line => new[] { "BluetoothUserService", "BTAGService", "BthAvctpSvc", "bthserv", "DeviceAssociationService", "DevicesFlowUserSvc", "DsmSvc", "NcbService", "WFDSConMgrSvc" }.Contains(line.Trim().TrimStart('#', ' ')) ? line.TrimStart('#', ' ') : new[] { "BthA2dp", "BthEnum", "BthHFAud", "BthHFEnum", "BthLEEnum", "BthMini", "BTHMODEM", "BthPan", "BTHPORT", "BTHUSB", "HidBth", "ibtusb", "Microsoft_Bluetooth_AvrcpTransport", "RFCOMM" }.Contains(line.Trim().TrimStart('#', ' ')) ? "# " + line.TrimStart('#') : line)]), () => bluetoothState == true),
                ("Updating lists.ini", async () => await File.WriteAllLinesAsync(list, [.. (await File.ReadAllLinesAsync(list)).Select(line => new[] { "msisadrv" }.Contains(line.Trim().TrimStart('#', ' ')) ? "# " + line.TrimStart('#') : line)]), () => laptopState == true),

                // build service list
                ("Building service list", async () => await Process.Start(new ProcessStartInfo { FileName = nsudoPath, Arguments = $@"-U:T -P:E -Wait -ShowWindowMode:Hide ""{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "Service-list-builder", "service-list-builder.exe")}"" --config ""{Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "lists.ini")}"" --disable-service-warning --output-dir ""{Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "build")}""", CreateNoWindow = true }).WaitForExitAsync(), null),

                // disable services & drivers
                ("Disabling Services & Drivers", async () => await Process.Start(new ProcessStartInfo { FileName = nsudoPath, Arguments = $"-U:T -P:E -Wait -ShowWindowMode:Hide \"{Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "build", Directory.GetDirectories(Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "build")).OrderByDescending(d => Directory.GetLastWriteTime(d)).FirstOrDefault()?.Split('\\').Last(), "Services-Disable.bat")}\"", CreateNoWindow = true }).WaitForExitAsync(), () => servicesState == false),
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