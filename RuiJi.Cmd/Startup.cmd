echo on
start call RuiJi.Cmd.exe start -u localhost:36000 -t cp -z localhost:2181
@echo off
choice /t 1 /d y /n >nul

echo on
start call RuiJi.Cmd.exe start -u localhost:37000 -t ep -z localhost:2181
@echo off
choice /t 1 /d y /n >nul

echo on
start call  RuiJi.Cmd.exe start -u localhost:36001 -t c -p localhost:36000  -z localhost:2181
@echo off
choice /t 1 /d y /n >nul

echo on
start call  RuiJi.Cmd.exe start -u localhost:36002 -t c -p localhost:36000 -z localhost:2181
@echo off
choice /t 1 /d y /n >nul

echo on
start call  RuiJi.Cmd.exe start -u localhost:37001 -t e -p localhost:37000 -z localhost:2181
@echo off
choice /t 1 /d y /n >nul

echo on
start call  RuiJi.Cmd.exe start -u localhost:37002 -t e -p localhost:37000 -z localhost:2181
@echo off
choice /t 1 /d y /n >nul
echo on