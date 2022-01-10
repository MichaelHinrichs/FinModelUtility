@echo off 
setlocal EnableDelayedExpansion

echo This will rebuild Bmd2Fbx via Visual Studio. Are you sure you wish to proceed?

pause

set bmd2fbxBasePath=%~dp0%bmd2fbx\

echo Deleting old bmd2fbx...
del /q "!bmd2fbxBasePath!*"

echo Building new bmd2fbx...
cd ../../
cd "FinModelUtility\Bmd2Fbx\"

dotnet publish -c Release

echo Copying new bmd2fbx...
move "bin\Release\net6.0\win-x86\publish\*" "!bmd2fbxBasePath!"

pause