@ECHO off
@TITLE FaultyBot
:auto
CD /D %~dp0FaultyBot\src\FaultyBot
dotnet run --configuration Release
goto auto
