ls $PSScriptRoot/*.txt -Recurse |
  % { resgen $_.FullName $_.FullName.Replace(".txt", ".resx") }