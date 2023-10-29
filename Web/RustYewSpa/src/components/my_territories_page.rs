use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::components::user_selector::UserSelector;
use crate::models::territories::TerritorySummary;
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
    Load(MyTerritoriesResult),
    RefreshFromSearchText(),
}

pub struct MyTerritoriesPage {
    _listener: LocationHandle,
    territories: Vec<TerritorySummary>,
    result: MyTerritoriesResult,
}

impl Component for MyTerritoriesPage {
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
            result: MyTerritoriesResult::default(),
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.territories = result.territories.clone();
                self.result.contract = result.contract.clone();
                true
            },
            Msg::RefreshFromSearchText() => {
                let search_text = ctx.search_query().search_text.clone().unwrap_or_default(); 
                let impersonate = ctx.search_query().impersonate.clone().unwrap_or_default();   
                ctx.link().send_future(async move {
                    Msg::Load(get_territories(search_text.clone(), impersonate.clone()).await)
                });
                false
            }            
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        //set_document_title("Territory Search");
        
        let _onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            // If we don't prevent_default() it will clear the box and search again
        });

        let impersonate = ctx.search_query().impersonate.clone().unwrap_or_default();  
        let navigator = ctx.link().navigator().unwrap();
        let _onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = MyTerritoriesQuery {
                    search_text: Some(value.clone()),
                    impersonate: Some(impersonate.clone()),
                };
                let _ = navigator.push_with_query(&Route::TerritorySearch, &query);
            })
        };

        //let assigner_state = assigner_state.clone();
        let navigator = ctx.link().navigator().unwrap();
        let assignee_onchange = {
            //let assigner_state = assigner_state.clone();
            Callback::from(move |assignee: String| {
                // let mut assigner = assigner_state.deref().clone();
                // assigner.assignee = assignee;
                // assigner_state.set(assigner);
                log!(format!("myt:impersonate:user_selected:{}", assignee));
                let query = MyTerritoriesQuery {
                    search_text: Some("".to_string()),
                    impersonate: Some(assignee),
                };
                let _ = navigator.push_with_query(&Route::MyTerritoriesPage, &query);
            })
        };
    
        let count = self.territories.len();
        let _search_text = ctx.search_query().search_text.clone().unwrap_or_default();  
        let full_name = self.result.contract.full_name.clone();
        let can_impersonate = self.result.contract.can_impersonate;

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
                    <div class="row">
                        <div class="col-6">
                            <span><h3 id="territory-title-span">{"My Territories"}</h3></span>
                        </div>
                        if can_impersonate {
                            <div class="col-6">
                                <label for="user-selector" class="form-label">{"Impersonate User"}</label>
                                <UserSelector id="user-selector" onchange={assignee_onchange} name_as_value={true} />
                            </div>
                        }
                        <hr/>
                    </div>
                    <div class="row">
                        <div class="d-flex flex-colum mb-2 shadow-sm">
                                    if self.result.load_error { 
                                <span class="mx-1 badge bg-danger">{"Error"}</span> 
                                <span class="mx-1" style="color:red;">{self.result.load_error_message.clone()}</span>
                            }    
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <span><h4>{full_name.clone()}</h4></span>
                        </div>                        
                        <div class="col-12">
                            <span id="territories-found-summary"><strong>{count}</strong>{" Territories Found"}</span>
                        </div>
                    </div>
                    <div class="row py-1" style="border-top: 1px solid gray;">
                        <div class="col-2 col-md-1"><strong>{"#"}</strong></div>
                        <div class="col-6 col-md-3"><strong>{"Description"}</strong></div>
                        <div class="col-4 col-md-3"><strong>{"Status Visited"}</strong></div>                        
                        <div class="col-6 col-md-2"><strong>{"Date"}</strong></div>
                        //<div class="col-1 col-md-1"><strong>{"Unvisited"}</strong></div>
                        <div class="col-6 col-md-3"><strong>{"Open"}</strong></div>
                    </div>
                    {
                        self.territories.iter().map(|territory| {   
                            let territory_id: i32 = territory.id.unwrap_or_default();
                            let edit_uri = format!("/app/territory-edit?id={territory_id:?}");     
                            let open_uri = territory.view_link.clone();
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
                                    <div class="row py-1" style="border-top: 1px solid lightgray;">                                                                  
                                        <div class="col-2 col-md-1" style="font-weight:bold;">
                                            {territory.number.clone()}
                                        </div>
                                        <div class="col-6 col-md-3">
                                            {territory.description.clone()}
                                        </div>
                                        <div class="col-4 col-md-3">
                                            <span class="badge" style={format!("border-radius:3px;border-width:1px;border-style:solid;color:white;background-color:{stage_color}")}>
                                                {stage.clone()}
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
                                    <div class="col-6 col-md-2">
                                        {territory.status_date.clone()}
                                    </div>
                                    <div class="col-6 col-md-3">
                                        <a class="btn btn-primary btn-sm float-end" href={open_uri}>{"Open"}</a>
                                        if can_impersonate {
                                            <a class="btn btn-primary btn-sm mx-1 float-end" href={edit_uri.clone()}>{"Edit"}</a>
                                        }
                                    </div>                                                          
                                </div>
                           }
                         }).collect::<Html>()
                    }
                </div>
            </>
        }
    }
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct MyTerritoriesContract {
    pub territories: Vec<TerritorySummary>,
    pub can_impersonate: bool,
    pub full_name: String,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct MyTerritoriesResult {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub territories: Vec<TerritorySummary>,
    pub contract: MyTerritoriesContract,
    pub load_error: bool,
    pub load_error_message: String,
}

async fn get_territories(search_text: String, impersonate: String) -> MyTerritoriesResult {
    let uri_string: String = format!("/api/territories/my?impersonate={impersonate}");
    let uri: &str = uri_string.as_str();
    let resp = Request::get(uri)
        .header("Content-Type", "application/json")
        .send()
        .await
        .expect("A result from the /api/territories/my endpoint");
    
    log!(format!("load territory from search result code: {}", resp.status().to_string()));

    let territory_result: MyTerritoriesContract = if resp.status() == 200 {
        resp
        .json()
        .await
        .expect("Valid MyTerritoriesContract result in JSON format")
    } else {
        MyTerritoriesContract {
            territories: vec![],
            can_impersonate: false,
            full_name: "".to_string(),
        }
    };
    
    log!(format!("myt:full_name: {}", territory_result.full_name.clone()));

    MyTerritoriesResult {
        success: (resp.status() == 200),
        count: territory_result.territories.len() as i32,
        territories: territory_result.territories.clone(),
        contract: territory_result.clone(),
        // full_name: territory_result.full_name.clone(),
        // can_impersonate: territory_result.can_impersonate,
        search_text: search_text.clone(),
        load_error: resp.status() != 200,
        load_error_message: if resp.status() == 401 {
                "Unauthorized".to_string()
            } else if resp.status() == 403 {
                "Forbidden".to_string()
            } else {
                format!("Error {:?}", resp.status())
            }
    }
}

#[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
pub struct MyTerritoriesQuery {
    pub search_text: Option<String>,
    pub impersonate: Option<String>,
}

pub trait SearchQuery {
    fn search_query(&self) -> MyTerritoriesQuery;
}

impl SearchQuery for &Context<MyTerritoriesPage> {
    fn search_query(&self) -> MyTerritoriesQuery {
        let location = self.link().location().expect("Location or URI");
        location.query::<MyTerritoriesQuery>().unwrap_or(MyTerritoriesQuery::default())    
    }
}
