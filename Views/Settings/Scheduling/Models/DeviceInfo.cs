using AutoOS.Views.Settings.Scheduling.Services;
using Microsoft.Win32;

namespace AutoOS.Views.Settings.Scheduling.Models;

public class DeviceInfo
{
    public IntPtr DeviceInfoSet { get; set; }
    public SP_DEVINFO_DATA DeviceInfoData { get; set; }
    public RegistryKey RegistryKey { get; set; }
    public string DeviceDesc { get; set; } = string.Empty;
    public string DevObjName { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string LocationInformation { get; set; } = string.Empty;
    public string PnpDeviceId { get; set; } = string.Empty;
    public uint MaxMSILimit { get; set; }
}

