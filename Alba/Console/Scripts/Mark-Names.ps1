####################################################################
"Mark rows with targeted names, in the same format as the input file"
####################################################################
parms (
    $nameFile,
    $rowFile,
    $markWith = "***"
)
$names = Get-Content $nameFile ; 
$rows = Get-Content $rowFile ;
$indexOfColumnWithName = 1

$all = @();
ForEach ( $row in $rows ) { 
    Foreach( $name in $names ) { 
        $f=$false; 
        $namesInRow = $address.Split("`t")[$indexOfColumnWithName].ToUpper().Split(" ")
        if($namesInRow.Contains($name.ToUpper())) { 
            Write-Host -ForegroundColor Green "$address ($name)"; 
            $all += "$markWith$address"; 
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
