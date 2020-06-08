Import-Module AWSPowerShell
Import-Module .\ClickOnce-Module.ps1

Write-Host "Building Project..."
msbuild.exe AppName.WinForms.csproj /p:Configuration=Release

Write-Host ""
# Optional Extra Commands Here
# Write-Host "Compressing UserManual folder..."
# 7z a -sfx UserManual.exe UserManual

Write-Host ""
Write-Host "Preparing list of files to publish..." 
# The first file is the "Entry Point" file, your main .exe.
$files = `
    "AppName.exe", `
    "AppName.pdb", `
    "AppName.exe.config", `
    "AppNameIcon.ico", `
    "UserManual.exe", `
    "AppName.Common.dll", `
    "AppName.Common.pdb"

try {
    Write-Host "Publish ClickOnce files..." 
    $publishedPaths = Publish-ClickOnce `
        -Files $files `
        -AppLongName "Full Application Name" `
        -AppShortName "AppName" `
        -IconFile "YourIconFile.ico" `
        -Publisher "Your Name" `
        -OutputDir "/YourLocalClickOncePublishFolder" `
        -CertFile "../../YourPrivateKeyFile.pfx" `
        -DeploymentRootUrl "http://downloads.domain.com/appname" `
        -FileExtension ".abc" `
        -FileExtDescription "Application Name ABC File" `
        -FileExtProgId "AppName.ABC" `
        -ErrorAction Stop
        
    if ($publishedPaths.Count -eq 0) {
        Write-Host "Error.  No files were published.  Nothing to upload." `
            -ForegroundColor Red
        popd
        return
    }

    Write-Host "Uploading published files to Amazon S3..."
    foreach ($path in $publishedPaths) {
        Write-Host $path -NoNewline
        Write-S3Object `
            -BucketName  "bucket.name.here" `
            -Region  "us-west-2" `
            -File $path `
            -Key "$($path)" `
            -CannedACLName public-read `
            -ErrorAction Stop
        Write-Host " DONE" -ForegroundColor Green
    }

    Write-Host "Finished uploading to Amazon S3." -ForegroundColor Green
} catch { 
    Write-Host "ERROR" -ForegroundColor Red
    Write-Host $Error -ForegroundColor Red
    popd
    return
}

popd
