@echo off

:: Set main folder path
set "MF=C:\Users\Jakei\OneDrive\Desktop\Coding\C#\Lethal Company\LethalSexV2 - v50"

:: Set project folder path
set "PF=%MF%\LethalSex"

:: Set TS folder path
set "TF=%MF%\Lethal Sex Core - TS"

:: Delete if exists
if exist "%TF%\LethalSex.zip" ( del "%TF%\LethalSex.zip" 2>nul )

:: .zip TS folder contents and save in main dir
tar -cf "%MF%\LethalSex.zip" -C "%TF%" *

:: Move the .zip to TS folder
move "%MF%\LethalSex.zip" "%TF%"

:: Copy README.md from project folder to TS folder
xcopy "%PF%\README.md" "%TF%" /Y

:: Copy manifest.json from project folder to TS folder
xcopy "%PF%\manifest.json" "%TF%" /Y