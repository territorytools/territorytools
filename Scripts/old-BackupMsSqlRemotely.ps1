$timestamp=(Get-Date -Format "yyyy-MM-dd_HHmmss")
$db_container_name="tt-db"
$database_name="TerritoryWeb"
$SA_PASSWORD=$Env:SA_PASSWORD

ssh "root@copper.do.md9.us" "docker exec -it $db_container_name `"/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -Q \`"BACKUP DATABASE $database_name TO DISK='/var/opt/mssql/$database_name.$timestamp.bak'\`"`""

cd "$HOME\Google Drive\Backups\TerritoryTools"
scp "root@copper.do.md9.us:/root/production/data/db/data/$database_name-$timestamp.bak" "./$database_name-$timestamp.bak"