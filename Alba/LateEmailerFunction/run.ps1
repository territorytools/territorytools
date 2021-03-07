# Input bindings are passed in via param block.
param($Timer)

# Get the current universal time in the default string format.
$currentUTCtime = (Get-Date).ToUniversalTime()

# The 'IsPastDue' property is 'true' when the current function invocation is later than scheduled.
if ($Timer.IsPastDue) {
    Write-Host "PowerShell timer is running late!"
}

# Write an information log with the current time.
Write-Host "PowerShell timer trigger function ran! TIME: $currentUTCtime"

$ErrorActionPreference = "Stop"

Write-Host "Importing TerritoryTools module from folder..."
Import-Module "$PSScriptRoot\TerritoryTools\TerritoryTools.dll"; 

Write-Host "Importing TerritoryTools *.psm1 modules from folder..."
Import-Module "$PSScriptRoot\TerritoryTools\*.psm1"

Write-Host "Getting password from key vault..."
$albaPassword = Get-AzKeyVaultSecret -VaultName TerritoryToolsEmailer -Name "ALBA-PASSWORD" -AsPlainText
$env:ALBA_PASSWORD = $albaPassword

Write-Host "Getting SendGrid API Key from key vault..."
$sendGridApiKey = Get-AzKeyVaultSecret -VaultName TerritoryToolsEmailer -Name "SEND-GRID-API-KEY" -AsPlainText
$env:SENDGRID_API_KEY = $sendGridApiKey

Write-Host "Connecting to Alba..."
Get-AlbaConnection -AlbaHost $env:ALBA_HOST -Account $env:ALBA_ACCOUNT -User $env:ALBA_USER -Password $albaPassword

Write-Host "Start file generating script..."
& $PSScriptRoot\Stage-1-GenerateUserReportFiles.ps1

Write-Host "Sending emails from Outbox..."
& $PSScriptRoot\Stage-2-SendOneEmailPerFile.ps1 -Confirm

Write-Host "Done"