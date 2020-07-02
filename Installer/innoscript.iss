; EDDLite script

#define MyAppName "EDDLite"
#ifndef MyAppVersion
#define MyAppVersion "0.5.0"
#endif
#define MyAppPublisher "Robby & EDDiscovery Team"
#define MyAppURL "https://github.com/EDDLite"
#define MyAppExeName "EDDLite.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AllowUNCPath=no
AppId={{578B2948-09E9-463F-8A5A-0D8D7ABA3C62}
AppName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppVerName={#MyAppName} {#MyAppVersion}
AppVersion={#MyAppVersion}
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableDirPage=no
DisableWelcomePage=no
DirExistsWarning=auto
LicenseFile="{#SourcePath}\..\EDDLite\Resources\EDD License.rtf"
OutputBaseFilename={#MyAppName}-{#MyAppVersion}
OutputDir="{#SourcePath}\installers"
SolidCompression=yes
SourceDir="{#SourcePath}\..\EDDLite\bin\Release"
UninstallDisplayIcon={app}\{#MyAppExeName}
UsePreviousTasks=no
UsePreviousAppDir=yes

WizardImageFile="{#SourcePath}\Logo.bmp"
WizardSmallImageFile="{#SourcePath}\Logosmall.bmp"
WizardImageStretch=no
WizardStyle=modern
WizardSizePercent=150

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "EDDLite.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "EDDLite.exe.config"; DestDir: "{app}"; Flags: ignoreversion

Source: "x64\*.*"; DestDir: "{app}\x64"; Flags: ignoreversion recursesubdirs createallsubdirs replacesameversion
Source: "x86\*.*"; DestDir: "{app}\x86"; Flags: ignoreversion recursesubdirs createallsubdirs replacesameversion
Source: "*.dll"; DestDir: "{app}"; Flags: ignoreversion;
Source: "*.pdb"; DestDir: "{app}"; Flags: ignoreversion;
Source: "..\..\..\Installer\ExtraFiles\EUROCAPS.TTF"; DestDir: "{app}"; Flags: ignoreversion;
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Messages]
SelectDirBrowseLabel=To continue, click Next.
ConfirmUninstall=Are you sure you want to completely remove %1 and all of its components? Note that all your user data is not removed by this uninstall and is still stored in your local app data

