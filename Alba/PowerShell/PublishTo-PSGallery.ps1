﻿$folderPath = "$PSScriptRoot\publish\TerritoryTools"
$timestamp = (Get-Date -Format "yyyy-MM-dd-HHmmss")

Publish-Module `
  -Path $folderPath  `
  -NuGetApiKey $env:PSGALLERY_NUGETAPIKEY `
  -Repository PSGallery 
