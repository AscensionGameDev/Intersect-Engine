@echo off
title Aetherion - Recuperer les ressources
echo Recuperation des ressources depuis le serveur (environ 2 minutes)...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0ResSync.ps1" Pull
echo.
pause
