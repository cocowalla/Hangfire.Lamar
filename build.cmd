dotnet restore .\Hangfire.Lamar.sln
dotnet build .\src\Hangfire.Lamar\Hangfire.Lamar.csproj --configuration Release

dotnet test --framework netcoreapp2.0 .\test\Hangfire.Lamar.Test\Hangfire.Lamar.Test.csproj
dotnet test --framework net461  .\test\Hangfire.Lamar.Test\Hangfire.Lamar.Test.csproj

dotnet pack .\src\Hangfire.Lamar -c Release
