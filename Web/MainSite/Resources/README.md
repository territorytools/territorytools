# How to Make Localization Files (Translations)
Run this command to create a resx file
````
  resgen .\Index.zh-HANT.txt .\Index.zh-HANT.resx
````

### Contents of an example .restxt or .txt translation file:  
````
# Example.zh-HANT.txt
# A resource file in text format

# Initial prompt to the user.
FrontPageExplanation=你好 什么 什么

# Format string to display the result.
WelcomeBanner=这里
````

### ﻿How to Create all .resx Files in PowerShell:
(From the same folder where this README.md is)
````
ls *.txt -Recurse |
  % { resgen $_.FullName $_.FullName.Replace(".txt", ".resx") }
````
