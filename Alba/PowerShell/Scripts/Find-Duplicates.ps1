Import-Module "$HOME\.nuget\packages\system.threading.tasks.extensions\4.5.2\lib\netstandard1.0\System.Threading.Tasks.Extensions.dll";
Import-Module "$HOME\.nuget\packages\microsoft.bcl.asyncinterfaces\1.1.0\lib\netstandard2.0\Microsoft.Bcl.AsyncInterfaces.dll";
Import-Module "$HOME\.nuget\packages\csvhelper\15.0.1\lib\netstandard2.0\CsvHelper.dll"; 

Set-Location $PSScriptRoot
Import-Module ".\TerritoryTools.Alba.PowerShell.dll"; 

$cities = Get-Content ".\Cities.txt"
"Cities: $($cities.Count)"

$master = Get-Content ".\MasterAddresses.txt" `
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
  
# This is a custom format with the following headers: Terr #, Name, Address, Apt #, City, Zip code
$source = Get-Content ".\cleaned.tsv" `
  | ConvertFrom-Csv -Delimiter `t `
  | Where "Terr #" -match "[*]{2}French[*]{2}.*" `
  | Select `
        Address_id, `
        Territory_id, `
        Language, `
        Status, `
        Name, `
        Address, `
        @{ Name = "Suite"; Expression = { $_."Apt #" } }, `
        City, `
        @{ Name = "Province"; Expression = { "NY" } }, `
        @{ Name = "Postal_Code"; Expression = { $_."Zip code" } }, `
        @{ Name = "Country"; Expression = { "USA" } }, `
        Latitude, `
        Longitude, `
        Telephone, `
        Notes, `
        Notes_private  `
    | ConvertTo-AlbaAddressImport

"Addresses in Source: $($source.Count)"

$source `
    | Skip-Duplicates -Cities $cities -MasterList $master `
    | ConvertTo-Csv -NoTypeInformation -Delimiter `t `
    > new.txt

$new = Get-Content new.txt | ConvertFrom-Csv -Delimiter `t
ForEach($n in $new) { 
    $n.Language = "French" 
    $n.Status = "New"
    $n.Notes_private = "IMPORTED-DATE: 2020-11-22 IMPORTED-FROM: Territory Tools Test" 
} 

$new | ConvertTo-Csv -Delimiter `t -NoTypeInformation > new-noted.txt