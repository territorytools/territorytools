#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/put_territories.json";

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "PUT";

// This is a good video: https://www.youtube.com/watch?v=2JNw-ftN6js
// This is the GitHub repo: https://github.com/brooks-builds/full-stack-todo-rust-course/blob/1d8acb28951d0a019558b2afc43650ae5a0e718c/frontend/rust/yew/solution/src/api/patch_task.rs

use crate::components::territory_edit_form::*;
use crate::components::menu_bar::MenuBar;
use crate::components::territory_edit_form::TerritoryEditForm;
use reqwasm::http::{Request, Method};
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct TerritoryEditResult {
    pub success: bool,
    pub status: u16,
    pub completed: bool,
}

#[derive(Properties, PartialEq, Clone)]
pub struct TerritoryEditPageProps {
    pub territory_number: String,
    // pub description: String,
    // pub group_id: String,
}

#[function_component(TerritoryEditPage)]
pub fn territory_edit_page(props: &TerritoryEditPageProps) -> Html {        
    let state = use_state(|| TerritoryEditResult::default());

    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |_modification: TerritoryModification| {
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            let fetched_territory: Territory = Request::get("/data/territory_modification.json")
                .send()
                .await
                .unwrap()
                .json()
                .await
                .unwrap();
            

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
                .body(format!("{{ 'territory_number': '{territory_number}', 'description': '{description}', 'group_id': '{group_id}' }}"))
                .send()
                .await
                .unwrap();

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
        });
    });

    html! {
        <>
            <MenuBar/>
            <TerritoryEditForm {onsubmit} 
                territory_number={props.territory_number.clone()}
                // description={props.description.clone()}
                // group_id={props.group_id.clone()}
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
