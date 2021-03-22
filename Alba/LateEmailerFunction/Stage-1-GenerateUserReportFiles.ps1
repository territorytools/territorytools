# Create a report and email file for each user that can be detected and sent
# by the Stage-2-SendOneEmailPerFile.ps1 script
Param(
    [Parameter()]
    [String]
    $OutputFolder,
    [Parameter()]
    [String]
    $LogFile,
    [Parameter()]
    [int]
    $LateAfterDays = 90,
	[Parameter()]
	[String[]]
	$NamesToUse = @()
)

$ErrorActionPreference = "Stop"

If(!$OutputFolder) { 
    $OutputFolder = "$PSScriptRoot/Outbox"
}

# This $timestamp is appended to file names
$timestamp = Get-Date -Format 'yyyy-MM-dd_HHmmss'

# Create the output folder if it does not already exist
New-Item $OutputFolder -ItemType Directory -ErrorAction SilentlyContinue

# Set default log file path
If(!$LogFile) { 
    $LogFile = "$PSScriptRoot/Logs/$(([System.IO.FileInfo]$PSCommandPath).BaseName).$($timestamp).log"
}

# Create log folder
[System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($LogFile))

Start-Transcript -Append -Path $LogFile

Set-Location $PSScriptRoot

$preContent = Get-Content -Raw -Path "$PSScriptRoot/PreContent.html"

# Load the list of full names to skip like admin, test, & visitor accounts
$namesToSkip = Get-Content -Path "$PSScriptRoot/NamesToSkip.txt"
Write-Host ""
Write-Host "Names to Skip:"
ForEach ($name in $namesToSkip) { Write-Host $name }
Write-Host "End of Names to Skip"
Write-Host ""

$connection = @{
    AlbaHost = $env:ALBA_HOST 
    Account = $env:ALBA_ACCOUNT 
    User = $env:ALBA_USER 
    Password = $env:ALBA_PASSWORD
}
Get-AlbaConnection @connection

# Create a map of full names to match to email addresses
$users = Get-AlbaUser
$userMap = @{}
ForEach($user in $users) {
    $userMap[$user.Name] = $user
}

$territories = Get-AlbaTerritory

$lateDate = (Get-Date).AddDays(-$LateAfterDays)
Write-Host "Late Date: $lateDate"

$territories |
    Where Status -eq Signed-out |
    Where { $namesToSkip -NotContains $_.SignedOutTo  } |
    Group -Property SignedOutTo |
    Sort Name |
    ForEach { 
        Write-Host "$($_.Name)"

        # Determine how many days ago the oldest territory was checked out 
        # by this user
        $group = $_.Group
        $late =  $group | 
            Where SignedOut -lt $lateDate 

        # Count territories that are late
        $lateTerritoryCount = $late.Count 

        Write-Host "  Late Count: $($late.Count )"

        # Generate email files only for users with late territories
        If($lateTerritoryCount -eq 0) { 
            Write-Host "  No late territories. Not emailed."; 
        } else { 
            Write-Host "  Preparing Email...";
                
            $postContent = "<p>$($_.Group.Count) Territories</p>"

            # Count territories that are late
            $lateTerritoryCount = 0;
            ForEach($t in $territoryList) { 
                If($t.SignedOut -lt $lateDate) { 
                    $lateTerritoryCount += 1 
                }
            };

            # Populate a list of territories signed out by the selected user,
            # including late and not late territories.
            $territoryList = $_.Group |
                Sort SignedOut |
                Select Number, 
                    Description, 
                    @{ 
                        Name = "Signed Out"; 
                        Expression = { $_.SignedOut.ToString("yyyy-MMM-dd") }
                        },
                    @{ 
                        Name = "Months"; 
                        Expression = { 
                            $m = ((Get-Date).Subtract($_.SignedOut).Days/30).ToString('F0'); 
                            "($m months)"; 
                            } 
                        },
                    @{ 
                        Name = "Link"; 
                        Expression = { $_.MobileLink } 
                        },
                    @{ 
                        Name = "Comments"; 
                        Expression = { 
                                $isLate = ($_.SignedOut -le $lateDate);
                                If($isLate) { 
                                    $comments = "(Late)"; 
                                } else { 
                                    $comments = "(Not Late)"; 
                                }
                                $comments
                            } 
                        }
      
            # This creates a JSON file will be used to get the email address 
            # and info for the email subject text
            @{
                FromName = "Bellevue Mandarin Territory System"
                FromEmail = "auto@territorytools.org"
                ToName = $_.Name
                ToEmail = $userMap[$_.Name].Email
                Subject = "You have $lateTerritoryCount late territories signed out" 
                MessagePath = "./$($_.Name).$timestamp.html"
            } |
                ConvertTo-Json |
                Out-File "$OutputFolder\$($_.Name).$timestamp.json" -Encoding utf8     
        
            # This creates an HTML file will be used as the email message 
            # body in the next stage.  It can be reviewed before sent.
            $territoryList |
                ConvertTo-Html -As Table -Fragment -PreContent $preContent -PostContent $postContent |
                # Convert link text into a clickable link
                ForEach { $_ -Replace "<td>(https://[^< ]*?)</td>", "<td><a clicktracking=off href=`"`$1`">link</a></td>" } |
                Out-File "$OutputFolder/$($_.Name).$timestamp.html" -Encoding utf8
        }
    }
    
Stop-Transcript
