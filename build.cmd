set START_DIR=%cd%

dotnet restore .\Hangfire.Lamar.sln
dotnet build .\src\Hangfire.Lamar\Hangfire.Lamar.csproj --configuration Release

rem dotnet-xunit is a CLI tool that can only be executed from in the test folder
cd .\test\Hangfire.Lamar.Test

dotnet test --framework netcoreapp2.0
dotnet test --framework net461

cd %START_DIR%

dotnet pack .\src\Hangfire.Lamar -c Release
