set LUBAN_DLL=Tools/Luban/Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    -d bin ^
    --conf %CONF_ROOT%/luban.conf ^
    -x outputCodeDir=../Assets/Scripts/Config ^
    -x outputDataDir=../Assets/AddressableAssets/Config

pause