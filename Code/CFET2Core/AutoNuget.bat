nuget pack
for /f "delims=" %%a in ('dir /b/a-d/oN *.nupkg') do set n=%%a
nuget push %n% jushen -Source http://222.20.94.134:14999/api/v2/package
pause
