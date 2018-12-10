#!/bin/bash
set -ev

dotnet restore ./Hangfire.Lamar.sln
dotnet build ./src/Hangfire.Lamar/Hangfire.Lamar.csproj --configuration Release

dotnet test --framework netcoreapp2.0
