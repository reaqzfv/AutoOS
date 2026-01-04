# Credit: Revi Team
# License: Attribution-NonCommercial-ShareAlike 4.0 International
# Source: https://github.com/meetrevision/playbook/blob/main/src/Executables/SVCGROUP.ps1
# Modified: Removed netprofm, TextInputManagementService, Appinfo, UserManager, ProfSvc, gpsvc, camsvc, StateRepository, CryptSvc in order to make them stoppable in service disabled state 

$registryPath = "HKLM:\SYSTEM\CurrentControlSet\Services\"
$services = @(
    "AppXSvc",
    "AudioEndpointBuilder",
    "BITS",
    "BrokerInfrastructure",
    "CDPSvc",
    "ClipSVC",
    "CoreMessagingRegistrar",
    "DcomLaunch",
    "DeviceAssociationService",
    "Dhcp",
    "DispBrokerDesktopSvc",
    "DisplayEnhancementService",
    "Dnscache",
    "DPS",
    "EventLog",
    "EventSystem",
    "FDResPub",
    "FontCache",
    "hidserv",
    "iphlpsvc",
    "KeyIso",
    "LanmanServer",
    "LanmanWorkstation",
    "LicenseManager",
    "lmhosts",
    "LSM",
    "NcbService",
    "NcdAutoSetup",
    "NlaSvc",
    "nsi",
    "PcaSvc",
    "Power",
    "SamSs",
    "Schedule",
    "SENS",
    "ShellHWDetection",
    "SSDPSRV",
    "SstpSvc",
    "StorSvc",
    "SysMain",
    "SystemEventsBroker",
    "Themes",
    "TimeBrokerSvc",
    "TokenBroker",
    "TrkWks",
    "UsoSvc",
    "VaultSvc",
    "WdiSystemHost",
    "WinHttpAutoProxySvc",
    "WpnService",
    "wuauserv"
)

foreach ($service in $services) {
    New-ItemProperty -Path "$registryPath\$service" -Name "SvcHostSplitDisable" -Value 1 -PropertyType DWord -Force
}

$userServices = @(
    "CDPUserSvc_*",
    "OneSyncSvc_*",
    "WpnUserService_*"
)

foreach ($service in $userServices) {
    $matchingServices = Get-Service | Where-Object { $_.Name -like $service }

    foreach ($matchingService in $matchingServices) {
		New-ItemProperty -Path "$registryPath\$($matchingService.Name)" -Name "SvcHostSplitDisable" -Value 1 -PropertyType  DWord -Force
    }
}