FOR /F "tokens=4 delims= " %%P IN ('netstat -a -n -o ^| findstr :2050') DO @ECHO TaskKill.exe /PID %%P
taskkill /f /im GameServer.exe
FOR /F "tokens=4 delims= " %%P IN ('netstat -a -n -o ^| findstr :2050') DO @ECHO TaskKill.exe /PID %%P
taskkill /f /im GameServer.exe
cd bin-server
start GameServer.exe
exit