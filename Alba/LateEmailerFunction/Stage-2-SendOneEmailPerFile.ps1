Param(
    [Parameter(Position=0)]
    [String]
    $InputFolder,
    [Parameter()]
    [String]
    $LogFile,
    # If you include -Confirm you are not prompted
    # Useful for automatic scheduled scripts
    [Parameter(Mandatory=$false)]
    [Switch]
    $Confirm
)

$ErrorActionPreference = "Stop"

If(!$InputFolder) { 
    $InputFolder = "$PSScriptRoot/Outbox"
}

# Set default log file path
If(!$LogFile) { 
    $LogFile = "$PSScriptRoot/Logs/$(([System.IO.FileInfo]$PSCommandPath).BaseName).$($timestamp).log"
}

# Create log folder
[System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($LogFile))

Start-Transcript -Append -Path $LogFile

Set-Location $PSScriptRoot

If(!$InputFolder -or !(Test-Path -Path $InputFolder -PathType Container)) {
    Throw "Cannot find folder: $InputFolder"
}

$files = Get-ChildItem -File -Filter *.json -Path $InputFolder

If(!$Confirm.IsPresent) {
    $prompt = "Are you sure you want to email $($files.Count) people from the folder $InputFolder ?"
    $response = $Host.UI.PromptForChoice("Email Users", $prompt, @("&Yes", "&No"), 1)
    If($response -eq 1) {
        throw "Canceled by user"
    }
}

$results = @()
New-Item -ItemType Directory -Path "$InputFolder/../Sent" -ErrorAction Ignore
ForEach($file in $files) {    
    $info = Get-Content $file.FullName | ConvertFrom-Json
    $html = Get-Content "$InputFolder/$($info.MessagePath)"
    $emailParams = @{
        From = "$($info.FromName) <$($info.FromEmail)>"
        To = "marcdurham@gmail.com"
		#To = $info.ToEmail
        Subject = $info.Subject
        Message = "$html"
        ContentType = "text/html"
    }
    Write-Host $info.ToName
    Write-Host "    $($info.ToName)"
    Write-Host "    $($emailParams.Subject)"

    $results = Send-SendGridMail @emailParams
    $results
    Move-Item -Path $file.FullName -Destination "$InputFolder/../Sent"
    Move-Item -Path "$InputFolder/$($info.MessagePath)" -Destination "$InputFolder/../Sent"
}

Stop-Transcript

