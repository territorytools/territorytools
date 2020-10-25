###############################
"Parse and Normalize Addresses"
###############################

$alba_console_path = "C:\Code\territory-tools\Alba\Console\bin\Debug\netcoreapp3.1\alba.exe"
$outfile_prefix  = "C:\Users\Marc\Downloads\business-territory-$(Get-Date -Format "yyyy-MM-dd-HHmmss")"

##################################
"Creating folder: $outfile_prefix"
##################################
mkdir -Path $outfile_prefix 

########################################
"Downloading Latest Copy of Document..."
########################################
$outfile = "$outfile_prefix\00-downloaded.tsv"
$googleDocId = "1240UufS8vO7Z9Z7NzAxtAgxZFQdy9p5TkNmroDO3c-o"
$googleDocUri = "https://docs.google.com/spreadsheets/d/$googleDocId/export?exportFormat=tsv"
Invoke-WebRequest -Uri $googleDocUri -OutFile $outfile
$infile = $outfile

#######################################
"Making re-encoded copy of original..."
#######################################
$outfile = "$outfile_prefix\00-utf-encoded.tsv"
$e = "[ ][ ]+"
$r = " "
Get-Content -Path $infile -Encoding UTF8 > $outfile
$infile = $outfile

###################
"Replace First Row"
###################
$outfile = "$outfile_prefix\01a-first-row-replaced.tsv"
$header = "Empty1`tEmpty2`tName`tAddress`tUnit`tCity`tState`tZip`tPhone`tComments"
$header > $outfile
Get-Content -Path $infile -Encoding UTF8 | Select -Skip 1 >> $outfile
$infile = $outfile

#####################
"Remove Extra Spaces"
#####################
$outfile = "$outfile_prefix\01b-remove-extra-spaces.tsv"
$e = "[ ][ ]+"
$r = " "
Get-Content -Path $infile -Encoding UTF8 | ForEach { "$($_ -replace $e, $r)" } > $outfile
$infile = $outfile

#####################################
"Split Address from City, State, Zip"
#####################################
$outfile = "$outfile_prefix\02-split-address-1.tsv"
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
$outfile = "$outfile_prefix\03-split-unit.tsv"
$e = "^\t\t([^\t]*)\t" + 
    "\s*([^,\t]*)\s*,\s*([^,\t]*)\s*\t" + # Unit portion
    "([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)$"

$r = "`t`t`$1`t`$2`t`$3`t`$4`t`$5`t`$6`t`$7`t`$8"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile
$infile = $outfile

############################################
"Add Blank Unit for Addresses Without Units"
############################################
$outfile = "$outfile_prefix\04-blank-unit.tsv"
$e = "^\t\t([^\t]*)\t" + 
    "([^\t]*)\t" + # Unit portion 
    "([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)$"

$r = "`t`t`$1`t`$2`t`t`$3`t`$4`t`$5`t`$6`t`$7"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile
$infile = $outfile

#####################################################
"Remove Abbreviation Dots (Only removes one)"
"(Not needed if there is another normalization step)"
#####################################################
$outfile = "$outfile_prefix\04b-normalize-abbrev.tsv"
$e = "^(\t\t[^\t]*\t)" +
    "(.+?)\.(.*?)" +
    "(\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*)$"
$r = "`$1`$2`$3`$4"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile
$infile = $outfile

###############################
"Normalize Phone Numbers"
###############################
$outfile = "$outfile_prefix\04c-normalize-phone.tsv"
$e = "^(\t\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*\t[^\t]*\t)[(]?(\d\d\d)[)-]\s*(\d\d\d)\s*-(\d\d\d\d)\s*(\t[^\t]*)$"
$r = "`$1`$2-`$3-`$4`$5"
#$r = "`$1`$5"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile
$infile = $outfile

###############################
"Isolate Names"
###############################
$outfile = "$outfile_prefix\10-isolate-names.tsv"
$e = "^[^\t]*\t[^\t]*\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)$"
$r = "`$1"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Address"
###############################
$outfile = "$outfile_prefix\11-isolate-address.tsv"
$r = "`$2"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Unit"
###############################
$outfile = "$outfile_prefix\12-isolate-unit.tsv"
$r = "`$3"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate City"
###############################
$outfile = "$outfile_prefix\13-isolate-city.tsv"
$r = "`$4"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Zip"
###############################
$outfile = "$outfile_prefix\14-isolate-zip.tsv"
$r = "`$6"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Phone"
###############################
$outfile = "$outfile_prefix\15-isolate-phone.tsv"
$r = "`$7"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

###############################
"Isolate Comments"
###############################
$outfile = "$outfile_prefix\16-isolate-comments.tsv"
$r = "`$8"
Get-Content -Path $infile -Encoding UTF8 | % { $_ -replace "$e", "$r" } > $outfile

########################
"Combine Isolated Files"
########################
$names = Get-Content -Path "$outfile_prefix\10-isolate-names.tsv" -Encoding UTF8 
$addresses = Get-Content -Path "$outfile_prefix\11-isolate-address.tsv" -Encoding UTF8 
$units = Get-Content -Path "$outfile_prefix\12-isolate-unit.tsv" -Encoding UTF8 
$cities = Get-Content -Path "$outfile_prefix\13-isolate-city.tsv" -Encoding UTF8 
# skips states
$zips = Get-Content -Path "$outfile_prefix\14-isolate-zip.tsv" -Encoding UTF8 
$phones = Get-Content -Path  "$outfile_prefix\15-isolate-phone.tsv" -Encoding UTF8 
$comments = Get-Content -Path "$outfile_prefix\16-isolate-comments.tsv" -Encoding UTF8 

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

$outFile = "$outfile_prefix\17-recombined.tsv"
Remove-Item -Path $outFile -Force -ErrorAction Ignore
For ($i = 0; $i -lt $names.Length; $i++) {
    "$($names[$i])`t$($addresses[$i])`t$($units[$i])`t$($cities[$i])`tWA`t$($zips[$i])`t$($phones[$i])`t$($comments[$i])" >> $outFile
}

#####################################################
"Combining Files into Export File in Alba TSV Format"
#####################################################
$outFile = "$outfile_prefix\18-alba.tsv" 
$null > $outFile
For ($i = 0; $i -lt $names.Length; $i++) {
    "$($names[$i])`t$($units[$i])`t$($addresses[$i])`t$($cities[$i])`tWA`t$($zips[$i])`t`t`t`t$($phones[$i])`t$($comments[$i])" >> "$outfile_prefix\18-alba.tsv"
}
$infile = $outfile

#######################################################
"Normalize Addresses with Territory Tools Alba Console"
#######################################################
$outFile = "$outfile_prefix\19-normalized.tsv" 
&"$alba_console_path"  "normalize-addresses" "--input-path" "$infile" "--output-path" "$outFile"

"Done"


