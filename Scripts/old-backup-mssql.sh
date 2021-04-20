declare -x db_container_name="db"
declare -x database_name="TerritoryWeb"
declare -x timestamp=`date -u "+%Y-%m-%d_%H%M%S"`
docker exec -it $db_container_name "/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -Q \"BACKUP DATABASE $database_name TO DISK='/var/opt/mssql/$database_name.$timestamp.bak'\""