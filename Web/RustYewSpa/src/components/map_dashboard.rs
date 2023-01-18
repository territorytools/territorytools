use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
use crate::components::map_menu::MapMenu;
use crate::models::territories::{Territory};
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use leaflet::{LatLng, Map, TileLayer, Polygon, Polyline, Control};
use reqwasm::http::{Request};
use yew::prelude::*;
use gloo_utils::document;
use gloo_console::log;
use gloo_timers::callback::Timeout;
use serde::{Serialize, Deserialize};
//use js_sys::{Array, Date};
use web_sys::{
    Document,
    Element,
    HtmlElement,
    Window,
    Node
};

#[derive(PartialEq, Properties, Clone)]
pub struct MapDashboardProperties {
    pub available: i32,
    pub signed_out: i32,
    pub completed: i32,
    pub total: i32,
    pub hidden: i32
}

#[function_component(MapDashboard)]
pub fn map_dashboard(props: &MapDashboardProperties) -> Html {
    html! {
        <div style={"
            background-color: white;
            font-family: arial;
            border-radius: 5px;
            display: block;
            border-style: solid;
            border-color: blue;
            border-width: 1px;
            position: absolute;
            height: auto;
            max-height: 60px;
            bottom: 1vh;
            right: 1vh;
            margin-right: 1vh; 
            width: auto; 
            z-index: 1000; /*Just above 'Leaflet' in the bottom right corner*/
            "}>
        //<p style={"margin:10px;background-color:white;"}>
            <MapMenu>        
                <TerritorySummary 
                available={props.available}
                signed_out={props.signed_out}
                completed={props.completed}
                total={props.total}
                hidden={props.hidden} />      
            </MapMenu>
        </div>
    }
}
