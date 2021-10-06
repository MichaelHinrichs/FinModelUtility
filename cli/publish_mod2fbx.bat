@echo off 
setlocal EnableDelayedExpansion

echo This will rebuild Mod2Fbx via Visual Studio. Are you sure you wish to proceed?

pause

set mod2fbxBasePath=%~dp0%mod2fbx\

echo Deleting old mod2fbx...
del /q "!mod2fbxBasePath!*"

echo Building new mod2fbx...
cd ../
cd "FinModelUtility\Mod2Fbx\"

dotnet publish -c Release

echo Copying new mod2fbx...
move "bin\Release\net5.0\win-x86\publish\*" "!mod2fbxBasePath!"

pause