[Setup]
AppName=LogScreen
AppVersion=1.0
AppId={{550E8400-E29B-41D4-A716-446655440000}}
DefaultDirName={pf}\LogScreen
DefaultGroupName=LogScreen
UninstallDisplayIcon={app}\LogScreen.exe
OutputDir=.
OutputBaseFilename=LogScreen_Setup

[Files]
Source: "D:\Repositories\Log-Screen\LogScreen\bin\Release\LogScreen.exe"; DestDir: "{app}"; Flags: ignoreversion restartreplace
Source: "D:\Repositories\Log-Screen\LogScreen\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion restartreplace
Source: "D:\Repositories\Log-Screen\LogScreen\bin\Release\LogScreen.exe.config"; DestDir: "{app}"; Flags: ignoreversion restartreplace

[Icons]
Name: "{group}\LogScreen"; Filename: "{app}\LogScreen.exe"

[Run]
Filename: "{app}\LogScreen.exe"; Description: "Launch MyApp"; Flags: nowait postinstall skipifsilent runascurrentuser

[UninstallRun]
Filename: "taskkill"; Parameters: "/F /IM LogScreen.exe"; Flags: runhidden

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
procedure RemoveAppFromRegistry(AppName: string);
var
  Key: string;
begin
  Key := 'Software\Microsoft\Windows\CurrentVersion\Run';
  if RegValueExists(HKEY_CURRENT_USER, Key, AppName) then
  begin
    RegDeleteValue(HKEY_CURRENT_USER, Key, AppName);
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    RemoveAppFromRegistry('LogScreen');
  end;
end;

function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
begin
  if Exec('taskkill', '/F /IM LogScreen.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    Sleep(1000);
  end;
  if FindWindowByClassName('LogScreen') <> 0 then
  begin
    MsgBox('Không thể đóng LogScreen. Vui lòng tắt thủ công trước khi cài đặt.', mbError, MB_OK);
    Result := False;
  end
  else
  begin
    Result := True;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssInstall then
  begin
    if RegKeyExists(HKEY_LOCAL_MACHINE, 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{550E8400-E29B-41D4-A716-446655440000}_is1') then
    begin
      Exec('cmd.exe', '/C "taskkill /F /IM LogScreen.exe"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Exec(ExpandConstant('{uninstallexe}'), '/SILENT', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Sleep(2000);
    end;
  end;
end;