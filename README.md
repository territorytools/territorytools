# Introduction 
Territory Tools currently has the following 
- **Territory Web** is a web site that suppliments the Alba territory system with a few reports and a territory check-out page.
- **Alba Sync Tool** Windows app that downloads borders, addresses, and assignments so that they can be manipulated in csv or spreadsheet files.
- **Alba Console** Console version of **Alba Sync Tool** that can run in Windows, Linux, and macOS terminals and shells.
- **PowerShell Module** ![.NET Core Desktop](https://github.com/territorytools/territory-tools/workflows/.NET%20Core%20Desktop/badge.svg) Mostly the same tools as the Alba Sync Tool but including address parsing and normalizing tools.

# Compressing The Vault Folder
Location is data/vault
Command:
    
    tar -zcvf archive-name.tar.gz directory-name

# Getting secrets
This is an example path:
https://territory.bellevuemandarin.org:8200/v1/kv/data/territory-web/alba-service-accounts/youraccountname

# Getting Started
To run the staging environment, run the following command from a production folder:

    cd Production-TerritoryWeb
    docker-compose --project-name Production-TerritoryWeb up web-staging 

## Staging Site
To run the staging environment, run the following command from a separate folder:

    cd Staging-TerritoryWeb
    docker-compose up web-staging 

TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Set Up Google Auth
(You'll have to have a App set up already)
Go to https://console.developers.google.com

Click 'OAuth consent screen'

Click Credentials

Under OAuth 2.0 Client IDs find your app

Under 'URIS' add the URI for your site

Or you may be able to just find it here  
https://console.developers.google.com/apis/credentials/oauthclient

# Add a Data Migration
From PowerShell:
````
  Add-Migration -Name AddAlbaAccountEtc
````
Make sure that the Properties/launchSettings.json file has a 
connection string set in it

