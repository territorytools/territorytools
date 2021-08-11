# Introduction 
Territory Tools currently has the following 
- **Territory Web** is a web site that suppliments the Alba territory system with a few reports and a territory check-out page.
- **Alba Sync Tool** Windows app that downloads borders, addresses, and assignments so that they can be manipulated in csv or spreadsheet files.
- **Alba Console** Console version of **Alba Sync Tool** that can run in Windows, Linux, and macOS terminals and shells.
- **PowerShell Module** ![.NET Core Desktop](https://github.com/territorytools/territory-tools/workflows/.NET%20Core%20Desktop/badge.svg) Mostly the same tools as the Alba Sync Tool but including address parsing and normalizing tools.
# Fav Icon Generator

Use an SVG file and upload it to 
https://realfavicongenerator.net/
in order to generate all the fav icon files.

# Troubleshooting

## Error: No space left on device
You may get an error like this:
````
ERROR: Service 'tt-web-test' failed to build: Error processing tar file(exit status 1): write /src/Web/MainSite/bin/Release/netcoreapp3.1/System.Configuration.ConfigurationManager.dll: no space left on device
````

Try pruning unused docker images

````
docker image prune
````

Reference link: https://docs.docker.com/engine/reference/commandline/image_prune/