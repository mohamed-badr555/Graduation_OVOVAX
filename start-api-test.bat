@echo off
echo ============================================
echo 🚀 OVOVAX API Testing Script
echo ============================================
echo.

echo 📁 Navigating to API directory...
cd /d "d:\4T2\Graduation\OVOVAX  Solution\OVOVAX.API"

echo.
echo 🔨 Building the project...
dotnet build --verbosity minimal

if %ERRORLEVEL% neq 0 (
    echo ❌ Build failed! Please check for errors.
    pause
    exit /b 1
)

echo.
echo ✅ Build successful!
echo.
echo 🌐 Starting API server...
echo 📍 API will be available at: http://localhost:5243
echo 📍 Swagger UI: http://localhost:5243/swagger
echo.
echo ⚠️  Once the server starts:
echo    1. Open browser to: http://localhost:5243/swagger
echo    2. Look for the 🔒 Authorize button
echo    3. Follow the STEP_BY_STEP_TESTING.md guide
echo.
echo 🔥 Starting server now...
echo.

dotnet run --urls "http://localhost:5243"
