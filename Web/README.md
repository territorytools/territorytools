
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

# Database README
[Web.Data README](./Data/README.md)