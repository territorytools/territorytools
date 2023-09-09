#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-list.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/list";

use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::territories::TerritorySummary;
use crate::functions::document_functions::set_document_title;
use crate::Route;

use gloo_console::log;
use std::ops::Deref;
use reqwasm::http::{Request};
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use yew_router::hooks::use_location;
use yew_router::prelude::use_navigator;
use yew_router::prelude::LocationHandle;
use yew_router::scope_ext::RouterScopeExt;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use serde::{Serialize, Deserialize};

pub enum Msg {
    Load(TerritorySearchResult),
    RefreshFromSearchText(),
}

pub struct TerritorySearchPage {
    _listener: LocationHandle,
    territories: Vec<TerritorySummary>,
    result: TerritorySearchResult,
}

impl Component for TerritorySearchPage {
    type Message = Msg;
    type Properties = ();
    
    fn create(ctx: &Context<Self>) -> Self {
        let link = ctx.link().clone();      
        let listener = ctx.link()
            .add_location_listener(
                Callback::from(move |_| {
                    link.send_message(Msg::RefreshFromSearchText());
                })
            )
            .unwrap();

        return Self {
            _listener: listener,
            territories: vec![],
            result: TerritorySearchResult::default(),
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.territories = result.territories.clone();
                true
            },
            Msg::RefreshFromSearchText() => {
                let search_text = ctx.search_query().search_text.clone().unwrap_or_default();  
                ctx.link().send_future(async move {
                    Msg::Load(get_territories(search_text.clone()).await)
                });
                false
            }
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        //set_document_title("Territory Search");
        
        let onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            // If we don't prevent_default() it will clear the box and search again
        });

        let navigator = ctx.link().navigator().unwrap();
        let onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = TerritorySearchQuery {
                    search_text: Some(value.clone()),
                };
                let _ = navigator.push_with_query(&Route::TerritorySearch, &query);
            })
        };
    
        let count = self.territories.len();
        let search_text = ctx.search_query().search_text.clone().unwrap_or_default();  

        html!{
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
                                <input {onchange} type="text" value={search_text} style="max-width:400px;" placeholder="Enter search text" class="form-control" />
                                // TODO: Add an onclick and send the RefreshFromSearchText() message?
                                <button type="submit" class="btn btn-primary">{"Search"}</button>
                                if self.result.load_error { 
                                    <span class="mx-1 badge bg-danger">{"Error"}</span> 
                                    <span class="mx-1" style="color:red;">{self.result.load_error_message.clone()}</span>
                                }    
                            </div>
                        </div>
                    </form>
                    <div class="row">
                        <div class="col">
                            <span><strong>{count}</strong>{" Territories Found"}</span>
                        </div>
                    </div>
                    <div class="row py-1" style="border-top: 1px solid gray;">
                        <div class="col-2 col-md-1"><strong>{"#"}</strong></div>
                        <div class="col-6 col-md-3"><strong>{"Description"}</strong></div>
                        <div class="col-4 col-md-2"><strong>{"Status"}</strong></div>
                        <div class="col-8 col-md-3"><strong>{"Publisher"}</strong></div>
                        <div class="col-4 col-md-2"><strong>{"Date"}</strong></div>
                    </div>
                    {
                        self.territories.iter().map(|territory| {   
                            let territory_id: i32 = territory.id.unwrap_or_default();
                            let edit_uri = format!("/app/territory-edit?id={territory_id:?}");        
                            html! {
                                 <a href={edit_uri} style="text-decoration:none;color:black;">
                                     <div class="row py-1" style="border-top: 1px solid lightgray;">
                                         <div class="col-2 col-md-1" style="font-weight:bold;">
                                             {territory.number.clone()}
                                         </div>
                                         <div class="col-6 col-md-3">
                                             {territory.description.clone()}
                                         </div>
                                         <div class="col-4 col-md-2">
                                             <span class="badge" style="border-radius:3px;border-width:1px;border-style:solid;border-color:green;color:black;">
                                                 {territory.stage.clone()}
                                             </span>
                                             <span style="ming-width:5px;">{" / "}</span>
                                            if territory.status.clone() == Some("Available".to_string()) {
                                                <span class="badge" style="background-color:green">{territory.status.clone()}</span> 
                                            } else if territory.status.clone() == Some("Out".to_string()) {
                                                <span class="badge" style="background-color:magenta">{territory.status.clone()}</span> 
                                            } else if territory.status.clone() == Some("Removed".to_string()) {
                                                <span class="badge" style="background-color:black">{territory.status.clone()}</span> 
                                            } else if territory.status.clone() == Some("Done".to_string()) {
                                                <span class="badge" style="background-color:blue">{territory.status.clone()}</span> 
                                            } else {
                                                <span class="badge" style="background-color:gray">{territory.status.clone()}</span> 
                                            }
                                        </div>
                                        <div class="col-8 col-md-3">
                                            {territory.publisher.clone()}
                                        </div>
                                        <div class="col-4 col-md-2">
                                            {territory.status_date.clone()}
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
}

pub trait SearchQuery {
    fn search_query(&self) -> TerritorySearchQuery;
}

impl SearchQuery for &Context<TerritorySearchPage> {
    fn search_query(&self) -> TerritorySearchQuery {
        let location = self.link().location().expect("Location or URI");
        location.query().unwrap_or(TerritorySearchQuery::default())    
    }
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct TerritorySearchResult {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub territories: Vec<TerritorySummary>,
    pub load_error: bool,
    pub load_error_message: String,
}

async fn get_territories(search_text: String) -> TerritorySearchResult {
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
    
    let result = TerritorySearchResult {
        success: (resp.status() == 200),
        count: territory_result.len() as i32,
        territories: territory_result,
        search_text: search_text.clone(),
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

#[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
pub struct TerritorySearchQuery {
    pub search_text: Option<String>,
}