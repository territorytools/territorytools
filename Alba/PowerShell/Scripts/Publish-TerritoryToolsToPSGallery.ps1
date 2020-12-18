$folderPath = "YOUR_REPOSITORY_PATH_HERE\Alba\PowerShell\bin\Release\netstandard2.0"
$timestamp = (Get-Date -Format "yyyy-MM-dd-HHmmss")
$destination =  ".\$timestamp\TerritoryTools.Alba.PowerShell"
mkdir $destination 
Copy -Recurse -Path $folderPath -Filter *.* -Destination $destination 
mv -Path "$destination\netstandard2.0\*.*" -Destination $destination 

Publish-Module `
-Path $destination `
-NuGetApiKey "PASTE_API_KEY_HERE" `
-Repository PSGallery 
