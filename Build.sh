#!/bin/bash

set -e

echo "Building project."
dotnet build src/Bones3.sln

echo "Running tests."
dotnet test src/Bones3.sln

echo "Cleaning Unity build directory."
rm -rf Assets/Wraithaven\ Games/Bones3\ Rebuilt/Scripts/Core
mkdir -p Assets/Wraithaven\ Games/Bones3\ Rebuilt/Scripts/Core

echo "Copying build data to Unity."
cp -a src/Bones3/. Assets/Wraithaven\ Games/Bones3\ Rebuilt/Scripts/Core
rm -rf Assets/Wraithaven\ Games/Bones3\ Rebuilt/Scripts/Core/bin
rm -rf Assets/Wraithaven\ Games/Bones3\ Rebuilt/Scripts/Core/obj
rm -f Assets/Wraithaven\ Games/Bones3\ Rebuilt/Scripts/Core/Bones3.csproj

echo "Done."
