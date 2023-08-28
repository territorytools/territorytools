use crate::libs::leaflet::{
    LatLng, 
    LatLngBounds,
    Map,
    Polygon, 
    TileLayer, 
    LayerGroup
};
use crate::models::territories::Territory;
use crate::components::map_component_functions::{
    TerritoryPolygon,
    polygon_from_territory_polygon,
    get_southwest_corner,
    get_northeast_corner,
};

use wasm_bindgen::{prelude::*, JsCast};
use gloo_utils::document;
use web_sys::{Element, HtmlElement, Node};
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
    pub user_roles: Option<String>,
    pub link_grants: Option<String>,
}

impl ImplicitClone for MapModel {}

pub enum Msg {
}

pub struct MapComponent {
    map: Map,
    container: HtmlElement,
    territory_map: MapModel,
    polygons: Vec<Polygon>,
    tpolygons: Vec<TerritoryPolygon>,
    id_list: Vec<i32>,
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
            id_list: vec![],
            layer_group: LayerGroup::new(),
        }
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
            //self.map.setView(&LatLng::new(self.territory_map.lat, self.territory_map.lat), 11.0);
            add_tile_layer(&self.map);
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, _msg: Self::Message) -> bool {
        false
    }

    fn changed(&mut self, ctx: &Context<Self>, _old_props: &Self::Properties) -> bool {
        let props = ctx.props();
        // if self.territory_map.lat == props.territory_map.lat && self.territory_map.lon == props.territory_map.lon {
        //     false
        // } else {
            self.territory_map = MapModel {
                territories: props.territory_map.territories.clone(),
                territories_is_loaded: true,
                local_load: false,
                lat:  props.territory_map.lat,
                lon: props.territory_map.lon,
                zoom: props.territory_map.zoom,
                group_visible: props.territory_map.group_visible.clone(),
                link_grants: Some("".to_string()),
                user_roles: Some("".to_string()),
            };

            self.tpolygons = props.tpolygons.clone();
            
            //self.map.setView(&LatLng::new(self.territory_map.lat, self.territory_map.lon), 7.0);
            
            log!(format!("map_component: changed: tpolygons len: {}", self.tpolygons.len()));

            for id in self.id_list.iter() {
                self.layer_group.removeLayer_byId(*id);
            }

            self.polygons.clear();
            self.id_list.clear();
            self.layer_group = LayerGroup::new();
            for tp in self.tpolygons.iter() {
                let p = polygon_from_territory_polygon(&tp);
                self.polygons.push(p); // TODO: I don't think we need this

                let p = polygon_from_territory_polygon(&tp);
                    
                p.addTo_LayerGroup(&self.layer_group);

                let layer_id = self.layer_group.getLayerId(&p);
                self.id_list.push(layer_id);
            }

            self.layer_group.addTo(&self.map);

            let sw = get_southwest_corner(self.tpolygons.clone());
            let ne = get_northeast_corner(self.tpolygons.clone());
            
            let bounds = LatLngBounds::new(
                &LatLng::new(ne.lat as f64, ne.lon as f64),
                &LatLng::new(sw.lat as f64, sw.lon as f64)
            );
            
            self.map.fitBounds(&bounds);

            // //self.map.clearLayers();
            // for p in self.polygons.iter() {
            //     p.removeFrom(&self.map);
            // }
            // for p in self.polygons.iter() {
            //     p.addTo(&self.map);
            // }

            // let outer_border_territories = self.territory_map.territories
            //     .iter()
            //     .filter(|t| t.number == "OUTER".to_string())
            //     .collect::<Vec<_>>();
            // log!("map_component: changed: 3");
            // let outer_polygon = polyline_from_territory(&outer_border_territories.first().unwrap());
            // self.map.fitBounds(&outer_polygon.getBounds());
            // outer_polygon.addTo(&self.map);
            //log!("map_component: changed: 4");
            true
        //}

    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
       

        html! {
            //<div style="background-color:yellow;height:100%;">
            // TODO: Move this whole header thing into the model.rs
              
                <div class="map map-container component-container"  style="height: calc(100% - 57px);background-color:blue;padding:0;border-width:0;">
                    {self.render_map()}
                </div>
            //</div>
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
