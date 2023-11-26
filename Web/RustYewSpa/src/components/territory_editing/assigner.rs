use gloo_console::log;
use reqwasm::http::{Request, Method};
use std::ops::Deref;
use serde::Serialize;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use web_sys::HtmlInputElement;
use yew::prelude::*;

use crate::components::button_with_confirm::ButtonWithConfirm;
use crate::components::email_section::EmailSection;
use crate::components::selector_option_bilingual::EnglishChineseIdOption;
use crate::components::sms_section::SmsSection;
use crate::components::territory_editing::stage_selector::StageSelector;
use crate::components::user_selector::UserSelector;
use crate::models::territory_links::TerritoryLinkContract;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AssignmentResult {
    pub link_contract: TerritoryLinkContract,
    pub success: bool,
    pub load_failed: bool,
    pub load_failed_message: String,
    pub status: u16,
    pub completed: bool,
}


#[derive(Properties, Default, Clone, PartialEq)]
pub struct Props {
    #[prop_or_default]
    pub hidden: bool,
    //pub onchange: Callback<i32>,
    //pub territory_id: i32,
    pub territory_number: Option<String>,
    pub signed_out_to: Option<String>,
    pub signed_out_date: Option<String>,
    pub assignee_change_callback: Callback<String>,
}


#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct TerritoryAssignerModel {
    pub territory_number: String,
    pub assignee: String,
    pub show_reassign: bool,
}

#[function_component(Assigner)]
pub fn assigner(props: &Props) -> Html {
    let territory_number = props.territory_number.clone().unwrap_or_default();
    let assignee = props.signed_out_to.clone().unwrap_or_default();
    log!(format!(
        "assigner: props: territory_number: {}, assignee: {}", 
        territory_number.clone(), 
        assignee.clone()));

    let territory_number_clone = territory_number.clone();
    let assignee_clone = assignee.clone();
    let assigner_state: yew::UseStateHandle<TerritoryAssignerModel> = use_state(move || TerritoryAssignerModel {
        territory_number: territory_number_clone,
        assignee: assignee_clone,
        show_reassign: false,
    });

    // let territory_number_clone = territory_number.clone();
    // let assignee_clone = assignee.clone();
    // assigner_state.set(TerritoryAssignerModel {
    //     territory_number: territory_number_clone,
    //     assignee: assignee_clone,
    // });

    log!(format!(
        "assigner: assigner_state: territory_number: {}, assignee: {}", 
        assigner_state.territory_number.clone(), 
        assigner_state.assignee.clone()));

    let assignment_result_state: yew::UseStateHandle<AssignmentResult> = use_state(AssignmentResult::default);
    let unassignment_result_state: yew::UseStateHandle<AssignmentResult> = use_state(AssignmentResult::default);
    let unassignment_result_state_clone = unassignment_result_state.clone();
    
    let assigner_state_clone = assigner_state.clone();
    let assignment_result_state_clone = assignment_result_state.clone();
    let props_clone = props.clone();
    let assigner_onsubmit = Callback::from(move |_: i32| { 
        //let cloned_state = cloned_state.clone();
        let assigner_state_clone = assigner_state_clone.clone();
        let assignment_result_state_clone = assignment_result_state_clone.clone();
        let unassignment_result_state_clone = unassignment_result_state_clone.clone();
        let props_clone = props_clone.clone();
        spawn_local(async move {
            log!("Posting Assignment or Reassignment...");
            let uri_string: String = format!("/api/territory-assignment/assignments?territoryNumber={number}&assignee={assignee}&assigner=check-session", 
                number = props_clone.territory_number.clone().unwrap_or_default(),
                assignee = assigner_state_clone.assignee.clone()
            );

            let uri: &str = uri_string.as_str();
            let resp = Request::new(uri)
                .method(Method::POST)
                .send()
                .await
                .expect("A result from the endpoint");

            log!(format!("Posting result code: {}", resp.status()));

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
                link_contract: link_contract.clone(),
                status: resp.status(),
                completed: true,
            };

            assignment_result_state_clone.set(result);
            unassignment_result_state_clone.set(AssignmentResult::default());

            if resp.status() == 200 {
                log!("assigner: The result was 200, successful");
                
                props_clone.assignee_change_callback.emit(link_contract.clone().assignee_name.clone());

                let mut modified_state = assignment_result_state_clone.deref().clone();
                // modified_state.link_contract.assignee_name = link_contract.clone().assignee_name;
                // modified_state.link_contract.assigned_date_utc = link_contract.clone().assigned_date_utc;
                modified_state.link_contract = link_contract.clone();
                //modified_state.territory.stage_id = Some(link_contract.clone().stage_id); 
                // // //modified_state.show_reassign = false;
                modified_state.success = true;
                assignment_result_state_clone.set(modified_state);

                let mut modified_state = assigner_state_clone.deref().clone();
                modified_state.show_reassign = false;
                assigner_state_clone.set(modified_state);

                
            }
        });
    });

    let cloned_state = assigner_state.clone();
    let assignment_result_state_clone = assignment_result_state.clone();
    let unassignment_result_state_clone = unassignment_result_state.clone();
    let props_clone = props.clone();
    let unassign_onclick = Callback::from(move |_| { 
        let cloned_state = cloned_state.clone();
        let assignment_result_state_clone = assignment_result_state_clone.clone();
        let unassignment_result_state_clone = unassignment_result_state_clone.clone();
        let props_clone = props_clone.clone();
        spawn_local(async move {
            log!("Deleting Assignment (Unassigning)...");
            let uri_string: String = format!("{path}?territoryNumber={number}&assignerEmail=check-session", 
                path = "/api/territory-assignment/assignments",
                number = props_clone.territory_number.clone().unwrap_or_default(),
            );

            let uri: &str = uri_string.as_str();
            let resp = Request::new(uri)
                .method(Method::DELETE)
                .send()
                .await
                .expect("A result from the endpoint");
            
            let result = AssignmentResult {
                success: (resp.status() == 200),
                load_failed: (resp.status() != 200),
                load_failed_message: match resp.status() {
                    405 => "Bad Method".to_string(),
                    _ => "Error".to_string(),
                },
                link_contract: TerritoryLinkContract::default(),
                status: resp.status(),
                completed: true,
            };

            assignment_result_state_clone.set(AssignmentResult::default());
            unassignment_result_state_clone.set(result);

            if resp.status() == 200 {
                let mut modified_state = cloned_state.deref().clone();
                ////modified_state.assignee = "".to_string();
                props_clone.assignee_change_callback.emit("".to_string());
                // modified_state.territory.signed_out = None;
                // modified_state.territory.stage_id = Some(1000); // TODO: Get a value from the return body
                modified_state.show_reassign = false;
                cloned_state.set(modified_state);
            }
        });
    });

    // let assigner_state_clone = assigner_state.clone();
    // let stage_id32_onchange = {
    //     let assigner_state_clone = assigner_state_clone.clone();
    //     Callback::from(move |stage_id: i32| {
    //         let mut modification = assigner_state_clone.deref().clone();
    //         modification.territory.stage_id = Some(stage_id);
    //         assigner_state_clone.set(modification);
    //     })
    // };
    let assigner_state_clone = assigner_state.clone();
    let assignee_onchange = {
        let assigner_state_clone: UseStateHandle<TerritoryAssignerModel> = assigner_state_clone.clone();
        Callback::from(move |assignee: String| {
            let mut assigner = assigner_state_clone.deref().clone();
            assigner.assignee = assignee;
            assigner_state_clone.set(assigner);
        })
    };

    let assigner_state_clone = assigner_state.clone();
    let show_reassign_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        let mut modification = assigner_state_clone.deref().clone();
        modification.show_reassign = !assigner_state_clone.show_reassign;
        assigner_state_clone.set(modification);
    });

    let assigner_state_clone = assigner_state.clone();
    
    let is_assigned = !props.signed_out_to.clone().unwrap_or_default().is_empty();
    //let is_assigned: bool = !state.territory.signed_out_to.clone().unwrap_or_default().is_empty();
    let assigned_date_trimmed = if props.signed_out_date.clone().is_some() && props.signed_out_date.clone().unwrap_or_default().len() >= 10 {
        props.signed_out_date.clone().unwrap_or_default().as_str()[0..10].to_string()
    } else {
        "".to_string()
    };
    
    let assigned_to = format!("{}  {}", 
        props.signed_out_to.clone().unwrap_or_default(), 
        assigned_date_trimmed);

    let show_reassign = assigner_state.show_reassign;

    let assignment_result_state_clone = assignment_result_state.clone();

    log!(format!("assigner: props.territory_number: {}, assigned_to: {}", props.territory_number.clone().unwrap_or_default(), props.signed_out_to.clone().unwrap_or_default()));
    log!(format!("assigner: territory_number: {}, assigned_to: {}", assignment_result_state.link_contract.territory_number.clone(), assigned_to.clone()));
    log!(format!("assigner: is_assigned 2: {is_assigned} assigner.assignee: {}", assigner_state.assignee.clone()));
    log!(format!("assigner: assignment_result_state_clone.success: {}", assignment_result_state_clone.success));
    
    html!{
        <>
            <div class="row p-2">    
                <div class="col-12">{if is_assigned { "is_assigned=true" } else { "is_assigned=false "}}</div>
                <div class="col-12">{format!("signed_out_to:{}", props.signed_out_to.clone().unwrap_or_default())}</div>
                if is_assigned {
                    <div class="col-12 col-sm-12 col-md-6">
                        <label class="form-label">{"Assigned to"}</label>
                        <div class="input-group">
                            <input 
                                id="assigned-to-input" 
                                readonly=true 
                                value={assigned_to} 
                                type="text" 
                                class="form-control shadow-sm" />
                            if show_reassign {
                                <ButtonWithConfirm 
                                    id="unassign-button" 
                                    button_text="Unassign" 
                                    on_confirm={unassign_onclick.clone()} 
                                    class="me-1 btn btn-danger shadow-sm"
                                />
                            } else {
                                <button onclick={show_reassign_onclick.clone()} class="btn btn-outline-primary">{"Change"}</button>
                            }
                        </div>
                    </div>
                    if unassignment_result_state.load_failed { 
                        <div class="row m-0 p-0">
                            <div class="col m-0 p-0">
                                <span class="m-0 badge bg-danger">{"Unassignment Error"}</span> 
                                <span class="mx-1" style="color:red;">{assignment_result_state.load_failed_message.clone()}</span>
                                <span class="mx-1 badge bg-danger">{assignment_result_state.status}</span>
                            </div>
                        </div>
                    }                   
                }
                if !is_assigned || show_reassign {
                    <div class="col-12 col-sm-9 col-md-6">
                        <label for="assignTo" class="form-label">{if is_assigned { "Reassign to" } else { "Assign" }}</label>
                        <div class="input-group">
                                <UserSelector id="assignee-user-selector" onchange={assignee_onchange} email_as_value={true} /> // email_as_value was true?
                                <ButtonWithConfirm 
                                    id="assign-button" 
                                    button_text={if is_assigned { "Reassign" } else { "Assign" }}
                                    on_confirm={assigner_onsubmit.clone()} 
                                    class={if is_assigned {"me-1 btn btn-success shadow-sm"} else {"me-1 btn btn-primary shadow-sm"}}
                                />
                        </div>
                    </div>
                    if show_reassign {
                        <div class="col-12 col-sm-9 col-md-6">
                            <button onclick={show_reassign_onclick.clone()} class="btn btn-secondary">{"Cancel Assignment Change"}</button>
                        </div>
                    }
                }
            </div>
            if assignment_result_state.load_failed { 
                <div class="row p-2">
                    <div class="col">
                        <span class="mx-1 badge bg-danger">{"Assignment Error"}</span> 
                        <span class="mx-1" style="color:red;">{assignment_result_state.load_failed_message.clone()}</span>
                        <span class="mx-1 badge bg-danger">{assignment_result_state.status}</span>
                    </div>
                </div>
            }            
            <div class="col-12">
                {if assignment_result_state_clone.success { "Success: true" } else { "Success: false"}}
            </div>
            if assignment_result_state_clone.success {
                <div class="col-12 col-sm-8 col-md-6 col-lg-4">
                    <span class="mx-1 badge bg-success">{"Success"}</span><br/>
                    <a 
                        style="color:blue;margin-bottom:10px;"
                        href={assignment_result_state.link_contract.territory_uri.clone()}>
                        {assignment_result_state.link_contract.territory_uri.clone()}
                    </a>
                    <SmsSection
                        territory_number={assignment_result_state.link_contract.territory_number.clone()}
                        assignee_phone={assignment_result_state.link_contract.assignee_phone.clone().unwrap_or_default()}
                        territory_uri={assignment_result_state.link_contract.territory_uri.clone().unwrap_or_default()}
                    />
                    <EmailSection 
                        territory_number={assignment_result_state.link_contract.territory_number.clone()}
                        assignee_email={assignment_result_state.link_contract.assignee_email.clone().unwrap_or_default()}
                        territory_uri={assignment_result_state.link_contract.territory_uri.clone().unwrap_or_default()}
                    />
                </div>
            } 
            // <div class="row p-2">
            //     <div class="col-12 col-sm-8 col-md-6 col-lg-4">
            //         <StageSelector 
            //             hidden={false} 
            //             onchange={stage_id32_onchange.clone()} 
            //             territory_id={cloned_state.territory.id.unwrap_or_default()} 
            //             stage_id={cloned_state.territory.stage_id.unwrap_or_default()} 
            //             signed_out_to={cloned_state.territory.signed_out_to.clone()} />
            //     </div>
            // </div>            
        </>
    }
}