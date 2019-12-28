dotnet --info
dotnet restore --verbosity --verbosity Minimal
dotnet build --version-suffix "prebuild" --no-restore --verbosity Minimal --configuration Release
dotnet test --no-restore --no-build --verbosity Minimal --configuration Release --results-directory .builds/artifacts --logger:trx
dotnet pack --version-suffix "prebuild" --no-restore --no-build --include-symbols --verbosity Minimal --configuration Release --output .builds/artifacts