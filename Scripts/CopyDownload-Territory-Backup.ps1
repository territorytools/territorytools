$datestamp=(Get-Date -Format "yyyy-MM-dd")
$database_name="territory"
$remote_path="/root/backups/$database_name.$($datestamp).bak.gz"
"Path: $remote_path"

cd "$HOME\OneDrive\Sisema"
scp "root@copper.do.md9.us:$remote_path" ./