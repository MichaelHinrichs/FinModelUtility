@echo off 
setlocal EnableDelayedExpansion

set outBasePath=%~dp0%out\
set bmd2gltfBasePath=%~dp0%bmd2gltf\
set szstoolsBasePath=%~dp0%szstools\

:: TODO: Take this as an arg.
set romPath=roms\pkmn1.gcm
set hierarchyPath=%romPath%_dir\
set fullHierarchyPath=%~dp0%!hierarchyPath!

:: Extracts file hierarchy from the given ROM
echo Checking for previously extracted file hierarchy.
if not exist "%hierarchyPath%" (
  echo|set /p="Extracting file hierarchy... "
  "%szstoolsBasePath%gcmdump.exe" "%romPath%"
  echo OK!
  echo.
)

pause