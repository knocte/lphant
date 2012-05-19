@ECHO OFF
Tools\NAnt\bin\nant -t:net-1.1
IF %ERRORLEVEL% NEQ 0 PAUSE
