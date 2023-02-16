#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/put_territories.json";

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/save";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "PUT";

// This is a good video: https://www.youtube.com/watch?v=2JNw-ftN6js
// This is the GitHub repo: https://github.com/brooks-builds/full-stack-todo-rust-course/blob/1d8acb28951d0a019558b2afc43650ae5a0e718c/frontend/rust/yew/solution/src/api/patch_task.rs

use crate::components::territory_edit_form::*;
use crate::components::menu_bar::MenuBar;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::components::territory_edit_form::TerritoryEditForm;
use crate::functions::document_functions::set_document_title;
//use crate::components::route_stuff::Route;
use gloo_console::log;
use reqwasm::http::{Request, Method};
use serde::Deserialize;
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use yew_router::hooks::use_location;
//use yew_router::prelude::use_navigator;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct TerritoryEditResult {
    pub success: bool,
    pub status: u16,
    pub completed: bool,
}

// App theme
#[derive(Clone, Debug, PartialEq, Deserialize)]
pub struct TerritoryEditPageParameters {
    pub description: String,
    pub group_id: String,
}

#[derive(Properties, PartialEq, Clone)]
pub struct TerritoryEditPageProps {
    pub territory_number: String,
    // pub description: String,
    // pub group_id: String,
}

#[function_component(TerritoryEditPage)]
pub fn territory_edit_page(props: &TerritoryEditPageProps) -> Html { 
    set_document_title("Territory Edit");       
    let state = use_state(|| TerritoryEditResult::default());
    //let navigator = use_navigator().unwrap();

    //let parameters = use_context::<TerritoryEditPageParameters>().expect("no ctx found");
    let location = use_location().expect("Should be a location");
    log!("Query: {}", location.query_str());
    let parameters: TerritoryEditPageParameters = location.query().expect("An object");
    log!("Query.groupid: {}", &parameters.group_id);
    //let parameters = user

    // log!("description: {}", parameters.description);
    // log!("group_id: {}", parameters.group_id);

    let cloned_state = state.clone();
    //let navigator = navigator.clone();
    let onsubmit = Callback::from(move |_modification: TerritoryModification| {
        let cloned_state = cloned_state.clone();
        //let navigator = navigator.clone();
        spawn_local(async move {
            let uri_string: String = format!("{path}", 
                path = DATA_API_PATH);

            let uri: &str = uri_string.as_str();
            
            let method: Method = match ASSIGN_METHOD {
                "PUT" => Method::PUT,
                "GET" => Method::GET,
                &_ =>  Method::GET,
            };

            let territory_number = _modification.territory_number;
            let description = _modification.description;
            let group_id = _modification.group_id;

            let resp = Request::new(uri)
                .method(method)
                .header("Content-Type", "application/json")
                .body(format!("{{ \"TerritoryNumber\": \"{territory_number}\", \"Description\": \"{description}\", \"GroupId\": \"{group_id}\" }}"))
                .send()
                .await
                .expect("A result from the endpoint");

            // let link_contract: TerritoryLinkContract = if resp.status() == 200 {
            //     resp.json().await.unwrap()
            // } else {
            //     TerritoryLinkContract::default()
            // };
            
            let result = TerritoryEditResult {
                success: (resp.status() == 200),
                status: resp.status(),
                completed: true,
            };

            cloned_state.set(result);
            
            // TODO: Check for errors
            // if resp.status() == 200 {
            //     navigator.push(&Route::Map);
            // }
        });
    });

    html! {
        <>
            <MenuBarV2>
                <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                    <li class={"nav-item"}>
                        <MapPageLink />
                    </li>  
                </ul>
            </MenuBarV2>
            // <Alert style={Color::Primary}>
            //     {"This is a primary alert!"}
            // </Alert>
            <TerritoryEditForm {onsubmit} 
                territory_number={props.territory_number.clone()}
                description={parameters.description}
                group_id={parameters.group_id}
            />

            if state.completed {
                <div class={"container"}>
                    if state.success {
                        <div>
                            <p style={"color:blue;"}>{"Success"}</p>
                        </div>
                    } else {
                        <div>
                            <p style={"color:red;"}>{"Failed"}</p>
                        </div>
                    }
                </div>
            }
        </>
    }
}
