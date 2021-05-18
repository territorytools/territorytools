# Web.Data

# Add a Data Migration
From PowerShell:
````
  Add-Migration -Name AddAlbaAccountEtc
````

From Console:
````
  cd Web\Data
  dotnet ef migrations add AddAssignmentsHistory --startup-project ..\MainSite\Web.MainSite.csproj
````

Make sure that the Properties/launchSettings.json file has a 
connection string set in it

# Install EFCF Tools
````
    dotnet tool install --global dotnet-ef
````

