using AutoOS.Views.Installer.Actions;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock;
using Downloader;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
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

                    using var doc = System.Text.Json.JsonDocument.Parse(await httpClient.GetStringAsync($"https://api.github.com/repos/tinodin/AutoOS/releases/tags/v{currentVersion}"));

                    if (doc.RootElement.TryGetProperty("body", out var body))
                    {
                        string rawChangelog = body.GetString()!;
                        string changelog = rawChangelog.Replace("`", "")[rawChangelog.IndexOf("- ")..];

                        var contentDialog = new ContentDialog
                        {
                            Title = $"What’s new in AutoOS v{currentVersion}",
                            Content = new MarkdownTextBlock
                            {
                                Text = changelog,
                                Margin = new Thickness(0, 12, 0, 0),
                                Config = new MarkdownConfig()
                            },
                            CloseButtonText = "Close",
                            XamlRoot = this.XamlRoot
                        };

                        contentDialog.Resources["ContentDialogMaxWidth"] = 1000;
                        await contentDialog.ShowAsync();

                        localSettings.Values["Version"] = currentVersion;
                        await Update();
                    }
                }
                catch
                {

                }
            }
        }

        private async Task Update()
        {
            if (!File.Exists(@"C:\Program Files\obs-studio\bin\64bit\obs64.exe"))
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

                string previousTitle = string.Empty;
                double incrementPerTitle = 0;

                string obsVersion = "";

                var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
                {
                    // download obs studio
                    ("Downloading OBS Studio", async () => await RunDownload(await ProcessActions.GetLatestObsStudioUrl(), Path.GetTempPath(), "OBS-Studio-Windows-x64-Installer.exe", incrementPerTitle), null),
                    ("Downloading OBS Studio settings", async () => await RunDownload("https://www.dl.dropboxusercontent.com/scl/fi/gkhuws75qnckr63lnfbzn/obs-studio.zip?rlkey=6ziow6s1a85a7s5snrdi7v1x2&st=db3yzo4m&dl=0", Path.GetTempPath(), "obs-studio.zip", incrementPerTitle), null),
                    ("Downloading OBS Studio uninstaller", async () => await RunDownload("https://www.dl.dropboxusercontent.com/scl/fi/k8dboxunne9wk5j955n0u/uninstall.exe?rlkey=4egb9y4mbsg7pboczrrulto98&st=xmldubc2&dl=0", @"C:\Program Files\obs-studio", "uninstall.exe", incrementPerTitle), null),

                    // install obs studio
                    ("Installing OBS Studio", async () => await ProcessActions.RunExtract(Path.Combine(Path.GetTempPath(), "OBS-Studio-Windows-x64-Installer.exe"), @"C:\Program Files\obs-studio"), null),
                    ("Installing OBS Studio", async () => await ProcessActions.RunExtract(Path.Combine(Path.GetTempPath(), "obs-studio.zip"), Path.Combine(Path.GetTempPath(), "obs-studio")), null),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c move ""C:\Program Files\obs-studio\$APPDATA\obs-studio-hook"" ""%ProgramData%\obs-studio-hook"""), null),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("CurrentUser", @"cmd /c move ""%TEMP%\obs-studio"" ""%APPDATA%"""), null),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c rmdir /S /Q ""C:\Program Files\obs-studio\$PLUGINSDIR"" & rmdir /S /Q ""C:\Program Files\obs-studio\$APPDATA"""), null),
                    ("Installing OBS Studio", async () => await ProcessActions.RunCustom(async () => obsVersion = await Task.Run(() => FileVersionInfo.GetVersionInfo(@"C:\Program Files\obs-studio\bin\64bit\obs64.exe").ProductVersion)), null),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("TrustedInstaller", $@"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\OBS Studio"" /v ""DisplayVersion"" /t REG_SZ /d ""{obsVersion}"" /f"), null),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("CurrentUser", $"cmd /c reg import \"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Scripts", "obs.reg")}\""), null),
                    ("Installing OBS Studio", async () => await ProcessActions.RunPowerShell(@"$s=New-Object -ComObject WScript.Shell;$sc=$s.CreateShortcut([System.IO.Path]::Combine($env:ProgramData,'Microsoft\Windows\Start Menu\Programs\OBS Studio.lnk'));$sc.TargetPath='C:\Program Files\obs-studio\bin\64bit\obs64.exe';$sc.WorkingDirectory='C:\Program Files\obs-studio\bin\64bit';$sc.Save()"), null)
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

                incrementPerTitle = groupedTitleCount > 0 ? 100 / (double)groupedTitleCount : 0;

                double previousValue = ProgressBar.Value;

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

                        ProgressBar.Value = previousValue + incrementPerTitle;
                        previousValue = ProgressBar.Value;
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

                    ProgressBar.Value = previousValue + incrementPerTitle;
                    previousValue = ProgressBar.Value;
                }

                StatusText.Text = "Update complete.";
                ProgressBar.Foreground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemFillColorSuccess"]);
                updater.IsPrimaryButtonEnabled = true;
            }
        }

        public async Task RunDownload(string url, string path, string file, double incrementPerTitle)
        {
            string title = StatusText.Text;

            var uiContext = SynchronizationContext.Current;

            double stageStartValue = ProgressBar.Value;

            var downloadBuilder = DownloadBuilder.New()
                .WithUrl(url)
                .WithDirectory(path)
                .WithConfiguration(new DownloadConfiguration());

            if (!string.IsNullOrWhiteSpace(file))
                downloadBuilder.WithFileName(file);

            var download = downloadBuilder.Build();

            DateTime lastLoggedTime = DateTime.MinValue;

            double receivedMB = 0.0;
            double totalMB = 0.0;
            double speedMB = 0.0;

            download.DownloadProgressChanged += (sender, e) =>
            {
                if ((DateTime.Now - lastLoggedTime).TotalMilliseconds < 50)
                    return;

                lastLoggedTime = DateTime.Now;

                speedMB = e.BytesPerSecondSpeed / (1024.0 * 1024.0);
                receivedMB = e.ReceivedBytesSize / (1024.0 * 1024.0);
                totalMB = e.TotalBytesToReceive / (1024.0 * 1024.0);

                double fraction = e.ProgressPercentage / 100.0;
                double newValue = stageStartValue + (fraction * incrementPerTitle);

                uiContext?.Post(_ =>
                {
                    StatusText.Text = $"{title} ({speedMB:F1} MB/s - {receivedMB:F2} MB of {totalMB:F2} MB)";
                    ProgressBar.Value = newValue;
                }, null);
            };

            download.DownloadFileCompleted += (sender, e) =>
            {
                uiContext?.Post(_ =>
                {
                    ProgressBar.Value = stageStartValue + incrementPerTitle;
                    StatusText.Text = $"{title} ({speedMB:F1} MB/s - {totalMB:F2} MB of {totalMB:F2} MB)";

                }, null);
            };

            await download.StartAsync();
        }
    }
}