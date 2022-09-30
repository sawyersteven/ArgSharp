Remove-Item .\TestResults -Recurse -ErrorAction Ignore
Write-Host "Running tests..."

dotnet test -l "console;verbosity=detailed" --collect:"Code Coverage" --settings:settings.runsettings

$TestResultFolder=(Get-ChildItem .\TestResults)[0].Name
$TestResultFile=(Get-ChildItem .\TestResults\$TestResultFolder)[0].Name

dotnet-coverage merge *.coverage -r -o TestResults\results.xml -f xml 

dotnet reportgenerator "-reports:TestResults\results.xml" "-targetdir:TestResults\target"

Invoke-Item TestResults\target\index.html