
host=$1
if [ -z "$host" ]
then
  echo "Usage:"
  echo "  ./deploy-to-staging.sh user@host.name.tld |prod|"
  exit
fi

slot=$2
if [[ "$slot" == "prod" ]]
then
  slot_folder=
else
  slot_folder=staging-
fi
  
bak_folder=$(date +"%Y-%m-%d-%H%M%S")
target_folder=/var/www/tt-web-${slot_folder}wasm

echo "Target Host: $host"
echo "Folder: $bak_folder"
echo "Target Folder: $target_folder"

cd "./dist/" \
    && mkdir "$bak_folder" \
    && scp "$host:$target_folder/*" $bak_folder \
    && scp -r $bak_folder "$host:$target_folder/bak/" \
    && scp ./* "$host:$target_folder"/