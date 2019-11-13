cd /root/TerritoryWeb
cp ../.env ./
docker-compose build web
docker-compose up web -d
exit