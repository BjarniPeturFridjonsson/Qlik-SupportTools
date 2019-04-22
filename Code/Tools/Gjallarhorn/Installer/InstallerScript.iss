; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Qlik Proactive Express"
#define MyAppVersion "1.4.9"
#define MyAppPublisher "Qlik Technologies Inc"
#define MyAppURL "http://www.qlik.com"
#define MyAppExeName "ProactiveExpress.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{C3C27955-AD07-4927-95E2-E72DD2C0E6C4}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename="Qlik Proactive Express Installer"
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\Gjallarhorn\bin\AnyCPU\Release\ProactiveExpress.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\Bifrost.Model.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\Ciloci.Flee.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\Eir.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\ProactiveExpress.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\SenseApiLibrary.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\FreyrCommon.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\QMS API.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Gjallarhorn\bin\AnyCPU\Release\settings.xml"; DestDir: "{app}"; Flags: ignoreversion
;Service installer batch file
Source: "installservice.bat"; DestDir: "{app}"; Flags: ignoreversion


[Run]
;Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
Filename: "{app}\installservice.bat"; Parameters: """{app}\ProactiveExpress.exe"""; Flags: runhidden shellexec

[UninstallRun]
;Remove service
Filename: "net"; Parameters: "stop QlikProactiveExpress"; Flags: runhidden
Filename: "sc"; Parameters: "delete QlikProactiveExpress"; Flags: runhidden
