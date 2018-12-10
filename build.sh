#!/bin/bash
set -ev

dotnet restore ./Hangfire.Lamar.sln
dotnet build ./src/Hangfire.Lamar/Hangfire.Lamar.csproj --configuration Release

# dotnet-xunit is a CLI tool that can only be executed from in the test folder
cd ./test/Hangfire.Lamar.Test

dotnet test --framework netcoreapp2.0

cd ${TRAVIS_BUILD_DIR}
