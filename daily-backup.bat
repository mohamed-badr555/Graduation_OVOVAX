@echo off
echo =====================================================
echo DAILY BACKUP - OVOVAX PROJECT  
echo =====================================================

echo Getting current date and time...
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"
set "datestamp=%YYYY%-%MM%-%DD%_%HH%-%Min%-%Sec%"

echo Creating backup folder...
mkdir "D:\4T2\Graduation\Backups\%datestamp%" 2>nul

echo Copying project files...
xcopy "D:\4T2\Graduation\OVOVAX  Solution\*" "D:\4T2\Graduation\Backups\%datestamp%\" /E /I /Y

echo.
echo Committing to Git...
git add .
git commit -m "Daily backup - %datestamp%"

echo.
echo ✅ BACKUP COMPLETE!
echo Location: D:\4T2\Graduation\Backups\%datestamp%
echo =====================================================
pause
