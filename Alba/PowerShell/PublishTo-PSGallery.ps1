$folderPath = "$PSScriptRoot\publish\TerritoryTools"
$timestamp = (Get-Date -Format "yyyy-MM-dd-HHmmss")

If(!$env:PSGALLERY_NUGETAPIKEY) {
    throw "Could not read NuGet API Key for PowerShell Gallery environment variable named PSGALLERY_NUGETAPIKEY"
    exit
}

Publish-Module `
  -Path $folderPath  `
  -NuGetApiKey $env:PSGALLERY_NUGETAPIKEY `
  -Repository PSGallery 
