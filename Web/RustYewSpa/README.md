# Leaflet-rs for Territory Tools Maps

An example that shows how to use library to use Leaflet within
a [yew.rs](https://yew.rs) component.

## How to run the example

```
cargo install trunk
# in current directory, so: src/examples/yew-componenta
trunk serve
```

Then open http://localhost:8080

## Build
```bash
trunk build --release --public-url "/app/"
```

## Run with Prox

Latest Method running with YARP TestingProxy in front of this
```
trunk serve --port 2288 --public-url "/app/"
```

Older Method: Replace 192.168.1.111 with your local machines IP address
```
trunk serve --port 2288 --proxy-insecure --proxy-backend http://192.168.1.111:5577/api
```
Make sure your API server is bound to 0.0.0.0, or your local ip address in the Properties/launchsettings.json
I have started to bind to my auth (TerritoryTools MainSite) the
ReverseProxyMiddleware will take anything starting with /api/ and forward it
to http://localhost:5123
But the ReverseProxyMiddleware has to be hacked with a hard-coded security bypass.
```
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "Territory.Api": {
      "commandName": "Project",
      "launchBrowser": true,
      // launchUrl can also be: http://localhost:5577
      // since the launch Url is what the browser opens
      // and applicationUrl is what it is bound too
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "TimerIntervalSeconds": 0
      },
      "applicationUrl": "http://0.0.0.0:5577",
      // I think this can also be "applicationUrl": "http://192.168.1.111:5577",
      "dotnetRunMessages": true
    }
  }
}
```



## To build

```
trunk build --release
```

## Build with subfolder prefix
```
trunk build --release --public-url "/app/"
```

## Deploy Rust Files (And Back Up Folder)
This has been tested.  So if it doesn't work maybe there's a auth problem.
```
cd ./dist/
host=root@host.domain
folder=$(date +"%Y-%m-%d-%H%M%S")
mkdir $folder
scp $host:/var/www/tt-web-staging-wasm/* $folder
scp -r $folder $host:/var/www/tt-web-staging-wasm/bak/
scp ./* $host:/var/www/tt-web-staging-wasm/
```

## Notes
Check this out: (Source: https://docs.rs/yew/0.10.0/yew/services/fetch/struct.Request.html)

Deserialize a request of bytes via json:
```
use http::Request;
use serde::de;

fn deserialize<T>(req: Request<Vec<u8>>) -> serde_json::Result<Request<T>>
    where for<'de> T: de::Deserialize<'de>,
{
    let (parts, body) = req.into_parts();
    let body = serde_json::from_slice(&body)?;
    Ok(Request::from_parts(parts, body))
}
```

Or alternatively, serialize the body of a request to json
```
use http::Request;
use serde::ser;

fn serialize<T>(req: Request<T>) -> serde_json::Result<Request<Vec<u8>>>
    where T: ser::Serialize,
{
    let (parts, body) = req.into_parts();
    let body = serde_json::to_vec(&body)?;
    Ok(Request::from_parts(parts, body))
}
```

# Screen Shots
Take at 815x1147 DPR 1