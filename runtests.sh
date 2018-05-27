dotnet --info
echo 'Run tests'
dotnet test RuiJi.Net.Test/RuiJi.Net.Test.csproj -f netcoreapp2.0 -c release
