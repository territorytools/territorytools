# Debugging a Docker Container in Visual Studio 2019
Add this to the launchSettings.json profiles array:

````
  "profiles": [
    ...
    "Docker": {
      "commandName": "Docker",
      "launchBrowser": true,
      "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}",
      "publishAllPorts": true,
      "useSSL": true
    }
  ]
````