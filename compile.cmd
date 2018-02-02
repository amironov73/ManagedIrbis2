@echo off

cd Source

SET OUTPUT=../../../Binaries
SET PARAMS=/consoleloggerparameters:ErrorsOnly /m

dotnet restore

dotnet msbuild /target:ReBuild /property:Configuration=Debug   /property:OutputPath=%OUTPUT%\DebugCore   %PARAMS%
dotnet msbuild /target:ReBuild /property:Configuration=Release /property:OutputPath=%OUTPUT%\ReleaseCore %PARAMS%

cd ..