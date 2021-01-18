# Use environment variables to connect get an Alba connection
function ConnectToAlba {
    return Get-AlbaConnection `
        -AlbaHost $env:ALBA_HOST `
        -Account $env:ALBA_ACCOUNT `
        -User $env:ALBA_USER `
        -Password $env:ALBA_PASSWORD
}
