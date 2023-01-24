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

## To build

```
trunk build --release
```

## Build with subfolder prefix
```
trunk build --release --public-url "/wasm/"
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