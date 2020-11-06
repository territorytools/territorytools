$an = Get-Content .\AllNamesList-1.txt | Where { $_ -ne "NAME" } ; 
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

##################################################################
"Mark addresses, in the same format as the input file, as Chinese"
##################################################################
$names = Get-Content .\ChineseNames.txt ; 
$addresses = Get-Content .\AllAddressesList-1.txt ;
$all = @();
ForEach ( $address in $addresses ) { 
    Foreach( $name in $names ) { 
        $f=$false; if( $address.Split("`t")[1].ToUpper().Split(" ").Contains($name.ToUpper())) { 
            Write-Host -ForegroundColor Green "$address ($name)"; 
            $all += "**CHINESE**$address"; 
            $f=$true; 
            break; 
        } 
    } 
    if(!$f) { 
        Write-Host -ForegroundColor Red "$address" 
        $all += "$address"; 
    } 
} ; 
$all > table.txt