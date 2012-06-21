@echo off
setLocal EnableDelayedExpansion
IF NOT EXIST ..\.git GOTO DIE

for /f "tokens=* delims= " %%a in (..\.git\logs\HEAD) do (
set var=%%a
)
for /f "tokens=1-7" %%a in ("%var%") do (
echo Virtual Reality Software: 0.1.1.2 Dev: %%b %%f>.version
)

:DIE
pause