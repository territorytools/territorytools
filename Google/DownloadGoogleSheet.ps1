# Downloads and saves each sheet from a Google Sheet document as a file in TSV format
# Google API Documentation Links:
#     https://developers.google.com/sheets/api/reference/rest/v4/spreadsheets/get
#     https://developers.google.com/sheets/api/reference/rest/v4/spreadsheets.values/get
Param(
    # Google Sheets Document ID (Found in the URL when viewing the document)
    # Example: https://docs.google.com/spreadsheets/d/GOOGLE_DOC_ID/edit?ts=5fd90f5e#gid=464854666
    [Parameter(Position = 0, Mandatory=$true)]
    [String]
    $GoogleDocId,
    # Google API Key for Sheets
    [String]
    $ApiKey = $env:GOOGLE_API_KEY,
    [Parameter()]
    [int]
    $HeaderRowIndex = -1,
    [Parameter()]
    [String]
    $SheetName
)

$docUri = "https://sheets.googleapis.com/v4/spreadsheets/$($GoogleDocId)?includeGridData=false&key=$ApiKey"

$docResult = Invoke-WebRequest -Uri $docUri
$docJson = $docResult.Content
$doc = $docJson | ConvertFrom-Json 

$headers = @()
ForEach ($s in $doc.sheets) {
    $currentSheetName = $s.properties.title

    If(!$SheetName -or $currentSheetName -eq $SheetName) {
        # $cellRange = "A1:ZZZ999999"
        # $currentSheetName with a $cellRange
        # %21 is the URL code for the exclamation point !
        # $range = "$($currentSheetName -replace " ", "%20")%21$($cellRange)"

        # If you leave out the cell range you get all populated rows and columns
        $range = $currentSheetName -replace " ", "%20"

        $sheetUri = "https://sheets.googleapis.com/v4/spreadsheets/$GoogleDocId/values/$($range)?key=$ApiKey&majorDimension=ROWS&dateTimeRenderOption=FORMATTED_STRING&valueRenderOption=FORMATTED_VALUE"
        $sheetResult = Invoke-WebRequest -Uri $sheetUri
        $sheetJson = $sheetResult.Content
        $sheet = $sheetJson | ConvertFrom-Json 

        Write-Host $currentSheetName    
        $headerRow = 0
        $header = @()
        $rows = @()
        $lineCount = $sheet.values.Count

        $widestLineIndex = 0
        $widestColumnCount = 0
        If($HeaderRowIndex -ge 0) {
            Write-Host "Header Row Index Supplied: $HeaderRowIndex"
            $headerRow = $HeaderRowIndex
        } Else {
            Write-Host "Header Row Index not supplied, scanning rows..."
            For($i = 0; $i -LT $lineCount; $i++) {
                If($sheet.values[$i].Count -gt $widestColumnCount) {
                    $widestLineIndex = $i
                    $widestColumnCount = $sheet.values[$i].Count
                    Write-Host "Line: $i Count: $($sheet.values[$i].Count)"
                    Write-Host "    $($sheet.values[$i])"
                }
            }
            
            Write-Host "Widest line detected at index $widestLineIndex (Width $widestColumnCount)" 
            $headerRow = $widestLineIndex
        }

        # Load the header row
        $header = @()
        $colIndex = 0
        ForEach($col in $sheet.values[$headerRow]) {
            If([string]::IsNullOrWhiteSpace($col)) {
                $header += "Empty$colIndex"
            } else {
                $header += $col
            }
            $colIndex++
        }

        Write-Host "Header: "
        Write-Host $header

        # Load all the rows after the header
        $file = @()    
        For ($i = ($headerRow + 1); $i -LT $lineCount; $i++) {
            $line = $sheet.values[$i]        
            $row = @()
            $fileRow = [ordered]@{}
            For ($c = 0; $c -LT $header.Count; $c++) {
                $row += $line[$c]
                # Header name becomes the property name
                $fileRow[$header[$c]] = $line[$c]
            }
            $rows += $row
            $file += [PSCustomObject]$fileRow
        }

        $file |  
            ConvertTo-Csv -Delimiter `t -NoTypeInformation |
            Out-File -FilePath "$currentSheetName.tsv" -Encoding utf8
    }
}

$headers | Out-File -FilePath "_headers.txt"