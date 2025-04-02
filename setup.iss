[Setup]
AppName=LogScreen
AppVersion=1.0
DefaultDirName={pf}\LogScreen
DefaultGroupName=LogScreen
UninstallDisplayIcon={app}\LogScreen.exe
OutputDir=.
OutputBaseFilename=LogScreen_Setup

[Files]
Source: "D:\Repositories\Log-Screen\LogScreen\bin\Release\LogScreen.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Repositories\Log-Screen\LogScreen\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Repositories\Log-Screen\LogScreen\bin\Release\LogScreen.exe.config"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\LogScreen"; Filename: "{app}\LogScreen.exe"

[Run]
Filename: "{app}\LogScreen.exe"; Description: "Launch MyApp"; Flags: nowait postinstall skipifsilent runascurrentuser

[UninstallRun]
Filename: "taskkill"; Parameters: "/F /IM LogScreen.exe"; Flags: runhidden

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
    RemoveAppFromRegistry('LogScreen');
  end;
end;