[Setup]
AppName=Monitoring
AppVersion=1.0.0.3
DefaultDirName={pf}\Monitoring
DefaultGroupName=Monitoring
UninstallDisplayIcon={app}\Monitoring.exe
OutputDir=.
OutputBaseFilename=Monitoring_Setup

[Files]
Source: "D:\Projects\LogScreen\LogScreen\bin\Release\Monitoring.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Projects\LogScreen\LogScreen\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Projects\LogScreen\LogScreen\bin\Release\Monitoring.exe.config"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Monitoring"; Filename: "{app}\Monitoring.exe"

[Run]
Filename: "{app}\Monitoring.exe"; Description: "Launch MyApp"; Flags: nowait postinstall skipifsilent runascurrentuser

[UninstallRun]
Filename: "taskkill"; Parameters: "/F /IM Monitoring.exe"; Flags: runhidden

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
// Xóa giá trị khỏi Registry
procedure RemoveAppFromRegistry(AppName: string);
var
  Key: string;
begin
  Key := 'Software\Microsoft\Windows\CurrentVersion\Run';

  // Kiểm tra và xóa giá trị nếu tồn tại
  if RegValueExists(HKEY_CURRENT_USER, Key, AppName) then
  begin
    RegDeleteValue(HKEY_CURRENT_USER, Key, AppName);
    MsgBox(AppName + ' has been removed from the startup list in the registry.', mbInformation, MB_OK);
  end
  else
  begin
    MsgBox(AppName + ' was not found in the startup list.', mbError, MB_OK);
  end;
end;

// Gọi hàm khi gỡ cài đặt
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    // Tên ứng dụng cần xóa
    RemoveAppFromRegistry('Monitoring');
  end;
end;