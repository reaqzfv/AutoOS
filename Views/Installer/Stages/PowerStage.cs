using AutoOS.Views.Installer.Actions;
using Microsoft.UI.Xaml.Media;

namespace AutoOS.Views.Installer.Stages;

public static class PowerStage
{
    public static async Task Run()
    {
        bool? Desktop = PreparingStage.Desktop;
        bool? IdleStates = PreparingStage.IdleStates;

        InstallPage.Status.Text = "Configuring Power Options...";

        string previousTitle = string.Empty;
        int stagePercentage = 2;

        var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
        {
            // power plan
            ("Switching to the high performance power plan", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"), null),
            ("Deleting balanced power scheme", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /delete 381b4222-f694-41f0-9685-ff5bb260df2e"), () => Desktop == true),
            ("Deleting power saver scheme", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /delete a1841308-3541-4fab-bc81-f71556f20b4a"), () => Desktop == true),
            ("Disabling automatic hard disk turn off", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 0012ee47-9041-4b5d-9b77-535fba8b1442 6738e2c4-e8a5-4a42-b16a-e040e769756e 0"), null),
            ("Disabling desktop background slideshow", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 0d7dbae2-4294-402a-ba8e-26777e8488cd 309dce9b-bef4-4119-9921-a851fb12f0f4 1"), null),
            ("Disabling hybrid sleep", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 238c9fa8-0aad-41ed-83f4-97be242c8f20 94ac6d29-73ce-41a6-809f-6363ba21b47e 0"), null),
            ("Disabling wake timers", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 238c9fa8-0aad-41ed-83f4-97be242c8f20 bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0"), null),
            ("Setting power button behavior to shut down", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 4f971e89-eebd-4455-a8de-9e59040e7347 a7066653-8d6c-40a8-910e-a1f54b84c7e5 2"), null),
            ("Disabling automatic display turn off", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 7516b95f-f776-4464-8c53-06167f40cc99 3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e 0"), null),
            ("Disabling critical battery notifications", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current e73a048d-bf27-4f12-9731-8b2076e8891f 5dbb7c9f-38e9-40d2-9749-4f8a0e9f640f 0"), null),
            ("Disabling critical battery actions", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current e73a048d-bf27-4f12-9731-8b2076e8891f 637ea02f-bbcb-4015-8e2c-a1c7b9c0b546 0"), null),
            ("Disabling low battery notifications", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current e73a048d-bf27-4f12-9731-8b2076e8891f bcded951-187b-4d05-bccc-f7e51960c258 0"), null),
            ("Disabling USB 3 link power management", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 2a737441-1930-4402-8d77-b2bebba308a3 d4e98f31-5ffe-4ce1-be31-1b38b384c009 0"), null),
            ("Disabling USB selective suspend", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 0"), null),
            ("Disabling CPU parking", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 54533251-82be-4824-96c1-47b60b740d00 0cc5b647-c1df-4637-891a-dec35c318583 100"), null),
            ("Disabling CPU parking", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 54533251-82be-4824-96c1-47b60b740d00 0cc5b647-c1df-4637-891a-dec35c318584 100"), null),
            ("Increasing the CPU performance time check interval to 5000", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current 54533251-82be-4824-96c1-47b60b740d00 4d2b0152-7d5c-498b-88e2-34345392a2c5 5000"), null),
            ("Disabling idle states", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setacvalueindex scheme_current sub_processor 5d76a2ca-e8c0-402f-a133-2158492d58ad 1"), () => IdleStates == false),
            ("Saving the power plan configuration", async () => await ProcessActions.RunNsudo("CurrentUser", @"powercfg /setactive scheme_current"), null)
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
                            InstallPage.Progress.Foreground = (Brush)Application.Current.Resources["AccentForegroundBrush"];
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
                        InstallPage.Progress.Foreground = (Brush)Application.Current.Resources["AccentForegroundBrush"];
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