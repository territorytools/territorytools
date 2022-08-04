﻿pushd ./
Import-Module ..\..\ClickOnceDeploymentScripts\ClickOnce-Module.psm1

Write-Host "Building Project..."
#msbuild.exe AlbaClientWpfUI.csproj /p:Configuration=Release

mkdir "Publish"

Write-Host ""
# Optional Extra Commands Here
# Write-Host "Compressing UserManual folder..."
# 7z a -sfx UserManual.exe UserManual

Write-Host ""
Write-Host "Preparing list of files to publish..." 
# The first file is the "Entry Point" file, your main .exe.
$files = `
    "AlbaSyncTool.exe", `
    "AlbaSyncTool.exe.config", `
    "AlbaSyncTool.pdb", `
    "TerritoryTools.Alba.Controllers.dll", `
    "TerritoryTools.Alba.Controllers.dll.config", `
    "TerritoryTools.Alba.Controllers.pdb", `
    "CommandLine.dll", `
    "CsvHelper.dll", `
    "HtmlAgilityPack.dll", `
    "Microsoft.Bcl.AsyncInterfaces.dll", `
    "Newtonsoft.Json.dll", `
    "System.Runtime.CompilerServices.Unsafe.dll", `
    "System.Threading.Tasks.Extensions.dll", `
    "System.ValueTuple.dll", `
    "TerritoryTools.Alba.SyncTool.Library.dll", `
    "TerritoryTools.Alba.SyncTool.Library.dll.config", `
    "TerritoryTools.Alba.SyncTool.Library.pdb", `
    "StreetTypes.txt", `
    "UnitTypes.txt", `
    "TerritoryTools.ico"

try {
    Write-Host "Publish ClickOnce files..." 
    $publishedPaths = Publish-ClickOnce `
        -Files $files `
        -AppLongName "Alba Sync Tool" `
        -AppShortName "AlbaSyncTool" `
        -IconFile "TerritoryTools.ico" `
        -Publisher "Marc Durham" `
        -OutputFolder "Publish" `
        -BinaryReleaseFolder "bin/Release/net472" `
        -CertFile "../../../TerritoryTools.pfx" `
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
