function ShowTerritoriesForUser {
	Param (
	  [Parameter(Position = 0, Mandatory=$true)]
	  [String]
	  $UserRealName     
	)

	$ErrorAction = "Stop"

	If(!$CurrentAlbaConnection) {
	  Get-AlbaConnection -AlbaHost $env:ALBA_HOST -Account $env:ALBA_ACCOUNT -User $env:ALBA_USER -Password $env:ALBA_PASSWORD
	}

	$territories = Get-AlbaTerritory 

	$territories |
	  Where SignedOutTo -eq $UserRealName |
	  Sort SignedOut |
	  Select Number, SignedOut, Description
}

