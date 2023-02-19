use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::state_selector::SelectAddressState;
use crate::models::addresses::Address;
use crate::functions::document_functions::set_document_title;
use gloo_console::log;
use gloo_utils::document;
use reqwasm::http::{Request, Method};
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use web_sys::{EventTarget, HtmlImageElement, MouseEvent, HtmlInputElement, Element, HtmlElement};
use web_sys::Node;
use yew::prelude::*;
use yew_router::hooks::use_location;

#[cfg(debug_assertions)]
const GET_ADDRESSES_API_PATH: &str = "/data/get_address.json?id=";

#[cfg(not(debug_assertions))]
const GET_ADDRESSES_API_PATH: &str = "/api/addresses/alba-address-id";

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "PUT";

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct TestMapModel {
    pub save_error: bool,
    #[prop_or_default]
    pub load_error: bool,
    pub error_message: String,
}

#[derive(Clone, Debug, PartialEq, Deserialize)]
pub struct TestMapParameters {
    pub alba_address_id: Option<i32>,
}

#[function_component(TestMap)]
pub fn test_map() -> Html {
   
    let onmousedown = {
        
        Callback::from(move |_event: MouseEvent| {
            log!(format!("Mouse down"));
        })
    };

    drag_image("my-image");

    html! {
        <>
        <MenuBarV2>
            <ul class="navbar-nav ms-2 me-auto mb-05 mb-lg-0">
                <li class="nav-item">
                    <a class="nav-link active" aria-current="page" href="/app/address-search">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-search" viewBox="0 0 16 16">
                            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                        </svg>
                        <span class="ms-1">{"Search"}</span>
                    </a>
                </li> 
            </ul>
        </MenuBarV2>
        <div {onmousedown}>
            <span>{"Test Map"}</span>
            <div style="width:200px;height:200px;margin-left:-100px;">        
                //<img id="my-image" width="100" height="100" src="/data/test.png" />
            </div>
        </div>
        </>
    }
}

#[wasm_bindgen]
pub fn drag_image(image_id: &str) {
    let image = get_image_element(image_id);
    let parent = image.parent_element().unwrap();
    let offset_x = image.offset_left() as f64;
    let offset_y = image.offset_top() as f64;
    let mut is_dragging = false;
    let mut mouse_x = 0.0;
    let mut mouse_y = 0.0;

    let image_clone = image.clone();
    let handle_mouse_move = Closure::wrap(Box::new(move |event: MouseEvent| {
        if is_dragging {
            let dx = event.client_x() as f64 - mouse_x;
            let dy = event.client_y() as f64 - mouse_y;
            let left = offset_x + dx;
            let top = offset_y + dy;
            image_clone.style().set_property("left", &format!("{}px", left)).unwrap();
            image_clone.style().set_property("top", &format!("{}px", top)).unwrap();
            mouse_x = event.client_x() as f64;
            mouse_y = event.client_y() as f64;
        }
    }) as Box<dyn FnMut(_)>);

    let handle_mouse_down = Closure::wrap(Box::new(move |event: MouseEvent| {
        is_dragging = true;
        mouse_x = event.client_x() as f64;
        mouse_y = event.client_y() as f64;
    }) as Box<dyn FnMut(_)>);

    let handle_mouse_up = Closure::wrap(Box::new(move |_event: MouseEvent| {
        is_dragging = false;
    }) as Box<dyn FnMut(_)>);

    image.style().set_property("position", "absolute").unwrap();
    image.style().set_property("left", &format!("{}px", offset_x)).unwrap();
    image.style().set_property("top", &format!("{}px", offset_y)).unwrap();

    let parent_clone = parent.clone();
    EventTarget::from(parent_clone)
        .add_event_listener_with_callback("mousemove", handle_mouse_move.as_ref().unchecked_ref())
        .unwrap();
    let parent_clone = parent.clone();
    EventTarget::from(parent_clone)
        .add_event_listener_with_callback("mousedown", handle_mouse_down.as_ref().unchecked_ref())
        .unwrap();
    let parent_clone = parent.clone();
    EventTarget::from(parent_clone)
        .add_event_listener_with_callback("mouseup", handle_mouse_up.as_ref().unchecked_ref())
        .unwrap();

    handle_mouse_move.forget();
    handle_mouse_down.forget();
    handle_mouse_up.forget();
}

fn get_image_element(id: &str) -> HtmlElement { //HtmlImageElement {
    //let document = web_sys::window().unwrap().
    //let document = document().expect("A document");    

    ////let yew = document().get_element_by_id("yew").expect("An element named 'yew'");
    
    let div: Element = document().create_element("div").expect("Created a div");
    //yew.append_child(&div);
    
    let image: Element = document().create_element("img").expect("Created an img");

    image.set_attribute("src", "/data/data-test.png");

    let image_clone = image.clone();
    div.append_child(&image_clone);
    let image_clone = image.clone();
    //image_clone.dyn_into::<HtmlImageElement>().expect("Converted it to an HtmlImageElement")
    div.dyn_into::<HtmlElement>().expect("Converted into HtmlElement")

    
}

fn render_map_test(element: &HtmlElement) -> Html {
    // Element must be passed as an address I guess
        let node: &Node = &element.clone().into();
        Html::VRef(node.clone())
}