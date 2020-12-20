function Connect-Alba {
    $cred = Get-Credential `
        -UserName $env:ALBA_USER `
        -Message "Enter your user name for the account $($env:ALBA_ACCOUNT)"

    return Get-AlbaConnection `
        -AlbaHost $env:ALBA_HOST `
        -Account $env:ALBA_ACCOUNT `
        -Credential $cred
}


Export-ModuleMember -Function * -Alias * -Variable *