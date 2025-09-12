using AutoOS.Views.Installer.Actions;
using CommunityToolkit.WinUI.Controls;
using Downloader;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using Windows.Storage;
using Microsoft.Win32;

namespace AutoOS.Views.Settings
{
    public sealed partial class HomeLandingPage : Page
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static readonly HttpClient httpClient = new();
        private readonly string list = Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "lists.ini");
        private readonly string nsudoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "NSudo", "NSudoLC.exe");
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

                        localSettings.Values["Version"] = currentVersion;
                    }
                }
                catch
                {

                }

                await Update();
            }
        }

        private async Task Update()
        {
            bool OBS = !File.Exists(@"C:\Program Files\obs-studio\bin\64bit\obs64.exe");
            bool Firefox = File.Exists(@"C:\Program Files\Mozilla Firefox\user.js");
            bool Zen = File.Exists(@"C:\Program Files\Zen Browser\user.js");
            bool Spotify = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spotify", "config.ini"));

            if (OBS || Firefox || Zen || Spotify)
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

                string obsVersion = "";
                string spotifyVersion = "";
                bool servicesState = false;
                bool wifiState = false;
                bool bluetoothState = false;
                bool laptopState = false;
                bool apexLegendsState = false;
                
                string previousTitle = string.Empty;

                var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
                {
                    // download obs studio
                    ("Downloading OBS Studio", async () => await RunDownload(await ProcessActions.GetLatestObsStudioUrl(), Path.GetTempPath(), "OBS-Studio-Windows-x64-Installer.exe"), () => OBS == true),
                    ("Downloading OBS Studio settings", async () => await RunDownload("https://www.dl.dropboxusercontent.com/scl/fi/gkhuws75qnckr63lnfbzn/obs-studio.zip?rlkey=6ziow6s1a85a7s5snrdi7v1x2&st=db3yzo4m&dl=0", Path.GetTempPath(), "obs-studio.zip"), () => OBS == true),
                    ("Downloading OBS Studio uninstaller", async () => await RunDownload("https://www.dl.dropboxusercontent.com/scl/fi/k8dboxunne9wk5j955n0u/uninstall.exe?rlkey=4egb9y4mbsg7pboczrrulto98&st=xmldubc2&dl=0", @"C:\Program Files\obs-studio", "uninstall.exe"), () => OBS == true),

                    // install obs studio
                    ("Installing OBS Studio", async () => await ProcessActions.RunExtract(Path.Combine(Path.GetTempPath(), "OBS-Studio-Windows-x64-Installer.exe"), @"C:\Program Files\obs-studio"), () => OBS == true),
                    ("Installing OBS Studio", async () => await ProcessActions.RunExtract(Path.Combine(Path.GetTempPath(), "obs-studio.zip"), Path.Combine(Path.GetTempPath(), "obs-studio")), () => OBS == true),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c move ""C:\Program Files\obs-studio\$APPDATA\obs-studio-hook"" ""%ProgramData%\obs-studio-hook"""), () => OBS == true),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("CurrentUser", @"cmd /c move ""%TEMP%\obs-studio"" ""%APPDATA%"""), () => OBS == true),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c rmdir /S /Q ""C:\Program Files\obs-studio\$PLUGINSDIR"" & rmdir /S /Q ""C:\Program Files\obs-studio\$APPDATA"""), () => OBS == true),
                    ("Installing OBS Studio", async () => obsVersion = await Task.Run(() => FileVersionInfo.GetVersionInfo(@"C:\Program Files\obs-studio\bin\64bit\obs64.exe").ProductVersion), () => OBS == true),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("TrustedInstaller", $@"reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\OBS Studio"" /v ""DisplayVersion"" /t REG_SZ /d ""{obsVersion}"" /f"), () => OBS == true),
                    ("Installing OBS Studio", async () => await ProcessActions.RunNsudo("CurrentUser", $"cmd /c reg import \"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Scripts", "obs.reg")}\""), () => OBS == true),
                    ("Installing OBS Studio", async () => await ProcessActions.RunPowerShell(@"$s=New-Object -ComObject WScript.Shell;$sc=$s.CreateShortcut([System.IO.Path]::Combine($env:ProgramData,'Microsoft\Windows\Start Menu\Programs\OBS Studio.lnk'));$sc.TargetPath='C:\Program Files\obs-studio\bin\64bit\obs64.exe';$sc.WorkingDirectory='C:\Program Files\obs-studio\bin\64bit';$sc.Save()"), () => OBS == true),

                    // optimize firefox settings
                    ("Optimizing Firefox settings", async () => await Task.Run(() => File.WriteAllText(Path.Combine(@"C:\Program Files\Mozilla Firefox", "firefox.cfg"), "defaultPref(\"app.shield.optoutstudies.enabled\", false);\ndefaultPref(\"browser.search.serpEventTelemetryCategorization.enabled\", false);\ndefaultPref(\"dom.security.unexpected_system_load_telemetry_enabled\", false);\ndefaultPref(\"identity.fxaccounts.telemetry.clientAssociationPing.enabled\", false);\ndefaultPref(\"network.trr.confirmation_telemetry_enabled\", false);\ndefaultPref(\"nimbus.telemetry.targetingContextEnabled\", false);\ndefaultPref(\"reader.parse-on-load.enabled\", false);\ndefaultPref(\"telemetry.fog.init_on_shutdown\", false);\ndefaultPref(\"widget.windows.mica.popups\", 1);\ndefaultPref(\"widget.windows.mica.toplevel-backdrop\", 0);")), () => Firefox == true),

                    // download arkenfox user.js
                    ("Downloading Arkenfox user.js", async () => await RunDownload("https://raw.githubusercontent.com/arkenfox/user.js/refs/heads/master/user.js", @"C:\Program Files\Mozilla Firefox", "user.js"), () => Firefox == true),

                    // optimize zen settings
                    ("Optimizing Zen settings", async () => await Task.Run(() => File.WriteAllText(Path.Combine(@"C:\Program Files\Zen Browser", "zen.cfg"), "defaultPref(\"app.shield.optoutstudies.enabled\", false);\ndefaultPref(\"browser.search.serpEventTelemetryCategorization.enabled\", false);\ndefaultPref(\"dom.security.unexpected_system_load_telemetry_enabled\", false);\ndefaultPref(\"identity.fxaccounts.telemetry.clientAssociationPing.enabled\", false);\ndefaultPref(\"network.trr.confirmation_telemetry_enabled\", false);\ndefaultPref(\"nimbus.telemetry.targetingContextEnabled\", false);\ndefaultPref(\"reader.parse-on-load.enabled\", false);\ndefaultPref(\"telemetry.fog.init_on_shutdown\", false);\ndefaultPref(\"zen.theme.accent-color\", \"#2c34fb\");\ndefaultPref(\"zen.urlbar.behavior\", \"float\");\ndefaultPref(\"zen.view.grey-out-inactive-windows\", false);\ndefaultPref(\"widget.windows.mica.popups\", 1);\ndefaultPref(\"widget.windows.mica.toplevel-backdrop\", 0);")), () => Zen == true),

                    // download arkenfox user.js
                    ("Downloading Arkenfox user.js", async () => await RunDownload("https://raw.githubusercontent.com/arkenfox/user.js/refs/heads/master/user.js", @"C:\Program Files\Zen Browser", "user.js"), () => Zen == true),

                    // removing blockthespot
                    ("Removing BlockTheSpot", async () => await ProcessActions.RunNsudo("CurrentUser", @"cmd /c del /f /q ""%APPDATA%\Spotify\dpapi.dll"" ""%APPDATA%\Spotify\config.ini"""), () => Spotify == true),

                    // download spotify
                    ("Downloading Spotify", async () => await RunDownload("https://download.scdn.co/SpotifyFullSetupX64.exe", Path.GetTempPath(), "SpotifyFullSetupX64.exe"), () => Spotify == true),

                    // install spotify
                    ("Installing Spotify", async () => spotifyVersion = await Task.Run(() => FileVersionInfo.GetVersionInfo(Environment.ExpandEnvironmentVariables(@"%TEMP%\SpotifyFullSetupX64.exe")).ProductVersion), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"""%TEMP%\SpotifyFullSetupX64.exe"" /extract ""%APPDATA%\Spotify"""), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""DisplayIcon"" /t REG_SZ /d ""%AppData%\Spotify\Spotify.exe"" /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""DisplayName"" /t REG_SZ /d ""Spotify"" /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", $@"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""DisplayVersion"" /t REG_SZ /d ""{spotifyVersion}"" /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""InstallLocation"" /t REG_SZ /d ""%AppData%\Spotify"" /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""NoModify"" /t REG_DWORD /d 1 /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""NoRepair"" /t REG_DWORD /d 1 /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""Publisher"" /t REG_SZ /d ""Spotify AB"" /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""UninstallString"" /t REG_SZ /d ""%AppData%\Spotify\Spotify.exe /uninstall"" /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", @"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""URLInfoAbout"" /t REG_SZ /d ""https://www.spotify.com"" /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunNsudo("CurrentUser", $@"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Spotify"" /v ""Version"" /t REG_SZ /d ""{spotifyVersion}"" /f"), () => Spotify == true),
                    ("Installing Spotify", async () => await ProcessActions.RunPowerShell(@"$Shell = New-Object -ComObject WScript.Shell; $Shortcut = $Shell.CreateShortcut([System.IO.Path]::Combine($env:APPDATA, 'Microsoft\Windows\Start Menu\Programs\Spotify.lnk')); $Shortcut.TargetPath = [System.IO.Path]::Combine($env:APPDATA, 'Spotify\Spotify.exe'); $Shortcut.Save()"), () => Spotify == true),

                    // disable spotify hardware acceleration
                    ("Disabling Spotify hardware acceleration", async () => await File.WriteAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spotify", "prefs"), "ui.hardware_acceleration=false"), () => Spotify == true),

                    // download spotx
                    ("Downloading SpotX", async () => await RunDownload("https://raw.githubusercontent.com/SpotX-Official/SpotX/main/run.ps1", Path.GetTempPath(), "run.ps1"), () => Spotify == true),

                    // install spotx
                    ("Installing SpotX", async () => await ProcessActions.RunPowerShell($@"& $env:TEMP\run.ps1 -new_theme -adsections_off -podcasts_off -block_update_off -version {spotifyVersion}-1234"), () => Spotify == true),
                
                    // remove spotify desktop shortcut
                    ("Removing Spotify desktop shortcut", async () => await ProcessActions.RunNsudo("CurrentUser", @"cmd /c del /f /q ""%HOMEPATH%\Desktop\Spotify.lnk"""), () => Spotify == true),

                    // save services state
                    ("Saving Services & Drivers state ", async () => await Task.Run(() => servicesState = (int)(Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Beep")?.GetValue("Start", 0) ?? 0) == 1), null),
                    ("Saving Services & Drivers state ", async () => await Task.Run(() => wifiState = new[] { "WlanSvc", "Dhcp", "Netman", "NetSetupSvc", "NlaSvc", "Wcmsvc", "WinHttpAutoProxySvc" }.All(service => File.ReadAllLines(list).Any(line => line.Trim() == service)) && new[] { "# tdx", "# vwififlt", "# Netwtw10", "# Netwtw14" }.All(driver => File.ReadAllLines(list).Any(line => line.Trim() == driver))), null),
                    ("Saving Services & Drivers state ", async () => await Task.Run(() => bluetoothState = new[] { "BluetoothUserService", "BTAGService", "BthAvctpSvc", "bthserv", "DeviceAssociationService", "DevicesFlowUserSvc", "DsmSvc", "NcbService", "WFDSConMgrSvc" }.All(service => File.ReadAllLines(list).Any(line => line.Trim() == service)) && new[] { "# BthA2dp", "# BthEnum", "# BthHFAud", "# BthHFEnum", "# BthLEEnum", "# BthMini", "# BTHMODEM", "# BthPan", "# BTHPORT", "# BTHUSB", "# HidBth", "# ibtusb", "# Microsoft_Bluetooth_AvrcpTransport", "# RFCOMM" }.All(driver => File.ReadAllLines(list).Any(line => line.Trim() == driver))), null),
                    ("Saving Services & Drivers state ", async () => await Task.Run(() => laptopState = new[] { "# msisadrv" }.All(driver => File.ReadAllLines(list).Any(line => line.Trim() == driver))), null),
                    ("Saving Services & Drivers state ", async () => await Task.Run(() => apexLegendsState = new[] { "KeyIso" }.All(service => File.ReadAllLines(list).Any(line => line.Trim() == service))), null),

                    // enable services & drivers
                    ("Enabling Services & Drivers", async () => await Process.Start(new ProcessStartInfo { FileName = nsudoPath, Arguments = $"-U:T -P:E -Wait -ShowWindowMode:Hide \"{Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "build", Directory.GetDirectories(Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "build")).OrderByDescending(d => Directory.GetLastWriteTime(d)).FirstOrDefault()?.Split('\\').Last(), "Services-Enable.bat")}\"", CreateNoWindow = true }).WaitForExitAsync(), () => servicesState == false),

                    // update lists.ini
                    ("Updating lists.ini", async () => await Task.Run(() => File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "Service-list-builder", "lists.ini"), Path.Combine(PathHelper.GetAppDataFolderPath(), "Service-list-builder", "lists.ini"), true)), null),
                    ("Updating lists.ini", async () => await File.WriteAllLinesAsync(list, [.. (await File.ReadAllLinesAsync(list)).Select(line => new[] { "WlanSvc", "Dhcp", "Netman", "NetSetupSvc", "NlaSvc", "Wcmsvc", "WinHttpAutoProxySvc" }.Contains(line.Trim().TrimStart('#', ' ')) ? line.TrimStart('#', ' ') : new[] { "tdx", "vwififlt", "Netwtw10", "Netwtw14" }.Contains(line.Trim().TrimStart('#', ' ')) ? "# " + line.TrimStart('#') : line)]), () => wifiState == true),
                    ("Updating lists.ini", async () => await File.WriteAllLinesAsync(list, [.. (await File.ReadAllLinesAsync(list)).Select(line => new[] { "BluetoothUserService", "BTAGService", "BthAvctpSvc", "bthserv", "DeviceAssociationService", "DevicesFlowUserSvc", "DsmSvc", "NcbService", "WFDSConMgrSvc" }.Contains(line.Trim().TrimStart('#', ' ')) ? line.TrimStart('#', ' ') : new[] { "BthA2dp", "BthEnum", "BthHFAud", "BthHFEnum", "BthLEEnum", "BthMini", "BTHMODEM", "BthPan", "BTHPORT", "BTHUSB", "HidBth", "ibtusb", "Microsoft_Bluetooth_AvrcpTransport", "RFCOMM" }.Contains(line.Trim().TrimStart('#', ' ')) ? "# " + line.TrimStart('#') : line)]), () => bluetoothState == true),
                    ("Updating lists.ini", async () => await File.WriteAllLinesAsync(list, [.. (await File.ReadAllLinesAsync(list)).Select(line => new[] { "msisadrv" }.Contains(line.Trim().TrimStart('#', ' ')) ? "# " + line.TrimStart('#') : line)]), () => laptopState == true),
                    ("Updating lists.ini", async () => await File.WriteAllLinesAsync(list, [.. (await File.ReadAllLinesAsync(list)).Select(line => new[] { "KeyIso" }.Contains(line.Trim().TrimStart('#', ' ')) ? line.TrimStart('#') : line)]), () => apexLegendsState == true),

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

                StatusText.Text = "Update complete.";
                ProgressBar.Foreground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemFillColorSuccess"]);
                updater.IsPrimaryButtonEnabled = true;
            }
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