use gloo_console::log;
use web_sys::HtmlElement;
use wasm_bindgen::JsCast;
use yew::prelude::*;

#[function_component(CanvasMap)]
pub fn canvas_map() -> Html {
    html! {
        <>
        <p>{"Canvas Map Here"}</p>
        </>
    }
}