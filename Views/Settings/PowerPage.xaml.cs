using Microsoft.Win32;
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
        // get active scheme
        string activeScheme = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes", "ActivePowerScheme", "").ToString();

        // get idle state
        int value = (int?)Registry.GetValue($@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes\{activeScheme}\54533251-82be-4824-96c1-47b60b740d00\5d76a2ca-e8c0-402f-a133-2158492d58ad", "ACSettingIndex", 0) ?? 0;

        IdleStates.IsOn = value == 0;
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
            Margin = new Thickness(4, -28, 4, 36)
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
            Margin = new Thickness(4, -28, 4, 36)
        });

        // delay 
        await Task.Delay(2000);

        // remove infobar
        PowerInfo.Children.Clear();
    }
}