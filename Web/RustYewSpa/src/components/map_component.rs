use gloo_utils::document;
//use leaflet::{LatLng, Map, TileLayer};
use crate::libs::leaflet::{LatLng, Map, TileLayer};
use wasm_bindgen::{prelude::*, JsCast};
use web_sys::{Element, HtmlElement, Node};
use yew::{html::ImplicitClone, prelude::*};
use crate::components::territory_map::TerritoryMapModel;
use crate::models::territories::Territory;
use gloo_console::log;
use reqwasm::http::Request;
use yew::prelude::*;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

pub enum Msg {
    LoadBorders(TerritoryMapModel),
}

pub struct MapComponent {
    map: Map,
    lat: Point,
    container: HtmlElement,
    territory_map: TerritoryMapModel,
}

#[derive(Copy, Clone, Debug, PartialEq)]
pub struct Point(pub f64, pub f64);

#[derive(PartialEq, Clone, Debug)]
pub struct City {
    pub name: String,
    pub lat: Point,
}

impl ImplicitClone for City {}

#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    pub city: City,
    pub territory_map: TerritoryMapModel,
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
            territory_map: TerritoryMapModel::default(),
        }
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
            self.map.setView(&LatLng::new(self.lat.0, self.lat.1), 11.0);
            add_tile_layer(&self.map);
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, _msg: Self::Message) -> bool {
        // match msg {
        //     Msg::LoadBorders(territoryMap) => {
        //         wasm_bindgen_futures::spawn_local(async move {
        //             let group_id: String = "2".to_string();//group_id;
        //             let uri: String =
        //                 format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

        //             let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
        //                 .send()
        //                 .await
        //                 .unwrap()
        //                 .json()
        //                 .await
        //                 .unwrap();

        //             let m = TerritoryMapModel {
        //                 territories: fetched_territories,
        //                 territories_is_loaded: true,
        //                 local_load: false,
        //                 lat: 47.66,
        //                 lon: -122.20,
        //                 zoom: 10.0,
        //                 group_visible: String::from("*"),
        //             };

        //             log!("Map Component got territory borders!");

        //             self.territoryMap = m;
        //         });
        //     }
        // }
        // true
        false
    }

    fn changed(&mut self, ctx: &Context<Self>, old_props: &Self::Properties) -> bool {
        let props = ctx.props();

        if self.lat == props.city.lat {
            false
        } else {
            self.lat = props.city.lat;
            self.map.setView(&LatLng::new(self.lat.0, self.lat.1), 11.0);
            
            self.territory_map = TerritoryMapModel {
                territories: props.territory_map.territories.clone(),
                territories_is_loaded: true,
                local_load: false,
                lat:  props.territory_map.lat,
                lon: props.territory_map.lon,
                zoom: props.territory_map.zoom,
                group_visible: props.territory_map.group_visible.clone(),
            };

            log!("map_component.update: territory_map loaded");

            //self.map.setView(&LatLng::new(self.lat.0, self.lat.1), 11.0);
            true
        }

    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
        html! {
            <div class="map-container component-container">
                {self.render_map()}
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