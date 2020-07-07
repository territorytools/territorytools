git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
docker-compose build tt-web && docker-compose up -d tt-web