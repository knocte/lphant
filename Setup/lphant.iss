; This file is part of eLePhant
; Copyright (C)2003 Juanjo<j_u_a_n_j_o@users.sourceforge.net / http://lphant.sourceforge.net >
;
; This program is free software; you can redistribute it and/or
; modify it under the terms of the GNU General Public License
; as published by the Free Software Foundation; either
; version 2 of the License,or (at your option) any later version.
;
; This program is distributed in the hope that it will be useful,
; but WITHOUT ANY WARRANTY; without even the implied warranty of
; MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
; GNU General Public License for more details.
;
; You should have received a copy of the GNU General Public License
; along with this program; if not,write to the Free Software
; Foundation,Inc.,675 Mass Ave,Cambridge,MA 02139,USA.

[Setup]
;Never change the value AppName=lphant : read AppId in InnoSetup help
AppName=lphant

;----------------- TO UPDATE FOR A NEW VERSION ----------------------
AppVerName=lphant v1.01
AppVersion=v1.01
VersionInfoVersion=1.0.1.0
;--------------------------------------------------------------------

AppPublisher=Juanjo
AppPublisherURL=http://www.lphant.com
AppSupportURL=http://www.lphant.com
AppUpdatesURL=http://www.lphant.com
AppMutex={lphant-DD14EC11-CB90-4956-B8F4-F5D6D708DC33}
DefaultDirName={pf}\lphant
DefaultGroupName=lphant
DisableProgramGroupPage=true
AllowNoIcons=true
LicenseFile=..\license.txt
AppCopyright=Copyright © Juanjo 2003
ShowLanguageDialog=auto
InfoBeforeFile=..\readme.txt
Compression=lzma
SolidCompression=true
UninstallDisplayIcon={app}\eLePhantClient.exe
VersionInfoCompany=Juanjo
VersionInfoDescription=lphant : P2P client
OutputDir=..\Setup

[Files]
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: ..\Source\Client\bin\Release\eLePhantClient.exe; DestDir: {app}; Flags: ignoreversion
Source: ..\Source\Client\bin\Release\eLePhant.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\Source\Client\bin\Release\eLePhant.Interface.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\Source\Client\bin\Release\MagicLibrary.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\Source\Client\bin\Release\ICSharpCode.SharpZipLib.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\Source\Client\bin\Release\server.met; DestDir: {app}; Flags: ignoreversion skipifsourcedoesntexist
;Source: ..\Source\Client\bin\Release\ipfilter.dat; DestDir: {app}; Flags: ignoreversion skipifsourcedoesntexist
Source: ..\Source\Client\bin\Release\webSearchs.xml; DestDir: {app}; Flags: ignoreversion; Check: BackupFile({app}\webSearchs.xml)
Source: ..\Source\Client\bin\Release\Language\*; DestDir: {app}\Language; Flags: ignoreversion recursesubdirs
Source: ..\Source\Client\bin\Release\Skins\*; DestDir: {app}\Skins; Flags: ignoreversion recursesubdirs
Source: ..\changelog.txt; DestDir: {app}; Flags: ignoreversion isreadme
Source: ..\readme.txt; DestDir: {app}; Flags: ignoreversion
Source: ..\license.txt; DestDir: {app}; Flags: ignoreversion

;Used only for custom messages
Source: Language\Custom-*; DestDir: {tmp}; Flags: dontcopy

[INI]
Filename: {app}\eLePhantClient.url; Section: InternetShortcut; Key: URL; String: http://www.lphant.com
Filename: {app}\dotnet.url; Section: InternetShortcut; Key: URL; String: http://www.microsoft.com/downloads/details.aspx?FamilyID=262d25e3-f589-4842-8157-034d1e7cf3a3&DisplayLang=en

[Icons]
Name: {group}\lphant; Filename: {app}\eLePhantClient.exe
Name: {group}\Readme; Filename: {app}\readme.txt
; NOTE: The following entry contains an English phrase ("on the Web"). You are free to translate it into another language if required.
Name: {group}\lphant on the Web; Filename: {app}\eLePhantClient.url
Name: {group}\Install .NET 1.1 Framework; Filename: {app}\dotnet.url
; NOTE: The following entry contains an English phrase ("Uninstall"). You are free to translate it into another language if required.
Name: {group}\Uninstall lphant; Filename: {uninstallexe}
Name: {userdesktop}\lphant; Filename: {app}\eLePhantClient.exe; Tasks: desktopicon

[Run]
; NOTE: The following entry contains an English phrase ("Launch"). You are free to translate it into another language if required.
Filename: {app}\eLePhantClient.exe; Description: {code:GetSectionMessages|Launchlphant}; Flags: nowait postinstall skipifsilent unchecked

[UninstallDelete]
Type: files; Name: {app}\eLePhantClient.url
Type: files; Name: {app}\dotnet.url

[Tasks]
; NOTE: The following entry contains English phrases ("Create a desktop icon" and "Additional icons"). You are free to translate them into another language if required.
Name: desktopicon; Description: {code:GetSectionMessages|CreateDesktopIcon}; GroupDescription: Additional icons:; Flags: unchecked

[Languages]
Name: en_US; MessagesFile: compiler:Default.isl
Name: es_ES; MessagesFile: Language\Spanish.isl
Name: fr_FR; MessagesFile: Language\French.isl
Name: de_DE; MessagesFile: Language\German.isl

[_ISTool]
UseAbsolutePaths=false

[Code]
var
	CustomMessagesFile : String;
	DefaultMessagesFile : String;

function GetValue(Section, Msg : String) : String;
var
	Value: String;
begin
	Value:=GetIniString(Section,Msg,'????',CustomMessagesFile);
	if Value='????' then
		Value:=GetIniString(Section,Msg,'????',DefaultMessagesFile);
	Result:=Value;
end;

function GetSectionMessages(Msg : String) : String;
begin
	Result:=GetValue('Messages', Msg);
end;

function BackupFile(FileName:String): Boolean;
begin
	if FileExists(FileName) then
		FileCopy(FileName,FileName+'.backup',False);
	Result:=True;
end;


function CheckdotNet(): Boolean;
var
  Dummy: Integer;
  LanguageCode: String;
begin
	LanguageCode := Copy(ActiveLanguage(),0,2);
	if not RegValueExists(HKLM,'SOFTWARE\Microsoft\.NETFramework\policy\v1.1','4322') then begin
		MsgBox(GetValue('Messages','WarningDotNet1')+#13#13+GetValue('Messages','WarningDotNet2'), mbError, MB_OK);
		InstShellExec('http://www.microsoft.com/downloads/details.aspx?FamilyID=262d25e3-f589-4842-8157-034d1e7cf3a3&DisplayLang='+LanguageCode, '', '', SW_SHOWNORMAL, Dummy);
		Result:=False;
	end else
		Result:=True;
end;

procedure URLLabelOnClick(Sender: TObject);
var
  Dummy: Integer;
begin
  InstShellExec('http://www.lphant.com', '', '', SW_SHOWNORMAL, Dummy);
end;

procedure InitializeWizard();
var
  URLLabel: TNewStaticText;
begin
  URLLabel := TNewStaticText.Create(WizardForm);
  URLLabel.Top := 333;
  URLLabel.Left := 20;
  URLLabel.Caption := 'www.lphant.com';
  URLLabel.Font.Style := URLLabel.Font.Style + [fsUnderLine];
  URLLabel.Font.Color := clBlue;
  URLLabel.Cursor := crHand;
  URLLabel.OnClick := @URLLabelOnClick;
  URLLabel.Parent := WizardForm;
end;

function InitializeSetup() : Boolean;
var
	DefaultFile:String;
begin
	DefaultFile:='Custom-en_US.txt';
	CustomMessagesFile:='Custom-'+ActiveLanguage()+'.txt';

	ExtractTemporaryFile(CustomMessagesFile);
	ExtractTemporaryFile(DefaultFile);

	CustomMessagesFile:=ExpandConstant('{tmp}\'+CustomMessagesFile);
	DefaultMessagesFile:=ExpandConstant('{tmp}\'+DefaultFile);

	Result:=True;
end;

function NextButtonClick(CurPage: Integer): Boolean;
// return False to surpress the click on the Next button
// return False to abort Setup
begin
	if CurPage=wpWelcome then
		CheckdotNet();
	Result:=true;
end;
