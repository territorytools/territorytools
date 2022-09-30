git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
docker-compose -f docker-compose.tt-web-b.yml build tt-web-b \
    && git tag tt-web-b/`date +%Y-%m-%d-%H%M%S`
    && git push --tags
    && docker-compose -f docker-compose.tt-web-b.yml up -d tt-web-b