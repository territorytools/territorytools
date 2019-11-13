cd /root/TerritoryWeb
cp ../.env ./
docker-compose build web
docker-compose -d up web
exit