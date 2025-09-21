using System.Management;

namespace AutoOS.Views.Settings.BIOS;

public class BiosSettingRecommendation
{
    public string SetupQuestion { get; set; } = string.Empty;
    public string Type { get; set; }
    public string RecommendedOption { get; set; } = string.Empty;
    public Func<bool> Condition { get; set; } = null;
}

public static class BiosSettingRecommendationsList
{
    public static bool Ryzen9 => new ManagementObjectSearcher("SELECT Name FROM Win32_Processor")
        .Get()
        .Cast<ManagementObject>()
        .Any(mo => mo["Name"].ToString().Contains("Ryzen 9"));

    public static bool RyzenX3D => new ManagementObjectSearcher("SELECT Name FROM Win32_Processor")
        .Get()
        .Cast<ManagementObject>()
        .Any(mo => mo["Name"].ToString().Contains("X3D"));

    public static readonly List<BiosSettingRecommendation> Rules =
    [
        new BiosSettingRecommendation { SetupQuestion = "DDR PowerDown and idle counter", Type = "Option", RecommendedOption = "PCODE" },
        new BiosSettingRecommendation { SetupQuestion = "For LPDDR Only: DDR PowerDown and idle counter", Type = "Option", RecommendedOption = "PCODE" },
        new BiosSettingRecommendation { SetupQuestion = "Power Down Mode", Type = "Option", RecommendedOption = "No Power Down" },
        new BiosSettingRecommendation { SetupQuestion = "LPMode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Per Bank Refresh", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PowerDown Energy Ch0Dimm0", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "PowerDown Energy Ch0Dimm1", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "PowerDown Energy Ch1Dimm0", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "PowerDown Energy Ch1Dimm1", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "EPG DIMM Idd3N", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "EPG DIMM Idd3P", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "C6DRAM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Command Tristate", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Power Down Unused Lanes", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Command Rate Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ECC Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Link Training Retry", Type = "Option", RecommendedOption = "Disabled" }, // or 2
        new BiosSettingRecommendation { SetupQuestion = "SA GV", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Total Memory Encryption", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "CPU C-states", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Package C State Limit", Type = "Option", RecommendedOption = "C0/C1" },
        new BiosSettingRecommendation { SetupQuestion = "C-States Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enhanced C-states", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "C-State Pre-Wake", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "C-State Auto Demotion", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "C-State Un-demotion", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Package C-State Demotion", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Package C-State Un-demotion", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Ring to Core offset", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Ring Down Bin", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel(R) Speed Shift Technology Interrupt Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel SpeedStep™", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Race To Halt (RTH)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Voltage Optimization", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TVB Voltage Optimizations", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enhanced TVB", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TVB Ratio Clipping", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TVB Ratio Clipping Enhanced", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BCLK Aware Adaptive Voltage", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Dual Tau Boost", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Bi-Directional PROCHOT#", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Vmax Stress", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "V-Max Stress", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPU VRM Thermal Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel RMT State", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "FIVR Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "RFI Spread Spectrum", Type = "Option", RecommendedOption = "0.5%" },
        new BiosSettingRecommendation { SetupQuestion = "Pcie Pll SSC", Type = "Option", RecommendedOption = "0.0%" },
        //new BiosSettingRecommendation { SetupQuestion = "Hyper-Threading", Type = "Option", RecommendedOption = "Disabled" }, // condition if >6 cores
        new BiosSettingRecommendation { SetupQuestion = "Active E-Cores", Type = "Option", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "Active Efficient Cores", Type = "Option", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "Thermal Throttling Level", Type = "Option", RecommendedOption = "Manual" },
        new BiosSettingRecommendation { SetupQuestion = "T0 Level", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "T1 Level", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "T2 Level", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "AP threads Idle Manner", Type = "Option", RecommendedOption = "RUN Loop" },
        new BiosSettingRecommendation { SetupQuestion = "Boot performance mode", Type = "Option", RecommendedOption = "Turbo Performance" },
        new BiosSettingRecommendation { SetupQuestion = "PCH Energy Reporting", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel Adaptive Thermal Monitor", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Thermal Monitor", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CFG Lock", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPU AES Instructions", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "VT-d", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel Virtualization Tech", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel VT-D Tech", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel (VMX) Virtualization Technology", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "L1 Substates", Type = "Option", RecommendedOption = "Disabled" },

        // for oc
        new BiosSettingRecommendation { SetupQuestion = "BCLK Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "CPU EIST Function", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        //new BiosSettingRecommendation { SetupQuestion = "EIST", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Intel(R) Adaptive Boost Technology", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Per Core P State OS control mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "HwP Autonomous Per Core P State", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASUS MultiCore Enhancement", Type = "Option", RecommendedOption = "Enabled  Remove All limits" },
        //new BiosSettingRecommendation { SetupQuestion = "CPU Core Ratio", Type = "Option", RecommendedOption = "Sync All Cores" },
        //new BiosSettingRecommendation { SetupQuestion = "CPU Ratio Mode", Type = "Option", RecommendedOption = "Fixed Mode" },
        new BiosSettingRecommendation { SetupQuestion = "CPU SVID Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "MCH Full Check", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "VRM Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Overclocking TVB", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Thermal Velocity Boost", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Intel Speed Shift Technology", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPU PkgC10 Long Sleep", Type = "Option", RecommendedOption = "Disabled" },

        // timer res
        new BiosSettingRecommendation { SetupQuestion = "System Time and Alarm Source", Type = "Option", RecommendedOption = "Legacy RTC" },

        new BiosSettingRecommendation { SetupQuestion = "Power Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCI Express Power Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Energy Efficient P-state", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Energy Efficient Turbo", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Energy Performance Gain", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "RSR", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "USB2 PHY Sus Well Power Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PUIS Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "EC Low Power Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "EC CS Debug Light", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACPI T-States", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "DPTF", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "ACPI D3Cold Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ZPODD", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "JTAG C10 Power Gate", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enable 8254 Clock Gate", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIe Spread Spectrum Clocking", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "RC6(Render Standby)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP CPU", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP Graphics", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP GNA", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP SATA", Type = "Option", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP enumerated SATA ports", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe Storage", Type = "Option", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe LAN", Type = "Option", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe WLAN", Type = "Option", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe GFX", Type = "Option", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe Other", Type = "Option", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP UART", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C0", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C1", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C2", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C3", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C4", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C5", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP SPI", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP XHCI", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP Audio", Type = "Option", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP CSME", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP HECI3", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP LAN(GBE)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP THC0", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP THC1", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP TCSS", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP VMD", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "HDC Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCH Cross Throttling", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCI Express Clock Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Native ASPM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DMI Link ASPM Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DMI Gen3 ASPM Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEG - ASPM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "OBFF", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "LTR", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "LTR Mechanism Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEG ASPM", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "PCH ASPM", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "DMI ASPM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "USB power delivery in Soft Off state (S5)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Low Power S0 Idle Capability", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Sensor Standby", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Power Loss Notification Feature", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "C1E Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Aggressive LPM Support", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "ACPI Standby State", Type = "Option", RecommendedOption = "Suspend Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACPI Sleep State", Type = "Option", RecommendedOption = "Suspend Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ErP Ready", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCI Latency Timer", Type = "Option", RecommendedOption = "32 PCI Bus Clocks" }, // depends
        new BiosSettingRecommendation { SetupQuestion = "PCI-X Latency Timer", Type = "Option", RecommendedOption = "32 PCI Bus Clocks" }, // depends
        new BiosSettingRecommendation { SetupQuestion = "Disable DSX ACPRESENT PullDown", Type = "Option", RecommendedOption = "Enable" },
        new BiosSettingRecommendation { SetupQuestion = "Disable Gen2 Pll Shutdown and L1 Controller Power gating", Type = "Option", RecommendedOption = "Enable" },
        new BiosSettingRecommendation { SetupQuestion = "Disable Energy Reporting", Type = "Option", RecommendedOption = "Enable" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "PCI Delay Optimization", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Legacy IO Low Latency", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel Ready Mode Technology", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "Bootup NumLock State", Type = "Option", RecommendedOption = "Off" },
        new BiosSettingRecommendation { SetupQuestion = "Interrupt Redirection Mode Selection", Type = "Option", RecommendedOption = "Round robin" },
        new BiosSettingRecommendation { SetupQuestion = "USB 2.0 Controller Mode", Type = "Option", RecommendedOption = "HiSpeed" },
        new BiosSettingRecommendation { SetupQuestion = "3DMark01 Enhancement", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Legacy Game Compatibility Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BIST", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BIST Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCH Trace Hub Enable Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Processor trace", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Processor trace memory allocation", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SMM Processor Trace", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CrashLog Feature", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PMC Debug Message Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SMART Self Test", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "Serial Port", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "Parallel Port", Type = "Value", RecommendedOption = "0" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Port 60/64 Emulation", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Port 61h Bit-4 Emulation", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        
        new BiosSettingRecommendation { SetupQuestion = "SR-IOV Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IOAPIC 24-119 Entries", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Probeless Trace", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory Trace", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Training Tracing", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enable/Disable IED", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "ALS Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel Rapid Recovery Technology", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Smart Response Technology", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Advanced Error Reporting", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "HW Notification", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "HDCP Support", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "WDT Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PECI", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Power Loading", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "BIOS Hot-Plug Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hot-Plug Support", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "PTID Support", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "MRC Fast Boot", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Fast Boot", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Debug Interface", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "PERR# Generation", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SERR# Generation", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "URR", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "FER", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "NFER", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CER", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CTO", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PME SCI", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Remote Platform Erase Feature", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIE Tunneling over USB4", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Discrete Thunderbolt(TM) Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IGD VTD Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IPU VTD Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IOP VTD Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Three Strike Counter", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Control Iommu Pre-boot Behavior", Type = "Option", RecommendedOption = "Disable IOMMU" },
        new BiosSettingRecommendation { SetupQuestion = "Cpu CrashLog", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Cpu CrashLog (Device 10)", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "BME DMA Mitigation", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PAVP Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enable RH Prevention", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Execute Disable Bit", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "AES", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASF Support", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Wake On WiGig", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Wake on WLAN and BT Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "USB DbC Enable Mode", Type = "Option", RecommendedOption = "Disabled" }, // (hard to disable)
        new BiosSettingRecommendation { SetupQuestion = "USB S5 Wakeup Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DeepSx Wake on WLAN and BT Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "WoV (Wake on Voice)", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "USB Provisioning of AMT", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "USB3 Type-C UFP2DFP Kernel/Platform Debug Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "XHCI Hand-off", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "IPv4 PXE Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IPv6 PXE Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Network Stack", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Network Stack Driver Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PS2 Keyboard and Mouse", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PS2 Devices Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Option ROM Messages", Type = "Option", RecommendedOption = "Keep Current" },
        new BiosSettingRecommendation { SetupQuestion = "Launch PXE OpROM policy", Type = "Option", RecommendedOption = "Do no launch" },
        new BiosSettingRecommendation { SetupQuestion = "EC Notification", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PS3 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PS4 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TDC Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PPCC Object", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PDRT Object", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ARTG Object", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PMAX Object", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PET Progress", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACS", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PTM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DPC", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "EDPC", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hardware Flow Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "HID Event Filter Driver", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "MachineCheck", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "Maximum Payload", Type = "Option", RecommendedOption = "4096 Bytes" },
        new BiosSettingRecommendation { SetupQuestion = "Maximum Read Request", Type = "Option", RecommendedOption = "4096 Bytes" },
        new BiosSettingRecommendation { SetupQuestion = "PEG0 Max Payload size", Type = "Option", RecommendedOption = "256" },
        new BiosSettingRecommendation { SetupQuestion = "PEG1 Max Payload size", Type = "Option", RecommendedOption = "256" },
        new BiosSettingRecommendation { SetupQuestion = "PEG2 Max Payload size", Type = "Option", RecommendedOption = "256" },
        new BiosSettingRecommendation { SetupQuestion = "PEG3 Max Payload size", Type = "Option", RecommendedOption = "256" },

        new BiosSettingRecommendation { SetupQuestion = "Above 4G Decoding", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Above 4GB MMIO BIOS assignment", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Above 4G memory/Crypto Currency mining", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Re-Size BAR Support", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIE Resizable BAR Support", Type = "Option", RecommendedOption = "Enabled" },

        new BiosSettingRecommendation { SetupQuestion = "OS Type", Type = "Option", RecommendedOption = "Windows UEFI mode" },
        new BiosSettingRecommendation { SetupQuestion = "Secure Boot Mode", Type = "Option", RecommendedOption = "Standard" },
        new BiosSettingRecommendation { SetupQuestion = "TPM State", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "TCM State", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Me State", Type = "Option", RecommendedOption = "Disabled" }, // might only work with bios mod (hard to disable)
        new BiosSettingRecommendation { SetupQuestion = "PTT", Type = "Option", RecommendedOption = "Disable" }, // is it necessary for TPM? (hard to disable)
        new BiosSettingRecommendation { SetupQuestion = "Security Device Support", Type = "Option", RecommendedOption = "Enabled" }, // required for TPM
        new BiosSettingRecommendation { SetupQuestion = "Intel Platform Trust Technology (PTT)", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Software Guard Extensions (SGX)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SW Guard Extension (security)", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Intel Trusted Execution Technology", Type = "Option", RecommendedOption = "Disabled" },

        // AMD
        new BiosSettingRecommendation { SetupQuestion = "Power Down Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DRAM Latency Enhance", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory Context Restore", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory Fast Boot", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory interleaving", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Channel Interleaving", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Bank Interleaving", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Gear Down Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CC6 memory region encryption", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "GMI encryption control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "xGMI encryption control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SRIS", Type = "Option", RecommendedOption = "Enable" },
        new BiosSettingRecommendation { SetupQuestion = "Memory P-State", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BankGroupSwap", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BankGroupSwapAlt", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Data Scramble", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DRAM ECC Enable", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "Global C-state Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DF Cstates", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DF C-state control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACPI _CST C1 Declaration", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "FCH Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SB Clock Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Int. Clk Differential Spread", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "NX Mode", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "SVM Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "OC Explore Mode", Type = "Option", RecommendedOption = "Expert" },
        new BiosSettingRecommendation { SetupQuestion = "CPU Over Temperature Alert", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPU temperature Warning Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ECO Mode", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "SoC/Uncore OC Mode", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Prochot VRM Throttling", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "Prochot VRM Throttling", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIe ARI Support", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "PCIe ARI Enumeration", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "PCIB Clock Run", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Relaxed EDC throttling", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Aggressive Link PM Capability", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "UCLK DIV1 MODE", Type = "Option", RecommendedOption = "UCLK==MEMCLK" },
        new BiosSettingRecommendation { SetupQuestion = "APBDIS", Type = "Option", RecommendedOption = "1" },
        new BiosSettingRecommendation { SetupQuestion = "Adaptive S4", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPPC", Type = "Option", RecommendedOption = "Enabled" }, // might have to test disabled
        new BiosSettingRecommendation { SetupQuestion = "CPPC Preferred Cores", Type = "Option", RecommendedOption = "Enabled" }, // might have to test disabled
        new BiosSettingRecommendation { SetupQuestion = "CPPC Dynamic Preferred Cores", Type = "Option", RecommendedOption = "Cache", Condition = () => Ryzen9 == true || RyzenX3D == true },
        new BiosSettingRecommendation { SetupQuestion = "X3D Gaming Mode", Type = "Option", RecommendedOption = "Enabled", Condition = () => Ryzen9 == true },
        //new BiosSettingRecommendation { SetupQuestion = "SMT Control", Type = "Option", RecommendedOption = "Disable" },

        new BiosSettingRecommendation { SetupQuestion = "PM L1 SS", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACP Power Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACP CLock Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "AB Clock Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Clock Power Management(CLKREQ#)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIe Power Management Features", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Opcache Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Unused GPP Clocks Off", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Onboard PCIE LAN PXE ROM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enhanced REP MOVSB/STOSB", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Fast Short REP MOVSB (FSRM)", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Disable DF to external downstream IP Sync Flood Propagation", Type = "Option", RecommendedOption = "Sync flood disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Disable DF to external downstream IP SyncFloodPropagation", Type = "Option", RecommendedOption = "Sync flood disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Disable DF sync flood propagation", Type = "Option", RecommendedOption = "Sync flood disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Freeze DF module queues on error", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "IOMMU", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PSPP Policy", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 0 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 1 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 2 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 3 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 4 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 5 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 0 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 1 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 2 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 3 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 4 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I2C 5 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 0 Enable", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 1 Enable", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 2 Enable", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 3 Enable", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Uart 0 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Uart 1 Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Uart 2 Enable (no HW FC)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Uart 3 Enable (no HW FC)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "UART 0 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "UART 1 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "UART 2 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "UART 3 D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SATA D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "EHCI D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "XHCI D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SD D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACPI D3 Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "D3 Cold Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Suspend to RAM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "S3 PCIe Save Restore Mode", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "C6 Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Device Sleep for AHCI Port 0", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "Device Sleep for AHCI Port 1", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "Device Sleep for AHCI Port 2", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "Device Sleep for AHCI Port 3", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "Deep Sleep", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "NPU Deep Sleep Enable", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SATA PHY PLL", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Chipset Power Saving Features", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SB C1E Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Remote Display Feature", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PSS Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Aggresive SATA Device Sleep Port 0", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Aggresive SATA Device Sleep Port 1", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "S3/Modern Standby Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Advanced Error Reporting (AER)", Type = "Option", RecommendedOption = "Not Supported" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Control for CPU PCIe", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Mode(Dev#1/Func#2)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Mode(Dev#2/Func#1)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Mode(Dev#2/Func#2)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Mode Control(Device4)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Mode Control(Device5)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Mode Control(Device6)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Mode Control(Device7)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hotplug Mode(Dev#1/Func#1)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hotplug Mode(Dev#1/Func#2)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hotplug Mode(Dev#2/Func#1)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hotplug Mode(Dev#2/Func#2)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hotplug Mode Control(Device4)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hotplug Mode Control(Device5)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hotplug Mode Control(Device6)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hotplug Mode Control(Device7)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Latency Killer", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SMM Isolation Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Serial(COM) Port0", Type = "Value", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "Power Supply Idle Control", Type = "Option", RecommendedOption = "Typical Current Idle" },

        new BiosSettingRecommendation { SetupQuestion = "Thunderbolt Wake Up Command", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Wake Up Event By", Type = "Option", RecommendedOption = "BIOS" },
        new BiosSettingRecommendation { SetupQuestion = "LN2 Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "LN2 Mode 1", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "LN2 Mode 2", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Game Boost", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TSME", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SMEE", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "Log Transparent Errors", Type = "Option", RecommendedOption = "Disabled" },

        // rgb
        new BiosSettingRecommendation { SetupQuestion = "When system is in working state", Type = "Option", RecommendedOption = "Aura Off" },
        new BiosSettingRecommendation { SetupQuestion = "When system is in sleep, hibernate or soft off states", Type = "Option", RecommendedOption = "Aura Off" },
        new BiosSettingRecommendation { SetupQuestion = "RGB Fusion", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "RGB Light", Type = "Value", RecommendedOption = "0" },

        // useful
        new BiosSettingRecommendation { SetupQuestion = "Boot Sector (MBR/GPT) Recovery Policy", Type = "Option", RecommendedOption = "Auto Recovery" },

        // ads
        new BiosSettingRecommendation { SetupQuestion = "Download & Install ARMOURY CRATE app", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Download & Install MyASUS service & app", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "MSI Driver Utility Installer", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "Enable Hibernation", Type = "Value", RecommendedOption = "0" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "Tcc Activation Offset", Type = "Value", RecommendedOption = "0" }, // exists both as Auto Setting and as value setting i think only the value setting should be changed to 0 (default is 0 anyway)
        //new BiosSettingRecommendation { SetupQuestion = "LAN Wake From DeepSx", Type = "Option", RecommendedOption = "Disabled" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "Wake on LAN Enable", Type = "Option", RecommendedOption = "Disabled" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "PME Enable", Type = "Option", RecommendedOption = "Enabled" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "Power On By PCI-E", Type = "Option", RecommendedOption = "Enabled" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "WWAN", Type = "Option", RecommendedOption = "Disabled" }, // required for wlan
        //new BiosSettingRecommendation { SetupQuestion = "WWAN Device", Type = "Option", RecommendedOption = "Disabled" }, // required for wlan and bt
        //new BiosSettingRecommendation { SetupQuestion = "WWAN participant", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "WWAN (WIFI)", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        //new BiosSettingRecommendation { SetupQuestion = "Wi-Fi Controller", Type = "Option", RecommendedOption = "Disabled" }, // required for wifi
        //new BiosSettingRecommendation { SetupQuestion = "Wi-Fi Core", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        //new BiosSettingRecommendation { SetupQuestion = "Bluetooth Controller", Type = "Option", RecommendedOption = "Disabled" }, // required for bluetooth
        //new BiosSettingRecommendation { SetupQuestion = "Bluetooth", Type = "Option", RecommendedOption = "Disabled" }, // required for bluetooth
        //new BiosSettingRecommendation { SetupQuestion = "BT Core", Type = "Option", RecommendedOption = "Disabled" }, // required for bluetooth
        //new BiosSettingRecommendation { SetupQuestion = "Onboard CNVi Module Control", Type = "Option", RecommendedOption = "Disable Integrated" },
        //new BiosSettingRecommendation { SetupQuestion = "Connectivity mode (Wi-Fi & Bluetooth)", Type = "Option", RecommendedOption = "Disabled" }, // required for wifi and bt
        //new BiosSettingRecommendation { SetupQuestion = "CS PL1 Value", Type = "Option", RecommendedOption = "Max" }, // need smth for max value fields
        //new BiosSettingRecommendation { SetupQuestion = "PCIe Speed", Type = "Option", RecommendedOption = "Max" }, // need smth for max value fields here though auto will be better
        //new BiosSettingRecommendation { SetupQuestion = "Max Link Speed", Type = "Option", RecommendedOption = "Max" }, // need smth for max value fields here though auto will be better
        //new BiosSettingRecommendation { SetupQuestion = "DMI Max Link Speed", Type = "Option", RecommendedOption = "Max" }, // need smth for max value fields here though auto will be better

        //new BiosSettingRecommendation { SetupQuestion = "Touch Pad", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Touch Panel", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Finger Print Sensor", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Type C Support", Type = "Option", RecommendedOption = "Disabled" },
    ];
}