use crate::components::{
    map_component::{MapComponent, MapModel},
    model_functions::*,
    map_component_functions::{
        TerritoryPolygon,
        tpoly_from_territory_w_button},
};
use crate::components::menu_bar_v2::MenuBarV2;
use crate::Route;

use regex::Regex;
use serde::{Serialize, Deserialize};
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::scope_ext::RouterScopeExt;
use yew_router::prelude::LocationHandle;
use gloo_console::log;

pub enum Msg {
    LoadBordersPath(MapModel, String, String), // Download a "key", which includes a default search
    Search(String),
    RefreshFromSearchText(), // Search what's already downloaded
}

#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    // If I name it just 'key' it doesn't work, it might be a reserved word
    pub link_key: Option<String>,
}

pub struct Model {
    _listener: LocationHandle,
    // city: City,  // These are interesting don't delete them yet
    // cities: Cities,
    territory_map: MapModel,
    tpolygons: Vec<TerritoryPolygon>,
    search: String,
    last_key: Option<String>,
}

impl Component for Model {
    type Message = Msg;
    type Properties = Props;

    fn create(ctx: &Context<Self>) -> Self {
        let link = ctx.link().clone();      
        let listener = ctx.link()
            .add_location_listener(
                Callback::from(move |_| {
                    link.send_message(Msg::RefreshFromSearchText());
                })
            )
            .unwrap();

            // Convert host/key/1234 to host/app?key=1234
        let navigator = ctx.link().navigator().unwrap();
        let key = ctx.props().link_key.clone().unwrap_or_default();
        if !key.is_empty() {
            let query = MapSearchQuery {
                search: None,
                key: Some(key.clone()),
                ..MapSearchQuery::default()
            };
            
            let _ = navigator.push_with_query(&Route::MapComponent, &query);
        }
        
        return Self {
            _listener: listener,
            // city, 
            // cities, // These are interesting dont' delete them yet
            territory_map: MapModel::default(), 
            search: "".to_string(), 
            tpolygons: vec![],
            last_key: None,
        }                
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::LoadBordersPath(map_model, key, search) => {
                self.territory_map = map_model.clone();
                self.last_key = Some(key); // TODO: Do I need this?

                let link_grants = self.territory_map.link_grants.clone().unwrap_or("null".to_string());

                // Description Contains Section
                // TODO: This part... maybe should be somewhere else
                // but it would contain the default search for the key
                let regex = Regex::new(r"description\-contains=([^;]+?);").expect("Valid RegEx");
                let link_grants_clone = link_grants.clone();
                let caps = regex.captures(link_grants_clone.as_str());
                let mut _description_contains: String = "".to_string();
                if caps.is_some() && caps.as_ref().unwrap().len() > 0usize {
                    _description_contains = caps
                        .as_ref()
                        .expect("description-contains in link_grants")
                        .get(1)
                        .map_or("".to_string(), |m| m.as_str().to_string());

                    self.search = _description_contains.clone();
                }
                ctx.link().send_message(Msg::Search(search.clone()));
                true
            },
            Msg::Search(search) => {
                self.search = search;
                self.tpolygons.clear();
                let search: String = self.search.trim().to_string();
                // TODO: Finish number finder github issue #18
                //let numbers: Vec<String> = vec![];
                let has_whitespace_regex = Regex::new(r"(\d*)[\S]").expect("Valid RegEx for digits");
                let whitespace_captures = has_whitespace_regex.captures(search.as_str());                
                if whitespace_captures.is_some() && whitespace_captures.as_ref().unwrap().len() > 0usize {
                    //numbers.push()
                    let numbers = whitespace_captures
                        .as_ref()
                        .expect("description-contains in link_grants")
                        .get(1)
                        .map_or("".to_string(), |m| m.as_str().to_string());
                    log!(format!("model:numbers:len(): {}", whitespace_captures.as_ref().unwrap().len()));
                    //self.search = _description_contains.clone();
                }

                let search_text = Some(search.clone().to_uppercase().trim().to_string());
                for t in self.territory_map.territories.iter() {
                    if search == "ALL".to_string(){
                        let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                        self.tpolygons.push(tp);
                    } else if (search == "*".to_string() || search.trim() == "".to_string()) 
                        && t.group_id != Some("outer".to_string())  
                        && t.number != "OUTER".to_string() {
                        let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                        self.tpolygons.push(tp);
                    } else if search == "OUTER".to_string() && t.number == "OUTER".to_string() {
                        let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                        self.tpolygons.push(tp);
                    } else if search == "outer".to_string() && t.group_id == Some("outer".to_string()) && t.number != "OUTER".to_string() {
                        let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                        self.tpolygons.push(tp);
                    // } else if search.starts_with('<') 
                    //     && t.signed_out.
                    //     && t.number != "OUTER".to_string() {
                    //     let tp = tpoly_from_territory(t);
                    //     self.tpolygons.push(tp);    
                    } else if Some(format!("WHO:{}", t.signed_out_to.clone().unwrap_or("".to_string()).to_uppercase())) == search_text
                        && t.group_id != Some("outer".to_string())
                        && t.number != "OUTER".to_string() {
                            let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                            self.tpolygons.push(tp);                    
                    } else if (Some(format!("STAGE:{}", t.stage.clone().unwrap_or_default().to_uppercase())) == search_text
                        || Some(format!("STAGE:{}", t.stage_id.unwrap_or(0))) == search_text)
                        && t.group_id != Some("outer".to_string())
                        && t.number != "OUTER".to_string() {
                            let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                            self.tpolygons.push(tp);                    
                    } else if Some(format!("GROUP:{}", t.group_id.clone().unwrap_or("".to_string()).to_uppercase())) == search_text
                        && t.group_id != Some("outer".to_string())
                        && t.number != "OUTER".to_string() {
                            let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                            self.tpolygons.push(tp);
                    } else if (Some(format!("g{}", t.group_id.clone().unwrap_or("".to_string()))) == Some(search.clone())
                      || Some(format!("group:{}", t.group_id.clone().unwrap_or("".to_string()))) == Some(search.clone())
                      || Some(format!("stage:{}", t.stage.clone().unwrap_or_default())) == Some(search.clone())
                      || Some(format!("stage:{}", t.stage_id.unwrap_or(0))) == Some(search.clone())
                      || (t.description.clone() != None && t.description.clone().unwrap().contains(&search.clone()))
                      || t.number == search.clone()
                      || t.signed_out_to == Some(search.clone()))
                      && t.group_id != Some("outer".to_string())
                      && t.number != "OUTER".to_string()  {
                        let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                        self.tpolygons.push(tp);
                    } 
                }
                true
            },
            Msg::RefreshFromSearchText() => {
                let search_text = ctx.search_query().search.clone().unwrap_or_default();  
                let key = if ctx.search_query().key.clone().unwrap_or_default().is_empty() 
                    && ctx.props().link_key.clone().unwrap_or_default().is_empty() {
                        self.last_key.clone().unwrap_or_default()
                } else if ctx.props().link_key.clone().unwrap_or_default().is_empty() {
                    ctx.search_query().key.clone().unwrap_or_default()
                } else {
                    ctx.props().link_key.clone().unwrap_or_default()
                };
                let as_of_date = ctx.search_query().as_of_date.clone();

                // This one is weird because all the territories are preloaded and searchable                
                if self.last_key != Some(key.to_string()) {
                    ctx.link().send_future(async move {
                        Msg::LoadBordersPath(
                            fetch_territory_map_w_key(
                                &key.to_string(), 
                                as_of_date).await, 
                            key.to_string(), 
                            search_text.clone())
                    });
                } else {
                    ctx.link().send_future(async move {
                        Msg::Search(search_text.clone())
                    });                    
                }
                false
            }          
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let search_text_onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default(); // Keep this here to prevent 2 searches
        });

        let navigator = ctx.link().navigator().unwrap();
        let key = ctx.search_query().key.clone().unwrap_or_default();  
        let search_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = MapSearchQuery {
                    search: Some(value.clone()),
                    key: Some(key.clone()),
                    ..MapSearchQuery::default()
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        let key = ctx.search_query().key.clone().unwrap_or_default();  
        let search_clear_onclick = {
            Callback::from(move |_event: MouseEvent| {
                let query = MapSearchQuery {
                    search: Some("".to_string()),
                    key: Some(key.clone()),
                    ..MapSearchQuery::default()
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let as_of_date = ctx.search_query().as_of_date.clone().unwrap_or_default();  
        let search_text = ctx.search_query().search.clone().unwrap_or_default();  
        let count = self.tpolygons.len();

        html! {
           <div style="background-color:yellow;height:100%;">
            <div id="menu-bar-header" style="height:57px;background-color:red;">
                    <MenuBarV2>
                        <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                            // <li class="nav-item">
                            //     <TerritorySearchLink />
                            // </li>
                            <li class="nav-item">
                                <div class="d-flex flex-column">
                                    <div class="input-group">
                                        <form onsubmit={search_text_onsubmit} id="search-form" style="max-width:150px;">
                                            <input onchange={search_text_onchange}
                                                value={search_text}
                                                type="text"
                                                class="form-control"
                                                placeholder="Search"  />
                                        </form>
                                        <button onclick={search_clear_onclick} class="btn btn-outline-primary">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-lg" viewBox="0 0 16 16">
                                                <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                                            </svg>
                                        </button>
                                        <button form="search-form" class="btn btn-primary" type="submit">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-search" viewBox="0 0 16 16">
                                                <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                                            </svg>
                                        </button>
                                        // <span>{"  Mouse: "}{mouse_click_model.mouse_click_x}{","}{mouse_click_model.mouse_click_y}</span>
                                        // <span>{"  LatLng: "}{format!("{:.4},{:.4}",latLng.lat(),latLng.lng())}</span>                                    
                                        <span class="p-2 flex-grow-3 ">
                                            {count}
                                        </span>
                                    </div>    
                                  
                                </div>
                            </li>
                        </ul>
                    </MenuBarV2>
                </div>
                <MapComponent 
                    territory_map={&self.territory_map} 
                    tpolygons={self.tpolygons.clone()} 
                    search={self.search.clone()}
                    as_of_date={as_of_date.clone()}/>
                // Leave this here for a bit, it's interesting
                //<Control select_city={cb} border_loader={tcb} cities={&self.cities}/>
            </div>
        }
    }
}

#[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
pub struct MapSearchQuery {
    pub search: Option<String>,
    pub key: Option<String>,
    pub as_of_date: Option<String>,
}

pub trait SearchQuery {
    fn search_query(&self) -> MapSearchQuery;
}

impl SearchQuery for &Context<Model> {
    fn search_query(&self) -> MapSearchQuery {
        let location = self.link().location().expect("Location or URI");
        location.query().unwrap_or(MapSearchQuery::default())    
    }
}

