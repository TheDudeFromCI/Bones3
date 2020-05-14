#!/bin/bash

set -e

echo "Building project."
dotnet build src/Bones3.sln

echo "Running tests."
dotnet test src/Bones3.sln

echo "Cleaning Unity build directory."
rm -f Assets/Wraithaven\ Games/Bones3\ Rebuilt/Scripts/Bones3.dll

echo "Copying build data to Unity."
cp -a src/Bones3/bin/Debug/netstandard2.0/Bones3.dll Assets/Wraithaven\ Games/Bones3\ Rebuilt/Scripts

echo "Done."
