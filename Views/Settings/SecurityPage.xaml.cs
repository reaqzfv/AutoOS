using AutoOS.Views.Installer.Actions;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace AutoOS.Views.Settings;

public sealed partial class SecurityPage : Page
{
    private string nsudoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "NSudo", "NSudoLC.exe");
    private bool initialUACState = false;
    private bool initialMemoryIntegrityState = false;
    private bool initialSpectreMeltdownState = false;
    private bool initialProcessMitigationsState = false;
    private bool isInitializingWindowsDefenderState = true;
    private bool isInitializingUACState = true;
    private bool isInitializingDEPState = true;
    private bool isInitializingMemoryIntegrityState = true;
    private bool isInitializingSpectreMeltdownState = true;
    private bool isInitializingProcessMitigationsState = true;

    [DllImport("kernel32.dll")]
    static extern int GetSystemDEPPolicy();
    public SecurityPage()
    {
        InitializeComponent();
        GetWindowsDefenderState();
        GetUACState();
        GetDEPState();
        GetSpectreMeltdownState();
        GetProcessMitigationsState();
        GetMemoryIntegrityState();
    }

    private void GetWindowsDefenderState()
    {
        // declare services and drivers
        var groups = new[]
        {
            (new[] { "SecurityHealthService", "Sense", "WdNisDrv", "WdNisSvc", "webthreatdefsvc" }, 3),
            (new[] { "webthreatdefusersvc", "WinDefend", "wscsvc"  }, 2),
            (new[] { "MsSecCore", "WdBoot", "WdFilter" }, 0)
        };

        // check if values match
        bool isEnabled = true;
        foreach (var group in groups)
        {
            foreach (var service in group.Item1)
            {
                using var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{service}");
                if (key == null) continue;

                var startValue = key.GetValue("Start");
                if (startValue == null || (int)startValue != group.Item2)
                {
                    isEnabled = false;
                    break;
                }
            }
            if (!isEnabled)
                break;
        }

        WindowsDefender.IsOn = isEnabled;

        var serviceController = new ServiceController("WinDefend");
        bool isRunning = serviceController.Status == ServiceControllerStatus.Running;
        if (WindowsDefender.IsOn && !isRunning || !WindowsDefender.IsOn && isRunning)
        {
            // remove infobar
            WindowsDefenderInfo.Children.Clear();

            // add infobar
            var infoBar = new InfoBar
            {
                Title = WindowsDefender.IsOn ? "Successfully enabled Windows Defender. A restart is required to apply the change." : "Successfully disabled Windows Defender. A restart is required to apply the change.",
                IsClosable = false,
                IsOpen = true,
                Severity = InfoBarSeverity.Success,
                Margin = new Thickness(4, -28, 4, 36)
            };
            infoBar.ActionButton = new Button
            {
                Content = "Restart",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            ((Button)infoBar.ActionButton).Click += (s, args) => Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
            WindowsDefenderInfo.Children.Add(infoBar);
        }
        isInitializingWindowsDefenderState = false;
    }

    private async void WindowsDefender_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingWindowsDefenderState) return;

        // disable hittestvisible to avoid double-clicking
        WindowsDefender.IsHitTestVisible = false;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        WindowsDefenderInfo.Children.Add(new InfoBar
        {
            Title = WindowsDefender.IsOn ? "Enabling Windows Defender..." : "Disabling Windows Defender...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(4, -28, 4, 36)
        });

        // toggle windows defender
        if (WindowsDefender.IsOn)
        {
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\MsSecCore"" /v Start /t REG_DWORD /d 0 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SecurityHealthService"" /v Start /t REG_DWORD /d 3 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Sense"" /v Start /t REG_DWORD /d 3 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdBoot"" /v Start /t REG_DWORD /d 0 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdFilter"" /v Start /t REG_DWORD /d 0 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdNisDrv"" /v Start /t REG_DWORD /d 3 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdNisSvc"" /v Start /t REG_DWORD /d 3 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\webthreatdefsvc"" /v Start /t REG_DWORD /d 3 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\webthreatdefusersvc"" /v Start /t REG_DWORD /d 2 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WinDefend"" /v Start /t REG_DWORD /d 2 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\wscsvc"" /v Start /t REG_DWORD /d 2 /f");
        }
        else
        {
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\MsSecCore"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SecurityHealthService"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Sense"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdBoot"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdFilter"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdNisDrv"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdNisSvc"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\webthreatdefsvc"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\webthreatdefusersvc"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WinDefend"" /v Start /t REG_DWORD /d 4 /f");
            await ProcessActions.RunNsudo("TrustedInstaller", @"reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\wscsvc"" /v Start /t REG_DWORD /d 4 /f");
        }

        // delay
        await Task.Delay(200);

        // re-enable hittestvisible
        WindowsDefender.IsHitTestVisible = true;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = WindowsDefender.IsOn ? "Successfully enabled Windows Defender." : "Successfully disabled Windows Defender.",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(4, -28, 4, 36)
        };
        WindowsDefenderInfo.Children.Add(infoBar);

        // add restart button
        var serviceController = new ServiceController("WinDefend");
        bool isRunning = serviceController.Status == ServiceControllerStatus.Running;

        if (WindowsDefender.IsOn && !isRunning || !WindowsDefender.IsOn && isRunning)
        {
            infoBar.Title += " A restart is required to apply the change.";
            infoBar.ActionButton = new Button
            {
                Content = "Restart",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            ((Button)infoBar.ActionButton).Click += (s, args) => Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
        }
        else
        {
            // delay
            await Task.Delay(2000);

            // remove infobar
            WindowsDefenderInfo.Children.Clear();
        }
    }

    private void GetUACState()
    {
        // check registry value
        if ((int?)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System")?.GetValue("EnableLUA") == 1)
        {
            UAC.IsOn = true;
            initialUACState = true;
        }
        isInitializingUACState = false;
    }

    private async void UAC_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingUACState) return;

        // disable hittestvisible to avoid double-clicking
        UAC.IsHitTestVisible = false;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        WindowsDefenderInfo.Children.Add(new InfoBar
        {
            Title = UAC.IsOn ? "Enabling User Account Control (UAC)..." : "Disabling User Account Control (UAC)...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(4, -28, 4, 36)
        });

        // toggle uac
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true))
        {
            key.SetValue("EnableLUA", UAC.IsOn ? 1 : 0, RegistryValueKind.DWord);
            key.SetValue("PromptOnSecureDesktop", UAC.IsOn ? 1 : 0, RegistryValueKind.DWord);
            key.SetValue("ConsentPromptBehaviorAdmin", UAC.IsOn ? 5 : 0, RegistryValueKind.DWord);
        }

        // delay
        await Task.Delay(500);

        // re-enable hittestvisible
        UAC.IsHitTestVisible = true;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = UAC.IsOn ? "Successfully enabled User Account Control (UAC)." : "Successfully disabled User Account Control (UAC).",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(4, -28, 4, 36)
        };
        WindowsDefenderInfo.Children.Add(infoBar);

        // add restart button if needed
        if (UAC.IsOn != initialUACState)
        {
            infoBar.Title += " A restart is required to apply the change.";
            infoBar.ActionButton = new Button
            {
                Content = "Restart",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            ((Button)infoBar.ActionButton).Click += (s, args) =>
            Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
        }
        else
        {
            // delay
            await Task.Delay(2000);

            // remove infobar
            WindowsDefenderInfo.Children.Clear();
        }
    }

    private void GetDEPState()
    {
        // get active state
        int policy = GetSystemDEPPolicy();

        // get state
        var output = Process.Start(new ProcessStartInfo("cmd.exe", "/c bcdedit /enum {current}") { CreateNoWindow = true, RedirectStandardOutput = true }).StandardOutput.ReadToEnd();

        if (output.Contains("nx                      OptIn"))
        {
            if (policy == 0)
            {
                // remove infobar
                WindowsDefenderInfo.Children.Clear();

                // add infobar
                var infoBar = new InfoBar
                {
                    Title = DEP.IsOn ? "Successfully disabled Data Execution Prevention (DEP). A restart is required to apply the change." : "Successfully enabled Data Execution Prevention (DEP). A restart is required to apply the change.",
                    IsClosable = false,
                    IsOpen = true,
                    Severity = InfoBarSeverity.Success,
                    Margin = new Thickness(4, -28, 4, 36)
                };
                infoBar.ActionButton = new Button
                {
                    Content = "Restart",
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                ((Button)infoBar.ActionButton).Click += (s, args) =>
                Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
                WindowsDefenderInfo.Children.Add(infoBar);
            }

            DEP.IsOn = true;
        }
        else if (output.Contains("nx                      AlwaysOff"))
        {
            if (policy == 2)
            {
                // remove infobar
                WindowsDefenderInfo.Children.Clear();

                // add infobar
                var infoBar = new InfoBar
                {
                    Title = DEP.IsOn ? "Successfully enabled Data Execution Prevention (DEP). A restart is required to apply the change." : "Successfully disabled Data Execution Prevention (DEP). A restart is required to apply the change.",
                    IsClosable = false,
                    IsOpen = true,
                    Severity = InfoBarSeverity.Success,
                    Margin = new Thickness(4, -28, 4, 36),
                    ActionButton = new Button
                    {
                        Content = "Restart",
                        HorizontalAlignment = HorizontalAlignment.Right
                    }
                };
                ((Button)infoBar.ActionButton).Click += (s, args) =>
                Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
                WindowsDefenderInfo.Children.Add(infoBar);
            }
        }

        isInitializingDEPState = false;
    }

    private async void DEP_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingDEPState) return;

        // disable hittestvisible to avoid double-clicking
        DEP.IsHitTestVisible = false;

        // get active state
        int policy = GetSystemDEPPolicy();

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        WindowsDefenderInfo.Children.Add(new InfoBar
        {
            Title = DEP.IsOn ? "Enabling Data Execution Prevention (DEP)..." : "Disabling Data Execution Prevention (DEP)...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(4, -28, 4, 36)
        });

        // toggle dep
        var output = Process.Start(new ProcessStartInfo("cmd.exe", $"/c {(DEP.IsOn ? "bcdedit /set nx OptIn" : "bcdedit /set nx AlwaysOff")}") { CreateNoWindow = true, RedirectStandardOutput = true }).StandardOutput.ReadToEnd();

        if (output.Contains("error"))
        {
            // delay
            await Task.Delay(500);

            // re-enable hittestvisible
            DEP.IsHitTestVisible = true;

            // remove infobar
            WindowsDefenderInfo.Children.Clear();

            // toggle back
            isInitializingDEPState = true;
            DEP.IsOn = !DEP.IsOn;
            isInitializingDEPState = false;

            // add infobar
            WindowsDefenderInfo.Children.Add(new InfoBar
            {
                Title = "Failed to disable Data Execution Prevention (DEP) because secure boot is enabled.",
                IsClosable = false,
                IsOpen = true,
                Severity = InfoBarSeverity.Error,
                Margin = new Thickness(4, -28, 4, 36)
            });

            // delay
            await Task.Delay(2000);

            // remove infobar
            WindowsDefenderInfo.Children.Clear();
        }
        else
        {
            // delay
            await Task.Delay(500);

            // re-enable hittestvisible
            DEP.IsHitTestVisible = true;

            // remove infobar
            WindowsDefenderInfo.Children.Clear();

            // add infobar
            var infoBar = new InfoBar
            {
                Title = DEP.IsOn ? "Successfully enabled Data Execution Prevention (DEP)." : "Successfully disabled Data Execution Prevention (DEP).",
                IsClosable = false,
                IsOpen = true,
                Severity = InfoBarSeverity.Success,
                Margin = new Thickness(4, -28, 4, 36)
            };
            WindowsDefenderInfo.Children.Add(infoBar);

            // add restart button if needed
            if ((DEP.IsOn && policy == 0) || (!DEP.IsOn && policy == 2))
            {
                infoBar.Title += " A restart is required to apply the change.";
                infoBar.ActionButton = new Button
                {
                    Content = "Restart",
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                ((Button)infoBar.ActionButton).Click += (s, args) =>
                Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
            }
            else
            {
                // delay
                await Task.Delay(2000);

                // remove infobar
                WindowsDefenderInfo.Children.Clear();
            }
        }
    }

    private void GetMemoryIntegrityState()
    {
        // get state
        if ((Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 0) is int val && val == 1))
        {
            MemoryIntegrity.IsOn = true;
            initialMemoryIntegrityState = true;
        }
        isInitializingMemoryIntegrityState = false;
    }

    private async void MemoryIntegrity_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingMemoryIntegrityState) return;

        // disable hittestvisible to avoid double-clicking
        MemoryIntegrity.IsHitTestVisible = false;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        WindowsDefenderInfo.Children.Add(new InfoBar
        {
            Title = MemoryIntegrity.IsOn ? "Enabling Hypervisor Enforced Code Integrity (HVCI)..." : "Disabling Hypervisor Enforced Code Integrity (HVCI)...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(4, -28, 4, 36)
        });

        // toggle
        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", MemoryIntegrity.IsOn ? 1 : 0, RegistryValueKind.DWord);

        // delay
        await Task.Delay(500);

        // re-enable hittestvisible
        MemoryIntegrity.IsHitTestVisible = true;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = MemoryIntegrity.IsOn ? "Successfully enabled Hypervisor Enforced Code Integrity (HVCI)." : "Successfully disabled Hypervisor Enforced Code Integrity (HVCI).",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(4, -28, 4, 36)
        };
        WindowsDefenderInfo.Children.Add(infoBar);

        // add restart button if needed
        if (MemoryIntegrity.IsOn != initialMemoryIntegrityState)
        {
            infoBar.Title += " A restart is required to apply the change.";
            infoBar.ActionButton = new Button
            {
                Content = "Restart",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            ((Button)infoBar.ActionButton).Click += (s, args) =>
            Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
        }
        else
        {
            // delay
            await Task.Delay(2000);

            // remove infobar
            WindowsDefenderInfo.Children.Clear();
        }
    }

    private void GetSpectreMeltdownState()
    {
        // check registry
        string cpuVendor = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "VendorIdentifier", null);
        int? featureSettings = (int?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettings", null);
        int? featureSettingsOverrideMask = (int?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverrideMask", null);
        int? featureSettingsOverride = (int?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverride", null);

        if (cpuVendor.Contains("GenuineIntel"))
        {
            if (featureSettings == 0 && featureSettingsOverrideMask == null && featureSettingsOverride == null)
            {
                initialSpectreMeltdownState = true;
                SpectreMeltdown.IsOn = true;
            }
        }
        else if (cpuVendor.Contains("AuthenticAMD"))
        {
            if (featureSettings == 1 && featureSettingsOverrideMask == 3 && featureSettingsOverride == 64)
            {
                initialSpectreMeltdownState = true;
                SpectreMeltdown.IsOn = true;
            }
        }
        isInitializingSpectreMeltdownState = false;
    }

    private async void SpectreMeltdown_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingSpectreMeltdownState) return;

        // disable hittestvisible to avoid double-clicking
        SpectreMeltdown.IsHitTestVisible = false;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        WindowsDefenderInfo.Children.Add(new InfoBar
        {
            Title = SpectreMeltdown.IsOn ? "Enabling Spectre & Meltdown Mitigations..." : "Disabling Spectre & Meltdown Mitigations...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(4, -28, 4, 36)
        });

        if (SpectreMeltdown.IsOn)
        {
            string cpuVendor = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "VendorIdentifier", null);

            if (cpuVendor.Contains("GenuineIntel"))
            {
                // restore default values for enabling on intel
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettings", 0, RegistryValueKind.DWord);
                Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", writable: true).DeleteValue("FeatureSettingsOverrideMask", throwOnMissingValue: false);
                Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", writable: true).DeleteValue("FeatureSettingsOverride", throwOnMissingValue: false);
            }
            else if (cpuVendor.Contains("AuthenticAMD"))
            {
                // set custom values to enable all mitigations on amd
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettings", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverrideMask", 3, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverride", 64, RegistryValueKind.DWord);
            }
        }
        else
        {
            // disable spectre & meltdown
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettings", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverrideMask", 3, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverride", 3, RegistryValueKind.DWord);
        }

        // delay
        await Task.Delay(500);

        // re-enable hittestvisible
        SpectreMeltdown.IsHitTestVisible = true;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = SpectreMeltdown.IsOn ? "Successfully enabled Spectre & Meltdown Mitigations." : "Successfully disabled Spectre & Meltdown Mitigations.",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(4, -28, 4, 36)
        };
        WindowsDefenderInfo.Children.Add(infoBar);

        // add restart button if needed
        if (SpectreMeltdown.IsOn != initialSpectreMeltdownState)
        {
            infoBar.Title += " A restart is required to apply the change.";
            infoBar.ActionButton = new Button
            {
                Content = "Restart",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            ((Button)infoBar.ActionButton).Click += (s, args) =>
            Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
        }
        else
        {
            // delay
            await Task.Delay(2000);

            // remove infobar
            WindowsDefenderInfo.Children.Clear();
        }
    }

    private void GetProcessMitigationsState()
    {
        // get state
        if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\kernel", "MitigationOptions", null) == null && Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\kernel", "MitigationAuditOptions", null) == null)
        {
            ProcessMitigations.IsOn = true;
            initialProcessMitigationsState = true;
        }
        isInitializingProcessMitigationsState = false;
    }

    private async void ProcessMitigations_Toggled(object sender, RoutedEventArgs e)
    {
        if (isInitializingProcessMitigationsState) return;

        // disable hittestvisible to avoid double-clicking
        ProcessMitigations.IsHitTestVisible = false;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        WindowsDefenderInfo.Children.Add(new InfoBar
        {
            Title = ProcessMitigations.IsOn ? "Enabling Process Mitigations..." : "Disabling Process Mitigations...",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Informational,
            Margin = new Thickness(4, -28, 4, 36)
        });

        // toggle process mitigations
        var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\kernel", true);
        if (key != null)
        {
            if (ProcessMitigations.IsOn)
            {
                key.DeleteValue("MitigationAuditOptions", false);
                key.DeleteValue("MitigationOptions", false);
            }
            else
            {
                key.SetValue("MitigationAuditOptions", new byte[] { 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22 });
                key.SetValue("MitigationOptions", new byte[] { 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22 });
            }
        }

        // delay
        await Task.Delay(500);

        // re-enable hittestvisible
        ProcessMitigations.IsHitTestVisible = true;

        // remove infobar
        WindowsDefenderInfo.Children.Clear();

        // add infobar
        var infoBar = new InfoBar
        {
            Title = ProcessMitigations.IsOn ? "Successfully enabled Process Mitigations." : "Successfully disabled Process Mitigations.",
            IsClosable = false,
            IsOpen = true,
            Severity = InfoBarSeverity.Success,
            Margin = new Thickness(4, -28, 4, 36)
        };
        WindowsDefenderInfo.Children.Add(infoBar);

        // add restart button if needed
        if (ProcessMitigations.IsOn != initialProcessMitigationsState)
        {
            infoBar.Title += " A restart is required to apply the change.";
            infoBar.ActionButton = new Button
            {
                Content = "Restart",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            ((Button)infoBar.ActionButton).Click += (s, args) =>
            Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0") { CreateNoWindow = true });
        }
        else
        {
            // delay
            await Task.Delay(2000);

            // remove infobar
            WindowsDefenderInfo.Children.Clear();
        }
    }
}