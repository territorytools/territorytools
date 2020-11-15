Import-Module "$HOME\.nuget\packages\system.threading.tasks.extensions\4.5.2\lib\netstandard1.0\System.Threading.Tasks.Extensions.dll";
Import-Module "$HOME\.nuget\packages\microsoft.bcl.asyncinterfaces\1.1.0\lib\netstandard2.0\Microsoft.Bcl.AsyncInterfaces.dll";
Import-Module "$HOME\.nuget\packages\csvhelper\15.0.1\lib\netstandard2.0\CsvHelper.dll"; 
Import-Module ".\TerritoryTools.Alba.PowerShell.dll"; 

$territoryIds = @( 1, 2, 3, 4, 5)
$cities = Get-Content "$HOME\Desktop\Cities.txt"
$masterPath = "$HOME\Downloads\Addresses.2020-11-06.0210.txt"
$masterFolder = Split-Path $masterPath 

$master = Get-Address `
    -Verbose `
    -Path $masterPath
#    -Path "$HOME\Downloads\business-territory-2020-10-29-152731.Done\Test-Find-Duplicates-Source.txt" 


#     -Path "$HOME\Downloads\business-territory-2020-10-29-152731.Done\18.1-alba-console-import.tsv" 

## $addresses= Get-Address `
##    -Verbose `
##    -Path "$HOME\Downloads\business-territory-2020-10-29-152731.Done\18.1-alba-console-import.tsv" 
#    -Path "$HOME\Downloads\business-territory-2020-10-29-152731.Done\Test-Find-Duplicates-1.txt" 
#    -Path "$HOME\Downloads\business-territory-2020-10-29-152731.Done\18.1-alba-console-import.TEST.tsv"

##ForEach($territoryId in $territoryIds) {
    #$addresses = $master | Where { $_.Territory_Id -eq $territoryId }
    $addresses = $master | Where { $territoryIds.Contains($_.Territory_Id)}

    "Territory ID: $territoryId"
    "Master List: $($master.Count)"
    "Target List: $($addresses.Count)"
    "City List: $($cities.Count)"

    "Finding duplicates..."

     $duplicates = $addresses `
        | Get-Duplicates -Verbose -MasterList $master -Cities $cities -IncludeSelf 

$duplicates | ConvertTo-Csv -Delimiter `t | Out-File -FilePath (Join-Path -Path $masterFolder -ChildPath "duplicates-all.tsv")


  $selfdups = $addresses `
        | Get-Duplicates -Verbose -MasterList $addresses -Cities $cities -IncludeSelf 

$selfdups | ConvertTo-Csv -Delimiter `t | Out-File -FilePath (Join-Path -Path $masterFolder -ChildPath "duplicates-self.tsv")

###    $addresses `
###        | Get-Duplicates -Verbose -MasterList $master -Cities $cities -IncludeSelf `
###        | Out-Address -Path "$HOME\Desktop\duplicates-All.txt"

#@        | Out-Address -Path "$HOME\Desktop\duplicates-$territoryId.txt"

#        | (Where { $_.Territory_Id -ne $territoryId } `
    # Write-Host "Duplicates: $($duplicates.Count)"
##}





