echo This will delete all goldens output files. Are you sure you wish to proceed?

pause

cd ../../FinModelUtility

for /D %%p in (*) do ( 
  cd "%%p"
  
  if exist goldens\ (
    set hierarchyListCmd="dir /b /s /ad *.* | sort"
    for /f "tokens=*" %%d in ('%hierarchyListCmd%') do (
      if exist input\ (
      	if exist output\ (
          cd output
		  
		  del *.*
  	    )
      )
	)
  )
)