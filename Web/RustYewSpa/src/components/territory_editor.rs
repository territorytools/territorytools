//use crate::components::menu_bar::MenuBar;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::TerritorySearchLink;
use crate::models::territories::{Territory, TerritoryEditRequest};
use crate::models::territory_links::TerritoryLinkContract;
use crate::functions::document_functions::set_document_title;

use gloo_console::log;
use reqwasm::http::{Request, Method};
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::hooks::use_location;
//use serde_json::ser::to_string;

#[cfg(debug_assertions)]
const PUT_TERRITORY_API_PATH: &str = "/data/put_territories_id.json";

#[cfg(not(debug_assertions))]
const PUT_TERRITORY_API_PATH: &str = "/api/territories/save";

#[cfg(debug_assertions)]
const GET_TERRITORY_API_PATH: &str = "/data/territories_id_1020.json";

#[cfg(not(debug_assertions))]
const GET_TERRITORY_API_PATH: &str = "/api/territories";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "POST";

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const EDIT_METHOD: &str = "PUT";

#[cfg(debug_assertions)]
const EDIT_METHOD: &str = "GET";


#[cfg(debug_assertions)]
const API_ASSIGN_URL: &str = "/data/post_assignments.json";

#[cfg(not(debug_assertions))]
const API_ASSIGN_URL: &str = "/api/territory-assignment/assignments";

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct TerritoryEditorModel {
    pub territory: Territory,
    // pub number: Option<String>,
    // pub id: i32,
    // pub description: Option<String>,
    // pub status: Option<String>,
    pub save_success: bool,
    pub save_error: bool,
    #[prop_or_default]
    pub load_error: bool,
    pub error_message: String,
}

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct TerritoryAssignerModel {
    //pub territory: Territory,
    pub assignee: String,
    // pub save_success: bool,
    // pub save_error: bool,
    // #[prop_or_default]
    // pub load_error: bool,
    // pub error_message: String,
}

#[derive(Clone, Debug, PartialEq, Deserialize, Serialize)]
pub struct TerritoryEditorParameters {
    pub id: Option<i32>,
    pub number: Option<String>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AssignmentResult {
    pub link_contract: TerritoryLinkContract,
    pub success: bool,
    pub load_failed: bool,
    pub load_failed_message: String,
    pub status: u16,
    pub completed: bool,
}

// pub fn add_field(state: yew::UseStateHandle<AddressEditModel>, field: &str) {
//     state.
// }

#[function_component(TerritoryEditorPage)]
pub fn territory_editor_page() -> Html {
    set_document_title("Territory Editor");

    let state: yew::UseStateHandle<TerritoryEditorModel> = use_state(|| TerritoryEditorModel::default());
    let assigner_state: yew::UseStateHandle<TerritoryAssignerModel> = use_state(|| TerritoryAssignerModel::default());
    let assignment_result_state: yew::UseStateHandle<AssignmentResult> = use_state(|| AssignmentResult::default());
    let cloned_state = state.clone();
    let location = use_location().expect("Should be a location to get query string");
    let parameters: TerritoryEditorParameters = location.query().expect("An object");
    let territory_id: i32 = match parameters.id {
        Some(v) => v,
        _ => 0,
    };

    let stage_id_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.territory.stage_id = Some(value.parse().unwrap_or_default());
            
            log!(format!("Territory stage id set to {stage_id:?}", stage_id = modification.territory.stage_id));

            state.set(modification);
        })
    };
  
    let number_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.territory.number = value.to_string();
            
            log!(format!("Territory number set to {number:?}", number = modification.territory.number));

            state.set(modification);
        })
    };
  
    let description_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.territory.description = Some(value);
            
            log!(format!("Territory description set to {description:?}", description = modification.territory.description));

            state.set(modification);
        })
    };
   
    let group_id_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.territory.group_id = Some(value);
            
            log!(format!("Territory group_id set to {group_id:?}", group_id = modification.territory.group_id));

            state.set(modification);
        })
    };
   
    let notes_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.territory.notes = Some(value);
            
            log!(format!("Territory notes set to {notes:?}", notes = modification.territory.notes));

            state.set(modification);
        })
    };
   
    // let delivery_status_onchange = {
    //     let state = cloned_state.clone();
    //     Callback::from(move |value: String| {
    //         let mut modification = state.deref().clone();
    //         modification.address.delivery_status_id = value.parse().unwrap();
    //         state.set(modification);
    //     })
    // };
  
    // let state_onchange = {
    //     let state = cloned_state.clone();
    //     Callback::from(move |value: String| {
    //         let mut modification = state.deref().clone();
    //         modification.address.state = Some(value);

    //         log!(format!("Address State set to {name:?}", name = modification.address.state.clone()));

    //         state.set(modification);
    //     })
    // };

    let cloned_state = state.clone();
    use_effect_with_deps(move |_| {
        let cloned_state = cloned_state.clone();
        let territory_number = parameters.number.clone().unwrap_or_default();
        wasm_bindgen_futures::spawn_local(async move {
            log!("Loading territory...");
            let territory_id: i32 = territory_id;
            let territory_number = territory_number.clone();
            let uri: String = format!(
                "{base_path}?id={territory_id}&territoryNumber={territory_number}", 
                base_path = GET_TERRITORY_API_PATH);

            let territory_response = Request::get(uri.as_str())
                .send()
                .await
                .expect("Address response (raw) from API");
            
            if territory_response.status() == 200 {
                let fetched_territory: Territory = territory_response
                    .json()
                    .await
                    .expect("Valid territory JSON from API");

                log!(format!(
                    "Fetched territory, id: {number:?}",
                    number = fetched_territory.number
                ));

                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: fetched_territory,
                    save_success: false,
                    save_error: false,
                    load_error: false,
                    error_message: "".to_string(),
                };

                log!(format!(
                    "Fetched territory, number: {number:?}",
                    number = model.territory.number
                ));

                cloned_state.set(model);
            } else if territory_response.status() == 401 {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: Territory::default(),
                    save_success: false,
                    save_error: false,
                    load_error: true,
                    error_message: "Unauthorized".to_string(),
                };

                cloned_state.set(model);
            }
        });
        || ()
    }, ());
    
    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |event: SubmitEvent| { 
        event.prevent_default();
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            log!("Spawing request for territory change...");
            let model = cloned_state.clone();
            let uri_string: String = format!("{path}", 
                path = PUT_TERRITORY_API_PATH);

            let uri: &str = uri_string.as_str();
            
            let _method: Method = match EDIT_METHOD {
                "PUT" => Method::PUT,
                "GET" => Method::PUT,
                &_ =>  Method::GET,
            };

            let body_model = &model.deref();
            let edit_request = TerritoryEditRequest {
                id: body_model.territory.id.unwrap_or_default(),
                territory_number: body_model.territory.number.clone(),
                description: body_model.territory.description.clone(),
                notes: body_model.territory.notes.clone(),
                group_id: body_model.territory.group_id.clone(),
                stage_id: body_model.territory.stage_id.unwrap_or_default(),
                modification_type: "Edit".to_string(),
            };

            let data_serialized = serde_json::to_string_pretty(&edit_request)
                .expect("Should be able to serialize territory edit request form into JSON");

            // TODO: FetchService::fetch accepts two parameters: a Request object and a Callback.
            // https://yew.rs/docs/0.18.0/concepts/services/fetch
            let resp = Request::new(uri)
                .method(Method::PUT)
                .header("Content-Type", "application/json")
                .body(data_serialized)
                .send()
                .await
                .expect("A result from the endpoint");

            if resp.status() == 200 {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: cloned_state.territory.clone(),
                    save_success: true,
                    save_error: false,
                    load_error: false,
                    error_message: "".to_string(),
                };
    
                cloned_state.set(model);
            } else if resp.status() == 401 {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: cloned_state.territory.clone(),
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    error_message: "Unauthorized".to_string(),
                };
    
                cloned_state.set(model);
            } else if resp.status() == 403 {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: cloned_state.territory.clone(),
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    error_message: "Forbidden".to_string(),
                };
    
                cloned_state.set(model);
            } else {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: cloned_state.territory.clone(),
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    error_message: format!("{}", resp.status()),
                };
    
                cloned_state.set(model);
            }
        });
    });

    let selected_stage_id: i32 = state.territory.stage_id.unwrap_or_default();
    //let selected_status_id: i32 = state.territory.status_id;
    
    let assigner_state = assigner_state.clone();
    let _assignee_onchange = {
        let assigner_state = assigner_state.clone();
        Callback::from(move |assignee: String| {
            let mut assigner = assigner_state.deref().clone();
            assigner.assignee = assignee;
            assigner_state.set(assigner);
        })
    };
    
   
    let cloned_state = state.clone();
    let assigner_state = assigner_state.clone();
    let assignment_result_state_clone = assignment_result_state.clone();
    let _assigner_onsubmit = Callback::from(move |event: SubmitEvent| { 
        event.prevent_default();
        
        let cloned_state = cloned_state.clone();
        let assigner_state = assigner_state.clone();
        let assignment_result_state_clone = assignment_result_state_clone.clone();
        spawn_local(async move {
            let _model = assigner_state.clone();
            let uri_string: String = format!("{path}?territoryNumber={number}&albaUserId={assignee}&assigner=check-session", 
                path = API_ASSIGN_URL,
                number = cloned_state.territory.number.clone(),
                assignee = assigner_state.assignee.clone()
            );

            let uri: &str = uri_string.as_str();
            
            let method: Method = match ASSIGN_METHOD {
                "POST" => Method::POST,
                "GET" => Method::GET,
                &_ =>  Method::GET,
            };
          
            let resp = Request::new(uri)
                .method(method)
                .header("Content-Type", "application/json")
                .send()
                .await
                .expect("A result from the endpoint");

           
            let link_contract: TerritoryLinkContract = if resp.status() == 200 {
                resp.json().await.unwrap()
            } else {
                TerritoryLinkContract::default()
            };
            
            let result = AssignmentResult {
                success: (resp.status() == 200),
                load_failed: (resp.status() != 200),
                load_failed_message: match resp.status() {
                    405 => "Bad Method".to_string(),
                    _ => "Error".to_string(),
                },
                link_contract: link_contract,
                status: resp.status(),
                completed: true,
            };

            assignment_result_state_clone.set(result);
        });
    });

    let _assignment_result_state = assignment_result_state.clone();
    let is_assigned: bool = !state.territory.signed_out_to.clone().unwrap_or_default().is_empty();

    let assign_uri = format!("/app/assign/{territory_number}/{description}/Current+Assignee", 
        territory_number = state.territory.number.clone(),
        description = state.territory.description.clone().unwrap_or("".to_string()));

    html! {
        <>
        <MenuBarV2>
            <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                <li class={"nav-item"}>
                    <TerritorySearchLink />
                </li>  
            </ul>
        </MenuBarV2>
        <div class="container">
            <span><strong>{"委派给 Assign Territory"}</strong></span>
            <hr/>
            if is_assigned {
                <div class="row">
                    <div class="col">
                        <span class="mx-1">{"Assigned to: "}{state.territory.signed_out_to.clone()}</span>
                        //<span class="mx-1">{" "}{state.territory.signed_out.clone()}</span>
                    </div>
                </div>
            }
            <a 
                style="margin-top:5px;color:white;"
                class="btn btn-primary btn-sm"
                href={assign_uri}>
                {"Open Territory Assign Page"}
            </a>
            // <form onsubmit={assigner_onsubmit} class="row g-3">
            //     <div class="col-12 col-sm-9 col-md-6 d-flex ">
            //         <UserSelector onchange={assignee_onchange} />
            //         <button type="submit" class="btn btn-primary">
            //             {if is_assigned { "Reassign" } else { "Assign" }}
            //         </button>
            //     </div>
            // </form>
            // if assignment_result_state.load_failed { 
            //     <div class="row">
            //         <div class="col">
            //             <span class="mx-1 badge bg-danger">{"Error"}</span> 
            //             <span class="mx-1" style="color:red;">{assignment_result_state.load_failed_message.clone()}</span>
            //             <span class="mx-1 badge bg-danger">{assignment_result_state.status}</span>
            //         </div>
            //     </div>
            // }
            <hr/>
            <span><strong>{"Edit Territory"}</strong></span>
            if state.save_success { 
                <span class="mx-1 badge bg-success">{"Saved"}</span> 
            }
            if state.save_error { 
                <span class="mx-1 badge bg-danger">{"Save Error"}</span> 
                <span class="mx-1" style="color:red;">{state.error_message.clone()}</span>
            }      
            if state.load_error { 
                <span class="mx-1 badge bg-danger">{"Error"}</span> 
                <span class="mx-1" style="color:red;">{state.error_message.clone()}</span>
            }        
            <hr/>
            <form {onsubmit} class="row g-3">
                // <div class="col-12 col-sm-6 col-md-4">
                //     <label for="input-status" class="form-label">{"Status"}</label>
                //     <select onchange={status_onchange} id="input-status" class="form-select shadow-sm">
                //         <EnglishChineseIdOption id={1} english="Available" chinese="" selected={selected_status_id} />
                //         <EnglishChineseIdOption id={2} english="Signed-out" chinese="" selected={selected_status_id} />
                //     </select>
                // </div>
                <div class="col-6 col-sm-6 col-md-4 col-lg-3">
                    <label for="inputNumber" class="form-label">{"区域号码 Territory No."}</label>
                    <input readonly=true value={state.territory.number.clone()} onchange={number_onchange} type="text" class="form-control shadow-sm" id="inputNumber" placeholder="Number"/>
                </div>
                <div class="col-6 col-sm-6 col-md-4 col-lg-3">
                    <label for="input-group-id" class="form-label">{"Group ID"}</label>
                    <input value={state.territory.group_id.clone()} onchange={group_id_onchange} type="text" rows="2" cols="30" class="form-control shadow-sm" id="input-group_id" placeholder="Group ID"/>
                </div>
                <div class="col-12 col-sm-6 col-md-4 col-lg-3">
                    <label for="input-stage" class="form-label">{"Stage"}</label>
                    // TODO: Load this dynamically, it has already changed
                    <select onchange={stage_id_onchange} id="input-stage" class="form-select shadow-sm">
                        <EnglishChineseIdOption id={1} english="None" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={1000} english="Available for Check Out" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={2000} english="Letter: Writing" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={2100} english="Letter: Sent" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={2200} english="Letter: Returned (Done)" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={3000} english="Phone: Calling" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={3100} english="Phone: Done" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={4000} english="Door-to-Door: Visiting" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={4005} english="Door-to-Door: Visiting Started" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={4010} english="Door-to-Door: Visiting Not-at-Homes" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={4020} english="Door-to-Door: Visiting Done" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={4100} english="Reserved: Done" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={5000} english="Reserved: Cooling Off" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={6000} english="Reserved" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={6500} english="Ready to Visit" chinese="" selected={selected_stage_id} />
                    </select>
                </div>
                <div class="col-12 col-sm-6 col-md-4 col-lg-3">
                // NOT DONE YET
                //<label for="input-status" class="form-label">{"Status"}</label>
                // <select id="input-status" class="form-select shadow-sm">
                //     <option value="Available" selected={state.territory.status.clone() == "Available"}>
                //         {"Available"}
                //     </option>
                //     <option value="Signed-out" selected={state.territory.status.clone() == "Signed-out"}>
                //         {"Signed-out"}
                //     </option>
                // </select>
            </div>                
                // <div class="col-12 col-sm-6 col-md-4">
                //     <label for="input-delivery-status" class="form-label">{"Mail Delivery Status"}</label>
                //     <AddressDeliveryStatusSelector 
                //         onchange={delivery_status_onchange} 
                //         id={state.address.delivery_status_id} />
                // </div>
                <div class="col-12 col-sm-9 col-md-6">
                    <label for="inputDescription" class="form-label">{"区域名称 Description"}</label>
                    <input value={state.territory.description.clone()} onchange={description_onchange} type="text" class="form-control shadow-sm" id="inputDescription" placeholder="Description"/>
                </div>
                <div class="col-12">
                    <label for="input-notes" class="form-label">{"笔记 Notes"}</label>
                    <textarea value={state.territory.notes.clone()} onchange={notes_onchange} type="text" rows="2" cols="30" class="form-control shadow-sm" id="input-notes" placeholder="Notes"/>
                </div>

                <div class="col-12">
                    <button type="submit" class="me-1 btn btn-primary shadow-sm">{"Save"}</button>
                    <a href="/app/address-search" class="mx-1 btn btn-secondary shadow-sm">{"Close"}</a>
                    if state.save_success { 
                        <span class="mx-1 badge bg-success">{"Saved"}</span> 
                    }
                    if state.save_error { 
                        <span class="mx-1 badge bg-danger">{"Save Error"}</span> 
                        <span class="mx-1" style="color:red;">{state.error_message.clone()}</span>
                    }
                // </div>
                // <div class="col-12">
                    <span><small>{"TID: "}{state.territory.id}</small></span>
                    <span><small>{" ATID: "}{state.territory.id}</small></span>
                </div>
            </form>
        </div>
        </>
    }
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct EnglishChineseOptionProps {
    pub english: String,
    pub chinese: String,
    pub selected: String,
}

#[function_component]
pub fn EnglishChineseOption(props: &EnglishChineseOptionProps) -> Html {
    html! {
        <option value={props.english.clone()} selected={props.english.clone() == props.selected.clone()}>
            {props.chinese.clone()}{" "}{props.english.clone()}
        </option>
    }
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct EnglishChineseIdOptionProps {
    pub id: i32,
    pub english: String,
    pub chinese: String,
    pub selected: i32,
}

#[function_component]
pub fn EnglishChineseIdOption(props: &EnglishChineseIdOptionProps) -> Html {
    html! {
        <option value={props.id.to_string()} selected={props.id == props.selected}>
            {props.chinese.clone()}{" "}{props.english.clone()}
        </option>
    }
}