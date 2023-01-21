#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
use crate::components::menu_bar::MenuBar;
use crate::components::assignment_form::AssignmentForm;
use crate::components::assign_form::*;
use crate::models::territories::{Territory};
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use leaflet::{LatLng, Map, TileLayer, Polygon, Polyline, Control};
use reqwasm::http::{Request};
use yew::prelude::*;
use yew::function_component;
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
use wasm_bindgen_futures::spawn_local;
//use yewdux::prelude::*;
use yew_router::prelude::use_navigator;

#[derive(Properties, PartialEq, Clone)]
pub struct AssignPageProps {
    pub id: String,
    pub assignee_name: String,
    pub description: String,
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
        //let history = use_navigator().unwrap();
        let onsubmit = {
            //let store_dispatch = store_dispatch.clone();
            Callback::from(move |assignment: TerritoryAssignment| {
                //let history = history.clone();
                //let store_dispatch = store_dispatch.clone();
    
                spawn_local(async move {
                    //let result = api::login(user.username, user.password).await;
                    //history.push(&Route::Home);
                    //login_reducer(result, store_dispatch);
                    log!(format!("Description: {}", assignment.description));
                    let uri: &str = DATA_API_PATH;

                    let fetched_territories: Vec<Territory> = Request::get(uri)
                        .send()
                        .await
                        .unwrap()
                        .json()
                        .await
                        .unwrap();                    
                });
            })
        };

        html! {
            <>
                <MenuBar/>
                <AssignmentForm 
                    territory_number={ctx.props().id.clone()} 
                    assignee_name={ctx.props().assignee_name.clone()}
                    description={ctx.props().description.clone()}/>
                <h3 style={"color:red;"}>{"This page does not work yet! Needs a result form."}</h3>
                <AssignForm {onsubmit} />
            </>
        }
    }
}

