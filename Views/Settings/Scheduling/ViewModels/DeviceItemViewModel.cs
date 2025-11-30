using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoOS.Views.Settings.Scheduling.Models;
using AutoOS.Views.Settings.Scheduling.Services;

namespace AutoOS.Views.Settings.Scheduling.ViewModels;

public class DeviceItemViewModel : INotifyPropertyChanged
{
    private static readonly Dictionary<uint, string> PolicyNames = new()
    {
        { 0, "Default" },
        { 1, "All Close Proc" },
        { 2, "One Close Proc" },
        { 3, "All Proc in Machine" },
        { 4, "Specified Proc" },
        { 5, "Spread Messages Across All Proc" }
    };

    private static readonly Dictionary<uint, string> PriorityNames = new()
    {
        { 0, "Undefined" },
        { 1, "Low" },
        { 2, "Normal" },
        { 3, "High" }
    };

    public DeviceType DeviceType { get; }
    public string DeviceDesc { get; }
    public string FriendlyName { get; }
    public string DevObjName { get; }
    public string LocationInformation { get; }

    private DeviceSettings _settings;

    public DeviceSettings Settings
    {
        get => _settings;
        private set
        {
            if (ReferenceEquals(_settings, value))
                return;
                
            _settings = value;
            OnPropertyChanged(nameof(Settings));
            OnPropertyChanged(nameof(MsiModeDisplay));
            OnPropertyChanged(nameof(MsiLimitDisplay));
            OnPropertyChanged(nameof(DevicePolicyDisplay));
            OnPropertyChanged(nameof(DevicePriorityDisplay));
            OnPropertyChanged(nameof(SpecifiedProcessorsDisplay));
            OnPropertyChanged(nameof(MaxMSILimitDisplay));
        }
    }

    public string DisplayName => string.IsNullOrWhiteSpace(FriendlyName) ? DeviceDesc : FriendlyName;
    public string MsiModeDisplay => Settings.MsiSupported == 1 ? "On" : "Off";
    public string MsiLimitDisplay => Settings.MessageNumberLimit == 0 ? "Auto" : Settings.MessageNumberLimit.ToString("F0");
    public string DevicePolicyDisplay => PolicyNames.TryGetValue(Settings.DevicePolicy, out var name) ? name : $"{Settings.DevicePolicy}";
    public string DevicePriorityDisplay => PriorityNames.TryGetValue(Settings.DevicePriority, out var name) ? name : $"{Settings.DevicePriority}";
    public string SpecifiedProcessorsDisplay => FormatProcessMask(Settings.AssignmentSetOverride);
    public string MaxMSILimitDisplay => Settings.MaxMSILimit == 0 ? string.Empty : Settings.MaxMSILimit.ToString("F0");

    public DeviceItemViewModel(DeviceType deviceType, DeviceInfo device)
    {
        DeviceType = deviceType;
        DeviceDesc = device.DeviceDesc;
        FriendlyName = device.FriendlyName;
        DevObjName = device.DevObjName;
        LocationInformation = device.LocationInformation;

        Settings = device.RegistryKey != null
            ? RegistryService.ReadDeviceSettings(device.RegistryKey, device.MaxMSILimit)
            : new DeviceSettings { MsiSupported = 2u, MaxMSILimit = device.MaxMSILimit };
    }

    public void RefreshSettings(DeviceInfo device)
    {
        if (device.RegistryKey != null)
            Settings = RegistryService.ReadDeviceSettings(device.RegistryKey, device.MaxMSILimit);
    }

    private static string FormatProcessMask(ulong mask)
    {
        if (mask == 0) return string.Empty;
        
        var processors = new List<string>();
        for (int index = 0; mask != 0; index++, mask >>= 1)
        {
            if ((mask & 1UL) != 0)
                processors.Add(index.ToString());
        }
        return string.Join(", ", processors);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
