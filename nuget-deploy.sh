ApiKey=$1
Source=$2

dotnet pack ./Nightingale/Nightingale.csproj --include-symbols
dotnet nuget push ./Nightingale/bin/Debug/Nightingale.*.symbols.nupkg --source $Source --api-key $ApiKey

exit 0