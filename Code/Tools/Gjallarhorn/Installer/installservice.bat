@echo off

rem Check whether the service is installed or not. Error level 1060 means that it's not.
sc qc QlikProactiveExpress
if ERRORLEVEL 1060 goto install
rem this updates old installations to fix recovery options.
sc failure QlikProactiveExpress reset= 0 actions= restart/60000
rem The service is already installed, so nothing more to do here.
echo QlikProactiveExpress is already installed
goto exit

rem Install the service using the provided path
:install
echo Installing QlikProactiveExpress
sc create QlikProactiveExpress start= auto DisplayName= "Qlik Proactive Express service" binPath= %1
sc description QlikProactiveExpress "Real-time active monitoring of Qlik products"
sc failure QlikProactiveExpress reset= 0 actions= restart/60000
:exit