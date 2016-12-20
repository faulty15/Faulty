@ECHO off
@TITLE FaultyBot
CD /D %~dp0FaultyBot\src\FaultyBot
dotnet run --configuration Release
ECHO FaultyBot has been succesfully stopped, press any key to close this window.
TITLE FaultyBot - Stopped
CD /D %~dp0
PAUSE >nul 2>&1
del FaultyRunNormal.bat
