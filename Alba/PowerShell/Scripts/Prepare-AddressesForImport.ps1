###############################
"Parse and Normalize Addresses"
###############################
$path_to_alba_exe = "C:\Code\territory-tools\Alba\Console\bin\Debug\netcoreapp3.1\alba.exe"
$timestamp = Get-Date -Format "yyyy-MM-dd-HHmmss"
$output_folder  = "$HOME\Downloads\business-territory-$timestamp"
$google_doc_id = $env:Google_Doc_Id
"Parameters Set:"
"  path_to_alba_exe: $path_to_alba_exe"
"  timestamp: $timestamp"
"  output_folder: $output_folder"
"  google_doc_id: $google_doc_id"

##################################
"Creating output folder: $output_folder"
##################################
mkdir -Path $output_folder 

########################################
"Downloading latest copy of Google document..."
########################################
$outfile = "$output_folder\00-downloaded.tsv"
$googleDocUri = "https://docs.google.com/spreadsheets/d/$google_doc_id/export?exportFormat=tsv"
Invoke-WebRequest -Uri $googleDocUri -OutFile $outfile
$infile = $outfile

###################
"Replacing first row..."
###################
$outfile = "$output_folder\01a-first-row-replaced.tsv"
$header = "Empty1`tEmpty2`tName`tAddress`tUnit`tCity`tState`tZip`tPhone`tComments"
$header > $outfile
Get-Content -Path $infile -Encoding UTF8 | Select -Skip 1 >> $outfile
$infile = $outfile

#####################
"Remove extra spaces..."
#####################
$outfile = "$output_folder\01b-remove-extra-spaces.tsv"
$e = "[ ][ ]+"
$r = " "
Get-Content -Path $infile -Encoding UTF8 | ForEach { "$($_ -replace $e, $r)" } > $outfile
$infile = $outfile

#####################################
"Split Address from City, State, Zip"
#####################################
$outfile = "$output_folder\02-split-address-1.tsv"
$e = "^\t\t" +
     "(?<name>[^\t]*)\t" +
     "(?<address>[a-zA-Z0-9 .#\-,/&]+),\s*" +
     "(?<city>[a-zA-Z0-9 .#\-]+),\s*" +
     "(?<state>WA),?\s+" +
     "(?<zip>\d\d\d\d\d)\s*\t" +
     "(?<phone>[^\t]*)\t" +
     "(?<comment>[^\t]*)"
$r = "`t`t`$1`t`$2`t`$3`tWA`t`$5`t`$6`t`$7"
Get-Content -Path $infile -Encoding UTF8 | ForEach { $_ -replace "$e", "$r" } > $outfile
$infile = $outfile

##############################################
"Split Unit Portion from Addresses With Units"
##############################################
$outfile = "$output_folder\03-split-unit.tsv"
$e = "^\t\t([^\t]*)\t" + 
    "\s*([^,\t]*)\s*,\s*([^,\t]*)\s*\t" + # Unit portion
    "([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)$"

$r = "`t`t`$1`t`$2`t`$3`t`$4`t`$5`t`$6`t`$7`t`$8"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile
$infile = $outfile

############################################
"Add Blank Unit for Addresses Without Units"
############################################
$outfile = "$output_folder\04-blank-unit.tsv"
$e = "^\t\t([^\t]*)\t" + 
    "([^\t]*)\t" + # Unit portion 
    "([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)$"

$r = "`t`t`$1`t`$2`t`t`$3`t`$4`t`$5`t`$6`t`$7"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile
$infile = $outfile

###############################
"Normalize Phone Numbers"
###############################
$outfile = "$output_folder\04c-normalize-phone.tsv"
$e = "^(\t\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*\t)[(]?(\d\d\d)[)-]\s*(\d\d\d)\s*-(\d\d\d\d)\s*(\t[^\t]*)$"
$r = "`$1`$2-`$3-`$4`$5"
#$r = "`$1`$5"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile
$infile = $outfile

###############################
"Isolate Names"
###############################
$outfile = "$output_folder\10-isolate-names.tsv"
$e = "^[^\t]*\t[^\t]*\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)$"
$r = "`$1"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Address"
###############################
$outfile = "$output_folder\11.0-isolate-address.txt"
$r = "`$2"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

#####################################################
"Remove Abbreviation Dots from Address"
"(Not needed if there is another normalization step)"
#####################################################
$outfile = "$output_folder\11.2-isolate-address-remove-dots.txt"
Get-Content -Path "$output_folder\11.0-isolate-address.txt" -Encoding UTF8 | % { $_.Replace(".", "") } > $outfile

#######################################################
"Normalize Addresses (only) with Territory Tools Alba Console"
#######################################################
$outFile = "$output_folder\11.3-isolate-address-normalize.txt" 
Get-Content -Path "$output_folder\11.2-isolate-address-remove-dots.txt" -Encoding UTF8 `
    | % { $_ `
          -replace "\bNorth\b", "N" -replace "\bSouth\b", "S" `
          -replace "\bEast\b", "E" -replace "\bWest\b", "W" `
          -replace "\bNortheast\b", "NE" -replace "\bNorthwest\b", "SW" `
          -replace "\bSouthwest\b", "NW" -replace "\bSoutheast\b", "SE" `
          -replace "\bStreet\b", "St" -replace "\bAvenue\b", "Ave" `
          -replace "\bDrive\b", "Dr" -replace "\bParkway\b", "Pkwy" `
          -replace "\bRoad\b", "Rd" -replace "\bHighway\b", "Hwy" `
        } `
    > $outfile

###############################
"Isolate Unit"
###############################
$outfile = "$output_folder\12-isolate-unit.tsv"
$r = "`$3"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate City"
###############################
$outfile = "$output_folder\13-isolate-city.tsv"
$r = "`$4"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Zip"
###############################
$outfile = "$output_folder\14-isolate-zip.tsv"
$r = "`$6"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Phone"
###############################
$outfile = "$output_folder\15-isolate-phone.tsv"
$r = "`$7"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Comments"
###############################
$outfile = "$output_folder\16-isolate-comments.tsv"
$r = "`$8"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

########################
"Combine Isolated Files"
########################
$names = Get-Content -Path "$output_folder\10-isolate-names.tsv" -Encoding UTF8 
$addresses = Get-Content -Path "$output_folder\11.3-isolate-address-normalize.txt" -Encoding UTF8 
$units = Get-Content -Path "$output_folder\12-isolate-unit.tsv" -Encoding UTF8 
$cities = Get-Content -Path "$output_folder\13-isolate-city.tsv" -Encoding UTF8 
# skips states
$zips = Get-Content -Path "$output_folder\14-isolate-zip.tsv" -Encoding UTF8 
$phones = Get-Content -Path  "$output_folder\15-isolate-phone.tsv" -Encoding UTF8 
$comments = Get-Content -Path "$output_folder\16-isolate-comments.tsv" -Encoding UTF8 

#####################################
"Count Rows in Each File and Compare"
#####################################
$nameCount =$names.Length
$addresseCount =$addresses.Length
$unitCount =$units.Length
$cityCount = ($cities.Length)
#$stateCount = ($states.Length)
$zipCount = ($zips.Length)
$phoneCount = ($phones.Length)
$commentCount = ($comments.Length)

##################################################
"Print Summary (All row counts must be the same):"
##################################################
"Names: $nameCount"
"Addresses: $addresseCount"
"Units: $unitCount"
"Cities: $cityCount"
#"States: $stateCount"
"Zips: $zipCount"
"Phones: $phoneCount"
"Comments: $commentCount"

# We are skipping the state/province in this script
#    -or $stateCount  -ne $nameCount `

if($addresseCount    -ne $nameCount `
    -or $unitCount   -ne $nameCount `
    -or $cityCount   -ne $nameCount `
    -or $zipCount    -ne $nameCount `
    -or $phoneCount  -ne $nameCount `
    -or $commentCount-ne $nameCount) {
    throw "Counts do not match, cannot continue"
}

$outFile = "$output_folder\17-recombined.tsv"
Remove-Item -Path $outFile -Force -ErrorAction Ignore
For ($i = 0; $i -lt $names.Length; $i++) {
    "$($names[$i])`t$($addresses[$i])`t$($units[$i])`t$($cities[$i])`tWA`t$($zips[$i])`t$($phones[$i])`t$($comments[$i])" >> $outFile
}

#####################################################
"Combining Files into Export File in Alba TSV Format"
#####################################################
$outFile = "$output_folder\18.0-alba.tsv" 
$null > $outFile
For ($i = 0; $i -lt $names.Length; $i++) {
    "$($names[$i])`t$($units[$i])`t$($addresses[$i])`t$($cities[$i])`tWA`t$($zips[$i])`t`t`t`t$($phones[$i])`t$($comments[$i])" >> "$output_folder\18.0-alba.tsv"
}
$infile = $outfile

#####################################################
"Combining Files into Export File in Alba Console TSV Import Format"
#####################################################
$outFile = "$output_folder\18.1-alba-console-import.tsv" 
#$null > $outfile
$header = "Address_ID`tTerritory_ID`tLanguage`tStatus`tName`tSuite`tAddress`tCity`tProvince`tPostal_code`tCountry`tLatitude`tLongitude`tTelephone`tNotes`tNotes_private"
$header > $outfile
$territory_index = 0
$territory_ids = (101, 102, 103, 104, 105)
# In case you want to skip the headers
$starting_row = 1
For ($i = $starting_row; $i -lt $names.Length; $i++) {
    "`t$($territory_ids[$territory_index])`tChinese Mandarin`tNew`t$($names[$i])`t$($units[$i])`t$($addresses[$i])`t$($cities[$i])`tWA`t$($zips[$i])`tUSA`t`t`t$($phones[$i])`t$($comments[$i])`t2020-10-29: Imported for November 2020 Campaign" `
    >> $outFile
    if($territory_index -eq 4) {
        $territory_index = 0
    } else {
        $territory_index++
    }
}
$infile = $outfile

#"Replacing first row (header)..."
#$outFile = "$output_folder\18.2-replace-header.tsv" 
#$header = "Address_ID`tTerritory_ID`tLanguage`tStatus`tName`tSuite`tAddress`tCity`tProvince`tPostal_code`tCountry`tLatitude`tLongitude`tTelephone`tNotes`tNotes_private"
#$header > $outfile
#Get-Content -Path $infile -Encoding UTF8 | Select -Skip 1 >> $outfile
#$infile = $outfile


#######################################################
#"Normalize Addresses with Territory Tools Alba Console"
#######################################################
#$outFile = "$output_folder\19-normalized.tsv" 
#&"$path_to_alba_exe"  "normalize-addresses" "--input-path" "$infile" "--output-path" "$outFile"

"Done"


