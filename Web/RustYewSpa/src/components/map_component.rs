use crate::libs::leaflet::{
    LatLng, 
    LatLngBounds,
    Map,
    Point,
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
// use geo::{Coordinate, LineString, Polygon as GeoPolygon};
// use geo::algorithm::contains::Contains;
// use geo_types::{coord};

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
    pub edit_territory_button_enabled: bool,
    pub territory_open_enabled: bool,
}

impl ImplicitClone for MapModel {}

pub enum Msg {
    MouseClick(i32, i32),
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
    // pub mouse_click_x: i32,
    // pub mouse_click_y: i32,
}

pub struct MapComponent {
    map: Map,
    container: HtmlElement,
    territory_map: MapModel,
    polygons: Vec<Polygon>,
    tpolygons: Vec<TerritoryPolygon>,
    id_list: Vec<i32>,
    layer_group: LayerGroup,
    mouse_click_x: i32,
    mouse_click_y: i32,
    selected: Vec<String>,
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
            mouse_click_x: 0,
            mouse_click_y: 0,
            selected: vec![],
        }
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
            //self.map.setView(&LatLng::new(self.territory_map.lat, self.territory_map.lat), 11.0);
            add_tile_layer(&self.map);
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg{
            Msg::MouseClick(x, y) => {
                //log!(format!("map_component:update: Map cover clicked {}, {}", x, y));
                let lat_lng = &self.map.containerPointToLatLng(
                    &Point::new(x as u32, y as u32));
                log!(format!("map_component:update: Map cover clicked LatLng {}, {}", lat_lng.lat(), lat_lng.lng()));


                for tp in self.tpolygons.clone().iter() {
                    let mut vertices: Vec<GeoPoint> = vec![];
                    for v in tp.border.iter() {
                        vertices.push(GeoPoint { x: v.lon as f64, y: v.lat as f64});
                    }
                    let inside = is_inside_polygon(vertices, &GeoPoint {x: lat_lng.lng() as f64, y: lat_lng.lat() as f64});
    
                    if inside { 
                        log!(format!("map_component:update: Map cover clicked Inside {} HTML: {}", inside, tp.tooltip_text.clone()));
                        self.selected.push(tp.tooltip_text.clone());
                        let mut new_tpolygons: Vec<TerritoryPolygon> = vec![];
                        for t in self.tpolygons.clone().iter() {
                            if t.tooltip_text == tp.tooltip_text {
                                log!(format!("mc:inside: found one tooltip: {}", t.tooltip_text.clone()));
                                let altered_tpolygon = TerritoryPolygon {
                                    layer_id: tp.layer_id,
                                    color: "red".to_string(), //tp.color.clone(),
                                    opacity: tp.opacity,
                                    border: tp.border.clone(),
                                    popup_html: tp.popup_html.clone(), 
                                    tooltip_text: tp.tooltip_text.clone(),
                                };
                                new_tpolygons.push(altered_tpolygon);
                                
                            } else {
                                new_tpolygons.push(t.clone());
                            }
                        }
                        self.tpolygons = new_tpolygons.clone();


                        // self.territory_map = MapModel {
                        //     territories: self.territory_map.territories.clone(),
                        //     territories_is_loaded: self.territory_map.territories_is_loaded,
                        //     local_load: self.territory_map.local_load,
                        //     lat:  self.territory_map.lat,
                        //     lon: self.territory_map.lon,
                        //     zoom: self.territory_map.zoom,
                        //     group_visible: self.territory_map.group_visible.clone(),
                        //     link_grants: self.territory_map.link_grants.clone(),
                        //     user_roles: self.territory_map.user_roles.clone(),
                        //     edit_territory_button_enabled: self.territory_map.edit_territory_button_enabled,
                        //     territory_open_enabled: self.territory_map.territory_open_enabled,
                        // };
                        return true;
                    } 
                    //return true;
                }



                false
            }
        }
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
                edit_territory_button_enabled: true,
                territory_open_enabled: false,
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
            log!(format!("mc:selected.len(): {}", self.selected.len()));
            for tp in self.tpolygons.iter() {
                
                let selected: bool = self.selected.contains(&tp.tooltip_text.clone());
                let p = polygon_from_territory_polygon(&tp, selected);
                self.polygons.push(p); // TODO: I don't think we need this

                //tp.color = "red".to_string();
                let p = polygon_from_territory_polygon(&tp, selected);
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

    fn view(&self, ctx: &Context<Self>) -> Html {
       
        ////let mouse_click_model_clone = mouse_click_model.clone();
        //let self_clone = &self;
        let link = ctx.link().clone();
        let map_cover_click = {
            ////let mouse_click_model_clone = mouse_click_model_clone.clone();
            ////let leaflet_map_clone = leaflet_map_clone.clone();
            //et self_clone = &self_clone;
            let link = link.clone();
            Callback::from(move |event: MouseEvent| {
                //print_click_lat_lng(leaflet_map.clone());
                log!(format!("map_component:view: Map cover clicked {}, {}", event.x(), event.y()-57));
                //link.send_message(Msg::MouseEvent(event.clone()));
                
                link.send_message(Msg::MouseClick(event.x(), event.y()-57));
                
                
                // let mouse_thing = MouseClickModel {
                //     mouse_click_x: event.x(),
                //     mouse_click_y: event.y(),
                // };
                ////mouse_click_model_clone.set(mouse_thing);
                
                // TODO: Figure this out, leafelet_map gets moved here
                // let latLng = &self_clone.map.layerPointToLatLng(
                //     &Point::new(
                //         event.x() as u32, 
                //         event.y() as u32));
            })
        };

        html! {
            //<div style="background-color:yellow;height:100%;">
            // TODO: Move this whole header thing into the model.rs
              
                <div 
                    class="map map-container component-container"  
                    style="height: calc(100% - 57px);background-color:blue;padding:0;border-width:0;"
                    onclick={map_cover_click}>
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

struct GeoPoint {
    x: f64,
    y: f64,
}

fn is_inside_polygon(vertices: Vec<GeoPoint>, point: &GeoPoint) -> bool {
    let n = vertices.len();
    if n < 3 {
        return false; // A polygon must have at least 3 vertices.
    }

    let mut inside = false;
    let mut j = n - 1;

    for i in 0..n {
        let vi = &vertices[i];
        let vj = &vertices[j];

        if (vi.y > point.y) != (vj.y > point.y)
            && point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x
        {
            inside = !inside;
        }

        j = i;
    }

    inside
}

