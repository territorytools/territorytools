
$connection = Get-AlbaConnection `
    -AlbaHost $env:ALBA_HOST `
    -Account $env:ALBA_ACCOUNT

$result = Get-AlbaAddress -Connection $connection -Search "Main St"

$result | Out-File -FilePath "addresses-$(Get-Date -Format 'yyyy-MM-dd-HHmm').txt" -Encoding utf8


cat bellevue.txt |
    % { 
        Send-SendGridMail 
        -To marcdurham@gmail.com 
        -From "Your Group Information <auto@territorytools.org>" 
        -Subject "Ignore Previous Messages" 
        -Message "Please ignore any schedules from this email address.  Apologies for any confusion." 
    }