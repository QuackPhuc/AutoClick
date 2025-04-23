@echo off
echo Building Auto Click Tool...
dotnet build -c Release src\AutoClick.csproj
if %ERRORLEVEL% NEQ 0 (
    echo Build failed. Please make sure you have .NET 6.0 SDK installed.
    echo You can download it from: https://dotnet.microsoft.com/download/dotnet/6.0
    pause
    exit /b %ERRORLEVEL%
)
echo Build successful!
echo Starting Auto Click Tool...
start "" "src\bin\Release\net6.0-windows\AutoClick.exe"
exit /b 0 