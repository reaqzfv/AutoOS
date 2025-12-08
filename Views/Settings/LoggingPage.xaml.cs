using Microsoft.Win32;
using System.Diagnostics;

namespace AutoOS.Views.Settings;

public sealed partial class LoggingPage : Page
{
    private bool ServicesEnabled = false;
    private bool initialETSState = false;
    private bool isInitializingETSState = true;

    public LoggingPage()
    {
        InitializeComponent();
        GetETSState();
    }
    public async void GetETSState()
    {
        // check services state
        using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Beep"))
        {
            ServicesEnabled = (int)(key?.GetValue("Start", 0) ?? 0) == 1;
        }

        // declare services
        var groups = new[]
        {
            (new[] { "EventLog", "EventSystem" }, 2)
        };

        // ensure services are enabled and check ets state 
        if (ServicesEnabled)
        {
            foreach (var service in groups[0].Item1)
            {
                using (var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{service}", writable: true))
                {
                    if (key != null)
                    {
                        var startValue = key.GetValue("Start", 0);
                        if ((int)startValue != groups[0].Item2)
                        {
                            Registry.SetValue($@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{service}", "Start", groups[0].Item2
                            );
                        }
                    }
                }
            }

            ETS.IsOn = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\WMI\Autologger") != null;
            initialETSState = ETS.IsOn;
            isInitializingETSState = false;
            return;
        }

        // check if values match
        foreach (var group in groups)
        {
            foreach (var service in group.Item1)
            {
                using (var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{service}"))
                {
                    if (key == null) continue;

                    var startValue = key.GetValue("Start");
                    if (startValue == null || (int)startValue != group.Item2)
                    {
                        initialETSState = false;
                        isInitializingETSState = false;
                        return;
                    }
                }
            }
        }

        // check registry
        ETS.IsOn = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\WMI\Autologger") != null;
        initialETSState = ETS.IsOn;

        isInitializingETSState = false;
    }

    private async void ETS_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingETSState) return;

        // remove infobar
        EventTraceSessionsInfo.Children.Clear();

        // add infobar
        EventTraceSessionsInfo.Children.Add(new InfoBar
        {
            Title = ETS.IsOn ? "Enabling Event Trace Sessions (ETS)..." : "Disabling Event Trace Sessions (ETS)...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(5)
        });

        if (!ServicesEnabled)
        {
            // declare services and drivers
            var groups = new[]
            {
                (new[] { "EventLog", "EventSystem" }, 2)
            };

            // set start values
            foreach (var group in groups)
            {
                foreach (var service in group.Item1)
                {
                    using (var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{service}", writable: true))
                    {
                        if (key == null) continue;

                        Registry.SetValue($@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{service}", "Start", ETS.IsOn ? group.Item2 : 4);
                    }
                }
            }
        }

        // toggle event trace sessions
        if (ETS.IsOn)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "NSudo", "NSudoLC.exe"),
                    Arguments = $"-U:T -P:E -Wait -ShowWindowMode:Hide regedit /s \"{Path.Combine(PathHelper.GetAppDataFolderPath(), "EventTraceSessions", "ets-enable.reg")}\"",
                    CreateNoWindow = true,
                }
            };
            process.Start();
        }
        else
        {
            Registry.LocalMachine.DeleteSubKeyTree(@"SYSTEM\CurrentControlSet\Control\WMI\Autologger", false);
        }

        // delay
        await Task.Delay(500);

        // remove infobar
        EventTraceSessionsInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = ETS.IsOn ? "Successfully enabled Event Trace Sessions (ETS)." : "Successfully disabled Event Trace Sessions (ETS).",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(5)
        };
        EventTraceSessionsInfo.Children.Add(infoBar);

        // add restart button if needed
        if (ETS.IsOn != initialETSState)
        {
            infoBar.Title += " A restart is required to apply the change.";
            infoBar.ActionButton = new Button
            {
                Content = "Restart",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            ((Button)infoBar.ActionButton).Click += (s, args) =>
            Process.Start("shutdown", "/r /f /t 0");
        }
        else
        {
            // delay
            await Task.Delay(2000);

            // remove infobar
            EventTraceSessionsInfo.Children.Clear();
        }
    }
}