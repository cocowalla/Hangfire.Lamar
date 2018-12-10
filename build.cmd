set START_DIR=%cd%

dotnet restore .\Hangfire.Lamar.sln
dotnet build .\src\Hangfire.Lamar\Hangfire.Lamar.csproj --configuration Release

dotnet test --framework netcoreapp2.0
dotnet test --framework net461

dotnet pack .\src\Hangfire.Lamar -c Release
