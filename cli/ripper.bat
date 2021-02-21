@echo off 
setlocal EnableDelayedExpansion

set outBasePath=%~dp0%out\
set bmd2gltfBasePath=%~dp0%bmd2gltf\
set szstoolsBasePath=%~dp0%szstools\

:: TODO: Take this as an arg.
set romPath=roms\pkmn2.gcm
set hierarchyPath=%romPath%_dir\


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

  :: Extracts archives in the current directory
  for %%i in (*.szs) do (
    if not exist "%%i_dir" if not exist "%%i 0.rarc" (
      echo Extracting contents from %%d\%%i...
      "%szstoolsBasePath%yaz0dec.exe" "%%i"
      echo OK!
      echo.
    )
  )
  for %%i in (*.rarc) do (
    if not exist "%%i_dir" (
      echo Extracting contents from %%d\%%i...
      "%szstoolsBasePath%rarcdump.exe" "%%i"
      echo OK!
      echo.
    )
  )

  :: Gets model/animations
  set modelName=
  set modelFile=
  set animFiles=

  :: Gets from separate model/animation szs (e.g. Enemies)
  for /d %%m in (".\model.szs*.rarc_dir") do (
    pushd "%%m"
    for %%b in (".\model\*.bmd") do (
      set modelName=%%~nd
      set modelFile="%%d%%m%%b"
    )
    popd
  )
  for /d %%a in (".\anim.szs*.rarc_dir") do (
    pushd "%%a"
    for %%b in (".\anim\*.bc*") do (
      set animFile=%%d%%a%%b
      set animFiles=!animFiles! "!animFile!"
    )
    popd
  )

  :: Gets from model/animations in same szs (e.g. user\Kando)
  for /d %%r in (".\arc.szs*.rarc_dir") do (
    pushd "%%r"
    for %%b in (".\arc\*.bmd") do (
      set modelName=%%~nd
      set modelFile="%%d%%r%%b"
    )
    for %%b in (".\arc\*.bc*") do (
      set animFile=%%d%%r%%b
      set animFiles=!animFiles! "!animFile!"
    )
    popd
  )

  :: Merges models w/ animations
  if defined modelFile if defined animFiles (
    set modelBasePath=%outBasePath%!modelName!
    set outputPath="!modelBasePath!\!modelName!.glb"

    if not exist "!modelBasePath!" mkdir "!modelBasePath!"

    echo Processing !modelName!...
    @echo on
    "%bmd2gltfBasePath%bmd2gltf.exe" !outputPath! !modelFile! !animFiles!
    @echo off
  )
)

:: Backs out from hierarchy
popd


:: TODO: Move merged files to out\


echo Done!

pause