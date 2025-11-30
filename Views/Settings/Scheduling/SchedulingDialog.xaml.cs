using AutoOS.Views.Settings.Scheduling.Services;
using AutoOS.Views.Settings.Scheduling.ViewModels;

namespace AutoOS.Views.Settings.Scheduling;

public sealed partial class SchedulingDialog : Page
{
    public DeviceAffinityViewModel ViewModel { get; }
    public string Title { get; }

    public SchedulingDialog(DeviceType deviceType, string title, string targetDevObjName)
    {
        InitializeComponent();
        Title = title;
        ViewModel = new DeviceAffinityViewModel(deviceType, targetDevObjName);
    }

    private void MessageNumberLimit_ValueChanged(NumberBox _, NumberBoxValueChangedEventArgs args)
    {
        ViewModel.MessageNumberLimit = args.NewValue;
    }
}

