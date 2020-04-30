git rev-parse HEAD > WebUI/wwwroot/commit.txt
docker-compose build tt-web-test && docker-compose up -d tt-web-test