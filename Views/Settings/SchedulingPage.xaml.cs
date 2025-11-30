using AutoOS.Views.Settings.Scheduling.Services;
using AutoOS.Views.Settings.Scheduling.ViewModels;
using AutoOS.Views.Settings.Scheduling;

namespace AutoOS.Views.Settings;

public sealed partial class SchedulingPage : Page
{
    public SchedulingPageViewModel ViewModel { get; } = new();

    public SchedulingPage()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }

    private async void Optimize_Checked(object sender, RoutedEventArgs e)
    {
        await Task.Delay(1000);
        await AutoAffinityService.ApplyAutoAffinities(ViewModel);
        Optimize.IsChecked = false;
    }

    private async Task ShowAffinityDialog(DeviceItemViewModel device, SettingsCard senderControl)
    {
        senderControl.IsEnabled = false;
        try
        {
            var schedulingDialog = new SchedulingDialog(device.DeviceType, device.DisplayName, device.DevObjName);
            var contentDialog = new ContentDialog
            {
                Content = schedulingDialog,
                PrimaryButtonText = "Apply",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot,
            };

            contentDialog.Resources["ContentDialogMaxWidth"] = 1350;
            contentDialog.Resources["ContentDialogMaxHeight"] = 900;

            DeviceSettingsService.ApplyResult applyResult = null;
            var applyEventCompleted = new TaskCompletionSource<bool>();
            
            schedulingDialog.ViewModel.OnSettingsApplied += result =>
            {
                applyResult = result;
                applyEventCompleted.TrySetResult(true);
            };

            contentDialog.PrimaryButtonClick += async (_, _) =>
            {
                schedulingDialog.ViewModel.ApplySettings();
                await applyEventCompleted.Task;
                
                if (applyResult != null && applyResult.Success)
                {
                    var devObjName = device.DevObjName ?? string.Empty;
                    ViewModel.UpdateDevice(device.DeviceType, devObjName);
                }
            };

            var result = await contentDialog.ShowAsync();

            if (result == ContentDialogResult.Primary && applyResult != null && applyResult.Success && applyResult.NeedsRestart)
            {
                var restartDialog = new ContentDialog
                {
                    Title = "Restart Device?",
                    Content = "Your changes will not take effect until the device is restarted.\nWould you like to attempt to restart the device now?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = XamlRoot
                };

                if (await restartDialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    var restartDevicesResult = await DeviceSettingsService.RestartDevicesAsync(applyResult.ChangedDevices);
                    
                    var message = restartDevicesResult.SuccessCount > 0 && restartDevicesResult.FailedCount == 0
                        ? "Device successfully restarted."
                        : restartDevicesResult.SuccessCount > 0
                        ? "Device was restarted. A reboot may be required."
                        : "Device could not be restarted. Changes will take effect the next time you reboot.";

                    await new ContentDialog
                    {
                        Title = "Device Restart",
                        Content = message,
                        PrimaryButtonText = "OK",
                        DefaultButton = ContentDialogButton.Primary,
                        XamlRoot = XamlRoot
                    }.ShowAsync();
                }
            }
        }
        finally
        {
            senderControl.IsEnabled = true;
        }
    }

    private async void DeviceCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is SettingsCard card && card.DataContext is DeviceItemViewModel device)
        {
            await ShowAffinityDialog(device, card);
        }
    }
}
