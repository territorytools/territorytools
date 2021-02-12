# Saves each sheet from a Google Sheet document as a file in tab separated format
# Google API Documentation Links:
#     https://developers.google.com/sheets/api/reference/rest/v4/spreadsheets/get
#     https://developers.google.com/sheets/api/reference/rest/v4/spreadsheets.values/get
Param(
    # Google Sheets Document ID (Found in the URL when viewing the document)
    # Example: https://docs.google.com/spreadsheets/d/GOOGLE_DOC_ID/edit?ts=5fd90f5e#gid=464854666
    [Parameter(Position = 0, Mandatory=$true)]
    [String]
    $googleDocId,
    # Google API Key for Sheets
    [String]
    $apiKey = $env:GOOGLE_API_KEY
)

$docUri = "https://sheets.googleapis.com/v4/spreadsheets/$($googleDocId)?includeGridData=false&key=$apiKey"

$docResult = Invoke-WebRequest -Uri $docUri
$docJson = $docResult.Content
$doc = $docJson | ConvertFrom-Json 

$headers = @()
ForEach ($s in $doc.sheets) {
    $sheetName = $s.properties.title

    # $cellRange = "A1:ZZZ999999"
    # $sheetName with a $cellRange
    # %21 is the URL code for the exclamation point !
    # $range = "$($sheetName -replace " ", "%20")%21$($cellRange)"

    # If you leave out the cell range you get all populated rows and columns
    $range = $sheetName -replace " ", "%20"

    $sheetUri = "https://sheets.googleapis.com/v4/spreadsheets/$googleDocId/values/$($range)?key=$apiKey&majorDimension=ROWS&dateTimeRenderOption=FORMATTED_STRING&valueRenderOption=FORMATTED_VALUE"
    $sheetResult = Invoke-WebRequest -Uri $sheetUri
    $sheetJson = $sheetResult.Content
    $sheet = $sheetJson | ConvertFrom-Json 

    Write-Host $sheetName    
    $headerRow = 0
    $header = @()
    $headerIndex = 0
    $rows = @()
    $lineCount = $sheet.values.Count
    For($headerIndex = 0; $headerIndex -LT $lineCount; $headerIndex++) {
        $line = $sheet.values[$headerIndex]
        If(($line.Count -eq 0) -or
            (($line.Count -eq 1) -and ($line[0] -eq $sheetName))) { 
            Write-Host "Skipping line $headerIndex in $sheetName, value: $line"
            continue 
        }
        $headers += "$($sheetName):"
        $header = @()
        $colIndex = 0
        ForEach($col in $line) {
            If([string]::IsNullOrWhiteSpace($col)) {
                $header += "Empty$colIndex"
            } else {
                $header += $col
            }
            $colIndex++
        }
        $headers += $header
        $headers += ""
        $rows += $header
        break
    }
    $file = @()    
    For ($i = ($headerIndex + 1); $i -LT $lineCount; $i++) {
        $line = $sheet.values[$i]        
        $row = @()
        $fileRow = @{}
        For ($c = 0; $c -LT $header.Count; $c++) {
            $row += $line[$c]
            $fileRow[$header[$c]] = $line[$c]
        }
        $rows += $row
        $file += [PSCustomObject]$fileRow
    }
    $file |  
        ConvertTo-Csv -Delimiter `t -NoTypeInformation |
        Out-File -FilePath "$sheetName.tsv" -Encoding utf8
}

$headers | Out-File -FilePath "_headers.txt"
