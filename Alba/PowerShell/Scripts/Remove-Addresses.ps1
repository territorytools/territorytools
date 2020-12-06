# This script was used just to delete some addresses that were accidently 
# imported into Alba. It requires the -Force parameter to be added to 
# the Remove-Address Cmdlet
#
# Do not use a for loop to delete all ids between the starting and ending there
# are gaps that may be in other group's territories and we have no way of 
# knowing if we are deleting them or not.
#
# Also only use this Cmdlet to delete addresses that were accidently imported,
# instead you can mark addresses as "Status=Duplicate" or "Status=Invalid"

# This is the range of addresses accidently added to Alba
$startingAddressId = 1000
$endingAddressId = 2000

# This is the file downloaded from Alba (tab separated)
$addressFile = ".\Addresses.2020-12-06.1130.NeedsDeleted.txt" 

Set-Location $PSScriptRoot

Import-Module "$HOME\.nuget\packages\system.threading.tasks.extensions\4.5.2\lib\netstandard1.0\System.Threading.Tasks.Extensions.dll";
Import-Module "$HOME\.nuget\packages\microsoft.bcl.asyncinterfaces\1.1.0\lib\netstandard2.0\Microsoft.Bcl.AsyncInterfaces.dll";
Import-Module "$HOME\.nuget\packages\HtmlAgilityPack\1.11.8\lib\netstandard2.0\HtmlAgilityPack.dll";
Import-Module "$HOME\.nuget\packages\Newtonsoft.Json\12.0.3\lib\netstandard2.0\Newtonsoft.Json.dll";
Import-Module "$HOME\.nuget\packages\csvhelper\15.0.1\lib\netstandard2.0\CsvHelper.dll"; 
Import-Module ".\TerritoryTools.Alba.PowerShell.dll"; 

$connection = Get-AlbaAConnection `
    -AlbaHost $env:ALBA_HOST `
    -Account $env:ALBA_ACCOUNT `
    -User $env:ALBA_USER `
    -Password $env:ALBA_PASSWORD

$addresses = Get-Content $addressFile `
    | ConvertFrom-Csv -Delimiter `t

# Find address IDs in our account within the id range 
$ids = $addresses `
    | Where { [int]$_.Address_ID -ge $startingAddressId `
         -and [int]$_.Address_ID -le $endingAddressId `
      } `
    | Select -ExpandProperty Address_ID

# You manually must add -Force to the Remove-AlbaAAddress command below.
# It has been removed to prevent accidentally running this script
$ids | Remove-AlbaAAddress -Verbose -Connection $connection