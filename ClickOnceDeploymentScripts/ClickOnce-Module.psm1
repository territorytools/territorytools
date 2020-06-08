function Publish-ClickOnce {
    param(
        [Parameter(Mandatory)]
        $Files, `
        [Parameter(Mandatory)]
        $AppLongName, `
        [Parameter(Mandatory)]
        $AppShortName, `
        [Parameter(Mandatory)]
        $IconFile, `
        [Parameter(Mandatory)]
        $Publisher, `
        [Parameter(Mandatory)]
        $OutputFolder, `
        [switch]
        $DeleteOutputFolder, `
        [Parameter(Mandatory)]
        $CertFile, `
        [Parameter(Mandatory)]
        $DeploymentRootUrl, `
        $FileExtension, `
        $FileExtDescription, `
        $FileExtProgId, `
        $Version = "0", `
        $Processor = "MSIL", `
        $BinaryReleaseFolder = ".\bin\Release", `
        $ProjectFolder = ".\", `
        $MinVersion = "none") 

    Write-Host "Creating ClickOnce deployment for $AppLongName..."
    
    if ($ProjectFolder -eq ".\") {
        $ProjectFolder = "$($(Get-Location).Path)"
    }

    Write-Host "Project Folder: $ProjectFolder"

    Write-Host "Preparing files to be published..." -ForegroundColor Green
    Write-Host "Changing current directory to binary release folder..."
    pushd .\
    cd $BinaryReleaseFolder

    $missing = 0
    Write-Host "Peparing $($Files.Count) files for publishing:"
    foreach ($f in $Files) {
        Write-Host "    $f" -NoNewline
        if (Test-Path $f) {
            Write-Host " EXISTS" -ForegroundColor Green
        } else {
            $missing += 1
            Write-Host " MISSING" -ForegroundColor Red
        }
    }

    if ($missing -gt 0) {
        Write-Host "$missing files missing. Aborting script." -ForegroundColor Red
        return
    }

    Write-Host "The first file, at index 0, will be the ""Entry Point"" file, or the main .exe."
    Write-Host "Entry Point: $($Files[0])"

    if ($Version -eq "0") {
        $versionFile =  "$ProjectFolder/Version.txt"
        Write-Host "Getting last version from $versionFile..."
        if (Test-Path $versionFile) {
            $versionContent = Get-Content $versionFile
            if ($versionContent.Contains(".")) {
                $lastVersionDot = $versionContent.LastIndexOf(".")
                $revisionString = $versionContent.SubString($lastVersionDot+1)
                $revision = [Int32]::Parse($revisionString) + 1
                $lastVersionPrefix = $versionContent.SubString(0, $lastVersionDot)
                $Version = "$lastVersionPrefix.$revision"
            } else {
                Write-Host "ERROR! Bad version file: $versionFile" -ForegroundColor Red
                Write-Host "The version file should contain a single line of text in the format 1.2.3.4" -ForegroundColor Red
                Write-Host "Version File Content: $versionContent" -ForegroundColor Yellow
                return
            }
        } else {
            $Version = "1.0.0.0"
            Write-Host "No version file exists or was supplied.  Using default version $Version" -ForegroundColor Yellow
        }
    }

    Write-Host "Publishing as version: $Version"
    
    Write-Host "Current Folder: $(Get-Location)"
    
    $appManifest = "$AppShortName.exe.manifest"
    $deployManifest = "$AppShortName.application"
    $signingAlgorithm = "sha256RSA"

    $OutputFolder = (Resolve-Path "$ProjectFolder/$OutputFolder")
    $releaseRelativePath = "Application Files/$AppShortName" + "_$($Version.Replace(".", "_"))"
    $releaseFolder = "$OutputFolder/$releaseRelativePath"
    
    Write-Host "Output Folder: $OutputFolder"
    Write-Host "Release Folder: $releaseFolder"

    $appCodeBasePath = "$releaseRelativePath/$appManifest"
    $appManifestPath = "$releaseFolder/$appManifest"
    $rootDeployManifestPath = "$OutputFolder/$deployManifest"
    $releaseDeployManifestPath = "$releaseFolder/$deployManifest"
    $releaseDeployUrl = "$DeploymentRootUrl/$releaseRelativePath/$deployManifest"

    Write-Host "Application Manifest Path: $appManifestPath"
    Write-Host "Root Deployment Manifest Path: $rootDeployManifestPath"
    Write-Host "Release Deployment Manifest Path: $releaseDeployManifestPath"
    
    Write-Host "Checking if output folder already exists..."
    if (Get-Item $releaseFolder -ErrorAction  SilentlyContinue) {
        Write-Host "    Output folder already exists." -ForegroundColor Red
        if ($DeleteOutputFolder) {
            Write-Host "    Deleting existing output folder..."
            Remove-Item -Recurse $releaseFolder
        } else {
            Write-Host "    Include the (dangerous) -DeleteOutputFolder parameter to automatically delete an existing output folder with the same name." -ForegroundColor Yellow
            return
        }
    } else {
         Write-Host "    Output folder does not already exist.  Continue."
    }

    if (-Not (Test-Path $OutputFolder)) {
        Write-Host "Creating output folder..." -ForegroundColor Yellow
        New-Item $OutputFolder -ItemType directory | Out-Null
    }

    Write-Host "Creating release folder..."
    New-Item $releaseFolder -ItemType directory | Out-Null
 
    Write-Host "Copying files into the release folder..."
    Copy-Item $Files -Destination $releaseFolder
    
    # Creating and signing app manifest file
    Write-Host "Creating and signing app manifest file..." -ForegroundColor Green
    Write-Host "Generating application manifest file: $appManifestPath"
    mage -New Application `
        -ToFile $appManifestPath `
        -Name $AppLongName `
        -Version $Version `
        -Processor $Processor `
        -FromDirectory $releaseFolder `
        -TrustLevel FullTrust `
        -Algorithm $signingAlgorithm `
        -IconFile $IconFile | Out-Host

    Write-Host "Adding file association to (unsigned) application manifest file... "
    [xml]$appManifestXml = Get-Content (Resolve-Path $appManifestPath)
    $association = $appManifestXml.CreateElement("fileAssociation")
    $association.SetAttribute("xmlns", "urn:schemas-microsoft-com:clickonce.v1")
    $association.SetAttribute("extension", $FileExtension)
    $association.SetAttribute("description", $FileExtDescription)
    $association.SetAttribute("progid", $FileExtProgId)
    $association.SetAttribute("defaultIcon", $IconFile)
    $appManifestXml.assembly.AppendChild($association) | Out-Null
    $appManifestXml.Save((Resolve-Path $appManifestPath))

    Write-Host "Signing application manifest..."
    mage -Sign $appManifestPath -CertFile $CertFile | Out-Host

    # Creating unsigned deployment manifest files
    Write-Host "Creating unsigned deployment manifest files..." -ForegroundColor Green
    Write-Host "Generating root deployment manifest file: $rootDeployManifestPath"
    mage -New Deployment `
        -ToFile $rootDeployManifestPath `
        -Name $AppLongName `
        -Version $Version `
        -MinVersion $MinVersion `
        -Processor $Processor `
        -AppManifest $appManifestPath `
        -AppCodeBase "$($appCodeBasePath.Replace(" ", "%20"))" `
        -IncludeProviderURL true `
        -ProviderURL "$DeploymentRootUrl/$deployManifest" `
        -Install true `
        -Publisher $Publisher `
        -Algorithm $signingAlgorithm | Out-Host

    Write-Host "Preparing and altering files for web deployment..." -ForegroundColor Green
    Write-Host "Renaming files for web deployment, append .deploy..."
    Get-ChildItem $releaseFolder | `
        Foreach-Object { `
            if (-not $_.FullName.EndsWith(".manifest")) { `
                Rename-Item $_.FullName "$($_.FullName).deploy" } } 
    
    Write-Host "Opening root deployment manifest to make changes..."
    $rootDeployManifestXml = [xml](Get-Content $rootDeployManifestPath)
    
    Write-Host "    Change application identiy name"
    $rootDeployManifestXml.assembly.assemblyIdentity.SetAttribute("name", $deployManifest)
    
    Write-Host "    Map file extensions to .deploy"
    $rootDeployManifestXml.assembly.deployment.SetAttribute("mapFileExtensions", "true")
    
    Write-Host "    Update ClickOnce every day"
    $rootDeployManifestXml.assembly.deployment.subscription.update.expiration.SetAttribute("maximumAge", "1")
    $rootDeployManifestXml.assembly.deployment.subscription.update.expiration.SetAttribute("unit", "days")
    
    Write-Host "Saving changes to root deployment manifest..."
    $rootDeployManifestXml.Save($rootDeployManifestPath)

    Write-Host "Copying root deployment manifest to release deployment manifest.."
    $releaseDeployManifestXml = [xml](Get-Content $rootDeployManifestPath)

    Write-Host "Changing deploymentProvider code base to URL relative to release folder URL..."
    $releaseDeployManifestXml.assembly.deployment.deploymentProvider.SetAttribute("codebase", "$($releaseDeployUrl.Replace(" ", "%20"))")

    Write-Host "Saving changes to release deployment manifest..."
    $releaseDeployManifestXml.Save($releaseDeployManifestPath)

    Write-Host "Signing root deployment manifest..." -ForegroundColor Green
    mage -Sign $rootDeployManifestPath -CertFile $CertFile | Out-Host

    Write-Host "Signing release deployment manifest..." -ForegroundColor Green
    mage -Sign $releaseDeployManifestPath -CertFile $CertFile | Out-Host
    
    Write-Host "Changing current directory to OutputFolder..."
    cd $OutputFolder
    Write-Host "Current Folder: $(Get-Location)"

    $publishFiles = dir $releaseFolder -Recurse -File

    $parentFolder = [System.IO.Path]::GetFullPath($OutputFolder)

    [System.Collections.ArrayList]$relativeFilePaths = @()
    foreach ($f in $publishFiles) {
        $relativeFilePath = "$($f.FullName.SubString($parentFolder.Length+1))"
        $relativeFilePaths.Add($relativeFilePath) | Out-Null
    }

    # Root deployment manifest file
    $realDeployManifestPath = [System.IO.Path]::GetFullPath($rootDeployManifestPath)
    $relativeFilePath = "$($realDeployManifestPath.SubString($parentFolder.Length+1))"
    $relativeFilePaths.Add($relativeFilePath) | Out-Null
    
    Write-Host "Saving just published version $Version to version file: $ProjectFolder/Version.txt..."
    Set-Content -Value "$Version" -Path "$ProjectFolder/Version.txt" -Encoding UTF8
    
    Write-Host "The ClickOnce publish was successful." -ForegroundColor Green
    Write-Host "You may need to upload the files to a web server.  See Deploy.Example.ps1"

    return $relativeFilePaths
}

