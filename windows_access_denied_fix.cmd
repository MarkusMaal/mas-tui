@REM This allows you to add an exception to Windows firewall so that MasAPI can be run and accessed on your local network
@REM Inspired by this article: https://www.programmersought.com/article/91211981662/
@echo off
setlocal
set "quit=false"
net session >nul 2>&1
if %errorlevel% NEQ 0 (
	echo This script requires elevated privileges.
	pause
	goto exitnow
)
REM Uncomment the line below if you're having issues:
REM netsh http delete urlacl url=http://+:14415/
netsh http add urlacl url=http://+:14415/ user=everyone
netsh advfirewall add rule name = \"command-line Web Access 14415 \" dir = in protocol = tcp localport = 14415 action = allow
:exitnow
endlocal
exit/b