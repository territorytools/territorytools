Set-Location $PSScriptRoot

Import-Module TerritoryTools

$inputFile = ".\originals-to-update.txt"
$languageFilePath = ".\languages.html"

$connection = Get-AlbaConnection `
    -AlbaHost $env:ALBA_HOST `
    -Account $env:ALBA_ACCOUNT

$addresses = Get-Content -Path $inputFile | ConvertFrom-Csv -Delimiter `t

# You can test an import on just a couple addresses at a time
# $addresses[0..1] | Edit-AlbaAAddress `
#    -LanguageFilePath $languageFilePath `
#    -Connection $connection 

$addresses | Edit-AlbaAddress `
    -LanguageFilePath $languageFilePath `
    -Connection $connection 
