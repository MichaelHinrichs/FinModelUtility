@echo off 
setlocal EnableDelayedExpansion

echo This will rebuild Universal Asset Tool via Visual Studio. Are you sure you wish to proceed?

pause

set universalAssetToolBasePath=%~dp0%universal_asset_tool\

echo Deleting old universal asset tool...
del /q "!universalAssetToolBasePath!*"

echo Building new universal asset tool...
cd ../../
cd "FinModelUtility\UniversalAssetTool\UniversalAssetTool.Ui\"

dotnet publish -c Release

echo Copying new universal asset tool...
move "bin\Release\net8.0-windows\win-x86\publish\*" "!universalAssetToolBasePath!"

pause