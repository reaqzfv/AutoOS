![AutoOS Hero Image](https://github.com/user-attachments/assets/65a294c9-603d-40ad-8fb2-20af203478e1)

<h1 align="center">
    AutoOS
</h1>

<div align="center">

[![Releases](https://img.shields.io/github/v/release/tinodin/AutoOS.svg?label=Release)](https://github.com/tinodin/AutoOS/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/tinodin/AutoOS/total?label=Total%20downloads)](https://github.com/tinodin/AutoOS/releases)
[![Discord](https://img.shields.io/badge/Discord-AutoOS-5865F2?style=flat&logo=discord&logoColor=white)](https://discord.gg/bZU4dMMWpg)
[![PayPal](https://img.shields.io/badge/Donate-PayPal-003087?logo=paypal&logoColor=fff)](https://www.paypal.com/donate/?hosted_button_id=GVEVUSHUWXEAG)

</div>

AutoOS is a WinUI3 application focused on automation to improve performance while ensuring privacy and compatibility.

## ✨ Features
- Automatically set every monitor to their highest supported refresh rate
- Automatically benchmark and apply GPU, XHCI Controller and NIC affinities and reserve them if enough cores
- Easily update GPU driver with one click
- Toggle XHCI Interrupt Moderation without having to restart your PC
- Toggle between service states with configured functionality
- Apply all hidden BIOS Settings (500+) in one click
- Automatically import Epic Games and Steam titles from old install
- Custom Game Launcher supporting (Epic Games, Steam, Ryujinx)
- Stop processes when running your game to automatically stop all unnecessary services and executables

## ⚠️ Current Issues
- **Blank screen after installing the Graphics Driver:** You may experience a blank screen in the App after installing the Graphics Driver. To fix this, resize the window from the left side until it rerenders the UI.

## 🚀 Getting Started

> [!NOTE]
> If you want to change the display language of Windows, do so **after** AutoOS is fully set up.

**Step 1:** Before installing, please join my [Discord Server](https://discord.gg/bZU4dMMWpg) so I can assist you with any issues while installing and get notified about new updates/changes.

**Step 2:** Open PowerShell as Admin and don't close it until finished.

**Step 3:**  Paste this into the PowerShell window to check if your disk is using the GPT partition style. 

```ps1
$DISKNUMBER = (Get-Partition -DriveLetter C | Get-Disk).Number; (Get-Partition -DriveLetter C | Get-Disk).PartitionStyle
```

If it outputs `MBR` you must first convert your disk to GPT using this command. After the conversion you want to make sure that you set `Boot Mode` to `UEFI` in your BIOS so that you will be able to boot.

```ps1
mbr2gpt /convert /disk:$DISKNUMBER /allowFullOS
```

**Step 4:**  Paste this into the PowerShell window to check if your disk is BitLocker encrypted. 

```ps1
(Get-BitLockerVolume -MountPoint "C:").VolumeStatus
```

If it outputs `FullyEncrypted` you must first disable BitLocker using this command.

```ps1
Disable-BitLocker -MountPoint "C:"
```

**Step 4:** Open Disk Management.

**Step 5:** Find your your C: partition / biggest partition.

**Step 6:** Right click on it and select "Shrink Volume".

**Step 7:** In "Enter the amount of space to shrink in MB:" input at least 65536 (=64GB) or higher 131072 (=128GB), 262144 (=256GB), 524288 (=512GB). If you can't shrink that much space, use [Minitool Partition Wizard Free](https://cdn2.minitool.com/?p=pw&e=pw-free) (decline each offer in the installer), then use the `Split` function to create a new partition.

**Step 8:** Right click on the "Unallocated" partition and select "New Simple Volume". Then just click next until you have a "New Volume". Then define this variable in the PowerShell window (e.g. `"E:"`).

```ps1
$TARGETDRIVE = 
```

**Step 9:** Go to the Drivers / Support page or your Mainboard / PC and download your LAN, Wi-Fi and Bluetooth driver (No Audio, Chipset, or anything else). On prebuilts you may also need the disk driver. Extract them all `(.exe/.zip)` into one folder using 7-Zip / NanaZip / WinRar etc. Then define this variable in the PowerShell window (e.g. `"I:\drivers"`).

```ps1
$DRIVERDIR = 
```

**Step 10:** Download the latest Windows ISO from the artifact [here](https://nightly.link/tinodin/uup-dump-get-windows-iso/workflows/23H2/main/23H2.zip). Other ISOs are going to give you worse results.

**Step 11:** Extract the downloaded zip file.

**Step 12:** Extract the ISO file using 7-Zip / NanaZip / WinRar etc. Then define this variable in the PowerShell window (e.g. `"C:\Users\user\Downloads\23H2\23H2"`).

```ps1
$EXTRACTED_ISO = 
```

**Step 13:** Paste this into the PowerShell window to apply the `install.wim` to the new partition.

```ps1
DISM /Apply-Image /ImageFile:$EXTRACTED_ISO\sources\install.wim /Index:1 /ApplyDir:$TARGETDRIVE
```

**Step 14:** Paste this into the PowerShell window to install the drivers you downloaded before.

```ps1
DISM /Image:$TARGETDRIVE\ /Add-Driver /Driver:$DRIVERDIR /Recurse
```

**Step 15:** Paste this into the PowerShell window to create the `Panther` folder and download the `unattend.xml`.

```ps1
New-Item -ItemType Directory -Path $TARGETDRIVE\Windows\Panther -Force | Out-Null; Invoke-WebRequest -Uri "https://raw.githubusercontent.com/tinodin/AutoOS/master/unattend.xml" -OutFile $TARGETDRIVE\Windows\Panther\unattend.xml
```

**Step 16:** Paste this into the PowerShell window to create the boot entry.

```ps1
bcdboot $TARGETDRIVE\Windows; bcdedit /set "{default}" description "AutoOS"; label $TARGETDRIVE "AutoOS"
```

**Step 18:** Restart your computer and boot into the default option. Then wait for Windows to finish installing.

**Step 19:** Once finished, wait for AutoOS to open up (On slower systems this may take a minute).

**Step 20:** Select your settings and click "Install AutoOS".

## 📷Screenshots
### Installer

<table>
<tr>
  <td align="center">Light</td>
  <td align="center">Dark</td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Home.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Home.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Personalization.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Personalization.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Browser.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Browser.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Applications.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Applications.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Display.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Display.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Graphics%20Card.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Graphics%20Card.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Per-CPU%20Scheduling.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Per-CPU%20Scheduling.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Bluetooth%20&%20Devices.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Bluetooth%20&%20Devices.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Network%20&%20Internet.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Network%20&%20Internet.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Energy%20&%20Power.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Energy%20&%20Power.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Services%20&%20Drivers.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Services%20&%20Drivers.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Light%29/Install%20AutoOS.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Installer%20%28Dark%29/Install%20AutoOS.png"/></td>
</tr>
</table>

### Settings

<table>
<tr>
  <td align="center">Light</td>
  <td align="center">Dark</td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Home.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Home.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Display.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Display.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Graphics%20Card.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Graphics%20Card.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Per-CPU%20Scheduling.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Per-CPU%20Scheduling.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Bluetooth%20&%20Devices.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Bluetooth%20&%20Devices.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Network%20&%20Internet.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Network%20&%20Internet.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Energy%20&%20Power.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Energy%20&%20Power.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Services%20&%20Drivers.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Services%20&%20Drivers.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Tracking%20&%20Logging.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Tracking%20&%20Logging.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/BIOS%20Settings.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/BIOS%20Settings.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Disk%20Cleanup.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Disk%20Cleanup.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Windows%20Security.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Windows%20Security.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Windows%20Update.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Windows%20Update.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Games.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Games.png"/></td>
</tr>
<tr>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Light%29/Settings.png"/></td>
  <td><img src="https://raw.githubusercontent.com/tinodin/AutoOS-Resources/main/AutoOS%20Settings%20%28Dark%29/Settings.png"/></td>
</tr>
</table>

## ⚙️ Build instructions

### 1. 🖥️ Visual Studio 2026 Insiders

Ensure that your installation includes the appropriate workloads:

- On the **Workloads** tab of the Visual Studio installer, check:
  - **.NET Desktop Development**
  - **WinUI Application Development**


### 2. 🔗 Clone the repository

Clone the repository and run this in the terminal inside of Visual Studio.  
```
dotnet nuget add source https://pkgs.dev.azure.com/dotnet/CommunityToolkit/_packaging/CommunityToolkit-Labs/nuget/v3/index.json -n CommunityToolkit-Labs
```

## 🙏 Credits

**Amitxv / Valleyofdoom:**  
Thank you for your [PC-Tuning Guide](https://github.com/valleyofdoom/PC-Tuning) and your useful utilities:  
- [AutoGpuAffinity](https://github.com/valleyofdoom/AutoGpuAffinity)  
- [service-list-builder](https://github.com/valleyofdoom/service-list-builder)  
- [TimerResolution](https://github.com/valleyofdoom/TimerResolution)  
- [ReservedCpuSets](https://github.com/valleyofdoom/ReservedCpuSets)  

Without your guide this project wouldn't exist. It inspired me to attempt to automate it and ultimately start this project.

---

**Revi Team:**  
Thank you for [SVCGROUP.ps1](https://github.com/meetrevision/playbook/blob/main/src/Executables/SVCGROUP.ps1), [useful registry keys](https://github.com/meetrevision/playbook/tree/main/src/Configuration/Tasks/registry) and for first introducing me to custom ISOs. Without the ReviOS project I don't know if I would have ever gotten into tweaking Windows.

---

**Imribiy:**  
Thank you for your research on [Configuring services and features](https://github.com/imribiy/XOS/tree/main/configure-services-and-features) and [AMD GPU Tweaks](https://github.com/imribiy/amd-gpu-tweaks).  

---

**Duckleeng:**  
Thank you for your research on [Receive Side Scaling (RSS)](https://github.com/Duckleeng/TweakCollection/tree/main/Research#which-nicsdrivers-support-receive-side-scaling-rss) and [Windows 11 24H2 AutoBoost Behavior](https://github.com/Duckleeng/TweakCollection/tree/main/Research#windows-11-24h2-autoboost-behavior).

---

**djdallmann:**  
Thank you for your research on [Network Performance](https://github.com/djdallmann/GamingPCSetup/blob/master/CONTENT/RESEARCH/NETWORK/README.md).

---

**m417z (Michael Maltsev):**  
Thank you for creating [Windhawk](https://github.com/ramensoftware/windhawk) and for helping me to publish my mod [Auto Theme Switcher](https://windhawk.net/mods/auto-theme-switcher).

---

**ghost1372 (Mahdi Hosseini):**  
Thank you for creating [DevWinUI](https://github.com/ghost1372/DevWinUI). It inspired me to learn C# and rewrite this project in WinUI 3. I appreciate your quick responses, fixes to issues, and the helpful [workflow file](https://github.com/ghost1372/DevWinUI/blob/main/.github/workflows/publish-release.yml), which I adapted for this project.

---

**rgl (Rui Lopes):**  
Thank you for creating [uup-dump-get-windows-iso](https://github.com/rgl/uup-dump-get-windows-iso), which I adaptet to automatically build the latest Windows release in order to speed up and simplify AutoOS installation.

---

**cschneegans (Christoph Schneegans):**  
Thank you for creating [unattend-generator](https://github.com/cschneegans/unattend-generator), which helps AutoOS installation to be seamless.

## 📜 License

This project is licensed under the **GNU General Public License v3.0**. See the `LICENSE` file for details.

### Third-Party Components

1. **NSudo**
   - Licensed under the **MIT License**.
   - Source: [M2TeamArchived/NSudo](https://github.com/M2TeamArchived/NSudo)

2. **nvidiaProfileInspector**
   - Licensed under the **MIT License**.
   - Source: [Orbmu2k/nvidiaProfileInspector](https://github.com/Orbmu2k/nvidiaProfileInspector)

3. **RadeonSoftwareSlimmer**
   - Licensed under the **GNU General Public License v3.0**.
   - Source: [GSDragoon/RadeonSoftwareSlimmer](https://github.com/GSDragoon/RadeonSoftwareSlimmer)
   - Changes: Added command line options for preinstall
   - Fork: [tinodin/RadeonSoftwareSlimmer](https://github.com/tinodin/RadeonSoftwareSlimmer)

4. **TimerResolution**
   - Licensed under the **GNU General Public License v3.0**.
   - Source: [valleyofdoom/TimerResolution](https://github.com/valleyofdoom/TimerResolution)

5. **AutoGpuAffinity**
   - Licensed under the **GNU General Public License v3.0**.
   - Source: [valleyofdoom/AutoGpuAffinity](https://github.com/valleyofdoom/AutoGpuAffinity)
   - Changes: Added functionality to also apply the affinity to the parent of the GPU and output the 3 best performing CPUs
   - Fork: [tinodin/AutoGpuAffinity](https://github.com/tinodin/AutoGpuAffinity)

6. **Service List Builder**
   - Licensed under the **GNU General Public License v3.0**.
   - Source: [valleyofdoom/service-list-builder](https://github.com/valleyofdoom/service-list-builder)
   - Changes: Removed `shutdown /r /t 0` from created lists, added `--output-dir` switch because of MSIX restrictions.
   - Fork: [tinodin/service-list-builder](https://github.com/tinodin/service-list-builder)

7. **LowAudioLatency**
    - Have to ask for explicit permission from [sppdl](https://github.com/spddl)
    - Source: [sppdl/LowAudioLatency](https://github.com/spddl/LowAudioLatency)

8. **7-Zip**
```
  7-Zip
  ~~~~~
  License for use and distribution
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

  7-Zip Copyright (C) 1999-2025 Igor Pavlov.

  The licenses for files are:

    - 7z.dll:
         - The "GNU LGPL" as main license for most of the code
         - The "GNU LGPL" with "unRAR license restriction" for some code
         - The "BSD 3-clause License" for some code
         - The "BSD 2-clause License" for some code
    - All other files: the "GNU LGPL".

  Redistributions in binary form must reproduce related license information from this file.

  Note:
    You can use 7-Zip on any computer, including a computer in a commercial
    organization. You don't need to register or pay for 7-Zip.


GNU LGPL information
--------------------

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You can receive a copy of the GNU Lesser General Public License from
    http://www.gnu.org/




BSD 3-clause License in 7-Zip code
----------------------------------

  The "BSD 3-clause License" is used for the following code in 7z.dll
    1) LZFSE data decompression.
       That code was derived from the code in the "LZFSE compression library" developed by Apple Inc,
       that also uses the "BSD 3-clause License".
    2) ZSTD data decompression.
       that code was developed using original zstd decoder code as reference code.
       The original zstd decoder code was developed by Facebook Inc,
       that also uses the "BSD 3-clause License".

  Copyright (c) 2015-2016, Apple Inc. All rights reserved.
  Copyright (c) Facebook, Inc. All rights reserved.
  Copyright (c) 2023-2025 Igor Pavlov.

Text of the "BSD 3-clause License"
----------------------------------

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may
   be used to endorse or promote products derived from this software without
   specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

---




BSD 2-clause License in 7-Zip code
----------------------------------

  The "BSD 2-clause License" is used for the XXH64 code in 7-Zip.

  XXH64 code in 7-Zip was derived from the original XXH64 code developed by Yann Collet.

  Copyright (c) 2012-2021 Yann Collet.
  Copyright (c) 2023-2025 Igor Pavlov.

Text of the "BSD 2-clause License"
----------------------------------

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

---




unRAR license restriction
-------------------------

The decompression engine for RAR archives was developed using source
code of unRAR program.
All copyrights to original unRAR code are owned by Alexander Roshal.

The license for original unRAR code has the following restriction:

  The unRAR sources cannot be used to re-create the RAR compression algorithm,
  which is proprietary. Distribution of modified unRAR sources in separate form
  or as a part of other software is permitted, provided that it is clearly
  stated in the documentation and source comments that the code may
  not be used to develop a RAR (WinRAR) compatible archiver.

--
```
- Source: [7-Zip](https://www.7-zip.org)
 
9. **Custom Resolution Utility (CRU)**
```
Copyright (C) 2012-2022 ToastyX
https://monitortests.com/custom-resolution-utility

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software the rights to use, copy, and/or distribute copies of the
software subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies of the software.

THE SOFTWARE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY CLAIM, DAMAGES, OR
OTHER LIABILITY IN CONNECTION WITH THE USE OF THE SOFTWARE.
```
- Source: [Custom Resolution Utility (CRU)](https://monitortests.com/custom-resolution-utility)

10. **DeviceCleanup**
```
Allowed:
- usage in any environment, including commercial
- include in software products, including commercial
- include on CD/DVD of computer magazines

Not allowed:
- modify any of the files
- offer for download by means of a "downloader" software
```
- Source: [DeviceCleanup](https://www.uwe-sieber.de/misc_tools.html#devicecleanup)

11. **DriveCleanup**
```
Allowed:
- usage in any environment, including commercial
- include in software products, including commercial
- include on CD/DVD of computer magazines

Not allowed:
- modify any of the files
- offer for download by means of a "downloader" software
```
- Source: [DriveCleanup](https://www.uwe-sieber.de/drivetools.html#drivecleanup)

12. **RwEverything**
- Have to ask for permission ._.
```
This utility should not be bundled (in any form) in commercial or consumer products.
```
