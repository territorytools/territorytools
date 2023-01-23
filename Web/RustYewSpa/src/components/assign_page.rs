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
use crate::models::territory_links::TerritoryLinkContract;
use gloo_console::log;
use reqwasm::http::{Request, Method};
use wasm_bindgen_futures::spawn_local;
use wasm_bindgen::JsCast;
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone)]
pub struct AssignPageProps {
    pub territory_number: String,
    pub assignee_name: String,
    pub description: String,
}

#[function_component(AssignPage)]
pub fn assign_form(props: &AssignPageProps) -> Html {        
    //let link_contract: TerritoryLinkContract::default; 
    let state = use_state(|| TerritoryLinkContract::default());

    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |assignment: TerritoryAssignment| {
        //let store_dispatch = store_dispatch.clone();
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            //let result = api::login(user.username, user.password).await;
            log!(format!("Territory Number: {}", assignment.territory_number));
            log!(format!("Description: {}", assignment.description));
            log!(format!("Assignee: {}", assignment.assignee));
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

            let window = web_sys::window().expect("no global `window` exists");
            let document = window.document().expect("should have a document on window");
            let result_success = document
                .get_element_by_id("result-success")
                .expect("should have #to_be_hidden on the page")
                .dyn_into::<web_sys::HtmlElement>()
                .expect("#to_be_hidden should be an `HtmlElement`");
            
            let result_failure = document
                .get_element_by_id("result-failure")
                .expect("should have #to_be_hidden on the page")
                .dyn_into::<web_sys::HtmlElement>()
                .expect("#to_be_hidden should be an `HtmlElement`");

            if resp.status() != 200 {
                log!("Sorry the assignment failed.".to_string());
                hide(result_success);
                
                let inner_text = format!("Failed to assign territory! Code: {}", resp.status());
                
                result_failure.set_inner_text(&inner_text);
                show(result_failure);
            } else {
                log!("Yay the assignment succeeded!".to_string());
                    
                let link_contract: TerritoryLinkContract = resp
                    .json()
                    .await
                    .unwrap();

                
                // territory_number = link_contract.territory_number.clone();
                // assignee_email = link_contract.assignee_email.clone();
                // territory_uri = link_contract.territory_uri.clone();
                hide(result_failure);

                let result_link = document
                    .get_element_by_id("result-link")
                    .expect("should have #result-link on the page")
                    .dyn_into::<web_sys::HtmlElement>()
                    .expect("#result-link should be an `HtmlElement`");
                    
                result_link.set_inner_text(&link_contract.territory_uri);
                result_link
                    .set_attribute("href", &link_contract.territory_uri)
                    .expect("Attribute href should have been set");
                
                let sms_link_href = format!(
                    "sms://{assignee_phone}?body=Territory%20{territory_number}%20{territory_uri}",
                    assignee_phone = link_contract.assignee_phone,
                    territory_number = link_contract.territory_number,
                    territory_uri = link_contract.territory_uri,
                );
                
                let sms_link = document
                    .get_element_by_id("sms-link")
                    .expect("should have #sms-link on the page")
                    .dyn_into::<web_sys::HtmlElement>()
                    .expect("#sms-link should be an `HtmlElement`");
                
                sms_link
                    .set_attribute("href", &sms_link_href)
                    .expect("Attribute href should have been set");
                
                let sms_number = document
                    .get_element_by_id("sms-number")
                    .expect("should have #sms-number on the page")
                    .dyn_into::<web_sys::HtmlElement>()
                    .expect("#sms-link should be an `HtmlElement`");

                sms_number
                    .set_attribute("value", &link_contract.assignee_phone)
                    .expect("Value should have been set");
                
                //let contract = cloned_state.deref().clone();    
                cloned_state.set(link_contract);
                
                // let email_link_href = format!(
                //     "mailto://{assignee_email}?body=Territory%20{territory_number}%20{territory_uri}",
                //     assignee_email = link_contract.assignee_email,
                //     territory_number = link_contract.territory_number,
                //     territory_uri = link_contract.territory_uri,
                // );

                // let email_link = document
                //     .get_element_by_id("email-link")
                //     .expect("should have #email-link on the page")
                //     .dyn_into::<web_sys::HtmlElement>()
                //     .expect("#email-link should be an `HtmlElement`");
            
                // email_link
                //     .set_attribute("href", &email_link_href)
                //     .expect("Attribute href should have been set");
                
                // let email_address = document
                //     .get_element_by_id("email-address")
                //     .expect("should have #email-address on the page")
                //     .dyn_into::<web_sys::HtmlElement>()
                //     .expect("#email-link should be an `HtmlElement`");

                // email_address
                //     .set_attribute("value", &link_contract.assignee_email)
                //     .expect("Value should have been set");
                
                show(result_success);
            }
        });
    });

    html! {
        <>
            <MenuBar/>
            <AssignForm {onsubmit} 
                action={Action::Login} 
                territory_number={props.territory_number.clone()}
                description={props.description.clone()}
                assignee_alba_id={"0"}
            />

            <div class={"container"}>
                <div id={"result-failure"} style={"display:none;color:red;"}>{"Failed"}</div>
                <div id={"result-success"} style={"display:none;"}>
                    <p style={"color:blue;"}>{"Success"}</p>
                    <a id={"result-link"} style={"color:blue;"} href={"#"}></a>
                    <div id={"sms-section"} class={"form-group"}>
                        <label for={"sms-number"}>{"Send as text message:"}</label>
                        <div class={"input-group-append"}>
                            <input id={"sms-number"} name={"phoneNumber"} type={"text"} class={"form-control"} readonly={true} />
                            <a class={"btn btn-primary"} id={"sms-link"} href={"#"}>{"Send"}</a>
                        </div>
                    </div>
                    <EmailSection 
                        territory_number={state.territory_number.clone()}
                        assignee_email={state.assignee_email.clone()}
                        territory_uri={state.territory_uri.clone()}
                    />
                </div>
            </div>
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
