git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
docker-compose -f docker-compose.tt-web-test.yml build && docker-compose -f docker-compose.tt-web-test.yml up -d