dotnet restore --verbosity --verbosity Minimal
dotnet build --no-restore --verbosity Minimal --configuration Release
dotnet test --no-restore --no-build --verbosity Minimal --configuration Release --results-directory .builds/artifacts --logger:trx -p:CollectCoverage=true -p:Exclude="[xunit.*]*" -p:CoverletOutputFormat=cobertura
dotnet pack -p:SymbolPackageFormat=snupkg --no-restore --no-build --include-symbols --verbosity Minimal --configuration Release --output .builds/artifacts