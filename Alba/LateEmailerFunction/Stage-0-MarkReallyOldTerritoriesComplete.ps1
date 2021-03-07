Param(
    [Parameter]
    [String]
    $LogFile,
    [int]
    $ExpireAfterMonths = 9
)

Write-Host "Mark expired territories as completed"

# This $timestamp is appended to file names
$timestamp = Get-Date -Format 'yyyy-MM-dd_HHmmss'
Write-Host "Timestamp: $timestamp"

# Set default log file path
If(!$LogFile) { 
    $LogFile = "$PSScriptRoot/Logs/$(([System.IO.FileInfo]$PSCommandPath).BaseName).$($timestamp).log"
}

# Create log folder
[System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($LogFile))

Start-Transcript -Append -Path $LogFile

Set-Location $PSScriptRoot

$territories = Get-AlbaTerritory

$expired = $territories |
    Where Status -eq "Signed-Out" |
    Where SignedOut -lt ((Get-Date).AddMonths(-$ExpireAfterMonths))

Write-Host "Count of expired territories: $($expired.Count)"

Write-Host "Marking expired territories as completed online..."
ForEach ($territory in $expired) {
        Write-Host "$($territory.Number) $($territory.Description) $($territory.SignedOutTo) $($territory.SignedOut)"
        Set-AlbaTerritoryComplete -TerritoryId $territory.Id
        Write-Host "  Marked as completed: $(Get-Date)"
    }

Write-Host "Done"
Stop-Transcript