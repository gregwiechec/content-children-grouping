@ECHO OFF
SETLOCAL


REM CALL yarn --cwd clientResources install
REM IF %errorlevel% NEQ 0 EXIT /B %errorlevel%


ECHO Copy modified webpack.config to node_modules
COPY ".\clientResources\customscripts\webpack.config.js" ".\clientResources\node_modules\react-scripts\config\webpack.config.js"

EXIT /B %ERRORLEVEL%
