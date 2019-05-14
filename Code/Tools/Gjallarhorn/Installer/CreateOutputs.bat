del "%~dp0\output\Qlik Proactive Express For QlikView Installer.exe"
del "%~dp0\output\Qlik Proactive Express For Qlik Sense Installer.exe"
del "%~dp0\output\Qlik Proactive Express For Qlik Sense Offline Installer.exe"


del "%~dp0..\Gjallarhorn\bin\AnyCPU\Release\Settings.xml"
ren "%~dp0..\Gjallarhorn\bin\AnyCPU\Release\SettingsProdQv.xml" Settings.xml
"C:\Program Files (x86)\Inno Setup 5\Compil32.exe" /cc "%~dp0\InstallerScript.iss"
ren "%~dp0\output\Qlik Proactive Express Installer.exe" "Qlik Proactive Express For QlikView Installer.exe"

del "%~dp0..\Gjallarhorn\bin\AnyCPU\Release\Settings.xml"
ren "%~dp0..\Gjallarhorn\bin\AnyCPU\Release\SettingsProdSense.xml" Settings.xml
"C:\Program Files (x86)\Inno Setup 5\Compil32.exe" /cc "%~dp0\InstallerScript.iss"
ren "%~dp0\output\Qlik Proactive Express Installer.exe" "Qlik Proactive Express For Qlik Sense Installer.exe""

del "%~dp0..\Gjallarhorn\bin\AnyCPU\Release\Settings.xml"
ren "%~dp0..\Gjallarhorn\bin\AnyCPU\Release\SettingsProdSenseOffline.xml" Settings.xml
"C:\Program Files (x86)\Inno Setup 5\Compil32.exe" /cc "%~dp0\InstallerScript.iss"
ren "%~dp0\output\Qlik Proactive Express Installer.exe" "Qlik Proactive Express For Qlik Sense Offline Installer.exe"

start explorer.exe "%~dp0\output\"