use crate::components::{
    map_component::{MapComponent, MapModel},
    model_functions::*,
    map_component_functions::{
        TerritoryPolygon,
        tpoly_from_territory_w_button,
        stage_color,
        stage_as_of_date, tpoly_from_area_w_button,
    },
};

use crate::Route;
use crate::functions::document_functions::set_document_title;

use chrono::{NaiveDate,Duration};
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
    Search(String, String, bool),
    RefreshFromSearchText(), // Search what's already downloaded
    ToggleMenu(MapMenu),
}

#[derive(PartialEq, Clone, Copy)]
pub enum MapMenu {
    Menu,
    Search,
    History,
    Options,
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
    show_menu: MapMenu,
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
        let as_of_date = ctx.search_query().as_of_date.clone();
        if !mtk.is_empty() {
            let query = MapSearchQuery {
                search: Some("".to_string()),
                mtk: Some(mtk),
                as_of_date: as_of_date,
                ..MapSearchQuery::default()
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
            show_menu: MapMenu::Search,
        }                
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::LoadBordersPath(map_model, mtk, search) => {
                log!(format!("model::Msg::LoadBordersPath(mtk: {}, search: {})", mtk.clone(), search.clone()));
                self.territory_map = map_model.clone();
                self.last_mtk = Some(mtk); // TODO: Do I need this?
                let as_of_date = ctx.search_query().as_of_date.clone().unwrap_or_default();
                let show_areas = ctx.search_query().show_areas;

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
                ctx.link().send_message(Msg::Search(search.clone(), as_of_date.clone(), show_areas));
                true
            },
            Msg::Search(search, as_of_date, show_areas) => {
                log!(format!("model::Msg::Search(search: {}, as_of_date: {})", search.clone(), as_of_date.clone()));
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
                        //|| t.group_id == Some("borders".to_string())
                        {
                        let mut tp = tpoly_from_territory_w_button(t, self.territory_map.popup_content_options.clone());
                        tp.color = stage_color(stage_as_of_date(t, as_of_date.clone()).as_str());
                        tpolygons.push(tp);
                    } 
                }

                if show_areas {
                    for area in self.territory_map.areas.iter() {
                        let mut tp = tpoly_from_area_w_button(area);
                        tp.color = "orange".to_string();
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

                if !as_of_date.is_empty() {
                    //self.show_menu = MapMenu::History;
                    ctx.link().clone().send_message(Msg::ToggleMenu(MapMenu::History));
                }

                true
            },
            Msg::RefreshFromSearchText() => {
                log!("RefreshFromSearchText called");
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
                let show_areas = ctx.search_query().show_areas;
                self.show_menu = if !as_of_date.clone().unwrap_or_default().is_empty() { MapMenu::History } else { self.show_menu };

                // This one is weird because all the territories are preloaded and searchable                
                if self.last_mtk != Some(mtk.to_string()) {
                    ctx.link().send_future(async move {
                        Msg::LoadBordersPath(
                            fetch_territory_map_w_mtk(
                                &mtk.to_string(), 
                                as_of_date.clone(),
                                show_areas
                            ).await, 
                            mtk.to_string(), 
                            search_text.clone())
                    });
                } else {
                    ctx.link().send_future(async move {
                        Msg::Search(search_text.clone(), as_of_date.clone().unwrap_or_default(), show_areas)
                    });                    
                }
                false
            },
            Msg::ToggleMenu(menu) => {
                self.show_menu = menu;
                true
            }          
        }
    }
    
    fn rendered(&mut self, ctx: &Context<Self>, first_render: bool) {
        if first_render && self.tpolygons.is_empty() {
            log!("Map not loaded after create, loading...");
            let navigator = ctx.link().navigator().unwrap();
            let mtk = ctx.search_query().mtk.clone().unwrap_or_default(); 
            let as_of_date = ctx.search_query().as_of_date.clone();            
            let query = MapSearchQuery {
                search: Some("".to_string()),
                mtk: Some(mtk.clone()),
                as_of_date,
                show_areas: ctx.search_query().show_areas,
            };

            let _ = navigator.push_with_query(&Route::MapComponent, &query);
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        set_document_title("Map");

        let show_menu = self.show_menu;
        let link = ctx.link().clone(); 
        let menu_button_onclick = {
            Callback::from(move |_event: MouseEvent| {
                if show_menu == MapMenu::Menu {
                    link.send_message(Msg::ToggleMenu(MapMenu::Search));
                } else {
                    link.send_message(Msg::ToggleMenu(MapMenu::Menu));
                }
            })
        };

        let link = ctx.link().clone(); 
        let search_menu_onclick = {
            Callback::from(move |_event: MouseEvent| {
                link.send_message(Msg::ToggleMenu(MapMenu::Search));
            })
        };

        let link = ctx.link().clone(); 
        let history_menu_onclick = {
            Callback::from(move |_event: MouseEvent| {
                link.send_message(Msg::ToggleMenu(MapMenu::History));
            })
        };
        
        let link = ctx.link().clone(); 
        let options_menu_onclick = {
            Callback::from(move |_event: MouseEvent| {
                link.send_message(Msg::ToggleMenu(MapMenu::Options));
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        let query_clone = ctx.search_query().clone();
        let show_areas = ctx.search_query().clone().show_areas;
        let areas_toggle_onclick = {
            Callback::from(move |_event: MouseEvent| {
                let query = MapSearchQuery {
                    mtk: query_clone.mtk.clone(),
                    search: query_clone.search.clone(),
                    as_of_date: query_clone.as_of_date.clone(),
                    show_areas: !show_areas,
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        let query_clone = ctx.search_query().clone();
        let asof_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = MapSearchQuery {
                    mtk: query_clone.mtk.clone(), //Some(mtk.clone()),
                    search: query_clone.search.clone(),
                    as_of_date: Some(value.clone()),
                    show_areas: query_clone.show_areas,
                };

                log!(format!("AdOfDateChanged to : {}", value.clone()));
                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        let query_clone = ctx.search_query().clone();
        let asof_back_click = {
            Callback::from(move |_event: MouseEvent| {
                let next_day_result 
                    = NaiveDate::parse_from_str(
                        query_clone.as_of_date.clone().unwrap_or_default().as_str(),
                        "%Y-%m-%d");
                                
                let next_day = next_day_result.clone().unwrap();
                let day_after = next_day - Duration::days(1);
                let query = MapSearchQuery {
                    mtk: query_clone.mtk.clone(),
                    search: query_clone.search.clone(),
                    as_of_date: Some(day_after.clone().format("%Y-%m-%d").to_string()),
                    show_areas: query_clone.show_areas,
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        let query_clone = ctx.search_query().clone();
        let asof_forward_click = {
            Callback::from(move |_event: MouseEvent| {
                let next_day_result 
                    = NaiveDate::parse_from_str(
                        query_clone.as_of_date.clone().unwrap_or_default().as_str(),
                        "%Y-%m-%d");
                                
                let next_day = next_day_result.clone().unwrap();
                let day_after = next_day + Duration::days(1);
                let query = MapSearchQuery {
                    mtk: query_clone.mtk.clone(), //Some(mtk.clone()),
                    search: query_clone.search.clone(),
                    as_of_date: Some(day_after.clone().format("%Y-%m-%d").to_string()),
                    show_areas: query_clone.show_areas,
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        let query_clone = ctx.search_query().clone();
        let _show_areas_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = MapSearchQuery {
                    search: Some(value.clone()),
                    mtk: query_clone.mtk.clone(),
                    as_of_date: query_clone.as_of_date.clone(),
                    show_areas: if value == "show_areas".to_string() { true } else { false },
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        // let mtk = ctx.search_query().mtk.clone().unwrap_or_default();  
        // let as_of_date = ctx.search_query().as_of_date.clone();
        let query_clone = ctx.search_query().clone();
        let search_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = MapSearchQuery {
                    search: Some(value.clone()),
                    mtk: Some(query_clone.mtk.clone().unwrap_or_default()),
                    as_of_date: query_clone.as_of_date.clone(),
                    show_areas: query_clone.show_areas,
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        // let mtk = ctx.search_query().mtk.clone().unwrap_or_default();  
        // let as_of_date = ctx.search_query().as_of_date.clone(); 
        let query_clone = ctx.search_query().clone();
        let search_clear_onclick = {
            Callback::from(move |_event: MouseEvent| {
                let query = MapSearchQuery {
                    search: Some("".to_string()),
                    mtk: Some(query_clone.mtk.clone().unwrap_or_default()),
                    as_of_date: query_clone.as_of_date.clone(),
                    show_areas: query_clone.show_areas,
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let navigator = ctx.link().navigator().unwrap();
        let query_clone = ctx.search_query().clone();
        let as_of_date_clear_onclick = {
            Callback::from(move |_event: MouseEvent| {
                let query = MapSearchQuery {
                    search: query_clone.search.clone(),
                    mtk: Some(query_clone.mtk.clone().unwrap_or_default()),
                    as_of_date: Some("".to_string()),
                    show_areas: query_clone.show_areas,
                };

                let _ = navigator.push_with_query(&Route::MapComponent, &query);
            })
        };

        let show_areas = ctx.search_query().clone().show_areas;
        let as_of_date = ctx.search_query().as_of_date.clone().unwrap_or_default();  
        let search_text = ctx.search_query().search.clone().unwrap_or_default();  
        let count = self.tpolygons.len();

        html! {
           <div style="background-color:yellow;height:100%;">
                <div id="menu-bar-header" style="height:57px;background-color:white;">
                                    //<form onsubmit={search_text_onsubmit} id="search-form">
                    //<MenuBarV2>
                        //<ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                            // <li class="nav-item">
                            //     <TerritorySearchLink />
                            // </li>
                            //<li class="nav-item">
                                <div class="container pt-2 ps-4">
                                //<div class="row">
                                <div class="row row-cols-auto">
                                if self.show_menu != MapMenu::Menu {
                                    <div class="col px-1">
                                        <button 
                                                id="menu-button"
                                                onclick={menu_button_onclick}
                                                class="btn btn-outline-primary">
                               
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16">
                                                    <path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/>
                                                </svg>    

                               
                                                    // <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16">
                                                    //     <path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/>
                                                    // </svg>
                                                // } else {
                                                //     <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16">
                                                //         <path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/>
                                                //     </svg>
                                                // }
                                            </button>
                                    </div>
                                }
                                <div class="col px-1">
                                    //<div class="input-group">
                                       
                                        if self.show_menu == MapMenu::History {
                                            <div class="input-group">
                                                <button 
                                                    id="date-back-button"
                                                    onclick={asof_back_click}
                                                    class="btn btn-outline-primary">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-caret-left" viewBox="0 0 16 16">
                                                        <path d="M10 12.796V3.204L4.519 8 10 12.796zm-.659.753-5.48-4.796a1 1 0 0 1 0-1.506l5.48-4.796A1 1 0 0 1 11 3.204v9.592a1 1 0 0 1-1.659.753z"/>
                                                    </svg>
                                                </button>
                                                <input 
                                                    id="asof-text-box"
                                                    style="max-width:130px;"
                                                    onchange={asof_text_onchange}
                                                    value={as_of_date.clone()}
                                                    type="text"
                                                    class="form-control"
                                                    placeholder="As of Date"  />
                                                <button 
                                                    id="clear-as-of-date-button"
                                                    onclick={as_of_date_clear_onclick} 
                                                    class="btn btn-outline-primary">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-lg" viewBox="0 0 16 16">
                                                        <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                                                    </svg>
                                                </button>                                                    
                                                <button 
                                                    id="date-forward-button"
                                                    onclick={asof_forward_click}
                                                    class="btn btn-outline-primary">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-caret-right" viewBox="0 0 16 16">
                                                        <path d="M6 12.796V3.204L11.481 8 6 12.796zm.659.753 5.48-4.796a1 1 0 0 0 0-1.506L6.66 2.451C6.011 1.885 5 2.345 5 3.204v9.592a1 1 0 0 0 1.659.753z"/>
                                                    </svg>
                                                </button>
                                            </div>
                                        } else if self.show_menu == MapMenu::Menu {
                                            <a 
                                                href="/app/menu"
                                                id="home-menu-button"
                                                class="btn btn-outline-primary">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-list" viewBox="0 0 16 16">
                                                    <path fill-rule="evenodd" d="M2.5 12a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5z"/>
                                                </svg>
                                            </a>
                                            <button 
                                                id="search-menu-button"
                                                onclick={search_menu_onclick}
                                                class="btn btn-outline-primary position-relative">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-search" viewBox="0 0 16 16">
                                                    <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                                                </svg>                                                
                                               // {" Search"}
                                               if !search_text.is_empty() {
                                                    <span id="search-exists-notification" class="position-absolute bottom-0 start-50 translate-middle-x p-1 bg-success border border-light rounded-circle"/> 
                                                }
                                            </button>
                                            <button 
                                                id="history-menu-button"
                                                onclick={history_menu_onclick}
                                                class="btn btn-outline-primary position-relative">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-calendar2-event" viewBox="0 0 16 16">
                                                    <path d="M11 7.5a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5v-1z"/>
                                                    <path d="M3.5 0a.5.5 0 0 1 .5.5V1h8V.5a.5.5 0 0 1 1 0V1h1a2 2 0 0 1 2 2v11a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V3a2 2 0 0 1 2-2h1V.5a.5.5 0 0 1 .5-.5zM2 2a1 1 0 0 0-1 1v11a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V3a1 1 0 0 0-1-1H2z"/>
                                                    <path d="M2.5 4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5H3a.5.5 0 0 1-.5-.5V4z"/>
                                                </svg>  
                                                if !as_of_date.is_empty() {
                                                    <span id="as-of-date-exists-notification" class="position-absolute bottom-0 start-50 translate-middle-x p-1 bg-success border border-light rounded-circle"/> 
                                                }
                                             </button>
                                             <button 
                                                id="options-menu-button"
                                                onclick={options_menu_onclick}
                                                class="btn btn-outline-primary position-relative">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-toggles" viewBox="0 0 16 16">
                                                    <path d="M4.5 9a3.5 3.5 0 1 0 0 7h7a3.5 3.5 0 1 0 0-7h-7zm7 6a2.5 2.5 0 1 1 0-5 2.5 2.5 0 0 1 0 5zm-7-14a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5zm2.45 0A3.49 3.49 0 0 1 8 3.5 3.49 3.49 0 0 1 6.95 6h4.55a2.5 2.5 0 0 0 0-5H6.95zM4.5 0h7a3.5 3.5 0 1 1 0 7h-7a3.5 3.5 0 1 1 0-7z"/>
                                                </svg>
                                            </button>                                             
                                        } else if self.show_menu == MapMenu::Options {
                                            <button 
                                                id="areas-toggle-button"
                                                onclick={areas_toggle_onclick}
                                                class="btn btn-outline-primary position-relative">
                                                if show_areas {
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-toggle-on" viewBox="0 0 16 16">
                                                        <path d="M5 3a5 5 0 0 0 0 10h6a5 5 0 0 0 0-10H5zm6 9a4 4 0 1 1 0-8 4 4 0 0 1 0 8z"/>
                                                    </svg>
                                                } else {
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-toggle-off" viewBox="0 0 16 16">
                                                        <path d="M11 4a4 4 0 0 1 0 8H8a4.992 4.992 0 0 0 2-4 4.992 4.992 0 0 0-2-4h3zm-6 8a4 4 0 1 1 0-8 4 4 0 0 1 0 8zM0 8a5 5 0 0 0 5 5h6a5 5 0 0 0 0-10H5a5 5 0 0 0-5 5z"/>
                                                    </svg>
                                                }
                                                {" Areas"}
                                            </button>                     
                                        } else {
                                            //<form onsubmit={search_text_onsubmit} id="search-form">
                                            <div class="input-group">
                                                <input 
                                                    id="search-text-box"
                                                    onchange={search_text_onchange}
                                                    value={search_text}
                                                    type="text"
                                                    class="form-control"
                                                    style="max-width:120px;min-width:100px;"
                                                    placeholder="Search"  />
                                                //</form>
                                                <button 
                                                    id="clear-search-button"
                                                    onclick={search_clear_onclick} 
                                                    class="btn btn-outline-primary">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-lg" viewBox="0 0 16 16">
                                                        <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                                                    </svg>
                                                </button>
                                                <button 
                                                    id="submit-search-button"
                                                    form="search-form" class="btn btn-primary" type="submit">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-search" viewBox="0 0 16 16">
                                                        <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                                                    </svg>
                                                </button>
                                            </div>
                                            // <span>{"  Mouse: "}{mouse_click_model.mouse_click_x}{","}{mouse_click_model.mouse_click_y}</span>
                                            // <span>{"  LatLng: "}{format!("{:.4},{:.4}",latLng.lat(),latLng.lng())}</span>                                    
                                          
                                            // <span id="search-result-summary" class="p-2 flex-grow-3 ">
                                            //     {count}                                 
                                            // </span>
                                            
                                        }
                                    </div>
                                    <div class="col px-1 pt-2">
                                        <span id="search-result-summary" class="py-2 flex-grow-3 ">
                                            {count}     
                                        </span>
                                    </div>
                                //</div>                            
                                if !self.search_error.clone().is_empty() {
                                    <div class="col px-0 pt-2">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="red" class="bi bi-slash-circle" viewBox="0 0 16 16">
                                            <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
                                            <path d="M11.354 4.646a.5.5 0 0 0-.708 0l-6 6a.5.5 0 0 0 .708.708l6-6a.5.5 0 0 0 0-.708z"/>
                                        </svg>
                                    </div>
                                    <div class="col px-0 pt-2">
                                        <span style="padding-left:5px;color:red;">{self.search_error.clone()}</span>
                                    </div>
                                } 
                                </div>
                                </div>
                            //</li>
                        //</ul>
                    //</MenuBarV2>
                           //</form>    
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
    pub show_areas: bool,
}

pub trait SearchQuery {
    fn search_query(&self) -> MapSearchQuery;
}

impl SearchQuery for &Context<Model> {
    fn search_query(&self) -> MapSearchQuery {
        let location = self.link().location().expect("Location or URI");
        location.query::<MapSearchQuery>().unwrap_or(MapSearchQuery::default())    
    }
}

