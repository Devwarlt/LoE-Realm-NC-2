FOR /F "tokens=4 delims= " %%P IN ('netstat -a -n -o ^| findstr :5556') DO @ECHO TaskKill.exe /PID %%P
taskkill /f /im GameServer.exe
FOR /F "tokens=4 delims= " %%P IN ('netstat -a -n -o ^| findstr :5556') DO @ECHO TaskKill.exe /PID %%P
taskkill /f /im GameServer.exe
cd bin
start GameServer.exe
exit