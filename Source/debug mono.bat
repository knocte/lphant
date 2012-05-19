@ECHO OFF
Tools\NAnt\bin\nant -t:mono-1.0 debug
IF %ERRORLEVEL% NEQ 0 PAUSE
