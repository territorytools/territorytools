use crate::libs::leaflet::{LatLng, LatLngBounds, Map, Polygon, Polyline, TileLayer, Point};
use crate::components::popup_content::popup_content;
use crate::models::territories::Territory;
use crate::components::menu_bar_v2::MenuBarV2;

use wasm_bindgen::{prelude::*, JsCast};
use gloo_utils::document;
use web_sys::{Element, HtmlElement, Node};
use yew::{html::ImplicitClone, prelude::*};
use gloo_console::log;
use reqwasm::http::Request;
use yew::prelude::*;
use serde::{Deserialize, Serialize};

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

#[derive(Serialize, Deserialize)]
struct PolylineOptions {
    color: String,
    opacity: f32,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct MapModel {
    pub territories: Vec<Territory>,
    pub territories_is_loaded: bool,
    pub local_load: bool,
    pub zoom: f64,
    pub lat: f64,
    pub lon: f64,
    pub group_visible: String,
}

impl ImplicitClone for MapModel {}

#[derive(Serialize, Deserialize)]
struct TooltipOptions {
    sticky: bool,
    permanent: bool,
    //direction: String,
    opacity: f32,
    //className: String
}

#[derive(Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
struct PopupOptions {
    auto_close: bool,
}


pub enum Msg {
    LoadBorders(MapModel),
}

pub struct MapComponent {
    map: Map,
    lat: PixelPoint,
    container: HtmlElement,
    territory_map: MapModel,
}

#[derive(Copy, Clone, Debug, PartialEq)]
pub struct PixelPoint(pub f64, pub f64);

#[derive(PartialEq, Clone, Debug)]
pub struct City {
    pub name: String,
    pub lat: PixelPoint,
}

impl ImplicitClone for City {}

#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    pub city: City,
    pub territory_map: MapModel,
}

impl MapComponent {
    fn render_map(&self) -> Html {
        let node: &Node = &self.container.clone().into();
        Html::VRef(node.clone())
    }

    // fn render_map(element: &HtmlElement) -> Html {
    //     // Element must be passed as an address I guess
    //     let node: &Node = &element.clone().into();
    //     Html::VRef(node.clone())
    // }
}

impl Component for MapComponent {
    type Message = Msg;
    type Properties = Props;

    fn create(ctx: &Context<Self>) -> Self {
        let props = ctx.props();

        let container: Element = document().create_element("div").unwrap();
        let container: HtmlElement = container.dyn_into().unwrap();
        container.set_class_name("map");
        let leaflet_map = Map::new_with_element(&container, &JsValue::NULL);
        Self {
            map: leaflet_map,
            container,
            lat: props.city.lat,
            territory_map: MapModel::default(),
        }
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
            log!(format!("map_component.rendered: self.territory_map.lat,lon: {:.4},{:.4}", self.territory_map.lat, self.territory_map.lon)); 
            //self.map.setView(&LatLng::new(self.territory_map.lat, self.territory_map.lat), 11.0);
            add_tile_layer(&self.map);
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, _msg: Self::Message) -> bool {
        false
    }

    fn changed(&mut self, ctx: &Context<Self>, old_props: &Self::Properties) -> bool {
        let props = ctx.props();
        log!("map_component.changed: running...");
        log!(format!("map_component.changed: props.territory_map.lat,lon: {:.4},{:.4}", props.territory_map.lat, props.territory_map.lon)); 
        if self.territory_map.lat == props.territory_map.lat && self.territory_map.lon == props.territory_map.lon {
            false
        } else {
            //self.lat = props.city.lat;
            //self.lat = LatLng::new(props.territory_map.lat, props.territory_map.lon);

            //self.map.setView(&LatLng::new(self.lat.0, self.lat.1), 11.0);
            
            self.territory_map = MapModel {
                territories: props.territory_map.territories.clone(),
                territories_is_loaded: true,
                local_load: false,
                lat:  props.territory_map.lat,
                lon: props.territory_map.lon,
                zoom: props.territory_map.zoom,
                group_visible: props.territory_map.group_visible.clone(),
            };

            
            log!("map_component.changed: territory_map loaded");
            
            log!(format!("map_component.changed: territory_map.lat,lon: {:.4},{:.4}", self.territory_map.lat, self.territory_map.lon));            
            self.map.setView(&LatLng::new(self.territory_map.lat, self.territory_map.lon), 7.0); //self.territory_map.zoom);
            
            log!(format!("map_component.territories.len: {}", self.territory_map.territories.len()));
            
            // let bounder  = self.territory_map.territories.iter().filter(|t| t.number == "OUTER".to_string()).iter().first().expect("At least one address on the first territory!");
            // let mut bounder_polygon: Vec<LatLng> = Vec::new();        
            // for v in &bounder.border {
            //     if v.len() > 1 {
            //         bounder_polygon.push(LatLng::new(v[0].into(), v[1].into()));
            //     }
            // }
            // let bounder_poly = Polygon::new_with_options(
            //     bounder_polygon.iter().map(JsValue::from).collect(),
            //     &serde_wasm_bindgen::to_value(&PolylineOptions {
            //         color: "red".to_string(),
            //         opacity: 1.0,
            //     })
            //     .expect("Unable to serialize bonder polygon options"),
            // );

            // //let bounder = self.territory_map.territories.iter().filter(|t| t.number == "10000".to_string()).first();            
            // let bounds = bounder_poly.getBounds();
            // self.map.fitBounds(&bounds);
            

            for t in self.territory_map.territories.iter() {
                // TerritorySummary // total_count += 1;
                //log!("Territory:");
                //log!(format!("  Number: {}", &t.number));

                //if t.number != "OUTER".to_string() { continue; }
                let mut polygon: Vec<LatLng> = Vec::new();
        
                for v in &t.border {
                    if v.len() > 1 {
                        //log!("Vertex: {},{}", v[0],v[1]);
                        polygon.push(LatLng::new(v[0].into(), v[1].into()));
                    }
                }
        
                let completed_by: String = {
                    match t.last_completed_by {
                        Some(_) => "yes".to_string(),
                        None => "no".to_string(),
                    }
                };
        
                let group_id: String = {
                    match &t.group_id {
                        Some(v) => v.to_string(),
                        None => "".to_string(),
                    }
                };
        
                let area_code: String = {
                    match t.area_code {
                        Some(_) => t.area_code.clone().unwrap(),
                        None => "".to_string(),
                    }
                };
        
                let territory_color: String = {
                    if area_code == "TER" {
                        "red".to_string()
                    } else if t.status == "Signed-out" {
                        // TerritorySummary // signed_out_count += 1;
                        "magenta".to_string()
                    } else if t.status == "Completed" || t.status == "Available" && completed_by == "yes" {
                        // TerritorySummary // completed_count += 1;
                        "blue".to_string() // Completed
                    } else if t.status == "Available" {
                        // TerritorySummary // available_count += 1;
                        "black".to_string()
                    } else {
                        "#090".to_string()
                    }
                };
        
                let opacity: f32 = {
                    if t.is_active {
                        1.0
                    } else {
                        0.01
                    }
                };
        
                if area_code == "TER" {
                    let polyline = Polyline::new_with_options(
                        polygon.iter().map(JsValue::from).collect(),
                        &serde_wasm_bindgen::to_value(&PolylineOptions {
                            color: territory_color.into(),
                            opacity: 1.0,
                        })
                        .expect("Unable to serialize polygon options"),
                    );
                 
                    log!("Fitting bounds 2...");
                    let bounds = polyline.getBounds();
                    self.map.fitBounds(&bounds);
                
                    // bounds = polyline.getBounds();
                    // leaflet_map.fitBounds(&bounds);
        
                    polyline.addTo(&self.map);
                    // TerritorySummary // hidden_count += 1;
                } else {
                    let poly = Polygon::new_with_options(
                        polygon.iter().map(JsValue::from).collect(),
                        &serde_wasm_bindgen::to_value(&PolylineOptions {
                            color: territory_color.into(),
                            opacity: opacity.into(),
                        })
                        .expect("Unable to serialize polygon options"),
                    );
                    
                    //log!(format!("t.number = {}", t.number));

                    // if t.number == "OUTER".to_string() {
                    //     log!("Fitting bounds 1...");
                    //     let bounds = poly.getBounds();
                    //     self.map.fitBounds(&bounds);
                    // }
        
                    let tooltip_text: String = format!("{group_id}: {area_code}: {}", t.number);
        
                    let popup_text = popup_content(&t); //".to_string(); // TODO: implement popup_conent?
        
                    if t.border.len() > 2 {
                        poly.bindTooltip(
                            &JsValue::from_str(&tooltip_text),
                            &serde_wasm_bindgen::to_value(&TooltipOptions {
                                sticky: true,
                                permanent: false,
                                opacity: 0.9,
                            })
                            .expect("Unable to serialize tooltip options"),
                        );
                    }
        
                    poly.bindPopup(
                        &JsValue::from_str(&popup_text),
                        &serde_wasm_bindgen::to_value(&PopupOptions { auto_close: true })
                            .expect("Unable to serialize popup options"),
                    );
        
                    if !t.is_hidden && t.group_id.clone().unwrap_or("".to_string()) != "outer".to_string() {
                        poly.addTo(&self.map);
                    }
                }
            
            }
            //self.map.setView(&LatLng::new(self.lat.0, self.lat.1), 11.0);
            //self.map.setView(&LatLng::new(self.territory_map.lat.0, self.territory_map.lat.1), 11.0);
            true
        }

    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
        let search_text_onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
        });
        let search_text_onchange = {
            Callback::from(move |event: Event| {

            })
        };
        let search_clear_onclick = {
            Callback::from(move |_event: MouseEvent| {
                
            })
        };
        // let container: Element = document().create_element("div").unwrap();
        // let container: HtmlElement = container.dyn_into().unwrap();
        // let map_container = self.render_map(&container);

        html! {
            <div style="background-color:yellow;height:100%;">
                <div id="menu-bar-header" style="height:57px;background-color:red;">
                    <MenuBarV2>
                        <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                            // <li class="nav-item">
                            //     <TerritorySearchLink />
                            // </li>
                            <li class="nav-item">
                                <div class="d-flex flex-colum shadow-sm">
                                    <div class="input-group">
                                        <form onsubmit={search_text_onsubmit} id="search-form" style="max-width:150px;">
                                            <input onchange={search_text_onchange}
                                                value="temp" //value={search_state.search_text.clone()}
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
                                    </div>
                                </div>
                            </li>
                        </ul>
                    </MenuBarV2>
                </div>
                <div class="map map-container component-container"  style="height: calc(100% - 57px);background-color:blue;padding:0;border-width:0;">
                    //<div style={"width:100%;"}>
                        {self.render_map()}
                    //</div>
                </div>
            </div>
        }
    }
}

fn add_tile_layer(map: &Map) {
    TileLayer::new(
        "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
        &JsValue::NULL,
    )
    .addTo(map);
}
