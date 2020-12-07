Set-Location $PSScriptRoot

Import-Module "$HOME\.nuget\packages\system.threading.tasks.extensions\4.5.2\lib\netstandard1.0\System.Threading.Tasks.Extensions.dll";
Import-Module "$HOME\.nuget\packages\microsoft.bcl.asyncinterfaces\1.1.0\lib\netstandard2.0\Microsoft.Bcl.AsyncInterfaces.dll";
Import-Module "$HOME\.nuget\packages\csvhelper\15.0.1\lib\netstandard2.0\CsvHelper.dll"; 
Import-Module "$HOME\.nuget\packages\HtmlAgilityPack\1.11.8\lib\netstandard2.0\HtmlAgilityPack.dll";
Import-Module "$HOME\.nuget\packages\Newtonsoft.Json\12.0.3\lib\netstandard2.0\Newtonsoft.Json.dll";
Import-Module ".\TerritoryTools.Alba.PowerShell.dll"; 

# Run one time for each of these files
#$inputFile = "2020-12-02-1334.M153.txt"
$masterAddressFile = ".\addresses-2020-12-06-1735.txt"
$dateString = "2020-12-06"
$importedFrom = "Imported From Somewhere"
$languagesHtmlFile = .\Languages.2020-11-22.0101.html

$cities = Get-Content ".\Cities.txt"
"Cities: $($cities.Count)"

# This is a custom format with the following headers: Terr #, Name, Address, Apt #, City, Zip code
# The column header names change frequently
$source = Get-Content ".\$inputFile" `
  | ConvertFrom-Csv -Delimiter `t `
  | .\ConvertFrom-ExampleToAlbaColumns.ps1 `
  | ConvertTo-AlbaAddressImport 

"Addresses in Source: $($source.Count)"

$master = Get-Content $masterAddressFile `
  | ConvertFrom-Csv -Delimiter `t `
  | Select `
        Address_id, `
        Territory_id, `
        Language, `
        Status, `
        Name, `
        Address, `
        Suite, `
        City, `
        Province, `
        Postal_Code, `
        Country, `
        Latitude, `
        Longitude, `
        Telephone, `
        Notes, `
        Notes_private `
    | ConvertTo-AlbaAddressImport

"Addresses in Master: $($master.Count)"

$notDuplicate = $source | Skip-Duplicates -MasterList $master -Cities $cities

"Not Duplicate $($notDuplicate.Count)"

$originals = $source | Get-Original -MasterList $master -Cities $cities 

"Duplicated $($originals.Count)"

$geocoded = $notDuplicate | Set-Coordinates -Key $env:AZURE_MAPS_KEY 

ForEach($n in $geocoded) { 
    $n.Language = "Spanish" 
    $n.Status = "New"
    $n.Notes_private = "IMPORTED-DATE: $dateString; IMPORTED-FROM: $importedFrom; IMPORTED-FROM-FILE: $inputFile" 
} 

$connection = Get-AlbaConnection `
    -AlbaHost $env:ALBA_HOST `
    -Account $env:ALBA_ACCOUNT `
    -User $env:ALBA_USER `
    -Password $env:ALBA_PASSWORD

$geocoded | Add-AlbaAddress -Connection $connection -LanguageFilePath $languagesHtmlFile

$geocoded `
  | ConvertTo-Csv -Delimiter `t -NoTypeInformation `
  | Out-File ".\$($inputFile.Replace('.txt','.imported.txt'))" -Encoding utf8
 