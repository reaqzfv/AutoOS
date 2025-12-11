Add-Type -AssemblyName System.Windows.Forms

$admin = [Security.Principal.WindowsPrincipal]::new([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $admin.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "This script must be run as Administrator."
    return
}

Write-Host "Please select the Windows ISO..."
$IsoPicker = New-Object System.Windows.Forms.OpenFileDialog
$IsoPicker.Filter = "ISO Files (*.iso)|*.iso"
$IsoPicker.Title = "Select the Windows ISO file"
$IsoPicker.Multiselect = $false
if ($IsoPicker.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) {
    Write-Host "No ISO selected. Exiting."
    return
}

$InstallDrivers = Read-Host "Do you want to install drivers? (y/n)"
if ($InstallDrivers -match '^[Yy]') {
    Write-Host "Please select your drivers folder..."
    $DriverPicker = New-Object System.Windows.Forms.FolderBrowserDialog
    $DriverPicker.Description = "Select the drivers folder"
    if ($DriverPicker.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) { return }
    $DriversDir = $DriverPicker.SelectedPath
}

Write-Host ""
Write-Host "===== Step 1: Check Partition Style ====="
Write-Host ""
$DiskNumber = (Get-Partition -DriveLetter C | Get-Disk).Number
if ((Get-Partition -DriveLetter "C" | Get-Disk).PartitionStyle -eq 'MBR') {
    Write-Host "Partition style is MBR. Converting to GPT..."
    mbr2gpt /convert /disk:$DiskNumber /allowFullOS
    Write-Host "Please set Boot Mode to UEFI in BIOS after conversion, then rerun this script."
    return
} else {
    Write-Host "Partition style is GPT"
}

Write-Host ""
Write-Host "===== Step 2: Check BitLocker State ====="
Write-Host ""
if ((Get-BitLockerVolume -MountPoint C:).VolumeStatus -eq "FullyEncrypted") {
    Write-Host "BitLocker is enabled. Disabling..."
    Disable-BitLocker -MountPoint C:
    Write-Host "Wait until decryption finishes, then rerun this script."
    return
} else {
    Write-Host "BitLocker is disabled"
}

Write-Host ""
Write-Host "===== Step 3: Check Shrinkable Space ====="
Write-Host ""
$Partitions = Get-Partition -DiskNumber $DiskNumber | Where-Object { $_.Type -eq 'Basic' -and $_.Size -gt 0 }
$ShrinkTargetsMB = @(524288, 262144, 131072, 65536)

foreach ($Partition in $Partitions) {
    try {
        $Supported = Get-PartitionSupportedSize -DriveLetter $Partition.DriveLetter
        $MaxShrinkMB = [math]::Floor(($Partition.Size - $Supported.SizeMin) / 1MB)
        Write-Host "Partition $($Partition.DriveLetter): $MaxShrinkMB MB shrinkable"
        $Partition | Add-Member -NotePropertyName MaxShrinkMB -NotePropertyValue $MaxShrinkMB
    } catch {
        Write-Host "Partition $($Partition.DriveLetter): Failed to query shrinkable space"
        $Partition | Add-Member -NotePropertyName MaxShrinkMB -NotePropertyValue 0
    }
}

$ShrinkablePartition = $null
$ShrinkAmountMB = 0
foreach ($Partition in $Partitions) {
    foreach ($Target in $ShrinkTargetsMB) {
        if ($Partition.MaxShrinkMB -ge $Target) {
            $ShrinkablePartition = $Partition
            $ShrinkAmountMB = $Target
            break
        }
    }
    if ($ShrinkablePartition) { break }
}

if (-not $ShrinkablePartition) {
    Write-Host "No partition with at least 64GB of shrinkable space found. Free up more space, reboot and try again."
    return
}

Write-Host ""
Write-Host "===== Step 4: Shrink Partition ====="
Write-Host ""
Write-Host "Shrinking partition $($ShrinkablePartition.DriveLetter): by $ShrinkAmountMB MB..."
$NewSize = $ShrinkablePartition.Size - ($ShrinkAmountMB * 1MB)
Resize-Partition -DriveLetter $ShrinkablePartition.DriveLetter -Size $NewSize

Write-Host ""
Write-Host "===== Step 5: Create new NTFS Partition ====="
Write-Host ""
$NewPartition = New-Partition -DiskNumber $DiskNumber -UseMaximumSize -AssignDriveLetter
$NewDriveLetter = $NewPartition.DriveLetter
$TargetDrive = "${NewDriveLetter}:"
Write-Host "Creating new NTFS partition $TargetDrive..."
Start-Process -FilePath "cmd.exe" -ArgumentList "/c ""format $TargetDrive /fs:ntfs /q /y /v:AutoOS > nul 2> nul""" -NoNewWindow -Wait

Write-Host ""
Write-Host "===== Step 6: Apply Windows Image ====="
Write-Host ""
Write-Host "Mounting ISO..."
try {
    $MountedIso = (Mount-DiskImage -ImagePath $IsoPicker.FileName -PassThru | Get-Volume).DriveLetter + ":"
    Write-Host "Copying install.wim..."
    $TempWim = "$env:TEMP\install.wim"
    Copy-Item -Path "$MountedIso\sources\install.wim" -Destination $TempWim -Force
    attrib -r $TempWim
    Write-Host "Unmounting ISO..."
} finally {
    Dismount-DiskImage -ImagePath $IsoPicker.FileName | Out-Null
}

Write-Host "Downloading PSTools..."
New-Item -ItemType Directory -Path $env:TEMP -Force | Out-Null
Invoke-WebRequest -Uri "https://download.sysinternals.com/files/PSTools.zip" -OutFile "$env:TEMP\PSTools.zip"
Write-Host "Extracting PSTools..."
Expand-Archive -Path "$env:TEMP\PSTools.zip" -DestinationPath "$env:TEMP\PSTools" -Force
New-Item -Path "HKCU:\Software\Sysinternals\PsExec" -Force | Out-Null
New-ItemProperty -Path "HKCU:\Software\Sysinternals\PsExec" -Name "EulaAccepted" -Value 1 -PropertyType DWord -Force | Out-Null

Write-Host "Mounting install.wim..."
$MountDirectory = "C:\mnt"
New-Item -Path $MountDirectory -ItemType Directory -Force | Out-Null

$Images = Get-WindowsImage -ImagePath $TempWim -ErrorAction Stop
foreach ($Image in $Images) {
    Mount-WindowsImage -Path $MountDirectory -ImagePath $TempWim -Name $Image.ImageName -ErrorAction Stop | Out-Null
    & "$env:TEMP\PSTools\PsExec64.exe" -s "$env:windir\system32\fsutil.exe" 8dot3name strip /f /s $MountDirectory;
    Write-Host "Unmounting install.wim..."
    Dismount-WindowsImage -Path $MountDirectory -Save -ErrorAction Stop | Out-Null
    Remove-Item -LiteralPath $MountDirectory -Force -ErrorAction SilentlyContinue
}

Remove-Item "$env:TEMP\PSTools" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "$env:TEMP\PSTools.zip" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "HKCU:\Software\Sysinternals\PsExec" -Recurse -Force

Write-Host "Applying Windows image to $TargetDrive..."
DISM /Apply-Image /ImageFile:$TempWim /Index:1 /ApplyDir:$TargetDrive
Remove-Item $TempWim -Force -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "===== Step 7: Install Drivers ====="
Write-Host ""
Write-Host "Installing drivers from $DriversDir..."
DISM /Image:$TargetDrive /Add-Driver /Driver:$DriversDir /Recurse

Write-Host ""
Write-Host "===== Step 8: Add unattend.xml ====="
Write-Host ""
Write-Host "Adding unattend.xml..."
New-Item -ItemType Directory -Path $TargetDrive\Windows\Panther -Force | Out-Null
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/tinodin/AutoOS/master/unattend.xml" -OutFile $TargetDrive\Windows\Panther\unattend.xml

Write-Host ""
Write-Host "===== Step 9: Create Boot Entry ====="
Write-Host ""
Write-Host "Creating boot entry..."
bcdboot $TargetDrive\Windows
bcdedit /set "{default}" description "AutoOS"
bcdedit /set bootmenupolicy legacy
bcdedit /timeout 6
Write-Host ""
Write-Host "===== AutoOS Deployment Completed Successfully! ====="
Write-Host "Press Enter to exit..."
if ($Host.Name -eq 'ConsoleHost') {
    [void][System.Console]::ReadLine()
    return
}
