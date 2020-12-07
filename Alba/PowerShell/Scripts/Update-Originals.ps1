Set-Location $PSScriptRoot

Import-Module "$HOME\.nuget\packages\system.threading.tasks.extensions\4.5.2\lib\netstandard1.0\System.Threading.Tasks.Extensions.dll";
Import-Module "$HOME\.nuget\packages\microsoft.bcl.asyncinterfaces\1.1.0\lib\netstandard2.0\Microsoft.Bcl.AsyncInterfaces.dll";
Import-Module "$HOME\.nuget\packages\HtmlAgilityPack\1.11.8\lib\netstandard2.0\HtmlAgilityPack.dll";
Import-Module "$HOME\.nuget\packages\Newtonsoft.Json\12.0.3\lib\netstandard2.0\Newtonsoft.Json.dll";
Import-Module "$HOME\.nuget\packages\csvhelper\15.0.1\lib\netstandard2.0\CsvHelper.dll"; 
Import-Module ".\TerritoryTools.Alba.PowerShell.dll"; 

$inputFile = ".\originals-to-update.txt"
$languageFilePath = ".\languages.html"

$connection = Get-AlbaConnection `
    -AlbaHost $env:ALBA_HOST `
    -Account $env:ALBA_ACCOUNT `
    -User $env:ALBA_USER `
    -Password $env:ALBA_PASSWORD

$addresses = Get-Content -Path $inputFile | ConvertFrom-Csv -Delimiter `t

# You can test an import on just a couple addresses at a time
# $addresses[0..1] | Edit-AlbaAAddress `
#    -LanguageFilePath $languageFilePath `
#    -Connection $connection 

$addresses | Edit-AlbaAddress `
    -LanguageFilePath $languageFilePath `
    -Connection $connection 
