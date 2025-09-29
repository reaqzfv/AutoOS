using CommunityToolkit.WinUI.Controls;
using Downloader;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Microsoft.Win32;
using System.Net;
using System.Text;
using System.Text.Json;
using Windows.Storage;
using AutoOS.Views.Installer.Actions;

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

            bool servicesState = (int)(Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Beep")?.GetValue("Start", 0) ?? 0) == 1;

            string previousTitle = string.Empty;

            var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
            {
                // optimize microsoft edge settings
                ("Optimizing Microsoft Edge settings", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\EdgeUI"" /v DisableMFUTracking /t REG_DWORD /d 1 /f"), null),

                // enable non-paged ntfs
                ("Enabling non-paged NTFS", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"fsutil behavior set enablenonpagedntfs 1"), null),

                // disable settings sync
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableApplicationSettingSync /t REG_DWORD /d 2 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableApplicationSettingSyncUserOverride /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableSettingSync /t REG_DWORD /d 2 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableSettingSyncUserOverride /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableWebBrowserSettingSync /t REG_DWORD /d 2 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableWebBrowserSettingSyncUserOverride /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableDesktopThemeSettingSync /t REG_DWORD /d 2 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableDesktopThemeSettingSyncUserOverride /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableSyncOnPaidNetwork /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableWindowsSettingSync /t REG_DWORD /d 2 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableWindowsSettingSyncUserOverride /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableCredentialsSettingSync /t REG_DWORD /d 2 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableCredentialsSettingSyncUserOverride /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisablePersonalizationSettingSync /t REG_DWORD /d 2 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisablePersonalizationSettingSyncUserOverride /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableStartLayoutSettingSync /t REG_DWORD /d 2 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync"" /v DisableStartLayoutSettingSyncUserOverride /t REG_DWORD /d 1 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Accessibility"" /v Enabled /t REG_DWORD /d 0 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Personalization"" /v Enabled /t REG_DWORD /d 0 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\SettingSync\Groups\BrowserSettings"" /v Enabled /t REG_DWORD /d 0 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Credentials"" /v Enabled /t REG_DWORD /d 0 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Windows"" /v Enabled /t REG_DWORD /d 0 /f"), null),
                ("Disabling settings sync", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\SettingSync"" /v SyncPolicy /t REG_DWORD /d 5 /f"), null),
            
                // disable new start menu layout
                ("Disabling new start menu layout", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Windhawk\Engine\Mods\windows-11-start-menu-styler\Settings"" /v disableNewStartMenuLayout /t REG_DWORD /d 1 /f"), null),

                // enable exclusive control
                ("Enabling exclusive control", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c for %k in (Capture Render) do for /f ""delims="" %a in ('reg query ""HKLM\Software\Microsoft\Windows\CurrentVersion\MMDevices\Audio\%k""') do reg add ""%a\Properties"" /v ""{b3f8fa53-0004-438e-9003-51a46e139bfc},3"" /t REG_DWORD /d 1 /f && reg add ""%a\Properties"" /v ""{b3f8fa53-0004-438e-9003-51a46e139bfc},4"" /t REG_DWORD /d 1 /f"), null),
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
    }
}