Import-Module "$HOME\.nuget\packages\system.threading.tasks.extensions\4.5.2\lib\netstandard1.0\System.Threading.Tasks.Extensions.dll";
Import-Module "$HOME\.nuget\packages\microsoft.bcl.asyncinterfaces\1.1.0\lib\netstandard2.0\Microsoft.Bcl.AsyncInterfaces.dll";
Import-Module "$HOME\.nuget\packages\csvhelper\15.0.1\lib\netstandard2.0\CsvHelper.dll"; 
Import-Module ".\TerritoryTools.Alba.PowerShell.dll"; 

$master = Get-Address `
    -Verbose `
    -Path "$HOME\Downloads\Addresses.2020-11-06.0210.txt"

#     -Path "$HOME\Downloads\business-territory-2020-10-29-152731.Done\18.1-alba-console-import.tsv" 

 $addresses= Get-Address `
    -Verbose `
    -Path "$HOME\Downloads\business-territory-2020-10-29-152731.Done\18.1-alba-console-import.tsv" 
#    -Path "$HOME\Downloads\business-territory-2020-10-29-152731.Done\18.1-alba-console-import.TEST.tsv"

"Finding duplicates..."
$addresses | Find-Duplicates -MasterList $master