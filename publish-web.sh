cd /root/production/TerritoryWeb
cp ../production.env ./.env
docker-compose build web
docker-compose up -d web
exit