git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
docker-compose build tt-web-a && docker-compose up -d tt-web-a