cd /root/staging/TerritoryWeb
cp ../staging.env ./.env
docker-compose build web-staging
docker-compose up -d web-staging
exit