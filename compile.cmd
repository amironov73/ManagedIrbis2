@echo off

cd Source

SET OUTPUT=../../../Binaries
SET PARAMS=/consoleloggerparameters:ErrorsOnly /m

dotnet restore

dotnet msbuild /target:ReBuild /property:Configuration=Debug   /property:OutputPath=%OUTPUT%\DebugCore   %PARAMS% ManagedIrbis.30.sln
dotnet msbuild /target:ReBuild /property:Configuration=Release /property:OutputPath=%OUTPUT%\ReleaseCore %PARAMS% ManagedIrbis.30.sln

cd ..