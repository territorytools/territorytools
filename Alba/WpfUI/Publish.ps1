Import-Module ..\..\ClickOnceDeploymentScripts\ClickOnce-Module.psm1

Write-Host "Building Project..."
#msbuild.exe AlbaClientWpfUI.csproj /p:Configuration=Release

Write-Host ""
# Optional Extra Commands Here
# Write-Host "Compressing UserManual folder..."
# 7z a -sfx UserManual.exe UserManual

Write-Host ""
Write-Host "Preparing list of files to publish..." 
# The first file is the "Entry Point" file, your main .exe.
$files = `
    "AlbaBackupClient.exe", `
    "AlbaBackupClient.exe.config", `
    "AlbaBackupClient.pdb", `
    "Controllers.dll", `
    "Controllers.dll.config", `
    "Controllers.pdb", `
    "CsvHelper.dll", `
    "TerritoryTools.ico", `
    "HtmlAgilityPack.dll", `
    "HtmlAgilityPack.pdb", `
    "Microsoft.Bcl.AsyncInterfaces.dll", `
    "Newtonsoft.Json.dll", `
    "StreetTypes.txt", `
    "System.Runtime.CompilerServices.Unsafe.dll", `
    "System.Threading.Tasks.Extensions.dll", `
    "System.ValueTuple.dll", `
    "TerritoryTools.Entities.dll", `
    "TerritoryTools.Entities.dll.config", `
    "TerritoryTools.Entities.pdb", `
    "UnitTypes.txt", `
    "WpfUILibrary.dll", `
    "WpfUILibrary.dll.config", `
    "WpfUILibrary.pdb"

try {
    Write-Host "Publish ClickOnce files..." 
    $publishedPaths = Publish-ClickOnce `
        -Files $files `
        -AppLongName "Alba Backup Client" `
        -AppShortName "AlbaBackupClient" `
        -IconFile "TerritoryTools.ico" `
        -Publisher "Marc Durham" `
        -OutputFolder "/Publish" `
        -CertFile "../../TerritoryTools.pfx" `
        -DeploymentRootUrl "http://downloads.md9.us" `
        -FileExtension ".csv" `
        -FileExtDescription "Alba Address Import CSV File" `
        -FileExtProgId "AlbaClient.CSV" `
        -ErrorAction Stop
        
    if ($publishedPaths.Count -eq 0) {
        Write-Host "Error.  No files were published.  Nothing to upload." `
            -ForegroundColor Red
        popd
        return
    }

    Write-Host "Finished publishing files, ready to upload to Amazon S3." -ForegroundColor Green
} catch { 
    Write-Host "ERROR" -ForegroundColor Red
    Write-Host $Error -ForegroundColor Red
    popd
    return
} 

popd
