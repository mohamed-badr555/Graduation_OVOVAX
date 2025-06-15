@echo off
echo =====================================================
echo QUICK SAVE - OVOVAX PROJECT
echo =====================================================

echo Checking Git status...
git status

echo.
echo Adding all changes...
git add .

echo.
echo Committing changes...
set /p message="Enter commit message (or press Enter for default): "
if "%message%"=="" set message=Quick save - work in progress

git commit -m "%message%"

echo.
echo Changes saved successfully!
echo =====================================================
pause
