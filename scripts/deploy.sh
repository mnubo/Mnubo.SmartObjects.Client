#!/bin/bash -ux

dotnet pack AspenTech.SmartObjects.Client/AspenTech.SmartObjects.Client.csproj -p:NuspecFile=../AspenTech.SmartObjects.Client.nuspec -c Release
dotnet nuget push AspenTech.SmartObjects.Client/bin/Release/*.nupkg --api-key $1 --source https://api.nuget.org/v3/index.json --skip-duplicate