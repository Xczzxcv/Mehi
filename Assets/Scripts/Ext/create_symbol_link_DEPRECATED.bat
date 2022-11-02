cd /d %~dp0
set /p project_dir="Project directory name: "
mklink /d "../%project_dir%/Assets/Scripts/Ext" "../Extensions"
pause