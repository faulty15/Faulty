@ECHO off
TITLE Downloading FaultyBot, please wait
::Setting convenient to read variables which don't delete the windows temp folder
SET root=%~dp0
CD /D %root%
SET rootdir=%cd%
SET build1=%root%FaultyInstall_Temp\FaultyBot\discord.net\src\Discord.Net\
SET build2=%root%FaultyInstall_Temp\FaultyBot\discord.net\src\Discord.Net.Commands\
SET build3=%root%FaultyInstall_Temp\FaultyBot\src\FaultyBot\
SET installtemp=%root%FaultyInstall_Temp\
::Deleting traces of last setup for the sake of clean folders, if by some miracle it still exists
IF EXIST %installtemp% ( RMDIR %installtemp% /S /Q >nul 2>&1)
::Checks that both git and dotnet are installed
dotnet --version >nul 2>&1 || GOTO :dotnet
git --version >nul 2>&1 || GOTO :git
::Creates the install directory to work in and get the current directory because spaces ruins everything otherwise
:start
MKDIR FaultyInstall_Temp
CD /D %installtemp%
::Downloads the latest version of Faulty
ECHO Downloading Faulty...
ECHO.
git clone -b master --recursive --depth 1 --progress https://github.com/Kwoth/FaultyBot.git >nul
IF %ERRORLEVEL% EQU 128 (GOTO :giterror)
TITLE Installing FaultyBot, please wait
ECHO.
ECHO Installing...
::Building Faulty
CD /D %build1%
dotnet restore >nul 2>&1
CD /D %build2%
dotnet restore >nul 2>&1
CD /D %build3%
dotnet restore >nul 2>&1
dotnet build --configuration Release >nul 2>&1
::Attempts to backup old files if they currently exist in the same folder as the batch file
IF EXIST "%root%FaultyBot\" (GOTO :backupinstall)
:freshinstall
	::Moves the FaultyBot folder to keep things tidy
	ROBOCOPY "%root%FaultyInstall_Temp" "%rootdir%" /E /MOVE >nul 2>&1
	IF %ERRORLEVEL% GEQ 8 (GOTO :copyerror)
	GOTO :end
:backupinstall
	TITLE Backing up old files
	ECHO.
	ECHO Make sure to close any files such as FaultyBot.db before PRESSing ANY KEY TO CONTINUE to prevent data loss
	PAUSE >nul 2>&1
	::Recursively copies all files and folders from FaultyBot to FaultyBot_Old
	ROBOCOPY "%root%FaultyBot" "%root%FaultyBot_Old" /MIR >nul 2>&1
	IF %ERRORLEVEL% GEQ 8 (GOTO :copyerror)
	ECHO.
	ECHO Old files backed up to FaultyBot_Old
	::Copies the credentials and database from the backed up data to the new folder
	COPY "%root%FaultyBot_Old\src\FaultyBot\credentials.json" "%installtemp%FaultyBot\src\FaultyBot\credentials.json" >nul 2>&1
	IF %ERRORLEVEL% GEQ 8 (GOTO :copyerror)
	ECHO.
	ECHO credentials.json copied to new folder
	ROBOCOPY "%root%FaultyBot_Old\src\FaultyBot\bin" "%installtemp%FaultyBot\src\FaultyBot\bin" /E >nul 2>&1
	IF %ERRORLEVEL% GEQ 8 (GOTO :copyerror)
	ECHO.
	ECHO Old bin folder copied to new folder
	ROBOCOPY "%root%FaultyBot_Old\src\FaultyBot\data" "%installtemp%FaultyBot\src\FaultyBot\data" /E >nul 2>&1
	IF %ERRORLEVEL% GEQ 8 (GOTO :copyerror)
	ECHO.
	ECHO Old data folder copied to new folder
	::Moves the setup Faulty folder
	RMDIR "%root%FaultyBot\" /S /Q >nul 2>&1
	ROBOCOPY "%root%FaultyInstall_Temp" "%rootdir%" /E /MOVE >nul 2>&1
	IF %ERRORLEVEL% GEQ 8 (GOTO :copyerror)
	GOTO :end
:dotnet
	::Terminates the batch script if it can't run dotnet --version
	TITLE Error!
	ECHO dotnet not found, make sure it's been installed as per the guides instructions!
	ECHO Press any key to exit.
	PAUSE >nul 2>&1
	CD /D "%root%"
	GOTO :EOF
:git
	::Terminates the batch script if it can't run git --version
	TITLE Error!
	ECHO git not found, make sure it's been installed as per the guides instructions!
	ECHO Press any key to exit.
	PAUSE >nul 2>&1
	CD /D "%root%"
	GOTO :EOF
:giterror
	ECHO.
	ECHO Git clone failed, trying again
	RMDIR %installtemp% /S /Q >nul 2>&1
	GOTO :start
:copyerror
	::If at any point a copy error is encountered 
	TITLE Error!
	ECHO.
	ECHO An error in copying data has been encountered, returning an exit code of %ERRORLEVEL%
	ECHO.
	ECHO Make sure to close any files, such as `FaultyBot.db` before continuing or try running the installer as an Administrator
	PAUSE >nul 2>&1
	CD /D "%root%"
	GOTO :EOF
:end
	::Normal execution of end of script
	TITLE Installation complete!
	CD /D "%root%"
	RMDIR /S /Q "%installtemp%" >nul 2>&1
	ECHO.
	ECHO Installation complete, press any key to close this window!
	PAUSE >nul 2>&1
	del Stable.bat
