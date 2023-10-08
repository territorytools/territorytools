use crate::libs::leaflet::{
    //DivIcon,
    LatLng, 
    LatLngBounds,
    Map,
    //Marker,
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
    stage_as_of_date,
    stage_color,
};
use crate::components::popup_content::PopupContentOptions;

use wasm_bindgen::{prelude::*, JsCast};
use gloo_utils::document;
use web_sys::{Element, HtmlElement, Node, SvgPathElement};
use yew::{html::ImplicitClone, prelude::*};
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
struct PolylineOptions {
    color: String,
    opacity: f32,
}

#[derive(Properties, PartialEq, Clone)]
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
    pub popup_content_options: PopupContentOptions,
}

impl Default for MapModel {
    fn default() -> Self {
        MapModel {
            territories: vec![],
            territories_is_loaded: false,
            local_load: false,
            zoom: 1.0,
            lat: 0.0,
            lon: 0.0,
            group_visible: "".to_string(),
            user_roles: Some("".to_string()),
            link_grants: Some("".to_string()),
            popup_content_options: PopupContentOptions::default(),
        }
    }
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
    pub territory_map: MapModel,
    pub tpolygons: Vec<TerritoryPolygon>,
    pub search: String,
    pub as_of_date: Option<String>,
}

pub struct MapComponent {
    map: Map,
    container: HtmlElement,
    territory_map: MapModel,
    polygons: Vec<Polygon>,
    tpolygons: Vec<TerritoryPolygon>,
    id_list: Vec<i32>,
    layer_group: LayerGroup,
    selected: Vec<String>, // TODO: Use territory ids (ints) instead of tags
    multi_select: bool,
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
            selected: vec![],     
            multi_select: false,
            // TODO: Try default()
        }
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
            add_tile_layer(&self.map);
        } 
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg{
            Msg::MouseClick(x, y) => {
                //log!(format!("mc:update:MouseClick({}, {})", x, y));
                let lat_lng = &self.map.containerPointToLatLng(
                    &Point::new(x as u32, y as u32));
                let as_of_date = ctx.props().as_of_date.clone();

                for tp in self.tpolygons.clone().iter() {
                    let mut vertices: Vec<GeoPoint> = vec![];
                    for v in tp.border.iter() {
                        vertices.push(GeoPoint { x: v.lon as f64, y: v.lat as f64});
                    }

                    let inside = is_inside_polygon(vertices, &GeoPoint {
                        x: lat_lng.lng(), 
                        y: lat_lng.lat()
                    });
    
                    if inside { 
                        let path: Element = document()
                            .get_element_by_id(format!("territory-id-{}", tp.territory_id.clone()).as_str())
                            .expect("Cannot find path with territory-id-x");
                        
                        let path: SvgPathElement = path.dyn_into().unwrap();
                        
                        let territories = self
                            .territory_map
                            .territories
                            .iter()
                            .map(|t| t.to_owned())
                            .filter(|t| t.number == tp.territory_id.clone())
                            .collect::<Vec<_>>();

                        //let territory_color = territory_color(&territories[0], as_of_date);
                        let stage = stage_as_of_date(&territories[0], as_of_date.clone().unwrap_or_default());
                        let territory_color = stage_color(stage.as_str());

                        if self.selected.contains(&tp.territory_id.clone()) {
                            let index = self.selected.iter().position(|x| *x == tp.territory_id.clone()).unwrap();
                            self.selected.remove(index);
                            let _ = path.set_attribute("fill", territory_color.as_str());
                            let _ = path.set_attribute("stroke", territory_color.as_str());
                        } else {
                            self.selected.push(tp.territory_id.clone());
                            // TODO: New feature, only when selecting, no button yet
                            if self.multi_select {
                                let _ = path.set_attribute("fill", "black");
                                let _ = path.set_attribute("stroke", "white");
                                // TODO: Bring it to the front with z?
                            }
                        }

                        //log!(format!("mc:update:MouseClick: inside:yes: self.selected.len() {}", self.selected.len()));

                        return true;
                    } 
                }

                false
            }
        }
    }

    fn changed(&mut self, ctx: &Context<Self>, _old_props: &Self::Properties) -> bool {
        let props = ctx.props();
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
            popup_content_options: PopupContentOptions {
                edit_territory_button_enabled: true,
                territory_open_enabled: false,
                show_stage: false,
                as_of_date: None,
            }
        };

        self.tpolygons = props.tpolygons.clone();

        for id in self.id_list.iter() {
            self.layer_group.removeLayer_byId(*id);
        }

        self.polygons.clear();
        self.id_list.clear();
        self.layer_group = LayerGroup::new();

        for tp in self.tpolygons.iter() {
            let selected: bool = self.selected.contains(&tp.tooltip_text.clone());
            let p = polygon_from_territory_polygon(&tp, selected);
            self.polygons.push(p); // TODO: I don't think we need this

            //tp.color = "red".to_string();
            let p = polygon_from_territory_polygon(&tp, selected);
            p.addTo_LayerGroup(&self.layer_group);

            let layer_id = self.layer_group.getLayerId(&p);
            self.id_list.push(layer_id);
            
            /*
            if tp.border.len() > 0 {
                let avg_lat: f32 = tp.border.iter().map(|v| v.lat as f32).sum::<f32>() / tp.border.len() as f32;
                let avg_lon: f32 = tp.border.iter().map(|v| v.lon as f32).sum::<f32>() / tp.border.len() as f32;
                //let marker_point = LatLng::new(tp.border[0].lat.into(), tp.border[0].lon.into());
                let marker_point = LatLng::new(avg_lat.into(), avg_lon.into());

                let icon = DivIcon::new({
                    &serde_wasm_bindgen::to_value( &DivIconOptions {
                        html: format!("<div style='pointer-events:none;'><span style='background-color:yellow;'>{}</span></div>", &tp.tooltip_text.clone()),
                        class_name: "tt-map-marker".to_string()
                    }).expect("Unable to serialzie DivIcon options")
                });

                let marker_options =  &serde_wasm_bindgen::to_value(&MarkerOptions {
                    //icon: icon,
                    //color: if selected { "#00A".to_string() } else { tpoly.color.to_string() },
                });
                let marker = Marker::new_with_options(
                    &marker_point, 
                    &serde_wasm_bindgen::to_value(&MarkerOptions {
                        //icon: icon,
                        //color: if selected { "#00A".to_string() } else { tpoly.color.to_string() },
                    }).expect("Unable to serialize marker options")
                );
                marker.setIcon(&icon);
                marker.addTo(&self.map);
            }*/
        }

        self.layer_group.addTo(&self.map);

        let sw = get_southwest_corner(self.tpolygons.clone());
        let ne = get_northeast_corner(self.tpolygons.clone());
        
        let bounds = LatLngBounds::new(
            &LatLng::new(ne.lat as f64, ne.lon as f64),
            &LatLng::new(sw.lat as f64, sw.lon as f64)
        );
        
        self.map.fitBounds(&bounds);

        true
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let link = ctx.link().clone();
        let map_cover_click = {
            let link = link.clone();
            Callback::from(move |event: MouseEvent| {
                // event.stop_propagation();
                // event.prevent_default();
                link.send_message(Msg::MouseClick(event.x(), event.y()-57));
            })
        };


        html! {
            //<div style="background-color:yellow;height:100%;">
            // TODO: Move this whole header thing into the model.rs
              
                <div 
                    class="map map-container component-container"  
                    style="height: calc(100% - 57px);background-color:blue;padding:0;border-width:0;xxxx-removed-xxxx-pointer-events:none;"
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

#[derive(Default)]
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

