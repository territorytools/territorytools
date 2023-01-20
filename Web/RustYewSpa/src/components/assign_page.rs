use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
use crate::components::menu_bar::MenuBar;
use crate::components::assignment_form::AssignmentForm;
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
            <>
                <MenuBar/>
                <AssignmentForm territory_number={ctx.props().id.clone()} />
                <h3 style={"color:red;"}>{"This page does not work yet! Needs a result form."}</h3>
            </>
        }
    }
}

