Set-Location $PSScriptRoot

Import-Module TerritoryTools

$duplicatesFile = ".\duplicates-to-update.txt"
$languageFilePath = ".\languages.html"

$connection = Get-AlbaConnection `
    -AlbaHost $env:ALBA_HOST `
    -Account $env:ALBA_ACCOUNT `
    -User $env:ALBA_USER `
    -Password $env:ALBA_PASSWORD
 
$addresses = Get-Content $inputFile `
    | ConvertFrom-Csv -Delimiter `t

$addresses | Edit-AlbaAddress `
    -LanguageFilePath $languageFilePath `
    -Connection $connection 

$duplicates = Get-Content $duplicatesFile `
    | ConvertFrom-Csv -Delimiter `t

$duplicates | Edit-AlbaAddress `
    -LanguageFilePath $languageFilePath `
    -Connection $connection 
