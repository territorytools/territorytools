git rev-parse HEAD > Web/MainSite/wwwroot/commit.txt
cp -f Web/favicon_package_red/* Web/MainSite/wwwroot
docker-compose build tt-web-test && docker-compose up -d tt-web-test