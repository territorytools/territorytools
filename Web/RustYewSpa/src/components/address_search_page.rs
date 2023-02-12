#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/addresses_search.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses/search";

use crate::components::menu_bar::MenuBar;
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

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Address {
    pub alba_address_id: i32,
    pub territory_number: Option<String>,
    pub name: Option<String>,
    pub street: Option<String>,
    pub city: Option<String>,
    pub postal_code: Option<String>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressSearchPage {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub addresses: Vec<Address>,
}

#[function_component(AddressSearch)]
pub fn address_search_page() -> Html {        
    let state = use_state(|| AddressSearchPage::default());
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
                
                let address_result: AddressSearchResults = resp
                    .json()
                    .await
                    .unwrap();
                
                let result = AddressSearchPage {
                    success: (resp.status() == 200),
                    count: address_result.count,
                    addresses: address_result.addresses,
                    search_text: "something".to_string(),
                };

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
            <MenuBar/>
            <div class="container">
                <span>{"Search for an Address"}</span>
                <form {onsubmit} >
                <div class="d-flex flex-row">
                    <div class="d-flex flex-colum">
                        <input {onchange} type="text" value="" style="max-width:400px;" placeholder="Enter part address" class="form-control" />
                        <button type="submit" class={"btn btn-primary"}>{"Search"}</button>
                    </div>
                </div>
                </form>
                <div class="row">
                    <div class="col">
                        <span>{"Count: "}{state.count}</span>
                    </div>
                </div>
                {
                    state.addresses.iter().map(|address| {   
                        html! {
                            <>
                                <div class="row" style="border-top: 1px solid gray;">
                                    <div class="col-2 col-md-1">
                                        {address.territory_number.clone()}
                                    </div>
                                    <div class="col-10 col-md-11" style="font-weight:bold;">
                                        {address.name.clone()}
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-2 col-md-1">
                                        <small>{address.alba_address_id}</small>
                                    </div>
                                    <div class="col-10 col-md-11">
                                        {address.street.clone()}
                                        {", "}
                                        {address.city.clone()}
                                        {", "}
                                        {address.postal_code.clone()}
                                    </div>
                                </div>
                            </>
                        }
                    }).collect::<Html>()
                }
            </div>
        </>
    }
}
