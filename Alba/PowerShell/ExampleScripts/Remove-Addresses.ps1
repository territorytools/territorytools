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

Import-Module TerritoryTools

$connection = Get-AlbaAConnection `
    -AlbaHost $env:ALBA_HOST `
    -Account $env:ALBA_ACCOUNT

$addresses = Get-Content $addressFile `
    | ConvertFrom-Csv -Delimiter `t

# Find address IDs in our account within the id range 
$ids = $addresses `
    | Where { [int]$_.Address_ID -ge $startingAddressId `
         -and [int]$_.Address_ID -le $endingAddressId `
      } `
    | Select -ExpandProperty Address_ID

"You must manually add -Force to the Remove-AlbaAAddress command below."
"It has been removed from this script to prevent accidentally running this script"
$ids | Remove-AlbaAAddress -Verbose -Connection $connection