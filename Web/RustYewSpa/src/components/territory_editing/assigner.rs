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
use crate::components::territory_editing::stage_selector::StageSelector;
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
    pub signed_out_to: Option<String>,
    pub territory_number: Option<String>,
}


#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct TerritoryAssignerModel {
    pub assignee: String,
}

#[function_component(Assigner)]
pub fn assigner(props: &Props) -> Html {
    let assigner_state: yew::UseStateHandle<TerritoryAssignerModel> = use_state(TerritoryAssignerModel::default);
    let assignment_result_state: yew::UseStateHandle<AssignmentResult> = use_state(AssignmentResult::default);
    let unassignment_result_state: yew::UseStateHandle<AssignmentResult> = use_state(AssignmentResult::default);
    let unassignment_result_state_clone = unassignment_result_state.clone();
        
    let assigner_onsubmit = Callback::from(move |_: i32| { 
        let cloned_state = cloned_state.clone();
        let assigner_state_clone = assigner_state_clone.clone();
        let assignment_result_state_clone = assignment_result_state_clone.clone();
        let unassignment_result_state_clone = unassignment_result_state_clone.clone();
        spawn_local(async move {
            log!("Posting Assignment or Reassignment...");
            let uri_string: String = format!("{path}?territoryNumber={number}&assignee={assignee}&assigner=check-session", 
                path = "/api/territory-assignment/assignments",
                number = props.territory_number.clone(),
                assignee = assigner_state_clone.assignee.clone()
            );

            let uri: &str = uri_string.as_str();
            let resp = Request::new(uri)
                .method(Method::POST)
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
                link_contract: link_contract.clone(),
                status: resp.status(),
                completed: true,
            };

            assignment_result_state_clone.set(result);
            unassignment_result_state_clone.set(AssignmentResult::default());

            if resp.status() == 200 {
                let mut modified_state = cloned_state.deref().clone();
                modified_state.territory.signed_out_to = Some(link_contract.clone().assignee_name);
                modified_state.territory.signed_out = link_contract.clone().assigned_date_utc;
                modified_state.territory.stage_id = Some(link_contract.clone().stage_id); 
                modified_state.show_reassign = false;
                cloned_state.set(modified_state);
            }
        });
    });
   

    html!{
        <>
            <div class="row p-2">    
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
                            if state.show_reassign {
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
                if !is_assigned || state.show_reassign {
                    <div class="col-12 col-sm-9 col-md-6">
                        <label for="assignTo" class="form-label">{if is_assigned { "Reassign to" } else { "Assign" }}</label>
                        <div class="input-group">
                                <UserSelector id="assignee-user-selector" onchange={assignee_onchange} email_as_value={true} />
                                <ButtonWithConfirm 
                                    id="assign-button" 
                                    button_text={if is_assigned { "Reassign" } else { "Assign" }}
                                    on_confirm={assigner_onsubmit.clone()} 
                                    class={if is_assigned {"me-1 btn btn-success shadow-sm"} else {"me-1 btn btn-primary shadow-sm"}}
                                />
                        </div>
                    </div>
                    if state.show_reassign {
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
            if assignment_result_state.success {
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
            <div class="row p-2">
                <div class="col-12 col-sm-8 col-md-6 col-lg-4">
                    <StageSelector 
                        hidden={false} 
                        onchange={stage_id32_onchange.clone()} 
                        territory_id={cloned_state.territory.id.unwrap_or_default()} 
                        stage_id={cloned_state.territory.stage_id.unwrap_or_default()} 
                        signed_out_to={cloned_state.territory.signed_out_to.clone()} />
                </div>
            </div>            
        </>
    }
}