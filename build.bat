@echo off
C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild

del /F /Q build
mkdir build
xcopy JSPackager\bin\Debug\JSPackager.dll build
xcopy jspkgCompiler\bin\Debug\jspkgCompiler.exe build
xcopy LICENSE.txt build
xcopy README.txt build
