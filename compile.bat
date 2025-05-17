@echo off
echo Compiling Abu Sleman Accounting System...
echo.

REM Check if .NET Framework is installed
where csc >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Error: C# compiler (csc) not found.
    echo Please make sure .NET Framework is installed.
    pause
    exit /b 1
)

REM Compile the application
csc /target:winexe /out:AbuSlemanAccounting.exe /reference:System.Windows.Forms.dll,System.Drawing.dll SimpleAccountingApp.cs

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Compilation failed.
    pause
    exit /b 1
) else (
    echo.
    echo Compilation successful!
    echo The application has been compiled to AbuSlemanAccounting.exe
    echo.
    echo Press any key to run the application...
    pause >nul
    start AbuSlemanAccounting.exe
)
