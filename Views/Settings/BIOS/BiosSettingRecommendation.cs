namespace AutoOS.Views.Settings.BIOS;

public class BiosSettingRecommendation
{
    public string SetupQuestion { get; set; } = string.Empty;
    public string RecommendedOption { get; set; } = string.Empty;
}

public static class BiosSettingRecommendationsList
{
    public static readonly List<BiosSettingRecommendation> Rules =
    [
        new BiosSettingRecommendation { SetupQuestion = "DDR PowerDown and idle counter", RecommendedOption = "PCODE" },
        new BiosSettingRecommendation { SetupQuestion = "For LPDDR Only: DDR PowerDown and idle counter", RecommendedOption = "PCODE" },
        new BiosSettingRecommendation { SetupQuestion = "Power Down Mode", RecommendedOption = "No Power Down" },
        new BiosSettingRecommendation { SetupQuestion = "LPMode", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Per Bank Refresh", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PowerDown Energy Ch0Dimm0", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "PowerDown Energy Ch0Dimm1", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "PowerDown Energy Ch1Dimm0", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "PowerDown Energy Ch1Dimm1", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "EPG DIMM Idd3N", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "EPG DIMM Idd3P", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "C6DRAM", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Command Tristate", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Power Down Unused Lanes", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Command Rate Support", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "ECC Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Link Training Retry", RecommendedOption = "Disabled" }, // or 2
        new BiosSettingRecommendation { SetupQuestion = "SA GV", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Total Memory Encryption", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "CPU C-states", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Package C State Limit", RecommendedOption = "C0/C1" },
        new BiosSettingRecommendation { SetupQuestion = "C-States Control", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enhanced C-states", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "C-State Pre-Wake", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "C-State Auto Demotion", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "C-State Un-demotion", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Package C-State Demotion", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Package C-State Un-demotion", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Ring to Core offset", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Ring Down Bin", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPU EIST Function", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "EIST", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel(R) Speed Shift Technology Interrupt Control", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel SpeedStep™", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Race To Halt (RTH)", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Voltage Optimization", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "TVB Voltage Optimizations", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TVB Ratio Clipping", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BCLK Aware Adaptive Voltage", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Dual Tau Boost", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Bi-Directional PROCHOT#", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Vmax Stress", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "V-Max Stress", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel RMT State", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "FIVR Spread Spectrum", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "RFI Spread Spectrum", RecommendedOption = "0.5%" },
        //new BiosSettingRecommendation { SetupQuestion = "Hyper-Threading", RecommendedOption = "Disabled" }, // condition if >6 cores
        new BiosSettingRecommendation { SetupQuestion = "Thermal Throttling Level", RecommendedOption = "Manual" },
        new BiosSettingRecommendation { SetupQuestion = "T0 Level", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "T1 Level", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "T2 Level", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "AP threads Idle Manner", RecommendedOption = "RUN Loop" },
        new BiosSettingRecommendation { SetupQuestion = "Boot performance mode", RecommendedOption = "Turbo Performance" },
        new BiosSettingRecommendation { SetupQuestion = "PCH Energy Reporting", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel Adaptive Thermal Monitor", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Thermal Monitor", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CFG Lock", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "VT-d", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel VT-D Tech", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel (VMX) Virtualization Technology", RecommendedOption = "Disabled" },

        // for oc
        new BiosSettingRecommendation { SetupQuestion = "BCLK Spread Spectrum", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Intel(R) Adaptive Boost Technology", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "ASUS MultiCore Enhancement", RecommendedOption = "Disabled  Enforce All limits" },
        //new BiosSettingRecommendation { SetupQuestion = "CPU Core Ratio", RecommendedOption = "Sync All Cores" },
        new BiosSettingRecommendation { SetupQuestion = "CPU SVID Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "MCH Full Check", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "VRM Spread Spectrum", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Overclocking TVB", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Intel Speed Shift Technology", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPU PkgC10 Long Sleep", RecommendedOption = "Disabled" },

        // timer res
        new BiosSettingRecommendation { SetupQuestion = "System Time and Alarm Source", RecommendedOption = "Legacy RTC" },

        new BiosSettingRecommendation { SetupQuestion = "Power Gating", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCI Express Power Gating", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Energy Efficient P-state", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Energy Efficient Turbo", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Energy Performance Gain", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "RSR", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "USB2 PHY Sus Well Power Gating", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PUIS Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "EC Low Power Mode", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACPI T-States", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "DPTF", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "ACPI D3Cold Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ZPODD", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "JTAG C10 Power Gate", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enable 8254 Clock Gate", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIe Spread Spectrum Clocking", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "RC6(Render Standby)", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP CPU", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP Graphics", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP GNA", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP SATA", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP enumerated SATA ports", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe Storage", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe LAN", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe WLAN", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe GFX", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP PCIe Other", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP UART", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C0", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C1", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C2", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C3", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C4", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP I2C5", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP SPI", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP XHCI", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP Audio", RecommendedOption = "No Constraint" },
        new BiosSettingRecommendation { SetupQuestion = "PEP CSME", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP HECI3", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP LAN(GBE)", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP THC0", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP THC1", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP TCSS", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEP VMD", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "HDC Control", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCH Cross Throttling", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCI Express Clock Gating", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Native ASPM", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DMI Link ASPM Control", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DMI Gen3 ASPM Control", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEG - ASPM", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASPM", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "OBFF", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "LTR", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PEG ASPM", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "PCH ASPM", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "DMI ASPM", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "USB power delivery in Soft Off state (S5)", RecommendedOption = "Disabled" },
        
        new BiosSettingRecommendation { SetupQuestion = "ACPI Standby State", RecommendedOption = "Suspend Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACPI Sleep State", RecommendedOption = "Suspend Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ErP Ready", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCI Latency Timer", RecommendedOption = "32 PCI Bus Clocks" }, // depends
        new BiosSettingRecommendation { SetupQuestion = "PCI-X Latency Timer", RecommendedOption = "32 PCI Bus Clocks" }, // depends
        new BiosSettingRecommendation { SetupQuestion = "Disable DSX ACPRESENT PullDown", RecommendedOption = "Enable" },
        new BiosSettingRecommendation { SetupQuestion = "Disabled Gen2 Pll Shutdown and L1 Controller Power gating", RecommendedOption = "Enable" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Disabled Energy Reporting", RecommendedOption = "Enable" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "PCI Delay Optimization", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Legacy IO Low Latency", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel Ready Mode Technology", RecommendedOption = "Disabled" },
        
        new BiosSettingRecommendation { SetupQuestion = "Bootup NumLock State", RecommendedOption = "Off" },
        new BiosSettingRecommendation { SetupQuestion = "Interrupt Redirection Mode Selection", RecommendedOption = "Round robin" },
        new BiosSettingRecommendation { SetupQuestion = "USB 2.0 Controller Mode", RecommendedOption = "HiSpeed" },
        new BiosSettingRecommendation { SetupQuestion = "3DMark01 Enhancement", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Legacy Game Compatibility Mode", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BIST", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "BIST Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCH Trace Hub Enable Mode", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Processor trace", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Processor trace memory allocation", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SMM Processor Trace", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CrashLog Feature", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PMC Debug Message Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SMART Self Test", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "Serial Port", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Parallel Port", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Port 60/64 Emulation", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Port 61h Bit-4 Emulation", RecommendedOption = "Disabled" }, // didn't find
        
        new BiosSettingRecommendation { SetupQuestion = "SR-IOV Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IOAPIC 24-119 Entries", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Probeless Trace", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory Trace", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Training Tracing", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enable/Disable IED", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "ALS Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Intel Rapid Recovery Technology", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Smart Response Technology", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Advanced Error Reporting", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "HW Notification", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enable Hibernation", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "HDCP Support", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "WDT Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PECI", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Power Loading", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "BIOS Hot-Plug Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hot-Plug Support", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "PTID Support", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "MRC Fast Boot", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Fast Boot", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Debug Interface", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "PERR# Generation", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "SERR# Generation", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "URR", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "FER", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "NFER", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CER", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PME SCI", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Remote Platform Erase Feature", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIE Tunneling over USB4", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Discrete Thunderbolt(TM) Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IGD VTD Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IPU VTD Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IOP VTD Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Three Strike Counter", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Control Iommu Pre-boot Behavior", RecommendedOption = "Disable IOMMU" },
        new BiosSettingRecommendation { SetupQuestion = "Cpu CrashLog", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Cpu CrashLog (Device 10)", RecommendedOption = "Disabled" },
        
        new BiosSettingRecommendation { SetupQuestion = "BME DMA Mitigation", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PAVP Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Enable RH Prevention", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Execute Disable Bit", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "AES", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ASF Support", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Wake On WiGig", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Wake on WLAN and BT Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "USB DbC Enable Mode", RecommendedOption = "Disabled" }, // (hard to disable)
        new BiosSettingRecommendation { SetupQuestion = "USB S5 Wakeup Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DeepSx Wake on WLAN and BT Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "WoV (Wake on Voice)", RecommendedOption = "0" },
        new BiosSettingRecommendation { SetupQuestion = "USB Provisioning of AMT", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "USB3 Type-C UFP2DFP Kernel/Platform Debug Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "XHCI Hand-off", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IPv4 PXE Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "IPv6 PXE Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Network Stack", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Network Stack Driver Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PS2 Keyboard and Mouse", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PS2 Devices Support", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Option ROM Messages", RecommendedOption = "Keep Current" },
        new BiosSettingRecommendation { SetupQuestion = "Launch PXE OpROM policy", RecommendedOption = "Do no launch" },
        new BiosSettingRecommendation { SetupQuestion = "EC Notification", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PS3 Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PS4 Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TDC Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PPCC Object", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PDRT Object", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ARTG Object", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PMAX Object", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PET Progress", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACS", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PTM", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "DPC", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "EDPC", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Hardware Flow Control", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "Maximum Payload", RecommendedOption = "4096 Bytes" },
        new BiosSettingRecommendation { SetupQuestion = "Maximum Read Request", RecommendedOption = "4096 Bytes" },
        new BiosSettingRecommendation { SetupQuestion = "PEG0 Max Payload size", RecommendedOption = "256" },
        new BiosSettingRecommendation { SetupQuestion = "PEG1 Max Payload size", RecommendedOption = "256" },
        new BiosSettingRecommendation { SetupQuestion = "PEG2 Max Payload size", RecommendedOption = "256" },
        new BiosSettingRecommendation { SetupQuestion = "PEG3 Max Payload size", RecommendedOption = "256" },

        new BiosSettingRecommendation { SetupQuestion = "Above 4G Decoding", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Above 4GB MMIO BIOS assignment", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Above 4G memory/Crypto Currency mining", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Re-Size BAR Support", RecommendedOption = "Enabled" },

        new BiosSettingRecommendation { SetupQuestion = "OS Type", RecommendedOption = "Windows UEFI mode" },
        new BiosSettingRecommendation { SetupQuestion = "Secure Boot Mode", RecommendedOption = "Standard" },
        new BiosSettingRecommendation { SetupQuestion = "TPM State", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "TCM State", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Me State", RecommendedOption = "Disabled" }, // might only work with bios mod (hard to disable)
        new BiosSettingRecommendation { SetupQuestion = "PTT", RecommendedOption = "Disable" }, // is it necessary for TPM? (hard to disable)
        new BiosSettingRecommendation { SetupQuestion = "Security Device Support", RecommendedOption = "Enabled" }, // required for TPM
        new BiosSettingRecommendation { SetupQuestion = "Intel Platform Trust Technology (PTT)", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Software Guard Extensions (SGX)", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "SW Guard Extension (security)", RecommendedOption = "Disabled" }, // didn't find
        new BiosSettingRecommendation { SetupQuestion = "Intel Trusted Execution Technology", RecommendedOption = "Disabled" },

        // AMD
        new BiosSettingRecommendation { SetupQuestion = "Power Down Enable", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory Fast Boot", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Memory interleaving", RecommendedOption = "Enabled" },
        new BiosSettingRecommendation { SetupQuestion = "Gear Down Mode", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "Global C-state Control", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "FCH Spread Spectrum", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "NX Mode", RecommendedOption = "Disabled" }, // valorant may need this enabled or virtualization
        new BiosSettingRecommendation { SetupQuestion = "SVM Mode", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "SMT Control", RecommendedOption = "Disable" },
        new BiosSettingRecommendation { SetupQuestion = "OC Explore Mode", RecommendedOption = "Expert" },
        new BiosSettingRecommendation { SetupQuestion = "CPU Over Temperature Alert", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "CPU temperature Warning Control", RecommendedOption = "Disabled" },

        new BiosSettingRecommendation { SetupQuestion = "PM L1 SS", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "ACP CLock Gating", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Clock Power Management(CLKREQ#)", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PCIe Power Management Features", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Opcache Control", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Onboard PCIE LAN PXE ROM", RecommendedOption = "Disabled" },
       
        new BiosSettingRecommendation { SetupQuestion = "IOMMU", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PSPP Policy", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 0 Enable", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 1 Enable", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 2 Enable", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "I3C/I2C 3 Enable", RecommendedOption = "Both Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "PSS Support", RecommendedOption = "Disabled" },
        
        new BiosSettingRecommendation { SetupQuestion = "Thunderbolt Wake Up Command", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Wake Up Event By", RecommendedOption = "BIOS" },
        new BiosSettingRecommendation { SetupQuestion = "LN2 Mode", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Game Boost", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "TSME", RecommendedOption = "Disabled" },

        // rgb
        new BiosSettingRecommendation { SetupQuestion = "When system is in working state", RecommendedOption = "Aura Off" },
        new BiosSettingRecommendation { SetupQuestion = "When system is in sleep, hibernate or soft off states", RecommendedOption = "Aura Off" },
        new BiosSettingRecommendation { SetupQuestion = "RGB Fusion", RecommendedOption = "Disabled" }, // didn't find

        // useful
        new BiosSettingRecommendation { SetupQuestion = "Boot Sector (MBR/GPT) Recovery Policy", RecommendedOption = "Auto Recovery" },

        // ads
        new BiosSettingRecommendation { SetupQuestion = "Download & Install ARMOURY CRATE app", RecommendedOption = "Disabled" },
        new BiosSettingRecommendation { SetupQuestion = "Download & Install MyASUS service & app", RecommendedOption = "Disabled" },

        //new BiosSettingRecommendation { SetupQuestion = "Tcc Activation Offset", RecommendedOption = "0" }, // exists both as Auto Setting and as value setting i think only the value setting should be changed to 0 (default is 0 anyway)
        //new BiosSettingRecommendation { SetupQuestion = "LAN Wake From DeepSx", RecommendedOption = "Disabled" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "Wake on LAN Enable", RecommendedOption = "Disabled" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "PME Enable", RecommendedOption = "Enabled" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "Power On By PCI-E", RecommendedOption = "Enabled" }, // required for wake on lan
        //new BiosSettingRecommendation { SetupQuestion = "WWAN", RecommendedOption = "Disabled" }, // required for wlan
        //new BiosSettingRecommendation { SetupQuestion = "WWAN Device", RecommendedOption = "Disabled" }, // required for wlan and bt
        //new BiosSettingRecommendation { SetupQuestion = "WWAN participant (WIFI)", RecommendedOption = "Disabled" }, // didn't find
        //new BiosSettingRecommendation { SetupQuestion = "WWAN (WIFI)", RecommendedOption = "Disabled" }, // didn't find
        //new BiosSettingRecommendation { SetupQuestion = "Wi-Fi Controller", RecommendedOption = "Disabled" }, // required for wifi
        //new BiosSettingRecommendation { SetupQuestion = "Wi-Fi Core", RecommendedOption = "Disabled" }, // didn't find
        //new BiosSettingRecommendation { SetupQuestion = "Bluetooth Controller", RecommendedOption = "Disabled" }, // required for bluetooth
        //new BiosSettingRecommendation { SetupQuestion = "Bluetooth", RecommendedOption = "Disabled" }, // required for bluetooth
        //new BiosSettingRecommendation { SetupQuestion = "BT Core", RecommendedOption = "Disabled" }, // required for bluetooth
        //new BiosSettingRecommendation { SetupQuestion = "Onboard CNVi Module Control (wifi & bt)", RecommendedOption = "Disabled" }, // didn't find
        //new BiosSettingRecommendation { SetupQuestion = "Connectivity mode (Wi-Fi & Bluetooth)", RecommendedOption = "Disabled" }, // required for wifi and bt
        //new BiosSettingRecommendation { SetupQuestion = "CS PL1 Value", RecommendedOption = "Max" }, // need smth for max value fields
        //new BiosSettingRecommendation { SetupQuestion = "PCIe Speed", RecommendedOption = "Max" }, // need smth for max value fields here though auto will be better
        //new BiosSettingRecommendation { SetupQuestion = "Max Link Speed", RecommendedOption = "Max" }, // need smth for max value fields here though auto will be better
        //new BiosSettingRecommendation { SetupQuestion = "DMI Max Link Speed", RecommendedOption = "Max" }, // need smth for max value fields here though auto will be better

        //new BiosSettingRecommendation { SetupQuestion = "Touch Pad", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Touch Panel", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Finger Print Sensor", RecommendedOption = "Disabled" },
        //new BiosSettingRecommendation { SetupQuestion = "Type C Support", RecommendedOption = "Disabled" }, // didn't find
    ];
}

