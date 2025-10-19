using AutoOS.Helpers;
using AutoOS.Views.Installer.Actions;
using Downloader;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.ServiceProcess;
using Windows.Storage;

namespace AutoOS.Views.Settings;

public sealed partial class GraphicsPage : Page
{
    private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

    private bool isInitializingHDCPState = true;
    private bool isInitializingHDMIDPAudioState = true;

    public GraphicsPage()
    {
        InitializeComponent();
        LoadGpus();
        GetHDCPState();
        GetHDMIDPAudioState();
    }

    private async void LoadGpus()
    {
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
                        Nvidia_SettingsGroup.Visibility = Visibility.Visible;
                        Nvidia_SettingsGroup.Description = "Current Version: " + (await Task.Run(() => Process.Start(new ProcessStartInfo("nvidia-smi", "--query-gpu=driver_version --format=csv,noheader") { CreateNoWindow = true, RedirectStandardOutput = true })?.StandardOutput.ReadToEndAsync()))?.Trim();
                        NvidiaUpdateCheck.IsChecked = true;
                    }
                    if (name.Contains("AMD", StringComparison.OrdinalIgnoreCase) || name.Contains("Radeon", StringComparison.OrdinalIgnoreCase))
                    {
                        Amd_SettingsGroup.Visibility = Visibility.Visible;
                        Amd_SettingsGroup.Description = $"Current Version: {AmdHelper.GetCurrentVersion()}";
                        AmdUpdateCheck.IsChecked = true;
                    }
                    if (name.Contains("Intel", StringComparison.OrdinalIgnoreCase))
                    {
                        Intel_SettingsGroup.Visibility = Visibility.Visible;
                        Intel_SettingsGroup.Description = "Current Version: " + (version?.Split('.')[2] + "." + version?.Split('.')[3]);
                        IntelUpdateCheck.IsChecked = true;
                    }
                }
            }
        }
    }

    private async void NvidiaUpdateCheck_Checked(object sender, RoutedEventArgs e)
    {
        if (NvidiaUpdateCheck.Content.ToString().Contains("Update to"))
        {
            NvidiaUpdateCheck.CheckedContent = NvidiaUpdateCheck.Content.ToString();

            if (new ServiceController("Beep").Status == ServiceControllerStatus.Running)
            {
                var (_, newestVersion, newestDownloadUrl) = await NvidiaHelper.CheckUpdate();

                NvidiaUpdateCheck.IsHitTestVisible = false;

                // download the nvidia driver   
                NvidiaUpdateCheck.CheckedContent = "Downloading the NVIDIA driver...";
                await RunDownload(newestDownloadUrl, Path.GetTempPath(), "driver.exe");

                // extract the driver
                NvidiaUpdateCheck.CheckedContent = "Extracting the NVIDIA driver...";
                await ProcessActions.RunExtract(Path.Combine(Path.GetTempPath(), "driver.exe"), Path.Combine(Path.GetTempPath(), "driver"));


                // strip the driver
                NvidiaUpdateCheck.CheckedContent = "Stripping the NVIDIA driver...";
                await ProcessActions.RunNvidiaStrip();

                // close obs studio
                if (Process.GetProcessesByName("obs64").Length > 0)
                {
                    foreach (var process in Process.GetProcessesByName("obs64"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }

                int gpuAffinity = -1;

                foreach (ManagementObject obj in new ManagementObjectSearcher("SELECT PNPDeviceID FROM Win32_VideoController").Get().Cast<ManagementObject>())
                {
                    string path = obj["PNPDeviceID"]?.ToString();
                    if (path?.StartsWith("PCI\\VEN_") != true)
                        continue;

                    using var affinityKey = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Enum\{path}\Device Parameters\Interrupt Management\Affinity Policy");
                    if (affinityKey?.GetValue("AssignmentSetOverride") is byte[] value && value.Length > 0)
                    {
                        for (int i = 0; i < value.Length; i++)
                        {
                            for (int bit = 0; bit < 8; bit++)
                            {
                                if ((value[i] & (1 << bit)) != 0)
                                {
                                    gpuAffinity = i * 8 + bit;
                                    break;
                                }
                            }
                            if (gpuAffinity != -1) break;
                        }
                    }
                }

                // update driver
                NvidiaUpdateCheck.CheckedContent = "Updating the NVIDIA driver...";
                await ProcessActions.RunNsudo("CurrentUser", @"""%TEMP%\driver\setup.exe"" /s");
                await ProcessActions.Sleep(3000);
                Nvidia_SettingsGroup.Description = "Current Version: " + (await Task.Run(() => Process.Start(new ProcessStartInfo("nvidia-smi", "--query-gpu=driver_version --format=csv,noheader") { CreateNoWindow = true, RedirectStandardOutput = true })?.StandardOutput.ReadToEndAsync()))?.Trim();

                // reapply gpu affinity
                NvidiaUpdateCheck.CheckedContent = "Reapplying GPU Affinity...";
                var process2 = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $@"/c {Path.Combine(PathHelper.GetAppDataFolderPath(), "AutoGpuAffinity", "AutoGpuAffinity.exe")} --apply-affinity {gpuAffinity}",
                        CreateNoWindow = true
                    }
                };

                process2.Start();
                await process2.WaitForExitAsync();

                // launch obs studio
                await Task.Run(() => Process.Start(new ProcessStartInfo("cmd.exe") { Arguments = @"/c del ""%APPDATA%\obs-studio\.sentinel"" /s /f /q" })?.WaitForExit());
                await Task.Run(() => Process.Start(new ProcessStartInfo { FileName = @"C:\Program Files\obs-studio\bin\64bit\obs64.exe", Arguments = "--disable-updater --startreplaybuffer --minimize-to-tray", WorkingDirectory = @"C:\Program Files\obs-studio\bin\64bit" }));

                NvidiaUpdateCheck.IsHitTestVisible = true;
                NvidiaUpdateCheck.IsChecked = false;
                NvidiaUpdateCheck.Content = "Checking for updates";
                NvidiaUpdateCheck.IsChecked = true;
            }
            else
            {
                NvidiaUpdateCheck.IsChecked = false;

                var dialog = new ContentDialog
                {
                    Title = "Services & Drivers are disabled",
                    Content = "Please enable Services & Drivers before updating.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
        else
        {
            NvidiaUpdateCheck.CheckedContent = "Checking for updates...";

            try
            {
                var (currentVersion, newestVersion, newestDownloadUrl) = await NvidiaHelper.CheckUpdate();

                // delay
                await Task.Delay(800);

                // check if update is needed
                if (string.Compare(newestVersion, currentVersion, StringComparison.Ordinal) > 0)
                {
                    NvidiaUpdateCheck.IsChecked = false;
                    NvidiaUpdateCheck.Content = "Update to " + newestVersion;
                }
                else if (string.Compare(newestVersion, currentVersion, StringComparison.Ordinal) == 0)
                {
                    NvidiaUpdateCheck.IsChecked = false;
                    NvidiaUpdateCheck.Content = "No updates available";
                }
            }
            catch
            {
                // delay
                await Task.Delay(800);

                NvidiaUpdateCheck.IsChecked = false;
                NvidiaUpdateCheck.Content = "Failed to check for updates";
            }
        }
    }

    public async Task RunDownload(string url, string path, string file)
    {
        var uiContext = SynchronizationContext.Current;

        var download = DownloadBuilder.New()
            .WithUrl(url)
            .WithDirectory(path)
            .WithFileName(file)
            .WithConfiguration(new DownloadConfiguration())
            .Build();

        double speedMB = 0.0;
        double receivedMB = 0.0;
        double totalMB = 0.0;
        double percentage = 0.0;

        DateTime lastLoggedTime = DateTime.MinValue;

        download.DownloadProgressChanged += (sender, e) =>
        {
            if ((DateTime.Now - lastLoggedTime).TotalMilliseconds < 50) return;

            lastLoggedTime = DateTime.Now;

            speedMB = e.BytesPerSecondSpeed / (1024.0 * 1024.0);
            receivedMB = e.ReceivedBytesSize / (1024.0 * 1024.0);
            totalMB = e.TotalBytesToReceive / (1024.0 * 1024.0);
            percentage = e.ProgressPercentage;

            uiContext?.Post(_ =>
            {
                NvidiaUpdateCheck.IsIndeterminate = false;
                NvidiaUpdateCheck.Progress = percentage;
            }, null);
        };

        download.DownloadFileCompleted += (sender, e) =>
        {
            uiContext?.Post(_ =>
            {
                NvidiaUpdateCheck.Progress = 100;
                NvidiaUpdateCheck.IsIndeterminate = true;
            }, null);
        };

        await download.StartAsync();
    }
    private async void AmdUpdateCheck_Checked(object sender, RoutedEventArgs e)
    {
        if (AmdUpdateCheck.Content.ToString().Contains("Update to"))
        {
            if (new ServiceController("Beep").Status == ServiceControllerStatus.Running)
            {
                var dialog = new ContentDialog
                {
                    Title = "Not implemented yet",
                    Content = "AMD Driver Update functionality has not been added yet.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await dialog.ShowAsync();

                AmdUpdateCheck.IsHitTestVisible = true;
                AmdUpdateCheck.IsChecked = false;
                AmdUpdateCheck.Content = "Checking for updates";
                AmdUpdateCheck.IsChecked = true;
            }
            else
            {
                AmdUpdateCheck.IsChecked = false;

                var dialog = new ContentDialog
                {
                    Title = "Services & Drivers are disabled",
                    Content = "Please enable Services & Drivers before updating.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
        else
        {
            AmdUpdateCheck.CheckedContent = "Checking for updates...";

            try
            {
                var (currentVersion, newestVersion, newestDownloadUrl) = await AmdHelper.CheckUpdate();

                // delay
                await Task.Delay(800);

                // check if update is needed
                if (string.Compare(newestVersion, currentVersion, StringComparison.Ordinal) > 0)
                {
                    AmdUpdateCheck.IsChecked = false;
                    AmdUpdateCheck.Content = "Update to " + newestVersion;
                }
                else if (string.Compare(newestVersion, currentVersion, StringComparison.Ordinal) == 0)
                {
                    AmdUpdateCheck.IsChecked = false;
                    AmdUpdateCheck.Content = "No updates available";
                }
            }
            catch
            {
                // delay
                await Task.Delay(800);

                AmdUpdateCheck.IsChecked = false;
                AmdUpdateCheck.Content = "Failed to check for updates";
            }
        }
    }

    private async void IntelUpdateCheck_Checked(object sender, RoutedEventArgs e)
    {
        if (IntelUpdateCheck.Content.ToString().Contains("Update to"))
        {
            if (new ServiceController("Beep").Status == ServiceControllerStatus.Running)
            {
                var dialog = new ContentDialog
                {
                    Title = "Not implemented yet",
                    Content = "Intel Driver Update functionality has not been added yet.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await dialog.ShowAsync();

                IntelUpdateCheck.IsHitTestVisible = true;
                IntelUpdateCheck.IsChecked = false;
                IntelUpdateCheck.Content = "Checking for updates";
                IntelUpdateCheck.IsChecked = true;
            }
            else
            {
                IntelUpdateCheck.IsChecked = false;

                var dialog = new ContentDialog
                {
                    Title = "Services & Drivers are disabled",
                    Content = "Please enable Services & Drivers before updating.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
        else
        {
            IntelUpdateCheck.CheckedContent = "Checking for updates...";

            try
            {
                string currentVersion = Intel_SettingsGroup.Description.ToString();
                string newestVersion = Intel_SettingsGroup.Description.ToString();

                // delay
                await Task.Delay(800);

                // check if update is needed
                if (string.Compare(newestVersion, currentVersion, StringComparison.Ordinal) > 0)
                {
                    IntelUpdateCheck.IsChecked = false;
                    IntelUpdateCheck.Content = "Update to " + newestVersion;
                }
                else if (string.Compare(newestVersion, currentVersion, StringComparison.Ordinal) == 0)
                {
                    IntelUpdateCheck.IsChecked = false;
                    IntelUpdateCheck.Content = "No updates available";
                }
            }
            catch
            {
                // delay
                await Task.Delay(800);

                IntelUpdateCheck.IsChecked = false;
                IntelUpdateCheck.Content = "Failed to check for updates";
            }
        }
    }

    private void GetHDCPState()
    {
        // get registry values
        for (int i = 0; i <= 9; i++)
        {
            if (Registry.GetValue($@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{{4d36e968-e325-11ce-bfc1-08002be10318}}\000{i}", "ProviderName", null)?.ToString() == "NVIDIA" &&
                (int?)Registry.GetValue($@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{{4d36e968-e325-11ce-bfc1-08002be10318}}\000{i}", "RMHdcpKeyglobZero", null) == 0)
            {
                HDCP.IsOn = true;
            }
        }
        isInitializingHDCPState = false;
    }

    private async void HDCP_Toggled(object sender, RoutedEventArgs e)
    {
        // return if still initializing
        if (isInitializingHDCPState) return;

        // remove infobar
        GpuInfo.Children.Clear();

        // add infobar
        GpuInfo.Children.Add(new InfoBar
        {
            Title = HDCP.IsOn ? "Enabling High-Bandwidth Digital Content Protection (HDCP)..." : "Disabling High-Bandwidth Digital Content Protection (HDCP)...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(5)
        });

        // toggle hdcp
        for (int i = 0; i <= 9; i++)
        {
            var path = $@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{{4d36e968-e325-11ce-bfc1-08002be10318}}\000{i}";
            if (Registry.GetValue(path, "ProviderName", null)?.ToString() == "NVIDIA")
            {
                if (HDCP.IsOn)
                {
                    Registry.SetValue(path, "RMHdcpKeyglobZero", 0, RegistryValueKind.DWord);
                    using var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Control\Class\{{4d36e968-e325-11ce-bfc1-08002be10318}}\000{i}", true);
                    key?.DeleteValue("RmDisableHdcp22", false);
                    key?.DeleteValue("RmSkipHdcp22Init", false);
                }
                else
                {
                    Registry.SetValue(path, "RMHdcpKeyglobZero", 1, RegistryValueKind.DWord);
                    Registry.SetValue(path, "RmDisableHdcp22", 1, RegistryValueKind.DWord);
                    Registry.SetValue(path, "RmSkipHdcp22Init", 1, RegistryValueKind.DWord);
                }
            }
        }

        // close obs studio
        if (Process.GetProcessesByName("obs64").Length > 0)
        {
            foreach (var process in Process.GetProcessesByName("obs64"))
            {
                process.Kill();
                process.WaitForExit();
            }
        }

        // delay
        await Task.Delay(400);

        // restart driver
        await Task.Run(() => Process.Start(new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "CRU", "restart64.exe")) { Arguments = "/q" })?.WaitForExit());

        // apply profile
        if (localSettings.Values["MsiProfile"] != null)
        {
            await Task.Run(() => Process.Start(new ProcessStartInfo(@"C:\Program Files (x86)\MSI Afterburner\MSIAfterburner.exe") { Arguments = "/Profile1 /q" })?.WaitForExit());
        }

        // launch obs studio
        await Task.Run(() => Process.Start(new ProcessStartInfo("cmd.exe") { Arguments = @"/c del ""%APPDATA%\obs-studio\.sentinel"" /s /f /q" })?.WaitForExit());
        await Task.Run(() => Process.Start(new ProcessStartInfo { FileName = @"C:\Program Files\obs-studio\bin\64bit\obs64.exe", Arguments = "--disable-updater --startreplaybuffer --minimize-to-tray", WorkingDirectory = @"C:\Program Files\obs-studio\bin\64bit" }));

        // remove infobar
        GpuInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = HDCP.IsOn ? "Successfully enabled High-Bandwidth Digital Content Protection (HDCP)." : "Successfully disabled High-Bandwidth Digital Content Protection (HDCP).",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(5)
        };
        GpuInfo.Children.Add(infoBar);

        // delay
        await Task.Delay(2000);

        // remove infobar
        GpuInfo.Children.Clear();
    }

    private async void GetHDMIDPAudioState()
    {
        var toggles = new[] { NVIDIA_HDMIDPAudio, AMD_HDMIDPAudio, INTEL_HDMIDPAudio };

        foreach (var toggle in toggles)
        {
            toggle.IsOn = await Task.Run(() =>
            {
                return new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PnPEntity WHERE Description = 'High Definition Audio Device'")
                       .Get()
                       .Cast<ManagementObject>()
                       .Any(device => device["Status"]?.ToString() == "OK");
            });
        }

        isInitializingHDMIDPAudioState = false;
    }

    private async void HDMIDPAudio_Toggled(object sender, RoutedEventArgs e)
    {
        // return if still initializing
        if (isInitializingHDMIDPAudioState) return;

        var toggle = sender as ToggleSwitch;

        // remove infobar
        GpuInfo.Children.Clear();

        // add infobar
        GpuInfo.Children.Add(new InfoBar
        {
            Title = toggle.IsOn ? "Enabling High-Definition Multimedia Interface (HDMI)/DisplayPort (DP) Audio..." : "Disabling High-Definition Multimedia Interface (HDMI)/DisplayPort (DP) Audio...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(5)
        });

        // toggle hdmi/dp audio
        bool isOn = toggle.IsOn;

        await Task.Run(() =>
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = @$"-ExecutionPolicy Bypass -Command ""Get-PnpDevice | Where-Object {{ $_.FriendlyName -eq 'High Definition Audio Device' }} | {(isOn ? "Enable" : "Disable")}-PnpDevice -Confirm:$false""",
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        });

        // delay
        await Task.Delay(400);

        // remove infobar
        GpuInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = toggle.IsOn ? "Successfully enabled High-Definition Multimedia Interface (HDMI)/DisplayPort (DP) Audio." : "Successfully disabled High-Definition Multimedia Interface (HDMI)/DisplayPort (DP) Audio.",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(5)
        };
        GpuInfo.Children.Add(infoBar);

        // delay
        await Task.Delay(2000);

        // remove infobar
        GpuInfo.Children.Clear();
    }

    private async void BrowseMsi_Click(object sender, RoutedEventArgs e)
    {
        // disable the button to avoid double-clicking
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;

        // remove infobar
        MsiAfterburnerInfo.Children.Clear();

        // add infobar
        MsiAfterburnerInfo.Children.Add(new InfoBar
        {
            Title = "Please select a MSI Afterburner profile (.cfg).",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(5)
        });

        // delay
        await Task.Delay(300);

        // launch file picker
        var picker = new FilePicker(App.MainWindow)
        {
            ShowAllFilesOption = false
        };
        picker.FileTypeChoices.Add("MSI Afterburner profile", new List<string> { "*.cfg" });
        var file = await picker.PickSingleFileAsync();

        if (file != null)
        {
            string fileContent = await FileIO.ReadTextAsync(file);

            if (fileContent.Contains("[Startup]"))
            {
                // re-enable the button
                senderButton.IsEnabled = true;

                // remove infobar
                MsiAfterburnerInfo.Children.Clear();

                // add infobar
                MsiAfterburnerInfo.Children.Add(new InfoBar
                {
                    Title = "Applying the MSI Afterburner profile...",
                    IsClosable = false,
                    IsOpen = true,
                    Severity = InfoBarSeverity.Informational,
                    Margin = new Thickness(5)
                });

                // delay
                await Task.Delay(300);

                // delete old profiles
                Directory.GetFiles(@"C:\Program Files (x86)\MSI Afterburner\Profiles")
                .Where(file => Path.GetFileName(file) != "MSIAfterburner.cfg")
                .ToList()
                .ForEach(File.Delete);

                // import profile
                File.Copy(file.Path, Path.Combine(@"C:\Program Files (x86)\MSI Afterburner\Profiles", file.Name), true);

                // apply profile
                await Task.Run(() => Process.Start(new ProcessStartInfo(@"C:\Program Files (x86)\MSI Afterburner\MSIAfterburner.exe") { Arguments = "/Profile1 /q" })?.WaitForExit());

                // remove infobar
                MsiAfterburnerInfo.Children.Clear();

                // add infobar
                MsiAfterburnerInfo.Children.Add(new InfoBar
                {
                    Title = "Successfully applied the MSI Afterburner profile.",
                    IsClosable = false,
                    IsOpen = true,
                    Severity = InfoBarSeverity.Success,
                    Margin = new Thickness(5)
                });

                // delay
                await Task.Delay(2000);

                // remove infobar
                MsiAfterburnerInfo.Children.Clear();
            }
            else
            {
                // re-enable the button
                senderButton.IsEnabled = true;

                // remove infobar
                MsiAfterburnerInfo.Children.Clear();

                // add infobar
                MsiAfterburnerInfo.Children.Add(new InfoBar
                {
                    Title = "The selected file is not a valid MSI Afterburner profile.",
                    IsClosable = false,
                    IsOpen = true,
                    Severity = InfoBarSeverity.Error,
                    Margin = new Thickness(5)
                });

                // delay
                await Task.Delay(2000);

                // remove infobar
                MsiAfterburnerInfo.Children.Clear();
            }
        }
        else
        {
            // re-enable the button
            senderButton.IsEnabled = true;

            // remove infobar
            MsiAfterburnerInfo.Children.Clear();
        }
    }

    private async void LaunchMsi_Click(object sender, RoutedEventArgs e)
    {
        // remove infobar
        MsiAfterburnerInfo.Children.Clear();

        // add infobar
        MsiAfterburnerInfo.Children.Add(new InfoBar
        {
            Title = "Launching MSI Afterburner...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(5)
        });

        // launch
        await Task.Run(() => Process.Start(@"C:\Program Files (x86)\MSI Afterburner\MSIAfterburner.exe")?.WaitForInputIdle());

        // remove infobar
        MsiAfterburnerInfo.Children.Clear();

        // add infobar
        MsiAfterburnerInfo.Children.Add(new InfoBar
        {
            Title = "Successfully launched MSI Afterburner.",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(5)
        });

        // delay
        await Task.Delay(2000);

        // remove infobar
        MsiAfterburnerInfo.Children.Clear();
    }
}

