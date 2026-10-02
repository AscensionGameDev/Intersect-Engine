@echo off
title Aetherion - Envoyer les ressources
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0ResSync.ps1" Push
echo.
pause
