#[cfg(debug_assertions)]
const DATA_API_PATH: &str =  "/api/assignments";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/assignments";

// This is a good video: https://www.youtube.com/watch?v=2JNw-ftN6js
// This is the GitHub repo: https://github.com/brooks-builds/full-stack-todo-rust-course/blob/1d8acb28951d0a019558b2afc43650ae5a0e718c/frontend/rust/yew/solution/src/api/patch_task.rs

use crate::components::menu_bar::MenuBar;
use crate::components::assign_form::*;
use crate::components::route_stuff::Route;
use gloo_console::log;
use reqwasm::http::{Request};
use wasm_bindgen_futures::spawn_local;
use wasm_bindgen::JsCast;
use yew::prelude::*;
use yew_router::prelude::use_navigator;

#[derive(Properties, PartialEq, Clone)]
pub struct AssignPageProps {
    pub territory_number: String,
    pub assignee_name: String,
    pub description: String,
}

#[function_component(AssignPage)]
pub fn assign_form(props: &AssignPageProps) -> Html {        
    //let history = use_navigator().unwrap();
    let navigator = use_navigator().unwrap();
    let onsubmit = {
        //let store_dispatch = store_dispatch.clone();
        Callback::from(move |assignment: TerritoryAssignment| {
            //let navigator = use_navigator().unwrap();
            //let history = history.clone();
            let navigator = navigator.clone();
            // // //let navigator = navigator.clone();
            //let store_dispatch = store_dispatch.clone();

            spawn_local(async move {
                //let result = api::login(user.username, user.password).await;
                //history.push(&Route::Home);
                //login_reducer(result, store_dispatch);
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

                let resp = Request::post(uri)
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
                    //navigator.push(&Route::NotFound);
                    show(result_failure);
                    hide(result_success);
                } else {
                    log!("Yay the assignment succeeded!".to_string());
                    //navigator.push(&Route::Map);
                    show(result_success);
                    hide(result_failure);
                }
            });
        })
    };

    html! {
        <>
            <MenuBar/>
            // <AssignmentForm 
            //     territory_number={ctx.props().territory_number.clone()} 
            //     assignee_name={ctx.props().assignee_name.clone()}
            //     description={ctx.props().description.clone()}/>
            // <h3 style={"color:red;"}>{"This page does not work yet! Needs a result form."}</h3>
            <AssignForm {onsubmit} 
                action={Action::Login} 
                territory_number={props.territory_number.clone()}
                description={props.description.clone()}
                assignee_alba_id={"0"}
            />

            <div class={"container"}>
                //<div id={"assignment-buttons"}>
                    <div id={"result-success"} style={"display:none;color:blue;"}>{"Success"}</div>
                    <div id={"result-failure"} style={"display:none;color:red;"}>{"Failed"}</div>
                //</div>
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