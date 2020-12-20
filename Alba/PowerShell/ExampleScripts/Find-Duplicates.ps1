Set-Location $PSScriptRoot
Import-Module TerritoryTools

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