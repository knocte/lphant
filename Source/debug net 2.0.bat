@ECHO OFF
Tools\NAnt\bin\nant -t:net-2.0 debug
IF %ERRORLEVEL% NEQ 0 PAUSE
