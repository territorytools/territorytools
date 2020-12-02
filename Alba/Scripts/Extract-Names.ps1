$an = Get-Content .\Rows.txt | Where { $_ -ne "NAME" } ; 
$cn = Get-Content .\ChineseNames.txt ; 
$found = @(); 
$all = @();
ForEach ( $name in $an ) { 
    Foreach( $c in $cn ) { 
        $f=$false; if( $name.ToUpper().Split(" ").Contains($c.ToUpper())) { 
            Write-Host -ForegroundColor Green "$name ($c)"; 
            $all += "$name*"; 
            $found += "$name*"; 
            $f=$true; 
            break; 
        } 
    } 
    if(!$f) { 
        Write-Host -ForegroundColor Red "$name" 
        $all += "$name"; 
    } 
} ; 
Write-Host "Total: $($an.Length)"
Write-Host "Chinese: $($found.Length) ($(($found.Length/$an.Length).ToString("0%")))" ;
$all > list.txt

