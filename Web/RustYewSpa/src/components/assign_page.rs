#[cfg(debug_assertions)]
const DATA_API_PATH: &str =  "/api/skip/territory-assignment/assignments";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/skip/territory-assignment/assignments";

// This is a good video: https://www.youtube.com/watch?v=2JNw-ftN6js
// This is the GitHub repo: https://github.com/brooks-builds/full-stack-todo-rust-course/blob/1d8acb28951d0a019558b2afc43650ae5a0e718c/frontend/rust/yew/solution/src/api/patch_task.rs

use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
use crate::components::menu_bar::MenuBar;
use crate::components::assignment_form::AssignmentForm;
use crate::components::assign_form::*;
use crate::models::territories::{Territory};
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use leaflet::{LatLng, Map, TileLayer, Polygon, Polyline, Control};
use yew::prelude::*;
use yew::function_component;
use gloo_utils::document;
use gloo_console::log;
use serde_json::json;
use reqwasm::http::{Request};
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
    pub territory_number: String,
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
                    log!(format!("Territory Number: {}", assignment.territory_number));
                    log!(format!("Description: {}", assignment.description));
                    log!(format!("Assignee: {}", assignment.assignee));
                    let uriString: String = format!("{path}?territoryNumber={number}&assigner=wasm_app&assignee=none&albaUserId={assignee}", 
                        path = DATA_API_PATH,
                        number = assignment.territory_number,
                        //descr = assignment.description,
                        assignee = assignment.assignee);

                    let description = assignment.description.clone();
                    let uri: &str = uriString.as_str();

                    let resp = Request::post(uri)
                        .header("Content-Type", "application/json")
                        //.body(format!("{{ 'description': '{description}' }}"))
                        .send()
                        .await
                        .unwrap();

                    if resp.status() != 200 {
                        log!("Sorry the assignment failed.".to_string());
                    } else {
                        log!("Yay the assignment succeeded!".to_string());
                    }
                });
            })
        };

        html! {
            <>
                <MenuBar/>
                <AssignmentForm 
                    territory_number={ctx.props().territory_number.clone()} 
                    assignee_name={ctx.props().assignee_name.clone()}
                    description={ctx.props().description.clone()}/>
                <h3 style={"color:red;"}>{"This page does not work yet! Needs a result form."}</h3>
                <AssignForm {onsubmit} action={Action::Login} />
            </>
        }
    }
}

