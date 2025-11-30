using System.Management;
using System.Runtime.InteropServices;
using AutoOS.Views.Settings.Scheduling.Models;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace AutoOS.Views.Settings.Scheduling.Services;

public enum DeviceType
{
    GPU,
    XHCI,
    NIC
}

public class DeviceDetectionService
{
    public static List<DeviceInfo> FindDevicesByType(DeviceType deviceType)
    {
        var devices = new List<DeviceInfo>();

        var pnpDeviceIds = GetPnpDeviceIdsFromWmi(deviceType);

        IntPtr deviceInfoSet = SetupApi.SetupDiGetClassDevs(
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero,
            DIGCF.DIGCF_ALLCLASSES | DIGCF.DIGCF_PRESENT
        );

        if (deviceInfoSet == IntPtr.Zero || deviceInfoSet == new IntPtr(-1))
        {
            return devices;
        }

        uint index = 0;
        while (true)
        {
            var deviceInfoData = new SP_DEVINFO_DATA
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVINFO_DATA))
            };

            if (!SetupApi.SetupDiEnumDeviceInfo(deviceInfoSet, index, ref deviceInfoData))
            {
                int error = Marshal.GetLastWin32Error();
                if (error == 259)
                    break;
            }

            index++;

            var device = GetDeviceInfo(deviceInfoSet, ref deviceInfoData);
            if (device == null)
                continue;

            if (pnpDeviceIds.Count == 0)
                continue;

            bool matches = false;

            if (!string.IsNullOrEmpty(device.PnpDeviceId))
            {
                matches = pnpDeviceIds.Any(id =>
                    device.PnpDeviceId.Equals(id, StringComparison.OrdinalIgnoreCase) ||
                    device.PnpDeviceId.StartsWith(id, StringComparison.OrdinalIgnoreCase) ||
                    id.StartsWith(device.PnpDeviceId, StringComparison.OrdinalIgnoreCase));
            }

            if (!matches && !string.IsNullOrEmpty(device.DevObjName))
            {
                var lastPart = device.DevObjName.Split('\\').LastOrDefault();
                if (lastPart != null)
                {
                    matches = pnpDeviceIds.Any(id =>
                        id.Contains(lastPart, StringComparison.OrdinalIgnoreCase) ||
                        device.DevObjName.Contains(id, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (!matches) continue;

            if (deviceType == DeviceType.GPU && (device.DeviceDesc?.Contains("Microsoft Basic Display Adapter", StringComparison.OrdinalIgnoreCase) ?? false))
                continue;

            if (deviceType == DeviceType.NIC && (device.DeviceDesc?.Contains("Bluetooth", StringComparison.OrdinalIgnoreCase) ?? false))
                continue;

            device.DeviceInfoSet = deviceInfoSet;
            device.DeviceInfoData = deviceInfoData;
            devices.Add(device);
        }

        return devices;
    }

    private static List<string> GetPnpDeviceIdsFromWmi(DeviceType deviceType)
    {
        var pnpDeviceIds = new List<string>();
        string wmiQuery = deviceType switch
        {
            DeviceType.GPU => "SELECT PNPDeviceID FROM Win32_VideoController",
            DeviceType.XHCI => "SELECT PNPDeviceID FROM Win32_USBController",
            DeviceType.NIC => "SELECT PNPDeviceID FROM Win32_NetworkAdapter WHERE PhysicalAdapter = TRUE",
            _ => throw new ArgumentException("Unknown device type")
        };

        using var searcher = new ManagementObjectSearcher(wmiQuery);
        foreach (ManagementObject obj in searcher.Get())
        {
            var pnpId = obj["PNPDeviceID"]?.ToString();
            if (!string.IsNullOrEmpty(pnpId))
                pnpDeviceIds.Add(pnpId);
        }

        return pnpDeviceIds;
    }

    private static DeviceInfo GetDeviceInfo(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData)
    {
        var device = new DeviceInfo
        {
            DeviceInfoData = deviceInfoData,
            DevObjName = GetDeviceRegistryPropertyString(deviceInfoSet, ref deviceInfoData, SPDRP.SPDRP_PHYSICAL_DEVICE_OBJECT_NAME),
            DeviceDesc = GetDeviceRegistryPropertyString(deviceInfoSet, ref deviceInfoData, SPDRP.SPDRP_DEVICEDESC)
        };

        if (string.IsNullOrEmpty(device.DeviceDesc))
            return null;

        device.FriendlyName = GetDeviceRegistryPropertyString(deviceInfoSet, ref deviceInfoData, SPDRP.SPDRP_FRIENDLYNAME);
        device.LocationInformation = GetDeviceRegistryPropertyString(deviceInfoSet, ref deviceInfoData, SPDRP.SPDRP_LOCATION_INFORMATION);

        var hardwareIds = GetDeviceRegistryPropertyMultiString(deviceInfoSet, ref deviceInfoData, SPDRP.SPDRP_HARDWAREID);
        if (hardwareIds?.Length > 0)
        {
            device.PnpDeviceId = hardwareIds[0];
            if (string.IsNullOrEmpty(device.PnpDeviceId))
            {
                var compatibleIds = GetDeviceRegistryPropertyMultiString(deviceInfoSet, ref deviceInfoData, SPDRP.SPDRP_COMPATIBLEIDS);
                device.PnpDeviceId = compatibleIds?.Length > 0 ? compatibleIds[0] : string.Empty;
            }
        }

        IntPtr regKeyHandle = SetupApi.SetupDiOpenDevRegKey(
            deviceInfoSet,
            ref deviceInfoData,
            DICS_FLAG.DICS_FLAG_GLOBAL,
            0,
            DIREG.DIREG_DEV,
            0x00020019
        );

        if (regKeyHandle != IntPtr.Zero && regKeyHandle != new IntPtr(-1))
        {
            var safeHandle = new SafeRegistryHandle(regKeyHandle, ownsHandle: true);
            device.RegistryKey = RegistryKey.FromHandle(safeHandle);
        }

        uint propertyType = 0;
        uint requiredSize = 0;
        byte[] buffer = new byte[16];

        if (SetupApi.SetupDiGetDeviceProperty(
            deviceInfoSet,
            ref deviceInfoData,
            ref SetupApi.DEVPKEY_PciDevice_InterruptMessageMaximum,
            out propertyType,
            buffer,
            (uint)buffer.Length,
            out requiredSize,
            0))
        {
            if (requiredSize >= 4)
            {
                device.MaxMSILimit = BitConverter.ToUInt32(buffer, 0);
            }
        }
        else if (Marshal.GetLastWin32Error() == 122)
        {
            buffer = new byte[requiredSize];
            if (SetupApi.SetupDiGetDeviceProperty(
                deviceInfoSet,
                ref deviceInfoData,
                ref SetupApi.DEVPKEY_PciDevice_InterruptMessageMaximum,
                out propertyType,
                buffer,
                (uint)buffer.Length,
                out requiredSize,
                0))
            {
                if (requiredSize >= 4)
                {
                    device.MaxMSILimit = BitConverter.ToUInt32(buffer, 0);
                }
            }
        }

        return device;
    }

    private static string GetDeviceRegistryPropertyString(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, SPDRP property)
    {
        SetupApi.SetupDiGetDeviceRegistryProperty(
            deviceInfoSet,
            ref deviceInfoData,
            property,
            out _,
            IntPtr.Zero,
            0,
            out var requiredSize
        );

        if (requiredSize == 0)
            return string.Empty;

        IntPtr buffer = Marshal.AllocHGlobal((int)requiredSize);
        try
        {
            if (SetupApi.SetupDiGetDeviceRegistryProperty(
                deviceInfoSet,
                ref deviceInfoData,
                property,
                out _,
                buffer,
                requiredSize,
                out _))
            {
                return Marshal.PtrToStringUni(buffer) ?? string.Empty;
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

        return string.Empty;
    }

    private static string[] GetDeviceRegistryPropertyMultiString(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, SPDRP property)
    {
        SetupApi.SetupDiGetDeviceRegistryProperty(
            deviceInfoSet,
            ref deviceInfoData,
            property,
            out _,
            IntPtr.Zero,
            0,
            out var requiredSize
        );

        if (requiredSize == 0)
            return null;

        IntPtr buffer = Marshal.AllocHGlobal((int)requiredSize);
        try
        {
            if (SetupApi.SetupDiGetDeviceRegistryProperty(
                deviceInfoSet,
                ref deviceInfoData,
                property,
                out _,
                buffer,
                requiredSize,
                out _))
            {
                var result = new List<string>();
                IntPtr ptr = buffer;
                while (true)
                {
                    string str = Marshal.PtrToStringUni(ptr);
                    if (string.IsNullOrEmpty(str))
                        break;
                    result.Add(str);
                    ptr = IntPtr.Add(ptr, (str.Length + 1) * 2);
                }
                return [.. result];
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

        return null;
    }
}
