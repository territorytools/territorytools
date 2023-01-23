#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/post_assignments.json";

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/assignments";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "POST";

// This is a good video: https://www.youtube.com/watch?v=2JNw-ftN6js
// This is the GitHub repo: https://github.com/brooks-builds/full-stack-todo-rust-course/blob/1d8acb28951d0a019558b2afc43650ae5a0e718c/frontend/rust/yew/solution/src/api/patch_task.rs

use crate::components::assign_form::*;
use crate::components::email_section::EmailSection;
use crate::components::menu_bar::MenuBar;
use crate::components::sms_section::SmsSection;
use crate::models::territory_links::TerritoryLinkContract;
use reqwasm::http::{Request, Method};
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AssignmentResult {
    pub link_contract: TerritoryLinkContract,
    pub success: bool,
    pub status: u16,
    pub completed: bool,
}

#[derive(Properties, PartialEq, Clone)]
pub struct AssignPageProps {
    pub territory_number: String,
    pub assignee_name: String,
    pub description: String,
}

#[function_component(AssignPage)]
pub fn assign_form(props: &AssignPageProps) -> Html {        
    let state = use_state(|| AssignmentResult::default());

    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |assignment: TerritoryAssignment| {
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            let uri_string: String = format!("{path}?territoryNumber={number}&albaUserId={assignee}", 
                path = DATA_API_PATH,
                number = assignment.territory_number,
                assignee = assignment.assignee);

            let uri: &str = uri_string.as_str();
            
            let method: Method = match ASSIGN_METHOD {
                "POST" => Method::POST,
                "GET" => Method::GET,
                &_ =>  Method::GET,
            };

            let resp = Request::new(uri)
                .method(method)
                .header("Content-Type", "application/json")
                //.body(format!("{{ 'description': '{description}' }}"))
                .send()
                .await
                .unwrap();

            let link_contract: TerritoryLinkContract = if resp.status() == 200 {
                resp.json().await.unwrap()
            } else {
                TerritoryLinkContract::default()
            };
            
            let result = AssignmentResult {
                success: (resp.status() == 200),
                link_contract: link_contract,
                status: resp.status(),
                completed: true,
            };

            cloned_state.set(result);
        });
    });

    html! {
        <>
            <MenuBar/>
            <AssignForm {onsubmit} 
                territory_number={props.territory_number.clone()}
                description={props.description.clone()}
                assignee_alba_id={"0"}
            />

            if state.completed {
                <div class={"container"}>
                    if state.success {
                        <div id={"result-success"} style={"display:block;"}>
                            <p style={"color:blue;"}>{"Success"}</p>
                            <a 
                                style={"color:blue;margin-bottom:10px;"} 
                                href={state.link_contract.territory_uri.clone()}>
                                {state.link_contract.territory_uri.clone()}
                            </a>
                            <SmsSection
                                territory_number={state.link_contract.territory_number.clone()}
                                assignee_phone={state.link_contract.assignee_phone.clone()}
                                territory_uri={state.link_contract.territory_uri.clone()}
                            />
                            <EmailSection 
                                territory_number={state.link_contract.territory_number.clone()}
                                assignee_email={state.link_contract.assignee_email.clone()}
                                territory_uri={state.link_contract.territory_uri.clone()}
                            />
                        </div>
                    } else {
                        <div id={"result-failure"} style={"display:block;color:red;"}>{"Failed"}</div>
                    }
                </div>
            }
        </>
    }
}
