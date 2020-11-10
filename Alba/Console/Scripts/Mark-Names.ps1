param(
   [Parameter(Mandatory)]
   [string]$nameFile,
   [Parameter(Mandatory)]
   [string]$rowFile,
   [Parameter()]
   [int]$indexOfNameColumn = 0,
   [Parameter()]
   [string]$markWith = "***"
)

####################################################################
"Mark rows with targeted names, in the same format as the input file"
####################################################################

$names = Get-Content $nameFile ; 
$rows = Get-Content $rowFile ;

$all = @();
ForEach ( $row in $rows ) { 
    Foreach( $name in $names ) { 
        $f=$false; 
        $namesInRow = $row.Split("`t")[$indexOfNameColumn].ToUpper().Split(" ")
        if($namesInRow.Contains($name.ToUpper())) { 
            Write-Host -ForegroundColor Green "$row ($name)"; 
            $all += "$markWith$row"; 
            $f=$true; 
            break; 
        } 
    } 
    if(!$f) { 
        Write-Host -ForegroundColor Red "$row" 
        $all += "$row"; 
    } 
} ; 
$all > "$rowFile(marked).txt"
