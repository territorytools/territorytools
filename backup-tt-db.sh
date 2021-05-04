declare -x timestamp=`date -u "+%Y-%m-%d_%H%M%S"`
docker cp ./backup-tt-db.sql tt-db:/var/opt/mssql/backup-tt-db.sql
docker exec -it tt-db /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -i /var/opt/mssql/backup-tt-db.sql
docker cp tt-db:/var/opt/mssql/territory.temp.bak ./territory.$timestamp.bak
echo /territory.$timestamp.bak
