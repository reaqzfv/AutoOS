namespace AutoOS.Views.Settings.BIOS;

public class BiosSettingRecommendation
{
    public string SetupQuestion { get; set; } = string.Empty;
    public string Type { get; set; }
    public string RecommendedOption { get; set; } = string.Empty;
}

public static class BiosSettingRecommendationsList
{
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
        new BiosSettingRecommendation { SetupQuestion = "TVB Ratio Clipping", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BCLK Aware Adaptive Voltage", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Dual Tau Boost", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Bi-Directional PROCHOT#", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Vmax Stress", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "V-Max Stress", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel RMT State", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "FIVR Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "RFI Spread Spectrum", Type = "Option", RecommendedOption = "0.5%" },
        new BiosSettingRecommendation { SetupQuestion = "Pcie Pll SSC", Type = "Option", RecommendedOption = "0.0%" },
        //new BiosSettingRecommendation { SetupQuestion = "Hyper-Threading", Type = "Option", RecommendedOption = "Disabled" }, // condition if >6 cores
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
        new BiosSettingRecommendation { SetupQuestion = "VT-d", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel VT-D Tech", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel (VMX) Virtualization Technology", Type = "Option", RecommendedOption = "Disabled" },

        // for oc
        new BiosSettingRecommendation { SetupQuestion = "BCLK Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "CPU EIST Function", Type = "Option", RecommendedOption = "Disabled" }, // didn't find
        //new BiosSettingRecommendation { SetupQuestion = "EIST", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Intel(R) Adaptive Boost Technology", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "ASUS MultiCore Enhancement", Type = "Option", RecommendedOption = "Disabled  Enforce All limits" },
        //new BiosSettingRecommendation { SetupQuestion = "CPU Core Ratio", Type = "Option", RecommendedOption = "Sync All Cores" },
        new BiosSettingRecommendation { SetupQuestion = "CPU SVID Support", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "MCH Full Check", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "VRM Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Overclocking TVB", Type = "Option", RecommendedOption = "Disabled" },
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
        new BiosSettingRecommendation { SetupQuestion = "Disabled Gen2 Pll Shutdown and L1 Controller Power gating", Type = "Option", RecommendedOption = "Enable" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Disabled Energy Reporting", Type = "Option", RecommendedOption = "Enable" }, // didn't find
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
        new BiosSettingRecommendation { SetupQuestion = "XHCI Hand-off", Type = "Option", RecommendedOption = "Disabled" },
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
        new BiosSettingRecommendation { SetupQuestion = "Memory Context Restore", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory Fast Boot", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory interleaving", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Gear Down Mode", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "Global C-state Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACPI _CST C1 Declaration", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "FCH Spread Spectrum", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "NX Mode", Type = "Option", RecommendedOption = "Disabled" }, // valorant may need this enabled or virtualization
        new BiosSettingRecommendation { SetupQuestion = "SVM Mode", Type = "Option", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "SMT Control", Type = "Option", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "OC Explore Mode", Type = "Option", RecommendedOption = "Expert" },
        new BiosSettingRecommendation { SetupQuestion = "CPU Over Temperature Alert", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPU temperature Warning Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ECO Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SoC/Uncore OC Mode", Type = "Option", RecommendedOption = "Enabled" },

        new BiosSettingRecommendation { SetupQuestion = "PM L1 SS", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACP Power Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACP CLock Gating", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Clock Power Management(CLKREQ#)", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIe Power Management Features", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Opcache Control", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Unused GPP Clocks Off", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Onboard PCIE LAN PXE ROM", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enhanced REP MOVSB/STOSB", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Fast Short REP MOVSB (FSRM)", Type = "Option", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Disable DF sync flood propagation", Type = "Option", RecommendedOption = "Sync flood disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Freeze DF module queues on error", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "IOMMU", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PSPP Policy", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 0 Enable", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 1 Enable", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 2 Enable", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 3 Enable", Type = "Option", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PSS Support", Type = "Option", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "Thunderbolt Wake Up Command", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Wake Up Event By", Type = "Option", RecommendedOption = "BIOS" },
        new BiosSettingRecommendation { SetupQuestion = "LN2 Mode", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Game Boost", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TSME", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SMEE", Type = "Option", RecommendedOption = "Disabled" },

        // rgb
        new BiosSettingRecommendation { SetupQuestion = "When system is in working state", Type = "Option", RecommendedOption = "Aura Off" },
        new BiosSettingRecommendation { SetupQuestion = "When system is in sleep, hibernate or soft off states", Type = "Option", RecommendedOption = "Aura Off" },
        new BiosSettingRecommendation { SetupQuestion = "RGB Fusion", Type = "Option", RecommendedOption = "Disabled" }, // didn't find

        // useful
        new BiosSettingRecommendation { SetupQuestion = "Boot Sector (MBR/GPT) Recovery Policy", Type = "Option", RecommendedOption = "Auto Recovery" },

        // ads
        new BiosSettingRecommendation { SetupQuestion = "Download & Install ARMOURY CRATE app", Type = "Option", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Download & Install MyASUS service & app", Type = "Option", RecommendedOption = "Disabled" },

        //new BiosSettingRecommendation { SetupQuestion = "Enable Hibernation", Type = "Value", RecommendedOption = "0" }, // required for wake on lan
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

