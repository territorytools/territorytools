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
