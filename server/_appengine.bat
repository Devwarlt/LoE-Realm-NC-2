FOR /F "tokens=4 delims= " %%P IN ('netstat -a -n -o ^| findstr :5555') DO @ECHO TaskKill.exe /PID %%P
taskkill /f /im AppEngine.exe
FOR /F "tokens=4 delims= " %%P IN ('netstat -a -n -o ^| findstr :5555') DO @ECHO TaskKill.exe /PID %%P
taskkill /f /im AppEngine.exe
cd bin-server
start AppEngine.exe
exit