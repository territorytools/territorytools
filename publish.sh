git rev-parse HEAD > WebUI/wwwroot/commit.txt
docker-compose build tt-web && docker-compose up -d tt-web