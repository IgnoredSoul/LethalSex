@echo off

:: Set main folder path
set "MF=C:\Users\Jakei\OneDrive\Desktop\Coding\C#\Lethal Company\LethalSexV2 - v50"

:: Set project folder path
set "PF=%MF%\LethalSanity"

:: Set TS folder path
set "TF=%MF%\Lethal Sanity - TS"

:: Delete if exists
if exist "%TF%\LethalSanity.zip" ( del "%TF%\LethalSanity.zip" 2>nul )

:: .zip TS folder contents and save in main dir
tar -cf "%MF%\LethalSanity.zip" -C "%TF%" *

:: Move the .zip to TS folder
move "%MF%\LethalSanity.zip" "%TF%"

:: Copy README.md from project folder to TS folder
xcopy "%PF%\README.md" "%TF%" /Y

:: Copy manifest.json from project folder to TS folder
xcopy "%PF%\manifest.json" "%TF%" /Y