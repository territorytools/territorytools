#!/bin/sh
# Other environment variables copied in from the .env file in this directory
# You can test this script with the following line:
# (export $(cat .env | xargs) && bash 'backup-sisema.sh')
# The parenthesis keep the export variables from polluting the current environment
# https://stackoverflow.com/questions/11668193/using-variables-in-sqlcmd-for-linux
# Script Variables (-v) are not supported in the Linux version of sqlcmd
# so I prepend the :setvar lines to the SQL input file
export TimeStamp=`date -u "+%Y-%m-%d"` \
  && echo "Backing up database to disk..." \
  && echo "ContainerName: $ContainerName" \
  && echo "Database: $DatabaseName" \
  && echo "TimeStamp: $TimeStamp" \
  && echo ":setvar ContainerName $ContainerName" > ./backup-$DatabaseName-db-to-disk.generated.sql \
  && echo ":setvar DatabaseName $DatabaseName" >> ./backup-$DatabaseName-db-to-disk.generated.sql \
  && echo ":setvar TimeStamp $TimeStamp" >> ./backup-$DatabaseName-db-to-disk.generated.sql \
  && cat ./backup-$DatabaseName-db-to-disk.sql >> ./backup-$DatabaseName-db-to-disk.generated.sql \
  && docker cp ./backup-$DatabaseName-db-to-disk.generated.sql tt-db:/var/opt/mssql \
  && docker exec tt-db /opt/mssql-tools/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P $SA_PASSWORD \
    -i /var/opt/mssql/backup-$DatabaseName-db-to-disk.generated.sql \
  && docker exec tt-db ls /var/opt/mssql/$DatabaseName.$TimeStamp.bak \
  && docker exec $ContainerName /bin/gzip /var/opt/mssql/$DatabaseName.$TimeStamp.bak \
  && ls /root/production/data/db/$DatabaseName.$TimeStamp.bak.gz \
  && mv /root/production/data/db/$DatabaseName.$TimeStamp.bak.gz /root/backups \
  && ls /root/backups/$DatabaseName.$TimeStamp.bak.gz \
  && az storage blob upload -f /root/backups/$DatabaseName.$TimeStamp.bak.gz --container-name x-fire-db-backups -n $DatabaseName.$TimeStamp.bak.gz

