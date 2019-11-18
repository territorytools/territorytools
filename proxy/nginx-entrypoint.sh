#!/bin/bash

export DOLLAR='$'
envsubst < /app/nginx.template.conf > /data/nginx.conf ;
if [ -s /data/fullchain.pem ] ;
then echo 'Certificate exists, no need to create a self-signed certificate.' ;
else echo 'No certificate found.  Generating temporary self signed certifcate...' ;
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout /data/privkey.pem -out /data/fullchain.pem -subj '/C=US/ST=Washington/L=Seattle/O=Temporary/CN=$domain' ;
fi ;
echo 'Starting NGINX and reload loops...' ;
while :; 
do sleep 4 & wait ${!}; 
echo 'Waiting for certificate copy file from cerbot...' ;
    nginx -s reload -c /data/nginx.conf ;
    if [ -s /data/certbot_certificates_copied.txt ]; 
    then echo 'Certificate copy file detected, stop checking...' ; 
        break ; 
    fi ; 
done && echo 'Starting 6 hour configuration reload loop...' && while :; 
do sleep 6h & wait ${!}; 
    echo 'Reloading configuration files...' ;
    nginx -s reload -c /data/nginx.conf ; 
    echo 'Sleeping for 6 hours...' ;
done & nginx -c /data/nginx.conf -g 'daemon off;'  