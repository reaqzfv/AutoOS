using Microsoft.Win32;
using System.Diagnostics;
using Windows.Storage;

namespace AutoOS.Views.Settings;

public sealed partial class UpdatePage : Page
{
    private bool isInitializingWindowsUpdateState = true;
    private bool isInitializingTargetVersion = true;

    private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

    public UpdatePage()
    {
        InitializeComponent();
        GetWindowsUpdateState();
        GetTargetVersion();
    }

    private void GetWindowsUpdateState()
    {
        // check registry
        if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "PauseUpdatesExpiryTime", null) == null)
        {
            WindowsUpdate.IsOn = true;
        }

        isInitializingWindowsUpdateState = false;
    }

    private async void Update_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingWindowsUpdateState) return;

        // remove infobar
        WindowsUpdateInfo.Children.Clear();

        // add infobar
        WindowsUpdateInfo.Children.Add(new InfoBar
        {
            Title = WindowsUpdate.IsOn ? "Enabling Windows Update..." : "Disabling Windows Update...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(5)
        });

        // toggle registry value
        localSettings.Values["PauseWindowsUpdate"] = WindowsUpdate.IsOn ? 0 : 1;

        // toggle windows update
        if (WindowsUpdate.IsOn)
        {
            // delete registry keys
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", true);
            key?.DeleteValue("PauseFeatureUpdatesStartTime", false);
            key?.DeleteValue("PauseFeatureUpdatesEndTime", false);
            key?.DeleteValue("PauseQualityUpdatesStartTime", false);
            key?.DeleteValue("PauseQualityUpdatesEndTime", false);
            key?.DeleteValue("PauseUpdatesStartTime", false);
            key?.DeleteValue("PauseUpdatesExpiryTime", false);
            key?.Close();

            // delay
            await Task.Delay(500);
        }
        else
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Scripts", "pausewindowsupdates.ps1")}\"",
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();
        }

        // delay
        await Task.Delay(200);

        // remove infobar
        WindowsUpdateInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = WindowsUpdate.IsOn ? "Successfully enabled Windows Update." : "Successfully disabled Windows Update.",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(5)
        };
        WindowsUpdateInfo.Children.Add(infoBar);

        // delay
        await Task.Delay(2000);

        // remove infobar
        WindowsUpdateInfo.Children.Clear();
    }

    private void ReadMoreButton_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/Duckleeng/TweakCollection/tree/main/Research#windows-11-24h2-autoboost-behavior",
            UseShellExecute = true
        });
    }

    private void GetTargetVersion()
    {
        string version = "Default";

        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", false);

        if (key?.GetValue("TargetReleaseVersion") is int trv && trv == 1)
        {
            version = key.GetValue("TargetReleaseVersionInfo") as string ?? "Default";
        }

        TargetVersion.SelectedIndex = version switch
        {
            "23H2" => 1,
            "24H2" => 2,
            "25H2" => 3,
            _ => 0
        };

        isInitializingTargetVersion = false;
    }

    private void TargetVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isInitializingTargetVersion) return;

        if (TargetVersion.SelectedItem is ComboBoxItem selectedItem)
        {
            string version = selectedItem.Content.ToString();

            using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", true);

            if (version == "Default")
            {
                key?.DeleteValue("TargetReleaseVersion", false);
                key?.DeleteValue("TargetReleaseVersionInfo", false);
            }
            else
            {
                key?.SetValue("TargetReleaseVersion", 1, RegistryValueKind.DWord);
                key?.SetValue("TargetReleaseVersionInfo", version, RegistryValueKind.String);
            }
        }
    }
}