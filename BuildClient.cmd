@ECHO OFF
SETLOCAL

ECHO Build react widget
CALL yarn --cwd clientResources build
REM IF %errorlevel% NEQ 0 EXIT /B %errorlevel%


ECHO Copy modified widgets to protected modules
Xcopy ".\clientResources\build\static" ".\src\AlloySample\modules\_protected\content-children-grouping\1.0.0\static" /E /H /C /I /Y

EXIT /B %ERRORLEVEL%
