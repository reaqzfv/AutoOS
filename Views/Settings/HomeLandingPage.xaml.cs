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
            this.Loaded += GetChangeLog;
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
                            XamlRoot = this.XamlRoot
                        };

                        contentDialog.Resources["ContentDialogMaxWidth"] = 1000;
                        await contentDialog.ShowAsync();
                    }
                }
                catch
                {

                }

                await Update();
                localSettings.Values["Version"] = currentVersion;
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
                XamlRoot = this.XamlRoot
            };

            _ = updater.ShowAsync();

            var gpuAffinities = new List<int?>();

            foreach (var query in new[]
            {
                "SELECT PNPDeviceID FROM Win32_VideoController",
                "SELECT PNPDeviceID FROM Win32_USBController",
                "SELECT PNPDeviceID FROM Win32_NetworkAdapter"
            })
            {
                foreach (ManagementObject obj in new ManagementObjectSearcher(query).Get().Cast<ManagementObject>())
                {
                    string path = obj["PNPDeviceID"]?.ToString();
                    if (path?.StartsWith("PCI\\VEN_") != true)
                        continue;

                    int? affinityCore = null;
                    using (var affinityKey = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Enum\{path}\Device Parameters\Interrupt Management\Affinity Policy"))
                    {
                        if (affinityKey?.GetValue("AssignmentSetOverride") is byte[] value && value.Length > 0)
                        {
                            for (int i = 0; i < value.Length; i++)
                            {
                                for (int bit = 0; bit < 8; bit++)
                                {
                                    if ((value[i] & (1 << bit)) != 0)
                                    {
                                        affinityCore = i * 8 + bit;
                                        break;
                                    }
                                }
                                if (affinityCore.HasValue) break;
                            }
                        }
                    }

                    if (query.Contains("VideoController"))
                    {
                        gpuAffinities.Add(affinityCore);
                    }
                    else if (query.Contains("USBController"))
                    {
                        if (affinityCore.HasValue)
                            localSettings.Values["XhciAffinity"] = affinityCore.Value;
                    }
                    else if (query.Contains("NetworkAdapter"))
                    {
                        using var driverKey = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Enum\{path}");
                        string driver = driverKey?.GetValue("Driver")?.ToString();
                        if (string.IsNullOrEmpty(driver) || !driver.Contains('\\'))
                            continue;

                        using var classKey = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Control\Class\{driver}");
                        if (classKey?.GetValue("*PhysicalMediaType")?.ToString() != "14")
                            continue;

                        if (int.TryParse(classKey.GetValue("*RssBaseProcNumber")?.ToString(), out int rssCore))
                        {
                            if (affinityCore.HasValue && rssCore == affinityCore.Value)
                                localSettings.Values["NicAffinity"] = rssCore;
                        }
                    }
                }
            }

            if (gpuAffinities.Count > 0 &&
                gpuAffinities.All(a => a.HasValue) &&
                gpuAffinities.Select(a => a.Value).Distinct().Count() == 1)
            {
                localSettings.Values["GpuAffinity"] = gpuAffinities[0].Value;
            }

            bool Reserve = new ManagementObjectSearcher("SELECT NumberOfCores FROM Win32_Processor")
                .Get()
                .Cast<ManagementObject>()
                .Sum(m => Convert.ToInt32(m["NumberOfCores"])) >= 6;

            string previousTitle = string.Empty;

            var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
            {
               // apply nic affinity
                ("Applying NIC Affinity", async () => await ProcessActions.ApplyNicAffinity(), null),

                // reserve cpus
                ("Reserving CPUs", async () => await ProcessActions.Sleep(1000), () => Reserve == true),
                ("Reserving CPUs", async () => await ProcessActions.ReserveCpus(), () => Reserve == true)
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

            StatusText.Text = "Update complete.";
            ProgressBar.Foreground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemFillColorSuccess"]);
            await RunRestart();
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

        public async Task RunRestart()
        {
            StatusText.Text = "Restarting in 3...";
            await Task.Delay(1000);
            StatusText.Text = "Restarting in 2...";
            await Task.Delay(1000);
            StatusText.Text = "Restarting in 1...";
            await Task.Delay(1000);
            StatusText.Text = "Restarting...";
            await Task.Delay(750);
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c shutdown /r /t 0",
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            Process.Start(processStartInfo);
        }
    }
}