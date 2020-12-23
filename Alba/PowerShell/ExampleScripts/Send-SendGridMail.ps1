param (
    [String]
    $ApiKey = $env:SENDGRID_API_KEY,
    [ValidateSet( "text/plain",  "text/html")]
    [String]
    $ContentType = "text/plain",
    [Parameter(Mandatory)]
    [String]
    $To,
    [String]
    $From = $env:SENDGRID_FROM,
    [Parameter(Mandatory)]
    [String]
    $Subject,
    [Parameter(Mandatory)]
    [String]
    $Message
)

# How to Create an API Key
# https://sendgrid.com/docs/ui/account-and-settings/api-keys/#creating-an-api-key

#$ContentType = "text/html"
#$Message = "<h1>This is the Message</h1><table><tr><td>hi</td><td>there</td></tr></table>"

# If ApiKey is missing use the variable
If(!$ApiKey) {
  throw "Missing 'ApiKey' parameter! Supply the ApiKey value or set the SENDGRID_API_KEY environment variable"
}

If(!$From) {
  throw "Missing 'From' parameter! Supply the From value or set the SENDGRID_FROM environment variable"
}

$data = @{
    personalizations = @(@{"to" = @( @{"email" = "$To"})})
    from = @{"email" = "$From"}
    subject = "$Subject"
    content = @(@{type = "$ContentType"; value = "$Message"})
}

Invoke-WebRequest `
     -Uri "https://api.sendgrid.com/v3/mail/send" `
     -ContentType "application/json" `
     -Headers  @{Authorization = "Bearer $ApiKey"} `
     -Method Post `
     -Body ($data | ConvertTo-Json -Depth 5 -Compress) 
    