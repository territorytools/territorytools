git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
docker-compose -f docker-compose.tt-web-b.yml build tt-web-b && docker-compose -f docker-compose.tt-web-b.yml up -d tt-web-b