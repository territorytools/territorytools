git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
docker-compose -f docker-compose.tt-web-test.yml build tt-web-test \
    && git tag tt-web-test/`date +%Y-%m-%d-%H%M%S`
    && git push --tags
    && docker-compose -f docker-compose.tt-web-test.yml up -d tt-web-test