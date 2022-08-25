cat -Encoding UTF8 .\addresses.txt |
   ConvertFrom-Csv -Delimiter `t |
   Select Address_ID, Address, Suite |
   ConvertTo-Csv -Delimiter `t |
   Out-File -Encoding UTF8 .\addresses.address-columns.txt

cat -Encoding UTF8 .\normalized.txt |
   ConvertFrom-Csv -Delimiter `t |
   Select Address_ID, Address, Suite |
   ConvertTo-Csv -Delimiter `t |
   Out-File -Encoding UTF8 .\normalized.address-columns.txt
