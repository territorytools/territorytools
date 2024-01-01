const DATA_API_PATH: &str = "/api/territories/list";

use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::functions::document_functions::set_document_title;
use crate::models::territories::TerritorySummary;
use crate::modals::unauthorized::UnauthorizedModal;
use crate::Route;

use gloo_console::log;
use reqwasm::http::Request;
use yew::prelude::*;
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
    show_unauthorized_modal: bool,
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

        Self {
            _listener: listener,
            territories: vec![],
            result: TerritorySearchResult::default(),
            show_unauthorized_modal: false,
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.territories = result.territories.clone();
                self.show_unauthorized_modal = result.load_error_message == "Unauthorized";
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
        set_document_title("Territories");
        
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
        let return_url = format!("%2Fapp%2Fterritory-search%3Fsearch_text%3D{}", search_text.clone());

        html!{
            <>
                <script>{"
                    var x = 1;
                    console.log(\"This is Javascript!\");
                    const myElement = document.getElementById(\"territory-title-span\");
                    myElement.style.color = \"black\";
                "}</script>
                <MenuBarV2>
                    <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                        <li class={"nav-item"}>
                            <MapPageLink />
                        </li>  
                    </ul>
                </MenuBarV2>
                <div class="container pt-3">
                    <span><strong id="territory-title-span">{"Territory Search"}</strong></span>
    
                    <hr/>
                    <form id="search-form" {onsubmit} >
                        <div class="d-flex flex-row">
                            <div class="d-flex flex-colum mb-2 shadow-sm">
                                <input 
                                    id="search-input"
                                    {onchange} 
                                    type="text" 
                                    value={search_text} 
                                    style="max-width:400px;" 
                                    placeholder="Enter search text" 
                                    class="form-control" />
                                // TODO: Add an onclick and send the RefreshFromSearchText() message?
                                <button 
                                    id="search-button"
                                    type="submit" 
                                    class="btn btn-primary">
                                    {"Search"}
                                </button>
                                if self.result.load_error { 
                                    <span class="mx-1 badge bg-danger">{"Error"}</span> 
                                    <span class="mx-1" style="color:red;">{self.result.load_error_message.clone()}</span>
                                }
                            </div>
                            <div class="d-flex flex-colum mb-2 shadow-sm">
                                <a href="/app/territory-edit?id=0" class="btn btn-primary ms-2">{"+ New"}</a>
                            </div> 
                        </div>
                    </form>
                    <div class="row">
                        <div class="col">
                            <span id="territories-found-summary"><strong>{count}</strong>{" Territories Found"}</span>
                        </div>
                    </div>
                    <div class="row py-1" style="border-top: 1px solid gray;">
                        <div class="col-2 col-md-1"><strong>{"#"}</strong></div>
                        <div class="col-5 col-md-3"><strong>{"Description"}</strong></div>
                        <div class="col-5 col-md-3"><strong>{"Status Visited"}</strong></div>
                        <div class="col-7 col-md-3"><strong>{"Publisher"}</strong></div>
                        <div class="col-5 col-md-2"><strong>{"Date"}</strong></div>
                    </div>
                    {
                        self.territories.iter().map(|territory| {   
                            let territory_id: i32 = territory.id.unwrap_or_default();
                            let edit_uri = format!("/app/territory-edit?id={territory_id:?}");        
                            let stage = territory.stage.clone().unwrap_or_default().clone();
                            let is_removed = territory.status.clone() == Some("Removed".to_string()) ;
                            let stage = if is_removed { "Removed" } else { stage.as_str() };
                            let stage_color = match stage {
                                "Available" => "green",
                                "Out" => "magenta",
                                "Visiting" => "magenta",
                                "Removed" => "black",
                                "Done" => "blue",
                                "Cooling Off" => "blue",
                                "Visiting Done" => "blue",
                                "Visiting Started" => "red",
                                _ => "gray"
                            };

                            let address_summary = format!(
                                "{}/{}", 
                                (territory.addresses_active-territory.addresses_unvisited), 
                                territory.addresses_active);

                            html! {
                                 <a href={edit_uri} style="text-decoration:none;color:black;">
                                     <div class="row py-1" style="border-top: 1px solid lightgray;">
                                         <div class="col-2 col-md-1" style="font-weight:bold;">
                                             {territory.number.clone()}
                                         </div>
                                         <div class="col-5 col-md-3">
                                             {territory.description.clone()}
                                         </div>
                                         <div class="col-5 col-md-3">
                                            <span class="badge" style={format!("border-radius:3px;border-width:1px;border-style:solid;color:white;background-color:{stage_color}")}>
                                                {stage}
                                            </span>
                                            if false { // TODO: Maybe turn this back on later as a feature
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
                                            }
                                             <span class="ms-1">{address_summary}</span>
                                        </div>
                                        <div class="col-7 col-md-3">
                                            {territory.publisher.clone()}
                                        </div>
                                        <div class="col-5 col-md-2">
                                            {territory.status_date.clone()}
                                        </div>
                                    </div>
                                 </a>
                           }
                         }).collect::<Html>()
                    }
                </div>
                if self.show_unauthorized_modal {
                    <UnauthorizedModal {return_url} />              
                }
            </>
        }
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

pub trait SearchQuery {
    fn search_query(&self) -> TerritorySearchQuery;
}

impl SearchQuery for &Context<TerritorySearchPage> {
    fn search_query(&self) -> TerritorySearchQuery {
        let location = self.link().location().expect("Location or URI");
        location.query::<TerritorySearchQuery>().unwrap_or(TerritorySearchQuery::default())    
    }
}
