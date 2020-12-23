Param {
  [Parameter(Mandatory=$true)]
  [String]
  $UserRealName     
}

# Only include three letter plus three digit areas, like ABC123
# Other patterns are special letter writing, business, 
#   or place-holder territories.
$includeTerritoryPattern = "^\w{3}\d{3}$"

# Exclude MER (Mercer Island) and BIZ (Business Territories)
$excludeTerritoryPattern = "^(MER|BIZ).*"

# This script gets the host and account values from environment 
#   variables, (ALBA_HOST and ALBA_ACCOUNT) they could also be
#   passed into this script as parameters.
Get-AlbaConnection -AlbaHost $env:ALBA_HOST -Account $env:ALBA_ACCOUNT
$territories = Get-AlbaTerritory 

$available = $territories `
  | Where Status -eq "Available" `
  | Where Description -Match $includeTerritoryPattern `
  | Where Description -NotMatch $excludeTerritoryPattern

# The first territory is the oldest, or the same age as the oldest
$oldest = $available | Sort MonthsAgoCompleted -Descending | Select -First 1

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