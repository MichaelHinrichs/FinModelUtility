@echo off 
setlocal EnableDelayedExpansion

echo This will rebuild Bmd2Gltf via Visual Studio. Are you sure you wish to proceed?

pause

set bmd2gltfBasePath=%~dp0%bmd2gltf\

echo Deleting old bmd2gltf...
del /q "!bmd2gltfBasePath!*"

echo Building new bmd2gltf...
cd ../
cd "MKDS Course Modifier\Bmd2Gltf\"

dotnet publish -c Release

echo Copying new bmd2gltf...
move "bin\Release\net5.0\win-x86\publish\*" "!bmd2gltfBasePath!"

pause