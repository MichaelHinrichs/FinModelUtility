@echo off 

set szstoolsPath=%~dp0%szstools\

:: TODO: Take this as an arg.
set romPath=roms\pkmn2.gcm
set hierarchyPath=%romPath%_dir\

:: Extracts file hierarchy from the given ROM
echo Checking for previously extracted file hierarchy.
if not exist %hierarchyPath% (
  echo|set /p="Extracting file hierarchy... "
  %szstoolsPath%gcmdump.exe %romPath%
  echo OK!
  echo.
)


:: Navigates to hierarchy directory to extract archives
pushd %hierarchyPath%

:: Iterates through all directories in the hierarchy
echo Checking for previously extracted archives.
set hierarchyListCmd="dir /b /s /ad *.* | sort"
for /f "tokens=*" %%d in ('%hierarchyListCmd%') do (
  cd %%d

  :: Extracts archives in the hierarchy.
  for %%i in (*.szs) do (
    if not exist "%%i_dir" if not exist "%%i 0.rarc"  (
	  echo Extracting contents from %%d\%%i...
      %szstoolsPath%yaz0dec.exe "%%i"
      echo OK!
      echo.
    )
  )
  for %%i in (*.rarc) do (
    if not exist "%%i_dir" (
	  echo Extracting contents from %%d\%%i...
      %szstoolsPath%rarcdump.exe "%%i"
      echo OK!
      echo.
	)
  )
)

:: Backs out from hierarchy
popd


echo Done!

pause