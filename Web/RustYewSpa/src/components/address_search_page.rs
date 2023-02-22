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
                .unwrap()
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
                        html! {
                            <a href={edit_uri} style="text-decoration:none;color:black;">
                                <div class="row" style="border-top: 1px solid gray;">
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
