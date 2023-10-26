
host=$1
slot=$2
if [ -z "$host" ] || [ -z "$slot" ]
then
  echo "Usage:"
  echo "  ./deploy-wasm-slot.sh user@host.name.tld |target-slot (a/b)|"
  exit
fi
  
bak_folder=$(date +"%Y-%m-%d-%H%M%S")
target_folder=/var/www/tt-spa-slot-${slot}/wasm

echo "Target Host: $host"
echo "Target Slot: $slot"
echo "Folder: $bak_folder"
echo "Target Folder: $target_folder"

cd ./dist \
  && mkdir $bak_folder \
  && echo "Done making new folder..." \
  && scp $host:$target_folder/favicon* $bak_folder \
  && echo "Done copying favicon..." \
  && scp $host:$target_folder/index.html $bak_folder \
  && echo "Done copying index.html..." \
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

