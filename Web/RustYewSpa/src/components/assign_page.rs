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

use crate::components::menu_bar::MenuBar;
use crate::components::assign_form::*;
use crate::components::email_section::EmailSection;
use crate::components::sms_section::SmsSection;
use crate::models::territory_links::TerritoryLinkContract;
use reqwasm::http::{Request, Method};
use wasm_bindgen_futures::spawn_local;
use wasm_bindgen::JsCast;
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
                //descr = assignment.description,
                assignee = assignment.assignee);

            //let description = assignment.description.clone();
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

            // let window = web_sys::window().expect("no global `window` exists");
            // let document = window.document().expect("should have a document on window");
            // let result_success = document
            //     .get_element_by_id("result-success")
            //     .expect("should have #to_be_hidden on the page")
            //     .dyn_into::<web_sys::HtmlElement>()
            //     .expect("#to_be_hidden should be an `HtmlElement`");
            
            // let result_failure = document
            //     .get_element_by_id("result-failure")
            //     .expect("should have #to_be_hidden on the page")
            //     .dyn_into::<web_sys::HtmlElement>()
            //     .expect("#to_be_hidden should be an `HtmlElement`");

            if resp.status() != 200 {
                //hide(result_success);
                
                //let inner_text = format!("Failed to assign territory! Code: {}", resp.status());
                
                //result_failure.set_inner_text(&inner_text);

                let result = AssignmentResult {
                    success: false,
                    link_contract: TerritoryLinkContract::default(),
                    status: resp.status(),
                    completed: true,
                };
                
                cloned_state.set(result);

                //show(result_failure);
            } else {
                //hide(result_failure);

                let link_contract: TerritoryLinkContract = resp
                    .json()
                    .await
                    .unwrap();
                
                let result = AssignmentResult {
                    success: true,
                    link_contract: link_contract,
                    status: resp.status(),
                    completed: true,
                };

                cloned_state.set(result);
                //show(result_success);
            }
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


fn show(element: web_sys::HtmlElement) {
    element
        .style()
        .set_property("display", "block")
        .expect("'display' should have been set to 'block'")
}

fn hide(element: web_sys::HtmlElement) {
    element
        .style()
        .set_property("display", "none")
        .expect("'display' should have been set to 'none'")
}

