# Modules should be imported viw the TerritoryTools.psd1 file
# Use the ModuleList and the FunctionsToExport settings
Import-Module $PSScriptRoot\Modules\AlbaCOnnectionFunctions.psm1
Import-Module $PSScriptRoot\Modules\AssignNewTerritoryTo.psm1
Import-Module $PSScriptRoot\Modules\Send-SendGridMail.psm1
Import-Module $PSScriptRoot\Modules\ShowTerritoriesForUser.psm1