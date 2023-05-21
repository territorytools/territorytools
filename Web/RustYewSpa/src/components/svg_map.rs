#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/addresses_search.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses/search";

//use crate::components::menu_bar::MenuBar;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::addresses::Address;
use crate::functions::document_functions::set_document_title;
use gloo_console::log;
use std::ops::Deref;
use reqwasm::http::{Request};
use serde::Deserialize;
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use web_sys::HtmlElement;

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct AddressSearchResults {
    pub count: i32,
    pub addresses: Vec<Address>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressSearchPage {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub addresses: Vec<Address>,
    pub load_error: bool,
    pub load_error_message: String,
}

#[function_component(AddressSearch)]
pub fn address_search_page() -> Html {        
    let state = use_state(|| AddressSearchPage::default());

    set_document_title("Address Search");

    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |event: SubmitEvent| {
        event.prevent_default();
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            let cloned_state = cloned_state.clone();
            let search_text = cloned_state.search_text.clone();
            if !search_text.is_empty() {
                let uri_string: String = format!("{path}?text={search_text}", path = DATA_API_PATH);
                let uri: &str = uri_string.as_str();
                let resp = Request::get(uri)
                    .header("Content-Type", "application/json")
                    .send()
                    .await
                    .expect("A result from the /api/addresses/search endpoint");
                
                log!(format!("load address from search result code: {}", resp.status().to_string()));

                let address_result: AddressSearchResults = if resp.status() == 200 {
                    resp
                    .json()
                    .await
                    .expect("Valid address search result in JSON format")
                } else {
                    AddressSearchResults {
                        count: 0,
                        addresses: vec![],
                        
                    }
                };
                
                let result = AddressSearchPage {
                    success: (resp.status() == 200),
                    count: address_result.count,
                    addresses: address_result.addresses,
                    search_text: "".to_string(),
                    load_error: resp.status() != 200,
                    load_error_message: if resp.status() == 401 {
                            "Unauthorized".to_string()
                        } else if resp.status() == 403 {
                            "Forbidden".to_string()
                        } else {
                            format!("Error {:?}", resp.status())
                        }
                    // search_text: "something".to_string(),
                    // load_error: (resp.status() != 200),
                    // load_error_message: c == 401 {
                    //     "Unauthorized".to_string()
                    // } else { "".to_string() },
                };
                // TODO: Clear search results if nothing is returned
                // TODO: Leave search text in the search box?
                cloned_state.set(result);
            }
        });
    });

    let onchange = {
        let state = state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .expect("An input value for an HtmlInputElement")
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.search_text = value;
            state.set(modification);
        })
    };

    html! {
        <>
            <MenuBarV2>
                <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                    <li class={"nav-item"}>
                        <MapPageLink />
                    </li>  
                </ul>
            </MenuBarV2>
            <div class="container">
                <span><strong>{"Address Search"}</strong></span>
                <TestSvg/>
                <hr/>
                <form {onsubmit} >
                <div class="d-flex flex-row">
                    <div class="d-flex flex-colum mb-2 shadow-sm">
                        <input {onchange} type="text" value="" style="max-width:400px;" placeholder="Enter part of address" class="form-control" />
                        <button type="submit" class="btn btn-primary">{"Search"}</button>
                        if state.load_error { 
                            <span class="mx-1 badge bg-danger">{"Error"}</span> 
                            <span class="mx-1" style="color:red;">{state.load_error_message.clone()}</span>
                        }    
                    </div>
                </div>
                </form>
                <div class="row">
                    <div class="col">
                        <span>{"Count: "}{state.count}</span>
                        <span class="ms-2 badge mb-2 bg-secondary">{"Language"}</span> 
                        <span class="ms-2 badge mb-2 bg-secondary">{"Visit Status"}</span> 
                        <span class="ms-2 badge mb-2 bg-secondary">{"Mail Status"}</span> 
                    </div>
                </div>
                {
                    state.addresses.iter().map(|address| {   
                        let alba_address_id = address.alba_address_id;
                        let edit_uri = format!("/app/address-edit?alba_address_id={alba_address_id}");
                        let unit_text: String = match &address.unit {
                            Some(v) => if v == "" { "".to_string() } else { format!(", {}", v.clone()) },
                            None => "".to_string()
                        };
    
                        html! {
                            <a href={edit_uri} style="text-decoration:none;color:black;">
                                <div class="row" style="border-top: 1px solid lightgray;">
                                    <div class="col-2 col-md-1">
                                        {address.territory_number.clone()}
                                    </div>
                                    <div class="col-10 col-md-11" style="font-weight:bold;">
                                        {address.name.clone()}
                                        <span class="ms-2 badge bg-secondary">{address.language.clone()}</span> 
                                        if address.status.clone() == Some("New".to_string()) {
                                            <span class="ms-2 badge bg-info">{address.status.clone()}</span> 
                                        } else if address.status.clone() == Some("Valid".to_string()) {
                                            <span class="ms-2 badge bg-success">{address.status.clone()}</span> 
                                        } else if address.status.clone() == Some("Do not call".to_string()) {
                                            <span class="ms-2 badge bg-danger">{address.status.clone()}</span> 
                                        } else if address.status.clone() == Some("Moved".to_string()) {
                                            <span class="ms-2 badge bg-warning">{address.status.clone()}</span> 
                                        } else {
                                            <span class="ms-2 badge bg-dark">{address.status.clone()}</span> 
                                        }
                                        if address.delivery_status.clone() == Some("None".to_string()) {
                                            <span class="ms-2 badge bg-secondary">{address.delivery_status.clone()}</span> 
                                        } else if address.delivery_status.clone() == Some("Assigned".to_string()) {
                                            <span class="ms-2 badge bg-info">{address.delivery_status.clone()}</span> 
                                        } else if address.delivery_status.clone() == Some("Sent".to_string()) {
                                            <span class="ms-2 badge bg-success">{address.delivery_status.clone()}</span> 
                                        } else if address.delivery_status.clone() == Some("Returned".to_string()) {
                                            <span class="ms-2 badge bg-warning">{address.delivery_status.clone()}</span> 
                                        } else if address.delivery_status.clone() == Some("Undeliverable".to_string()) {
                                            <span class="ms-2 badge bg-warning">{address.delivery_status.clone()}</span> 
                                        } else {
                                            <span class="ms-2 badge bg-dark">{address.delivery_status.clone()}</span> 
                                        }
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-2 col-md-1">
                                        <small style="color:lightgray;">{address.alba_address_id}</small>
                                    </div>
                                    <div class="col-10 col-md-11">
                                        {address.street.clone()}
                                        {unit_text}
                                        {", "}
                                        {address.city.clone()}
                                        {", "}
                                        {address.postal_code.clone()}
                                    </div>
                                </div>
                            </a>
                        }
                    }).collect::<Html>()
                }
            </div>
        </>
    }
}

#[derive(Properties, PartialEq, Clone, Default)] //, Serialize)]
pub struct TestSvgModel {
    pub mx: f64,
    pub my: f64,
    pub mouse_down: bool,
    pub pane_x: f64,
    pub pane_y: f64,
    pub start_pan_x: f64,
    pub start_pan_y: f64,
    pub pane_start_x: f64,
    pub pane_start_y: f64,
    pub x_delta: f64,
    pub y_delta: f64,
    pub wheel_delta: f64,
    pub zoom: f64,
}


#[function_component(TestSvg)]
pub fn test_svg() -> Html { 
    let state: yew::UseStateHandle<TestSvgModel> = use_state(|| TestSvgModel { 
        mx: 0.0, 
        my: 0.0, 
        mouse_down: false,
        pane_x: 0.0, 
        pane_y: 0.0, 
        pane_start_x: 0.0, 
        pane_start_y: 0.0, 
        start_pan_x: 0.0, 
        start_pan_y: 0.0, 
        x_delta: 0.0, 
        y_delta: 0.0, 
        wheel_delta: 0.0, 
        zoom: 1.0, 
    });
    let cloned_state = state.clone();
    let onmousemove = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.client_x() as f64) - rect.left();
            let y = (e.client_y() as f64) - rect.top();
            log!(format!("Left? : {} ; Top? : {}", x, y));
            if cloned_state.mouse_down {
                let x_delta = cloned_state.mx - x;
                let y_delta = cloned_state.my - y;
                cloned_state.set(TestSvgModel { 
                    mx: x, 
                    my: y,
                    mouse_down: cloned_state.mouse_down,
                    pane_x: cloned_state.pane_x - x_delta,
                    pane_y: cloned_state.pane_y - y_delta,                    
                    pane_start_x: cloned_state.pane_start_x,
                    pane_start_y: cloned_state.pane_start_y,
                    start_pan_x: cloned_state.start_pan_x,
                    start_pan_y: cloned_state.start_pan_y,
                    x_delta: x_delta,
                    y_delta: y_delta,
                    wheel_delta: cloned_state.wheel_delta,
                    zoom: cloned_state.zoom,
                });
            } else {
                cloned_state.set(TestSvgModel { 
                    mx: x, 
                    my: y,
                    mouse_down: cloned_state.mouse_down,
                    pane_x: cloned_state.pane_x,
                    pane_y: cloned_state.pane_y,
                    pane_start_x: cloned_state.pane_start_x,
                    pane_start_y: cloned_state.pane_start_y,
                    start_pan_x: 0.0,
                    start_pan_y: 0.0,
                    x_delta: 0.0,
                    y_delta: 0.0,
                    wheel_delta: cloned_state.wheel_delta,
                    zoom: cloned_state.zoom,
                });
            }
        }
    });

    let cloned_state = state.clone();
    let ontouchstart = Callback::from(move |e: TouchEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.touches().item(0).expect("One touch object").client_x() as f64) - rect.left();
            let y = (e.touches().item(0).expect("One touch object").client_y() as f64) - rect.top();
            //log!(format!("Left? : {} ; Top? : {}", x, y));
                cloned_state.set(TestSvgModel { 
                    mx: x, 
                    my: y,
                    mouse_down: cloned_state.mouse_down,
                    pane_x: cloned_state.pane_x,
                    pane_y: cloned_state.pane_y,                    
                    pane_start_x: cloned_state.pane_start_x,
                    pane_start_y: cloned_state.pane_start_y,
                    start_pan_x: cloned_state.start_pan_x,
                    start_pan_y: cloned_state.start_pan_y,
                    x_delta: cloned_state.x_delta,
                    y_delta: cloned_state.y_delta,
                    wheel_delta: cloned_state.wheel_delta,
                    zoom: cloned_state.zoom,
                });
            
        }
    });

    let cloned_state = state.clone();
    let ontouchmove = Callback::from(move |e: TouchEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.touches().item(0).expect("One touch object").client_x() as f64) - rect.left();
            let y = (e.touches().item(0).expect("One touch object").client_y() as f64) - rect.top();
            log!(format!("Left? : {} ; Top? : {}", x, y));
                let x_delta = cloned_state.mx - x;
                let y_delta = cloned_state.my - y;
                cloned_state.set(TestSvgModel { 
                    mx: x, 
                    my: y,
                    mouse_down: cloned_state.mouse_down,
                    pane_x: cloned_state.pane_x - x_delta,
                    pane_y: cloned_state.pane_y - y_delta,                    
                    pane_start_x: cloned_state.pane_start_x,
                    pane_start_y: cloned_state.pane_start_y,
                    start_pan_x: cloned_state.start_pan_x,
                    start_pan_y: cloned_state.start_pan_y,
                    x_delta: cloned_state.x_delta,
                    y_delta: cloned_state.y_delta,
                    wheel_delta: cloned_state.wheel_delta,
                    zoom: cloned_state.zoom,
                });
            
        }
    });

    let cloned_state = state.clone();
    let onmousedown = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.client_x() as f64) - rect.left();
            let y = (e.client_y() as f64) - rect.top();
            // let x_delta = cloned_state.start_pan_x - cloned_state.mx;
            // let y_delta = cloned_state.start_pan_y - cloned_state.my;
            cloned_state.set(TestSvgModel { 
                mx: x, //cloned_state.mx,
                my: y, //cloned_state.my, 
                mouse_down: true,
                pane_x: cloned_state.pane_x,
                pane_y: cloned_state.pane_y,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,                
                start_pan_x: cloned_state.mx,
                start_pan_y: cloned_state.my,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
                zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let onmouseup = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {

            // let x_delta = cloned_state.start_pan_x - cloned_state.mx;
            // let y_delta = cloned_state.start_pan_y - cloned_state.my;

           cloned_state.set(TestSvgModel { 
                mx: cloned_state.mx,
                my: cloned_state.my, 
                mouse_down: false,
                pane_x: cloned_state.pane_x, // - x_delta,
                pane_y: cloned_state.pane_y, // - y_delta,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: 0.0,
                start_pan_y: 0.0,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
                zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let onmouseleave = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {

            // let x_delta = cloned_state.start_pan_x - cloned_state.mx;
            // let y_delta = cloned_state.start_pan_y - cloned_state.my;

           cloned_state.set(TestSvgModel { 
                mx: cloned_state.mx,
                my: cloned_state.my, 
                mouse_down: false,
                pane_x: cloned_state.pane_x, // - x_delta,
                pane_y: cloned_state.pane_y, // - y_delta,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: 0.0,
                start_pan_y: 0.0,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
                zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let onwheel = Callback::from(move |e: WheelEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {

            let wheel_y = e.delta_y() as f64;
            //let mut wheel_delta = cloned_state.wheel_delta + wheel_y;
            let mut zoom = cloned_state.zoom + (wheel_y / 300.0);
            //if wheel_delta > 1000.0 {
               // zoom = cloned_state.zoom + 1.0;
                //wheel_delta = 0.0;
            //} else if wheel_delta < -1000.0 {
            //    zoom = cloned_state.zoom - 1.0;
                //wheel_delta = 0.0;
            //};

            if zoom > 12.0 { zoom = 12.0 };
            if zoom < 0.1 { zoom = 0.1 };

            cloned_state.set(TestSvgModel { 
                mx: cloned_state.mx,
                my: cloned_state.my, 
                mouse_down: false,
                pane_x: cloned_state.pane_x, // - x_delta,
                pane_y: cloned_state.pane_y, // - y_delta,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: cloned_state.start_pan_x,
                start_pan_y: cloned_state.start_pan_y,
                x_delta: cloned_state.x_delta,
                y_delta: cloned_state.y_delta,
                wheel_delta: cloned_state.wheel_delta,
                zoom: zoom,
            });
        }
    });

    let cloned_state = state.clone();
    //{"width:auto;background-color:red;transform: translate3d(" + cloned_state.mx + "px, " +  cloned_state.my + "px, 0px);"} 

    let mouse_x = cloned_state.mx;
    let mouse_y = cloned_state.my;
    let pane_x = cloned_state.pane_x;
    let pane_y = cloned_state.pane_y;

    html!{
        <div>
        <span>{cloned_state.mx}{", "}{cloned_state.my}{" Mouse Down:"}{cloned_state.mouse_down}</span><br/>
        <span>{"Pane: "}{cloned_state.pane_x}{", "}{cloned_state.pane_y}</span><br/>
        <span>{"Start: "}{cloned_state.start_pan_x}{", "}{cloned_state.start_pan_y}</span><br/>
        <span>{"Delta: "}{cloned_state.x_delta}{", "}{cloned_state.y_delta}{" Wheel: "}{cloned_state.wheel_delta}</span>
        

        <div {onmousemove} {onmousedown} {onmouseup} {onmouseleave} {onwheel} {ontouchmove} {ontouchstart}
            style="width:500px;height:500px;background-color:gray;overflow:hidden;">
        <div style={format!(" pointer-events: none;width:auto;background-color:red;transform: translate3d({}px, {}px, 0px) scale({}, {});", pane_x, pane_y, cloned_state.zoom, cloned_state.zoom)} >
        
        //<svg width="149" height="147" viewBox="0 0 149 147" fill="none" xmlns="http://www.w3.org/2000/svg">
        <svg width="700" height="1024" viewBox="0 0 700 512" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M60.5776 13.8268L51.8673 42.6431L77.7475 37.331L60.5776 13.8268Z" fill="#DEB819"/>
            <path d="M108.361 94.9937L138.708 90.686L115.342 69.8642" stroke="black" stroke-width="4" stroke-linecap="round" stroke-linejoin="round"/>
            <g>
                <circle cx="75.3326" cy="73.4918" r="55" fill="#FDD630"/>
                <circle cx="75.3326" cy="73.4918" r="52.5" stroke="black" stroke-width="5"/>
            </g>
            <circle cx="71" cy="99" r="5" fill="white" fill-opacity="0.75" stroke="black" stroke-width="3"/>
            <image x="200" y="100"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/329/711.png"
                />
            <image x="200" y="356"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/329/712.png"
                />
                <image x="200" y="612"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/329/713.png"
                />
        </svg>
        </div>
        </div>
        </div>
    }
} 