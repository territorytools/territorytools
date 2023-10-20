
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

cd ./dist \
  && mkdir $bak_folder \
  && echo "Done making new folder..." \
  && scp $host:$target_folder/favicon* $bak_folder \
  && scp $host:$target_folder/index.html $bak_folder \
  && scp $host:$target_folder/territory-tools-component-* $bak_folder \
  && scp $host:$target_folder/style-* $bak_folder \
  && scp $host:$target_folder/leaflet-* $bak_folder \
  && echo "Done copying down..." \
  && scp -r $bak_folder $host:$target_folder/bak \
  && echo "Done copying up..." \
  && scp ./favicon* $host:$target_folder/ \
  && scp ./index.html $host:$target_folder/ \
  && scp ./territory-tools-component-* $host:$target_folder/ \
  && scp ./style-* $host:$target_folder/ \
  && scp ./app/leaflet-* $host:$target_folder/app/ \
  && echo "Done copying int target..."
  
echo "Done"

