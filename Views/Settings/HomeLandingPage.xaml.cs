using AutoOS.Views.Installer.Actions;
using CommunityToolkit.WinUI.Controls;
using Downloader;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using System.Net;
using System.Text;
using System.Text.Json;
using Windows.Storage;
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

            string previousTitle = string.Empty;
            
            string edgeVersion = "";
            bool Office = Directory.Exists(@"C:\Program Files\Microsoft Office");
            bool AMD = (localSettings.Values["GpuBrand"]?.ToString().Contains("AMD") ?? false);

            var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
            {
                // fix broken registry path
                ("Fixing broken registry path", async () => await ProcessActions.RunNsudo("TrustedInstaller", $@"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Active Setup\Installed Components\AutorunsDisabled\{{9459C573-B17A-45AE-9F64-1857B5D58CEE}}"" /v ""StubPath"" /f"), null),
                ("Fixing broken registry path", async () => edgeVersion = await Task.Run(() => FileVersionInfo.GetVersionInfo(Environment.ExpandEnvironmentVariables(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe")).ProductVersion), null),
                ("Fixing broken registry path", async () => await ProcessActions.RunNsudo("TrustedInstaller", $@"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Active Setup\Installed Components\AutorunsDisabled\{{9459C573-B17A-45AE-9F64-1857B5D58CEE}}"" /v ""StubPath"" /t REG_SZ /d ""\""C:\Program Files (x86)\Microsoft\Edge\Application\{edgeVersion}\Installer\setup.exe\"" --configure-user-settings --verbose-logging --system-level --msedge --channel=stable"" /f"), null),

                // disable office startup entries
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_CLASSES_ROOT\PROTOCOLS\Filter\AutorunsDisabled\text/xml\CLSID"" /t REG_SZ /d ""{807583E5-5146-11D5-A672-00B0D022E945}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_CLASSES_ROOT\PROTOCOLS\Filter\text/xml"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_CLASSES_ROOT\PROTOCOLS\Handler\AutorunsDisabled\mso-minsb-roaming.16\CLSID"" /t REG_SZ /d ""{83C25742-A9F7-49FB-9138-434302C88D07}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_CLASSES_ROOT\PROTOCOLS\Handler\mso-minsb-roaming.16"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_CLASSES_ROOT\PROTOCOLS\Handler\AutorunsDisabled\mso-minsb.16\CLSID"" /t REG_SZ /d ""{42089D2D-912D-4018-9087-2B87803E93FB}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_CLASSES_ROOT\PROTOCOLS\Handler\AutorunsDisabled\mso-minsb.16"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_CLASSES_ROOT\PROTOCOLS\Handler\AutorunsDisabled\osf-roaming.16\CLSID"" /t REG_SZ /d ""{42089D2D-912D-4018-9087-2B87803E93FB}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_CLASSES_ROOT\PROTOCOLS\Handler\osf-roaming.16"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_CLASSES_ROOT\PROTOCOLS\Handler\AutorunsDisabled\osf.16\CLSID"" /t REG_SZ /d ""{5504BE45-A83B-4808-900A-3A5C36E7F77A}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_CLASSES_ROOT\PROTOCOLS\Handler\osf.16"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""(Default)"" /t REG_SZ /d ""Lync Click to Call BHO"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""NoExplorer"" /t REG_SZ /d ""1"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""(Default)"" /t REG_SZ /d ""Lync Click to Call"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""MenuText"" /t REG_SZ /d ""Lync Click to Call"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""Icon"" /t REG_SZ /d ""C:\Program Files\Microsoft Office\root\VFS\ProgramFilesX86\Microsoft Office\Office16\lync.exe,1"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""HotIcon"" /t REG_SZ /d ""C:\Program Files\Microsoft Office\root\VFS\ProgramFilesX86\Microsoft Office\Office16\lync.exe,1"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""CLSID"" /t REG_SZ /d ""{1FBA04EE-3024-11d2-8F1F-0000F87ABD16}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""ClsidExtension"" /t REG_SZ /d ""{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""Default Visible"" /t REG_SZ /d ""Yes"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\AutorunsDisabled\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /v ""ButtonText"" /t REG_SZ /d ""Lync Click to Call"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Extensions\{31D09BA0-12F5-4CCE-BE8A-2923E76605DA}"" /f"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"schtasks /Change /TN ""\Microsoft\Office\Office Actions Server"" /Disable"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"schtasks /Change /TN ""\Microsoft\Office\Office Automatic Updates 2.0"" /Disable"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"schtasks /Change /TN ""\Microsoft\Office\Office Background Push Maintenance"" /Disable"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"schtasks /Change /TN ""\Microsoft\Office\Office ClickToRun Service Monitor"" /Disable"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"schtasks /Change /TN ""\Microsoft\Office\Office Feature Updates"" /Disable"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"schtasks /Change /TN ""\Microsoft\Office\Office Feature Updates Logon"" /Disable"), () => Office == true),
                ("Disabling Office startup entries", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"schtasks /Change /TN ""\Microsoft\Office\Office Performance Monitor"" /Disable"), () => Office == true),

                // disable office telemetry
                ("Disabling Office telemetry", async () => await ProcessActions.RunPowerShellScript("disableofficetelemetry.ps1", ""), () => Office == true),

                // configure amd settings
                ("Configuring AMD settings", async () => await ProcessActions.RunPowerShellScript("amdsettings.ps1", ""), () => AMD == true),
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