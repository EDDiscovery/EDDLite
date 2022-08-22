if "%1" == "" (
set /P vno=Version Number (10.1.2 etc) :
) else (
echo set to %1
set vno=%1
)

find "%vno%" ..\eddlite\properties\AssemblyInfo.cs
if %ERRORLEVEL%==1 goto :error

echo Build %vno%

"\Program Files (x86)\Inno Setup 6\iscc.exe" /DMyAppVersion=%vno% innoscript.iss
copy ..\EDDLite\bin\Release\EDDLite.Portable.Zip installers\EDDLite.Portable.%vno%.zip
certutil -hashfile installers\EDDLite-%vno%.exe SHA256 >installers\checksums.%vno%.txt
certutil -hashfile installers\EDDLite.Portable.%vno%.zip SHA256 >>installers\checksums.%vno%.txt

exit /b

:error
echo Version %vno% does not correspond to AssemblyInfo.cs

