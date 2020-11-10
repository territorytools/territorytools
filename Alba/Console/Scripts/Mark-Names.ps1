####################################################################
"Mark rows with Chinese names, in the same format as the input file"
####################################################################
$names = Get-Content .\ChineseNames.txt ; 
$addresses = Get-Content .\Rows.txt ;
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
