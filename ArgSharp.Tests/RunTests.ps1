Remove-Item .\TestResults -Recurse -ErrorAction Ignore
Write-Host "Running tests..."

dotnet test --collect:"Code Coverage" --settings:settings.runsettings

$TestResultFolder=(Get-ChildItem .\TestResults)[0].Name
$TestResultFile=(Get-ChildItem .\TestResults\$TestResultFolder)[0].Name

CodeCoverage analyze /output:TestResults\results.xml TestResults\$TestResultFolder\$TestResultFile

dotnet reportgenerator "-reports:TestResults\results.xml" "-targetdir:TestResults\target"

Invoke-Item TestResults\target\index.html