use crate::libs::leaflet::{LatLng, Map, Polygon, Polyline, TileLayer, LayerGroup};
use crate::models::territories::Territory;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::map_component_functions::{
    TerritoryPolygon,
    polygon_from_territory,
    polygon_from_territory_polygon,
    tpoly_from_territory};

use wasm_bindgen::{prelude::*, JsCast};
use gloo_utils::document;
use web_sys::{Element, HtmlElement, HtmlInputElement, Node};
use yew::{html::ImplicitClone, prelude::*};
use serde::{Deserialize, Serialize};
use gloo_console::log;

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

pub enum Msg {
    Search(String),
}

pub struct MapComponent {
    map: Map,
    container: HtmlElement,
    territory_map: MapModel,
    polygons: Vec<Polygon>,
    tpolygons: Vec<TerritoryPolygon>,
    layer_group: LayerGroup,
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
    pub tpolygons: Vec<TerritoryPolygon>,
    pub search: String,
}

impl MapComponent {
    fn render_map(&self) -> Html {
        let node: &Node = &self.container.clone().into();
        Html::VRef(node.clone())
    }
}

impl Component for MapComponent {
    type Message = Msg;
    type Properties = Props;

    fn create(_ctx: &Context<Self>) -> Self {
        let container: Element = document().create_element("div").unwrap();
        let container: HtmlElement = container.dyn_into().unwrap();
        container.set_class_name("map");
        let leaflet_map = Map::new_with_element(&container, &JsValue::NULL);
        Self {
            map: leaflet_map,
            container,
            territory_map: MapModel::default(),
            polygons: vec![],
            tpolygons: vec![],
            layer_group: LayerGroup::new(),
        }
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
            //self.map.setView(&LatLng::new(self.territory_map.lat, self.territory_map.lat), 11.0);
            add_tile_layer(&self.map);
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Search(search) => {
                log!(format!("map_component: Search: {}", search));
                let mut new_territories: Vec<Territory> = vec![];
                for t in self.territory_map.territories.iter() {
                    //t.is_visible = false;
                    let nt = Territory {
                        id: t.id.clone(),
                        number: t.number.clone(),
                        status: t.status.clone(),
                        stage_id: t.stage_id.clone(),
                        description: t.description.clone(),
                        notes: t.notes.clone(),
                        address_count: t.address_count.clone(),
                        area_code: t.area_code.clone(),
                        last_completed_by: t.last_completed_by.clone(),
                        signed_out_to: t.signed_out_to.clone(),
                        group_id: t.group_id.clone(),
                        sub_group_id: t.sub_group_id.clone(),
                        is_hidden: t.group_id.clone().unwrap_or("".to_string()) == "outer".to_string(),
                        is_active: true,
                        // is_active: (number.to_string().is_empty()
                        //     || t.number.clone() == number.to_string()
                        //     || t.signed_out_to.clone() == Some(number.to_string())
                        //     || t.description
                        //         .clone()
                        //         .unwrap_or("".to_string())
                        //         .contains(number)
                        //     || format!("g{}", t.group_id.clone().unwrap_or("".to_string()))
                        //         == number.to_string()
                        //     || t.status.clone() == number.to_string()),
                        border: t.border.clone(),
                        addresses: t.addresses.clone(),
                    };
                    new_territories.push(nt);
                }
                self.territory_map.territories = new_territories;
                
                //let layerGroup = LayerGroup::new();
                self.layer_group = LayerGroup::new();
                

                for p in self.polygons.iter() {
                    p.removeFrom(&self.map);
                    //&self.map.removeLayer(p);
                }

                //&self.map.clearLayers();
                
                self.tpolygons.clear();
                let mut id_list = vec![];
                for t in self.territory_map.territories.iter() {
                    if t.group_id != Some("outer".to_string()) && t.number != "OUTER".to_string() {
                        let tp = tpoly_from_territory(t);
                        self.tpolygons.push(tp);
                    }            
                }

                for tp in self.tpolygons.iter() {
                    let p = polygon_from_territory_polygon(&tp);
                    self.polygons.push(p); // TODO: I don't think we need this

                    let p = polygon_from_territory_polygon(&tp);
                        
                    p.addTo_LayerGroup(&self.layer_group);

                    let layer_id = self.layer_group.getLayerId(&p);
                    id_list.push(layer_id);
                }

                for layer_id in id_list.iter() {
                    //log!(format!("removing Layer.id: {}", layer_id));
                    let this_layer = self.layer_group.getLayer(*layer_id);
                    //search
                    let three = layer_id % 3 == 0;
                    if three {
                        self.layer_group.removeLayer_byId(*layer_id);
                    }

                }

                self.layer_group.addTo(&self.map);
            },
        }
        true
    }

    fn changed(&mut self, ctx: &Context<Self>, _old_props: &Self::Properties) -> bool {
        let props = ctx.props();
        if self.territory_map.lat == props.territory_map.lat && self.territory_map.lon == props.territory_map.lon {
            false
        } else {
            self.territory_map = MapModel {
                territories: props.territory_map.territories.clone(),
                territories_is_loaded: true,
                local_load: false,
                lat:  props.territory_map.lat,
                lon: props.territory_map.lon,
                zoom: props.territory_map.zoom,
                group_visible: props.territory_map.group_visible.clone(),
            };
            
            self.map.setView(&LatLng::new(self.territory_map.lat, self.territory_map.lon), 7.0);
            
            for t in self.territory_map.territories.iter() {
                if t.group_id != Some("outer".to_string()) && t.number != "OUTER".to_string() {
                    let tp = tpoly_from_territory(t);
                    // let p = polygon_from_territory_polygon(&tp);
                    // self.polygons.push(p);
                    self.tpolygons.push(tp);
                }            
            }

            for tp in self.tpolygons.iter() {
                let p = polygon_from_territory_polygon(&tp);
                self.polygons.push(p);
            }
            
            //self.map.clearLayers();
            for p in self.polygons.iter() {
                p.removeFrom(&self.map);
            }
            for p in self.polygons.iter() {
                p.addTo(&self.map);
            }

            let outer_border_territories = self.territory_map.territories
                .iter()
                .filter(|t| t.number == "OUTER".to_string())
                .collect::<Vec<_>>();

            let outer_polygon = polyline_from_territory(&outer_border_territories.first().unwrap());
            self.map.fitBounds(&outer_polygon.getBounds());
            outer_polygon.addTo(&self.map);

            true
        }

    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let search_text_onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
        });

        let link = ctx.link().clone();
        let search_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();
                
                log!(format!("map_comp: search_text_onchange: value: {}", value));

                link.send_message(Msg::Search(value));
            })
        };

        let search_clear_onclick = {
            Callback::from(move |_event: MouseEvent| {
                
            })
        };

        html! {
            <div style="background-color:yellow;height:100%;">
            // TODO: Move this whole header thing into the model.rs
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
                    {self.render_map()}
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

fn polyline_from_territory(t: &Territory) -> Polyline {
    let mut vertexes: Vec<LatLng> = Vec::new();
    for v in &t.border {
        if v.len() > 1 {
            vertexes.push(LatLng::new(v[0].into(), v[1].into()));
        }
    }

    Polyline::new_with_options(
        vertexes.iter().map(JsValue::from).collect(),
        &serde_wasm_bindgen::to_value(&PolylineOptions {
            color: "red".to_string(),
            opacity: 1.0,
        })
        .expect("Unable to serialize polygon options"),
    )
}
