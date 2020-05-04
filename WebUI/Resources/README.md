# How to Create .resx Files in PowerShell
````
 ls *.txt | % { resgen $_.Name $_.Name.Replace(".txt", ".resx") }
 ````
