#!/bin/sh
# Other environment variables copied in from the .env file in this directory
# You can test this script with the following line:
# (export $(cat .env | xargs) && bash 'backup-sisema.sh')
# Or try this
# env $(cat .env | xargs) ./backup-sisema-db.sh
# The parenthesis keep the export variables from polluting the current environment
# https://stackoverflow.com/questions/11668193/using-variables-in-sqlcmd-for-linux
# Script Variables (-v) are not supported in the Linux version of sqlcmd
# so I prepend the :setvar lines to the SQL input file

# Exit script on exit
set -e

# Check to see if environment variables are set
if [ -z "${SA_PASSWORD}" ]; then echo "The environment variable SA_PASSWORD must be set first"; exit 1; fi
if [ -z "${AZURE_STORAGE_CONNECTION_STRING}" ]; then echo "The environment variable AZURE_STORAGE_CONNECTION_STRING must be set first"; exit 1; fi
if [ -z "${SqlDockerContainerName}" ]; then echo "The environment variable SqlDockerContainerName must be set first"; exit 1; fi
if [ -z "${DatabaseName}" ]; then echo "The environment variable DatabaseName must be set first"; exit 1; fi
if [ -z "${AzureStorageContainerName}" ]; then echo "The environment variable AzureStorageContainerName must be set first"; exit 1; fi

export TimeStamp=$(date -u "+%Y-%m-%d_%H%M")
export GeneratedSqlFileRelativePath=./backup-$DatabaseName-db-to-disk.generated.sql
export BackupFile=$DatabaseName.$TimeStamp.bak
export ArchiveFileName=$BackupFile.gz
export DockerVolumeDbFolderPath=/root/production/data/db
export TargetFolderPath=/root/backups

echo "Backing up database to disk..."
echo "SqlDockerContainerName: $SqlDockerContainerName"
echo "Database: $DatabaseName"
echo "TimeStamp: $TimeStamp"

# Generate SQL Script
echo ":setvar SqlDockerContainerName $SqlDockerContainerName" > $GeneratedSqlFileRelativePath
echo ":setvar DatabaseName $DatabaseName" >> $GeneratedSqlFileRelativePath
echo ":setvar TimeStamp $TimeStamp" >> $GeneratedSqlFileRelativePath
cat ./backup-db-to-disk.sql >> $GeneratedSqlFileRelativePath

# Connect to MS-SQL Docker Container
docker cp $GeneratedSqlFileRelativePath $SqlDockerContainerName:/var/opt/mssql

# Clean up logs older than 120 days
find $TargetFolderPath/db/log -mtime +120 -type f -delete

# Create back-up file
docker exec $SqlDockerContainerName /opt/mssql-tools/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P $SA_PASSWORD \
    -i /var/opt/mssql/$GeneratedSqlFileRelativePath

docker exec $SqlDockerContainerName ls /var/opt/mssql/$BackupFile
docker exec $SqlDockerContainerName /bin/gzip /var/opt/mssql/$BackupFile
ls $DockerVolumeDbFolderPath/$ArchiveFileName
mv $DockerVolumeDbFolderPath/$ArchiveFileName $TargetFolderPath
ls $TargetFolderPath/$ArchiveFileName
az storage blob upload -f $TargetFolderPath/$ArchiveFileName --container-name x-fire-db-backups -n $ArchiveFileName

