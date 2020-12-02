git rev-parse HEAD > Web/UrlShortener/wwwroot/commit.txt
docker-compose build tt-url-test && docker-compose up -d tt-url-test