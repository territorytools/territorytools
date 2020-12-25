mkdir "$HOME\Desktop\TerritoriesHtml" -ErrorAction SilentlyContinue
$territories = Get-AlbaTerritory
$territories `
  | Where Status -eq Signed-out `
  | Group -Property SignedOutTo `
  | % { `
    $_.Group `
      | Sort MonthsSignedOut `
      | Select Number, Description, MonthsSignedOut, MobileLink `
      | ConvertTo-Html `
        -As Table `
        -Fragment `
        -PreContent "<h1>$($_.Name)</h1>" `
        -PostContent "<p>$($_.Group.Count) Territories</p>" `
      | % { $_ -Replace "<td>(https://[^< ]*?)</td>", "<td><a href=`"`$1`">link</a></td>" } `
      > "$HOME\Desktop\TerritoriesHtml\$($_.Name).html"; `
    $group = $_.Group | Sort MonthsSignedOut; `
    }

   