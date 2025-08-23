reg load HKU\TEMP "$env:LOCALAPPDATA\Packages\Microsoft.WindowsNotepad_8wekyb3d8bbwe\Settings\settings.dat" >$null

$regContent = @'
Windows Registry Editor Version 5.00

[HKEY_USERS\TEMP\LocalState]
"AutoCorrect"=hex(5f5e10b):00,cd,ff,04,45,95,13,dc,01
"GhostFile"=hex(5f5e10b):00,fc,13,31,4b,95,13,dc,01
"OpenFile"=hex(5f5e104):01,00,00,00,9f,01,46,4c,95,13,dc,01
"RecentFilesEnabled"=hex(5f5e10b):00,64,5d,84,4a,95,13,dc,01
'@

New-Item "$env:TEMP\Notepad.reg" -Value $regContent -Force | Out-Null

regedit.exe /s "$env:TEMP\Notepad.reg"
Start-Sleep 1
reg unload HKU\TEMP >$null
Remove-Item "$env:TEMP\Notepad.reg" -Force -ErrorAction SilentlyContinue