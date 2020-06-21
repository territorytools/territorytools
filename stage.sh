git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
docker-compose build tt-web-test && docker-compose up -d tt-web-test