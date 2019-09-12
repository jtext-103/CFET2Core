for /f "delims=" %%a in ('dir /b/a-d/oN *.nupkg') do set n=%%a
nuget push %n% jushen -Source http://211.67.27.239:14999/api/v2/package
pause

