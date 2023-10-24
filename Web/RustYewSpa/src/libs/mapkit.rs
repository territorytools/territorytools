//use js_sys::Object;
use wasm_bindgen::prelude::*;
//use web_sys::HtmlElement;

#[wasm_bindgen]
extern "C" {

    // Apple mapkit ...just and idea at the moment
    #[allow(non_camel_case_types)]
    #[derive(Debug)]
    pub type mapkit;

    #[wasm_bindgen(method)]
    pub fn init(this: &mapkit, options: &JsValue);

    // MapKitInitOptions

    #[derive(Debug, Clone)]
    pub type MapKitInitOptions;

    //#[wasm_bindgen(constructor, js_namespace = L)]
    //pub fn new(options: &JsValue) -> Icon;
}