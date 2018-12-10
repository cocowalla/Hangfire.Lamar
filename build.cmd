set START_DIR=%cd%

dotnet restore .\Hangfire.Lamar.sln
dotnet build .\src\Hangfire.Lamar\Hangfire.Lamar.csproj --configuration Release

rem dotnet-xunit is a CLI tool that can only be executed from in the test folder
cd .\test\Hangfire.Lamar.Test

dotnet xunit -framework netcoreapp1.1 -fxversion 1.1.2
dotnet xunit -framework netcoreapp2.0 -fxversion 2.0.6
dotnet xunit -framework net452

cd %START_DIR%

dotnet pack .\src\Hangfire.Lamar -c Release
