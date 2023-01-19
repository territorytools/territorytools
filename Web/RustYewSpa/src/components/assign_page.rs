use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
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

#[derive(Properties, PartialEq, Clone)]
pub struct AssignPageProps {
    pub id: String,
}

pub struct AssignPage;
//  {
//     props: MapMenuProps,
//     //link: ComponentLink<Self>,
// }

pub enum AssignPageMsg {

}

impl Component for AssignPage {
    type Message = AssignPageMsg;
    type Properties = AssignPageProps;

    fn create(_ctx: &Context<Self>) -> Self {
        Self {}
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        true
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        html! {
           <div>
           <h3>{"Assignment Page"}</h3>
           <p>{"You picked id:"} { ctx.props().id.clone() }</p>
           </div>
        }
    }
}
