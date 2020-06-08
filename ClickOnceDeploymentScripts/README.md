# Deploy ClickOnce Application
To deploy your ClickOnce application run the following command from PowerShell:

    .\Deploy.ps1 <Revision Number>

#### Example:
    .\Deploy.ps1 12

### How to set up your environment:
Download and install AWS Tools for PowerShell:  <https://sdk-for-net.amazonwebservices.com/latest/AWSToolsAndSDKForNet.msi>

Get Region and endpoints from here:  <https://docs.aws.amazon.com/general/latest/gr/rande.html#s3_region>

Get Access Keys for Amazon AWS from here:  <https://console.aws.amazon.com/iam/home?#/security_credential>

### Enable PowerShell scripts to work on your machine:
    Set-ExecutionPolicy -ExecutionPolicy Unrestricted

### Import the PowerShell module after it is installed:
    Import-Module AWSPowerShell

### Set up credentials:
    Set-AWSCredentials -AccessKey Access_Key_Here -SecretKey Secret_Key_Here -StoreAs AppNameCreds

### Test your credentials:
    Get-S3BucketWebsite -BucketName appname.md9.us -EndpointUrl https://s3.us-west-2.amazonaws.com -Region us-west-2

### Test site before domain is set up:
    http://appname.md9.us.s3-website.us-west-2.amazonaws.com/AppName.application

### Use Amazon Route 53 to point the domain to the bucket.  
If you have a third level domain, like appname.md9.us you can use the Route 53 DNS servers as your NS name servers.

Set up an "A" host record to point to the Alias for the bucket, in this case appname.md9.us.etc.  The S3 bucket should show up in a drop down.  If it does not: you may need to close the console page and re-open it again to get all of you buckets to show up.
