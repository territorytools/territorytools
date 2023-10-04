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
//use gloo_console::log;

pub enum Msg {
    LoadBordersPath(MapModel, String, String), // Download a "key", which includes a default search
    Search(String),
    RefreshFromSearchText(), // Search what's already downloaded
}

#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    // If I name it just 'key' it doesn't work, it might be a reserved word
    pub mtk: Option<String>,
}

pub struct Model {
    _listener: LocationHandle,
    // city: City,  // These are interesting don't delete them yet
    // cities: Cities,
    territory_map: MapModel,
    tpolygons: Vec<TerritoryPolygon>,
    search: String,
    search_error: String,
    last_mtk: Option<String>,
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

        // Convert host/key/1234 to host/app?mtk=1234
        let navigator = ctx.link().navigator().unwrap();
        let mtk = ctx.props().mtk.clone().unwrap_or_default();
        if !mtk.is_empty() {
            let query = MapSearchQuery {
                search: Some("".to_string()),
                mtk: Some(mtk),
                ..MapSearchQuery::default()
                //as_of_date: Some("".to_string()),
            };
            
            let _ = navigator.push_with_query(&Route::MapComponent, &query);
        }
        
        Self {
            _listener: listener,
            // city, 
            // cities, // These are interesting dont' delete them yet
            territory_map: MapModel::default(), 
            search: "".to_string(), 
            search_error: "".to_string(),
            tpolygons: vec![],
            last_mtk: None,
        }                
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::LoadBordersPath(map_model, mtk, search) => {
                self.territory_map = map_model.clone();
                self.last_mtk = Some(mtk); // TODO: Do I need this?

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
                
                let search: String = self.search.trim().to_string();
                
                let prefix_removed: String = if search.to_uppercase().trim().starts_with("NUMBERS:") {
                    search["NUMBERS:".len()..].to_string().clone()
                } else { "".to_string() };

                let numbers = if !prefix_removed.is_empty() {
                    let regex = Regex::new(r"[\s,]+").expect("Valid RegEx for separators");
                    regex.split(prefix_removed.as_str()).collect::<Vec<&str>>()
                } else { vec![] };
                               
                let mut tpolygons: Vec<TerritoryPolygon> = vec![];
                let search_text = Some(search.clone().to_uppercase().trim().to_string());
                for t in self.territory_map.territories.iter() {
                    if (!numbers.is_empty() 
                            && !t.number.is_empty() 
                            && numbers.contains(&t.number.as_str()) )
                        || (search.as_str() == "ALL")
                        || ((search.as_str() == "*" || search.trim().is_empty()) 
                            && t.group_id != Some("outer".to_string())  
                            && t.number.as_str() != "OUTER")
                        || (search.as_str() == "OUTER" && t.number.as_str() == "OUTER")
                        || (search.as_str() == "outer" && t.group_id == Some("outer".to_string()) && t.number.as_str() != "OUTER")
                        || (Some(format!("WHO:{}", t.signed_out_to.clone().unwrap_or("".to_string()).to_uppercase())) == search_text
                            && t.group_id != Some("outer".to_string())
                            && t.number.as_str() != "OUTER")
                        || ((Some(format!("STAGE:{}", t.stage.clone().unwrap_or_default().to_uppercase())) == search_text
                        || Some(format!("STAGE:{}", t.stage_id.unwrap_or(0))) == search_text)
                            && t.group_id != Some("outer".to_string())
                            && t.number.as_str() != "OUTER")
                        || (Some(format!("GROUP:{}", t.group_id.clone().unwrap_or("".to_string()).to_uppercase())) == search_text
                            && t.group_id != Some("outer".to_string())
                            && t.number.as_str() != "OUTER")
                        || ((Some(format!("g{}", t.group_id.clone().unwrap_or("".to_string()))) == Some(search.clone())
                            || Some(format!("group:{}", t.group_id.clone().unwrap_or("".to_string()))) == Some(search.clone())
                            || Some(format!("stage:{}", t.stage.clone().unwrap_or_default())) == Some(search.clone())
                            || Some(format!("stage:{}", t.stage_id.unwrap_or(0))) == Some(search.clone())
                            || (t.description.clone().is_some() && t.description.clone().unwrap().contains(&search.clone()))
                            || t.number == search.clone()
                            || t.signed_out_to == Some(search.clone()))
                            && t.group_id != Some("outer".to_string())
                            && t.number.as_str() != "OUTER")
                        {
                        let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                        tpolygons.push(tp);
                    } 
                }

                if !tpolygons.is_empty() {
                    self.tpolygons = tpolygons;
                    self.search_error = "".to_string();
                } else {
                    self.search_error = "No matches!".to_string();
                    if self.tpolygons.is_empty() {
                        // Just reload everything
                        for t in self.territory_map.territories.iter() {
                            let tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                            self.tpolygons.push(tp);
                        }
                    }
                }

                true
            },
            Msg::RefreshFromSearchText() => {
                let search_text = ctx.search_query().search.clone().unwrap_or_default();  
                let mtk = if ctx.search_query().mtk.clone().unwrap_or_default().is_empty() 
                    && ctx.props().mtk.clone().unwrap_or_default().is_empty() {
                        self.last_mtk.clone().unwrap_or_default()
                } else if ctx.props().mtk.clone().unwrap_or_default().is_empty() {
                    ctx.search_query().mtk.clone().unwrap_or_default()
                } else {
                    ctx.props().mtk.clone().unwrap_or_default()
                };
                let as_of_date = ctx.search_query().as_of_date.clone();

                // This one is weird because all the territories are preloaded and searchable                
                if self.last_mtk != Some(mtk.to_string()) {
                    ctx.link().send_future(async move {
                        Msg::LoadBordersPath(
                            fetch_territory_map_w_mtk(
                                &mtk.to_string(), 
                                as_of_date).await, 
                            mtk.to_string(), 
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
    
    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
            // let navigator = ctx.link().navigator().unwrap();
            // let mtk = ctx.search_query().mtk.clone().unwrap_or_default(); 
            // let query = MapSearchQuery {
            //     search: Some("".to_string()),
            //     mtk: Some(mtk.clone()),
            //     ..MapSearchQuery::default()
            // };

            // let _ = navigator.push_with_query(&Route::MapComponent, &query);
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let search_text_onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default(); // Keep this here to prevent 2 searches
        });

        let navigator = ctx.link().navigator().unwrap();
        let mtk = ctx.search_query().mtk.clone().unwrap_or_default();  
        let search_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = MapSearchQuery {
                    search: Some(value.clone()),
                    mtk: Some(mtk.clone()),
                    ..MapSearchQuery::default()
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        let mtk = ctx.search_query().mtk.clone().unwrap_or_default();  
        let search_clear_onclick = {
            Callback::from(move |_event: MouseEvent| {
                let query = MapSearchQuery {
                    search: Some("".to_string()),
                    mtk: Some(mtk.clone()),
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
            <div id="menu-bar-header" style="height:57px;background-color:white;">
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
                                            {
                                                if !self.search_error.clone().is_empty() {
                                                    html! {<span style="padding-left:5px;color:red;">{self.search_error.clone()}</span>}
                                                } else { html!{}}
                                            }
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
    pub mtk: Option<String>,
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

