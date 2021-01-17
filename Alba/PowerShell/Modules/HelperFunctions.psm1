function Set-AlbaOldestTerritoryUser {
Param (
 [Parameter(Mandatory=$true)]
 [String]
 $UserRealName     
)
$ErrorAction = "Stop"

#Import-Module $Home\OneDrive\Scripts\AlbaBackUps\TerritoryTools

# Only include three letter plus three digit areas, as others are special
#   letter writing, business, or place-holder territories.
$includeTerritoryPattern = "^\w{3}\d{3}$"

# Exclude MER = 'Mercer Island' and BIZ = 'Business Territories'
$excludeTerritoryPattern = "^(MER|BIZ).*"

# This script gets the host and account values from environment 
#   variables, they could also be passed into this script as 
#   parameters.
Get-AlbaConnection -AlbaHost $env:ALBA_HOST -Account $env:ALBA_ACCOUNT -User $env:ALBA_USER -Password $env:ALBA_PASSWORD
$territories = Get-AlbaTerritory 

$available = $territories `
  | Where Status -eq "Available" `
  | Where Description -Match $includeTerritoryPattern `
  | Where Description -NotMatch $excludeTerritoryPattern

$oldest = $available | Sort LastCompleted | Select -First 1

$users = Get-AlbaUser

# This could be easily done with Email or UserName instead of Name
$userId = $users | Where Name -eq $UserRealName | Select -ExpandProperty Id

Set-AlbaTerritoryUser -TerritoryId $oldest.Id -UserId $userId

# Download territories and get newly generated mobile link
$territories = Get-AlbaTerritory 

# Find the territory we just assigned which will have a new MobileLink
$assigned = $territories | Where Id -eq $oldest.Id | Select -First 1

# Print territory number, description, and the newly generated mobile link 
#   to the output
Write-Output "$($assigned.Number) $($assigned.Description) $($assigned.MobileLink)"
}

Export-ModuleMember -Function * -Alias * -Variable *