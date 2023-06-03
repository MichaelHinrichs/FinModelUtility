@echo off 
echo This will delete all goldens output files. Are you sure you wish to proceed?

pause

cd ../../FinModelUtility

set hierarchyListCmd="dir /b /s /ad *.* | sort"

for /d %%p in (*) do ( 
  pushd "./"
  
  if exist "%%p Tests" (
    cd "%%p Tests"
    
    if exist goldens\ (
      for /f "tokens=*" %%d in ('%hierarchyListCmd%') do (
        pushd "./"

        cd "%%d"

        if exist input\ (
          if exist output\ (
            cd output\
            del /q *.*
          )
        )

        popd
      )
    )
  )
  
  popd
  pushd "./"
  
  if not exist "%%p Tests" (
    cd "%%p"

    if exist "%%p Tests" (
      cd "%%p Tests"
    
      if exist goldens\ (
        for /f "tokens=*" %%d in ('%hierarchyListCmd%') do (
          pushd "./"

          cd "%%d"

          if exist input\ (
            if exist output\ (
              cd output\
              del /q *.*
            )
          )

          popd
        )
      )
    )
  )

  popd
)

pause