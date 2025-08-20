using System.Diagnostics;
using System.Management;

namespace AutoOS.Views.Settings;

public sealed partial class PowerPage : Page
{
    private bool isInitializingIdleStatesState = true;
    public PowerPage()
    {
        InitializeComponent();
        GetIdleState();
    }

    public async void GetIdleState()
    {
        // hide toggle switch
        IdleStates.Visibility = Visibility.Collapsed;

        // get idle state
        var idleEnabled = await Task.Run(() =>
        {
            var searcher = new ManagementObjectSearcher("SELECT PercentIdleTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='_Total'");
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                if (obj["PercentIdleTime"] != null && Convert.ToInt32(obj["PercentIdleTime"]) > 0)
                    return true;
            }

            return false;
        });

        // hide progress ring
        IdleStatesProgress.Visibility = Visibility.Collapsed;

        // show toggle
        IdleStates.Visibility = Visibility.Visible;

        // toggle idle state
        IdleStates.IsOn = idleEnabled;
        IdleStates.IsEnabled = true;
        isInitializingIdleStatesState = false;
    }

    private async void IdleStates_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingIdleStatesState) return;

        // remove infobar
        PowerInfo.Children.Clear();

        // add infobar
        PowerInfo.Children.Add(new InfoBar
        {
            Title = IdleStates.IsOn ? "Enabling idle states..." : "Disabling idle states...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(5)
        });

        // delay
        await Task.Delay(400);

        // toggle idle state
        using (var process = Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {(IdleStates.IsOn ? "powercfg /setacvalueindex scheme_current sub_processor 5d76a2ca-e8c0-402f-a133-2158492d58ad 0 && powercfg /setactive scheme_current" : "powercfg /setacvalueindex scheme_current sub_processor 5d76a2ca-e8c0-402f-a133-2158492d58ad 1 && powercfg /setactive scheme_current")}",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        }))
        {
            process.WaitForExit();
        }


        // remove infobar
        PowerInfo.Children.Clear();

        // add infobar
        PowerInfo.Children.Add(new InfoBar
        {
            Title = IdleStates.IsOn ? "Successfully enabled idle states." : "Successfully disabled idle states.",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(5)
        });

        // delay 
        await Task.Delay(2000);

        // remove infobar
        PowerInfo.Children.Clear();
    }
}

