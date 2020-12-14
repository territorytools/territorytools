# Territory Tools for Alba PowerShell Module
This is a first attempt at creating a PowerShell module that can be installed
from a PowerShell console.

## Cmdlets That Connect to an Alba Server
- Get-AlbaConnection
- Get-AlbaAddress
- Add-AlbaAddress
- Edit-AlbaAddress
- Import-AlbaAddressFile
- Remove-AlbaAddress
- Get-AlbaTerritory
- Get-AlbaTerritoryWithBorder
- Get-AlbaUser

## Cmdlets That Run Locally
- ConvertTo-AlbaAddressImport
- Get-DuplicateAddress
- Skip-DuplicateAddresses
- Get-NormalizedAddress
- Get-OriginalAddress

## Cmdlet That Connects to Azure Maps
- Set-Coordinates