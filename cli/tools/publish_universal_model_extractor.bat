@echo off 
setlocal EnableDelayedExpansion

echo This will rebuild Universal Model Extractor via Visual Studio. Are you sure you wish to proceed?

pause

set universalModelExtractorBasePath=%~dp0%universal_model_extractor\

echo Deleting old universal model extractor...
del /q "!universalModelExtractorBasePath!*"

echo Building new universal model extractor...
cd ../../
cd "FinModelUtility\UniversalModelExtractor\"

dotnet publish -c Release

echo Copying new universal model extractor...
move "bin\Release\net5.0\win-x86\publish\*" "!universalModelExtractorBasePath!"

pause