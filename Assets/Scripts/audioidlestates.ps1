Get-WmiObject -Class Win32_SoundDevice | Where-Object { $_.PNPDeviceID -like "HDAUDIO*" } | ForEach-Object {
    $pnpDeviceId = $_.PNPDeviceID
    $regPath = "HKLM:\SYSTEM\CurrentControlSet\Enum\$pnpDeviceId"
    $driver = (Get-ItemProperty -Path $regPath -Name "Driver" -ErrorAction SilentlyContinue).Driver
    $classKey = "HKLM:\SYSTEM\CurrentControlSet\Control\Class\$driver\PowerSettings"
    Set-ItemProperty -Path $classKey -Name "ConservationIdleTime" -Value ([byte[]](0xFF,0xFF,0xFF,0xFF))
    Set-ItemProperty -Path $classKey -Name "PerformanceIdleTime" -Value ([byte[]](0xFF,0xFF,0xFF,0xFF))
    Set-ItemProperty -Path $classKey -Name "IdlePowerState" -Value ([byte[]](0x01,0x00,0x00,0x00))
}
