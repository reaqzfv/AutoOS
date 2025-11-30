using Microsoft.Win32;

namespace AutoOS.Views.Settings.Scheduling.Services;

public class DeviceSettings
{
    public uint MsiSupported { get; set; }
    public uint MessageNumberLimit { get; set; }
    public uint DevicePolicy { get; set; }
    public uint DevicePriority { get; set; }
    public ulong AssignmentSetOverride { get; set; }
    public uint MaxMSILimit { get; set; }
}

public static class RegistryService
{
    public static DeviceSettings ReadDeviceSettings(RegistryKey deviceRegKey, uint maxMSILimit = 0)
    {
        var settings = new DeviceSettings();


        using var affinityKey = deviceRegKey.OpenSubKey(@"Interrupt Management\Affinity Policy");
        if (affinityKey != null)
        {
            var policyValue = affinityKey.GetValue("DevicePolicy");
            settings.DevicePolicy = policyValue is int intPolicy ? (uint)intPolicy : policyValue is uint uintPolicy ? uintPolicy : policyValue is long longPolicy ? (uint)longPolicy : 0u;
            var priorityValue = affinityKey.GetValue("DevicePriority");
            settings.DevicePriority = priorityValue is int intPriority ? (uint)intPriority : priorityValue is uint uintPriority ? uintPriority : priorityValue is long longPriority ? (uint)longPriority : 0u;

            if (affinityKey.GetValue("AssignmentSetOverride") is byte[] assignmentBytes && assignmentBytes.Length > 0)
            {
                byte[] fullBytes = new byte[8];
                Array.Copy(assignmentBytes, fullBytes, Math.Min(assignmentBytes.Length, 8));
                settings.AssignmentSetOverride = BitConverter.ToUInt64(fullBytes, 0);
            }
        }

        using var msiKey = deviceRegKey.OpenSubKey(@"Interrupt Management\MessageSignaledInterruptProperties");
        if (msiKey != null)
        {
            var msiValue = msiKey.GetValue("MSISupported");
            settings.MsiSupported = msiValue is int intMsi ? (uint)intMsi : msiValue is uint uintMsi ? uintMsi : msiValue is long longMsi ? (uint)longMsi : 0u;
            var limitValue = msiKey.GetValue("MessageNumberLimit");
            settings.MessageNumberLimit = limitValue is int intLimit ? (uint)intLimit : limitValue is uint uintLimit ? uintLimit : limitValue is long longLimit ? (uint)longLimit : 0u;
        }
        else
        {
            settings.MsiSupported = 2;
        }

        settings.MaxMSILimit = maxMSILimit;
        return settings;
    }

    public static void SetMSIMode(RegistryKey deviceRegKey, bool msiSupported, uint messageNumberLimit)
    {
        if (msiSupported)
        {
            using var key = deviceRegKey.CreateSubKey(@"Interrupt Management\MessageSignaledInterruptProperties");
            key?.SetValue("MSISupported", 1, RegistryValueKind.DWord);

            if (messageNumberLimit == 0)
                key?.DeleteValue("MessageNumberLimit", false);
            else
                key?.SetValue("MessageNumberLimit", messageNumberLimit, RegistryValueKind.DWord);
        }
        else
        {
            deviceRegKey.DeleteSubKeyTree(@"Interrupt Management\MessageSignaledInterruptProperties", false);
        }
    }

    public static void SetAffinityPolicy(RegistryKey deviceRegKey, uint devicePolicy, uint devicePriority, ulong assignmentSetOverride)
    {
        if (devicePolicy == 0 && devicePriority == 0 && assignmentSetOverride == 0)
        {
            deviceRegKey.DeleteSubKeyTree(@"Interrupt Management\Affinity Policy", false);
            return;
        }

        using var key = deviceRegKey.CreateSubKey(@"Interrupt Management\Affinity Policy");

        key.SetValue("DevicePolicy", devicePolicy, RegistryValueKind.DWord);

        if (devicePolicy != 4)
        {
            key.DeleteValue("AssignmentSetOverride", false);
        }
        else
        {
            byte[] bytes = BitConverter.GetBytes(assignmentSetOverride);
            int length = bytes.Length;
            while (length > 1 && bytes[length - 1] == 0)
            {
                length--;
            }
            byte[] trimmedBytes = new byte[length];
            Array.Copy(bytes, trimmedBytes, length);
            key.SetValue("AssignmentSetOverride", trimmedBytes, RegistryValueKind.Binary);
        }

        if (devicePriority == 0)
        {
            key.DeleteValue("DevicePriority", false);
        }
        else
        {
            key.SetValue("DevicePriority", devicePriority, RegistryValueKind.DWord);
        }
    }
}
