git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
docker-compose -f docker-compose.tt-web-a.yml build tt-web-a \
    && git tag tt-web-a/`date +%Y-%m-%d-%H%M%S`
    && git push --tags
    && docker-compose -f docker-compose.tt-web-a.yml up -d tt-web-a