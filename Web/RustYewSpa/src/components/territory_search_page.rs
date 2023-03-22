#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-list.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/list";

use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::territories::TerritorySummary;
use crate::functions::document_functions::set_document_title;
use gloo_console::log;
use std::ops::Deref;
use reqwasm::http::{Request};
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct TerritorySearchPage {
    pub success: bool,
    //pub count: i32,
    pub search_text: String,
    pub territories: Vec<TerritorySummary>,
    pub load_error: bool,
    pub load_error_message: String,
}

#[function_component(TerritorySearch)]
pub fn address_search_page() -> Html {        
    let state = use_state(|| TerritorySearchPage::default());

    set_document_title("Territory Search");

    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |event: SubmitEvent| {
        event.prevent_default();
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            let cloned_state = cloned_state.clone();
            let search_text = cloned_state.search_text.clone();
            //if !search_text.is_empty() {
                // TODO: Clear search results if nothing is returned
                // TODO: Leave search text in the search box?
                let result = get_territories(search_text).await;
                cloned_state.set(result);
           // }
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
                <span><strong>{"Territory Search"}</strong></span>
      
                <hr/>
                <form {onsubmit} >
                <div class="d-flex flex-row">
                    <div class="d-flex flex-colum mb-2 shadow-sm">
                        <input {onchange} type="text" value="" style="max-width:400px;" placeholder="Enter search text" class="form-control" />
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
                        <span class="ms-2 badge mb-2 bg-secondary">{"Number"}</span> 
                        <span class="ms-2 badge mb-2 bg-secondary">{"Description"}</span> 
                        <span class="ms-2 badge mb-2 bg-secondary">{"Status"}</span> 
                    </div>
                </div>
                {
                    state.territories.iter().map(|territory| {   
                        let territory_id: i32 = territory.id.unwrap_or_default();
                        let edit_uri = format!("/app/territory-edit?id={territory_id:?}");
    
                        html! {
                            <a href={edit_uri} style="text-decoration:none;color:black;">
                                <div class="row" style="border-top: 1px solid gray;">
                                    <div class="col-2 col-md-1" style="font-weight:bold;">
                                        {territory.number.clone()}
                                    </div>
                                    <div class="col-5 col-md-3">
                                        {territory.description.clone()}
                                    </div>
                                    <div class="col-2 col-md-2">
                                        if territory.status.clone() == Some("Available".to_string()) {
                                            <span class="ms-2 badge bg-success">{territory.status.clone()}</span> 
                                        } else if territory.status.clone() == Some("Out".to_string()) {
                                            <span class="ms-2 badge bg-warning">{territory.status.clone()}</span> 
                                        } else if territory.status.clone() == Some("Removed".to_string()) {
                                            <span class="ms-2 badge bg-danger">{territory.status.clone()}</span> 
                                        } else if territory.status.clone() == Some("Done".to_string()) {
                                            <span class="ms-2 badge bg-info">{territory.status.clone()}</span> 
                                        } else {
                                            <span class="ms-2 badge bg-dark">{territory.status.clone()}</span> 
                                        }
                                    </div>
                                    <div class="col-5 col-md-3">
                                        {territory.publisher.clone()}
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

async fn get_territories(search_text: String) -> TerritorySearchPage {
    let uri_string: String = format!("{path}?filter={search_text}", path = DATA_API_PATH);
    let uri: &str = uri_string.as_str();
    let resp = Request::get(uri)
        .header("Content-Type", "application/json")
        .send()
        .await
        .expect("A result from the /api/territories/list endpoint");
    
    log!(format!("load territory from search result code: {}", resp.status().to_string()));

    let territory_result: Vec<TerritorySummary> = if resp.status() == 200 {
        resp
        .json()
        .await
        .expect("Valid territory search result in JSON format")
    } else {
        vec![]
    };
    
    let result = TerritorySearchPage {
        success: (resp.status() == 200),
        territories: territory_result,
        search_text: "".to_string(),
        load_error: resp.status() != 200,
        load_error_message: if resp.status() == 401 {
                "Unauthorized".to_string()
            } else if resp.status() == 403 {
                "Forbidden".to_string()
            } else {
                format!("Error {:?}", resp.status())
            }
    };

    result
}