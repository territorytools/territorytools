﻿param(
   [Parameter(Mandatory)]
   [string]$nameFile,
   [Parameter(Mandatory)]
   [string]$sourceFile,
   [Parameter()]
   [string]$markedOutputFile = "$sourceFile.marked.txt",
   [Parameter()]
   [string]$extractedOutputFile = "$sourceFile.extracted.txt",
   [Parameter()]
   [int]$indexOfNameColumn = 0,
   [Parameter()]
   [string]$markWith = "***"
)

#################################################################################
"Mark rows with matched names, output in the same format as the input file"
#################################################################################

$names = Get-Content $nameFile 
$rows = Get-Content $sourceFile

$all = @()
$marked = @()
ForEach($row in $rows) { 
    ForEach($name in $names) { 
        $matched=$false
        $nameColumn = $row.Split("`t")[$indexOfNameColumn]
        
        # Remove characters that would confuse the split 
        # Example: SMITH, ALICE+BOB becomes SMITH ALICE BOB
        $cleaned = $nameColumn.ToUpper().Replace("+"," ").Replace(";"," ").Replace(","," ").Replace("&"," ")
        
        # Splits all names like surnames from given names
        $namesInRow = $cleaned.Split(" ")

        # Compare each name with each name from NameFile
        if($namesInRow.Contains($name.ToUpper())) { 
            #Write-Host -ForegroundColor Green "$row ($name)"; 
            $all += "$markWith$row"
            $marked += "$row"
            $markCount++
            $matched=$true
            break
        } 
    } 
    if(!$matched) { 
        #Write-Host -ForegroundColor Red "$row" 
        $all += "$row"
    } 
} 

$all > $outputFile
$marked > $extractedOutputFile

Write-Host "$($names.Count) names in NameFile: $nameFile"
Write-Host "$($rows.Count) rows in original SourceFile: $sourceFile"
Write-Host "$($marked.Count) rows marked in MarkedOutputFile: $markedOutputFile"
Write-Host "$($marked.Count) rows extracted into ExtractedOutputFile: $extractedOutputFile"
