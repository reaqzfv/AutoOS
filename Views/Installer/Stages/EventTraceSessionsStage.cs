using AutoOS.Views.Installer.Actions;
using Microsoft.UI.Xaml.Media;

namespace AutoOS.Views.Installer.Stages;

public static class EventTraceSessionsStage
{
    public static async Task Run()
    {
        InstallPage.Status.Text = "Configuring Event Trace Sessions...";

        string previousTitle = string.Empty;
        int stagePercentage = 2;

        var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
        {
            // saving event trace session (ets) data
            ("Saving Event Trace Session (ETS) data", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"reg export ""HKLM\SYSTEM\CurrentControlSet\Control\WMI\Autologger"" ""C:\ets-enable.reg"""), null),
            ("Saving Event Trace Session (ETS) data", async () => await Task.Run(() => Directory.CreateDirectory(Path.Combine(PathHelper.GetAppDataFolderPath(), "EventTraceSessions"))), null),
            ("Saving Event Trace Session (ETS) data", async () => await ProcessActions.RunNsudo("TrustedInstaller", @$"cmd /c move ""C:\ets-enable.reg"" ""{Path.Combine(PathHelper.GetAppDataFolderPath(), "EventTraceSessions", "ets-enable.reg")}"""), null),
            ("Saving Event Trace Session (ETS) data", async () => await ProcessActions.Sleep(500), null),

            // disable event trace sessions
            ("Disabling Event Trace Sessions (ETS)", async () => await ProcessActions.RunPowerShell(@"Get-EventLog -LogName * | ForEach-Object { Clear-EventLog $_.Log }"), null),
            ("Disabling Event Trace Sessions (ETS)", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"powershell -Command ""Get-ChildItem -Path ""$env:SystemRoot"" -Filter *.log -File -Recurse -Force | Remove-Item -Recurse -Force"""), null),
            ("Disabling Event Trace Sessions (ETS)", async () => await ProcessActions.RunNsudo("TrustedInstaller", $"cmd /c reg import \"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Scripts", "ets-disable.reg")}\""), null),
            ("Disabling Event Trace Sessions (ETS)", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog"" /v Start /t REG_DWORD /d 4 /f & sc stop EventLog"), null),
            ("Disabling Event Trace Sessions (ETS)", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"cmd /c reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventSystem"" /v Start /t REG_DWORD /d 4 /f & sc stop EventSystem"), null),
            ("Disabling Event Trace Sessions (ETS)", async () => await ProcessActions.Sleep(500), null),

            // disable sleep study
            ("Disabling sleep study", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"wevtutil sl Microsoft-Windows-SleepStudy/Diagnostic /e:false"), null),
            ("Disabling sleep study", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"wevtutil sl Microsoft-Windows-Kernel-Processor-Power/Diagnostic /e:false"), null),
            ("Disabling sleep study", async () => await ProcessActions.RunNsudo("TrustedInstaller", @"wevtutil sl Microsoft-Windows-UserModePowerService/Diagnostic /e:false"), null),
            ("Disabling sleep study", async () => await ProcessActions.Sleep(500), null),
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

        double incrementPerTitle = groupedTitleCount > 0 ? stagePercentage / (double)groupedTitleCount : 0;

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
                        InstallPage.Info.Title += ": " + ex.Message;
                        InstallPage.Info.Severity = InfoBarSeverity.Error;
                        InstallPage.Progress.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                        InstallPage.ProgressRingControl.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                        InstallPage.ProgressRingControl.Visibility = Visibility.Collapsed;
                        InstallPage.ResumeButton.Visibility = Visibility.Visible;

                        var tcs = new TaskCompletionSource<bool>();

                        InstallPage.ResumeButton.Click += (sender, e) =>
                        {
                            tcs.TrySetResult(true);
                            InstallPage.Info.Severity = InfoBarSeverity.Informational;
                            InstallPage.Progress.ClearValue(ProgressBar.ForegroundProperty);
                            InstallPage.ProgressRingControl.Foreground = null;
                            InstallPage.ProgressRingControl.Visibility = Visibility.Visible;
                            InstallPage.ResumeButton.Visibility = Visibility.Collapsed;
                        };

                        await tcs.Task;
                    }
                }

                InstallPage.Progress.Value += incrementPerTitle;
                await Task.Delay(150);
                currentGroup.Clear();
            }

            InstallPage.Info.Title = title + "...";
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
                    InstallPage.Info.Title += ": " + ex.Message;
                    InstallPage.Info.Severity = InfoBarSeverity.Error;
                    InstallPage.Progress.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                    InstallPage.ProgressRingControl.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                    InstallPage.ProgressRingControl.Visibility = Visibility.Collapsed;
                    InstallPage.ResumeButton.Visibility = Visibility.Visible;

                    var tcs = new TaskCompletionSource<bool>();

                    InstallPage.ResumeButton.Click += (sender, e) =>
                    {
                        tcs.TrySetResult(true);
                        InstallPage.Info.Severity = InfoBarSeverity.Informational;
                        InstallPage.Progress.ClearValue(ProgressBar.ForegroundProperty);
                        InstallPage.ProgressRingControl.Foreground = null;
                        InstallPage.ProgressRingControl.Visibility = Visibility.Visible;
                        InstallPage.ResumeButton.Visibility = Visibility.Collapsed;
                    };

                    await tcs.Task;
                }
            }

            InstallPage.Progress.Value += incrementPerTitle;
        }
    }
}