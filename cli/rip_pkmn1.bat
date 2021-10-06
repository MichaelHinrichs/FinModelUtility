@echo off 
setlocal EnableDelayedExpansion

set outBasePath=%~dp0%out\pkmn1\
set mod2fbxBasePath=%~dp0%mod2fbx\
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


:: Navigates to hierarchy directory
pushd "%hierarchyPath%"

:: Iterates through all directories in the hierarchy
echo Checking for previously extracted archives.
set hierarchyListCmd="dir /b /s /ad *.* | sort"
for /f "tokens=*" %%d in ('%hierarchyListCmd%') do (
  cd "%%d"
  
  set localDir=%%d
  call set localDir=%%localDir:!fullHierarchyPath!=%%

  :: Gets model/animations
  set modelName=
  
  :: Gets models in current directory
  for %%b in (".\*.mod") do (
    set modelName=!localDir!
  )

  :: Merges models + animations w/ automatic inputs
  if defined modelName (
    set modelBasePath=%outBasePath%!localDir!

    >nul 2>nul dir /a-d "!modelBasePath!\*" && (set isModelProcessed=1) || (set isModelProcessed=)
    if not defined isModelProcessed (
      echo Processing !modelName! w/ automatic inputs...
      @echo on
      "%mod2fbxBasePath%mod2fbx.exe" automatic --out "!modelBasePath!" --verbose
      @echo off
    )
    if defined isModelProcessed (
      echo Model already processed for !modelName!
    )
  )
)

:: Backs out from hierarchy
popd



echo Done!

pause