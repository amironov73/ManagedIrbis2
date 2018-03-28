@echo off

dotnet build -c Release --verbosity=minimal Source\Benchmarks
dotnet run   -c Release -p Source\Benchmarks