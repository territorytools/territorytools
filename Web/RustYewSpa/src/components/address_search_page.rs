#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/addresses_search.json";

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses/search";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "PUT";

// This is a good video: https://www.youtube.com/watch?v=2JNw-ftN6js
// This is the GitHub repo: https://github.com/brooks-builds/full-stack-todo-rust-course/blob/1d8acb28951d0a019558b2afc43650ae5a0e718c/frontend/rust/yew/solution/src/api/patch_task.rs

use crate::components::territory_edit_form::*;
use crate::components::menu_bar::MenuBar;
use crate::components::territory_edit_form::TerritoryEditForm;
use crate::components::{
  //  bb_button::BBButton,
    bb_text_input::{BBTextInput, InputType},
};
//use crate::components::route_stuff::Route;
use std::ops::Deref;
use gloo_console::log;
use reqwasm::http::{Request, Method};
use serde::Deserialize;
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use yew_router::hooks::use_location;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
//use yew_router::prelude::use_navigator;

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
    //let navigator = use_navigator().unwrap();
    
    let cloned_state = state.clone();
    //let navigator = navigator.clone();
    let onclick = Callback::from(move |_modification: MouseEvent| {
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            let cloned_state = cloned_state.clone();
            let search_text = cloned_state.search_text.clone();
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
        });
    });

    
    let search_text_onchange = {
        let state = state.clone();
        Callback::from(move |search_text: String| {
            let mut modification = state.deref().clone();
            modification.search_text = search_text;
            state.set(modification);
        })
    };

    let onchange = {
        //let emit_onchange = props.onchange.clone();
        let state = state.clone();
        Callback::from(move |event: Event| {
        //Callback::from(move |search_text: String| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();
            
            modification.search_text = value;
            //emit_onchange.emit(value.clone());
            state.set(modification);
        })
    };

    html! {
        <>
            <MenuBar/>
            <div class="container">
                <span>{"Search for an Address"}</span>
                <div class="d-flex flex-row">
                    <div class="d-flex flex-colum">
                            //<BBTextInput value={""} data_test="address_text" label="" placeholder="Enter part address" class="form-control" input_type={InputType::Text} 
                            <input type="text" value="" style="width:400px;" placeholder="Enter part address" class="form-control" 
                            {onchange}    />
                            //onchange={search_text_onchange}  />
                            
                            <button class={"btn btn-primary"} {onclick}>{"Search"}</button>
                    </div>
                </div>
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
                                    <div class="col-1">
                                        {address.territory_number.clone()}
                                    </div>
                                    <div class="col" style="font-weight:bold;">
                                        {address.name.clone()}
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-1">
                                        <small>{address.alba_address_id}</small>
                                    </div>
                                    <div class="col">
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
