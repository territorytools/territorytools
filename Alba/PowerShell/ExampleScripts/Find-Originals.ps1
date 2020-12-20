Set-Location $PSScriptRoot

Import-Module TerritoryTools

$dateString = "2020-12-05"
$cityFile = ".\Cities.txt"
$addressFile = ".\Addresses.2020-12-05.0922.txt" 
$sourceTerritoryIds = @(1111, 2222, 33, 4444, 5555)

$cities = Get-Content $cityFile -Encoding utf8
"Cities: $($cities.Count)"

$master = Get-Content $addressFile -Encoding utf8 `
  | ConvertFrom-Csv -Delimiter `t `
  | ConvertTo-AlbaAddressImport

"Addresses in Master: $($master.Count)"

$source = $master `
    | Where { $campaignTerritoryIds.Contains($_.Territory_ID) } `
    | ConvertTo-AlbaAddressImport

$source `
    | ConvertTo-Csv -NoTypeInformation -Delimiter `t `
    | Out-File source-addresses.txt -Encoding utf8

"Addresses in Source: $($source.Count)"

# Ignore duplicates within the campaign territories
$duplicated = $source `
    | Get-Original -Cities $cities -MasterList $master `
    | Where { $_.Original.Address_ID -ne $_.Duplicate.Address_ID -and !$sourceTerritoryIds.Contains($_.Original.Territory_ID) } 

$duplicated `
    | Select -ExpandProperty Original `
    | ConvertTo-Csv -NoTypeInformation -Delimiter `t `
    | Out-File originals.txt -Encoding utf8

$duplicated `
    | Select -ExpandProperty Duplicate `
    | ConvertTo-Csv -NoTypeInformation -Delimiter `t `
    | Out-File duplicates.txt -Encoding utf8

"Addresses Duplicated: $($duplicated.Count)"

$notDuplicated = $source `
    | Get-Original -Cities $cities -MasterList $master `
    | Where { $_.Original.Address_ID -eq $_.Duplicate.Address_ID -or $sourceTerritoryIds.Contains($_.Original.Territory_ID) } 

$notDuplicated `
    | Select -ExpandProperty Duplicate `
    | ConvertTo-Csv -NoTypeInformation -Delimiter `t `
    | Out-File not-duplicated.txt -Encoding utf8

"Addresses Not Duplicated: $($notDuplicated.Count)"

$duplicateIds = $duplicated `
    | Select -ExpandProperty Duplicate `
    | Select -ExpandProperty Address_ID

$nonDuplicateIds = $notDuplicated `
    | Select -ExpandProperty Duplicate `
    | Select -ExpandProperty Address_ID

$processedIds = $duplicateIds + $nonDuplicateIds

$unprocessed = $source `
    | Where { !$processedIds.Contains($_.Address_ID) } `

$missed `
    | ConvertTo-Csv -NoTypeInformation -Delimiter `t `
    | Out-File unprocessed.txt -Encoding utf8

if($missed.Count -gt 0) {
    Write-Host "There were $($missed.Count) addresses missed"
} else {
    Write-Host "No addresses were missed"
}

# Modify Address Record
ForEach($n in $duplicated) {     
    # Add notes to the original address
    $note = "; DUPLICATE-FOUND-DATE: $dateString"
    $note += "; DUPLICATE-NOTES: Keep original and move to new territory, update name to newer name."
    $note += "; ORIGINAL-ADDRESS-ID: $($n.Original.Address_ID)"
    $note += "; DUPLICATE-ADDRESS-ID: $($n.Duplicate.Address_ID)"
    $note += "; MOVED-FROM-TERRITORY-ID: $(if($n.Original.Territory_ID) { $n.Original.Territory_ID } else { "Auto Assign" })"
    $note += "; MOVED-TO-TERRITORY-ID: $($n.Duplicate.Territory_ID)"
    $n.Original.Notes_private += $note
    $n.Original.Territory_ID = $n.Duplicate.Territory_ID    

    if($n.Original.Name -notmatch $n.Duplicate.Name) { 
        $note += "; NAMED-CHANGED-FROM: $($n.Original.Name)"
        $note += "; NAMED-CHANGED-TO: $($n.Duplicate.Name)"
        $n.Original.Notes_private += $note
        $n.Original.Name = $n.Duplicate.Name
    }

    $n.Original.Notes_private += "; "

    # Add notes to the duplicate address
    $dupNote = "; DUPLICATE-FOUND-DATE: $dateString"
    $dupNote += "; DUPLICATE-NOTES: This is a duplicate."
    $dupNote += "; ORIGINAL-ADDRESS-ID: $($n.Original.Address_ID)"
    $dupNote += "; "
    $n.Duplicate.Notes_private += $dupNote
    $n.Duplicate.Status = "Duplicate"
} 

$duplicated `
    | Select -ExpandProperty Original `
    | ConvertTo-Csv -NoTypeInformation -Delimiter `t `
    | Out-File "originals-to-update.txt" -Encoding utf8

$duplicated `
    | Select -ExpandProperty Duplicate `
    | ConvertTo-Csv -NoTypeInformation -Delimiter `t `
    | Out-File "duplicates-to-update.txt" -Encoding utf8

