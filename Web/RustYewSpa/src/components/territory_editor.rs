//use crate::components::menu_bar::MenuBar;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::components::user_selector::UserSelector;
use crate::components::button_with_confirm::ButtonWithConfirm;
use crate::models::territories::{Territory, TerritoryEditRequest};
use crate::models::territory_links::TerritoryLinkContract;
use crate::functions::document_functions::set_document_title;
use crate::components::email_section::EmailSection;
use crate::components::sms_section::SmsSection;
use crate::components::collapsible_section::CollapsibleSection;

use chrono::{DateTime,Local,TimeZone};
use gloo_console::log;
use reqwasm::http::{Request, Method};
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_hooks::use_clipboard;
use yew_router::hooks::use_location;
//use serde_json::ser::to_string;

// Uncomment for debugging without an API server
// const GET_TERRITORY_API_PATH: &str = "/data/territories_id_1020.json";
// const ASSIGN_METHOD: &str = "GET";
// const EDIT_METHOD: &str = "GET";

const GET_TERRITORY_API_PATH: &str = "/api/territories";

#[derive(Properties, PartialEq, Clone, Serialize)]
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
    #[prop_or_default]
    pub show_changes: bool,
    #[prop_or_default]
    pub show_reassign: bool,
    #[prop_or_default]
    pub show_status_section: bool,
    #[prop_or_default]
    pub show_details_section: bool,
    #[prop_or_default]
    pub show_history_section: bool,
}

impl Default for TerritoryEditorModel {
    fn default() -> Self {
        TerritoryEditorModel {
            territory: Territory::default(),
            save_success: false,
            save_error: false,
            load_error: false,
            error_message: "".to_string(),
            show_changes: false,
            show_reassign: false,
            show_status_section: true,
            show_details_section: false,
            show_history_section: false,
        }
    }
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

    let state: yew::UseStateHandle<TerritoryEditorModel> = use_state(TerritoryEditorModel::default);
    let assigner_state: yew::UseStateHandle<TerritoryAssignerModel> = use_state(TerritoryAssignerModel::default);
    let assignment_result_state: yew::UseStateHandle<AssignmentResult> = use_state(AssignmentResult::default);
    let unassignment_result_state: yew::UseStateHandle<AssignmentResult> = use_state(AssignmentResult::default);
    let stage_change_result_state: yew::UseStateHandle<AssignmentResult> = use_state(AssignmentResult::default);
    let clipboard = use_clipboard();
    let cloned_state = state.clone();
    let location = use_location().expect("Should be a location to get query string");
    let parameters: TerritoryEditorParameters = location.query::<TerritoryEditorParameters>().expect("An object");
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
    use_effect_with((), move |_| {
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
                .expect("Territory response (raw) from API");
            
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
                    ..TerritoryEditorModel::default()
                };

                log!(format!(
                    "Fetched territory, number: {number:?}",
                    number = model.territory.number
                ));

                cloned_state.set(model);
            } else if territory_response.status() == 401 {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: Territory::default(),
                    load_error: true,
                    error_message: "Unauthorized".to_string(),
                    ..TerritoryEditorModel::default()
                };

                cloned_state.set(model);
            }
        });
        || ()
    });
    
    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |_event: i32 /*SubmitEvent*/| { 
        //event.prevent_default();
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            log!("Spawing request for territory change...");
            let model = cloned_state.clone();
            let uri: &str = "/api/territories/save";
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
                    ..TerritoryEditorModel::default()
                };
    
                cloned_state.set(model);
            } else if resp.status() == 401 {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: cloned_state.territory.clone(),
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    error_message: "Unauthorized".to_string(),
                    ..TerritoryEditorModel::default()
                };
    
                cloned_state.set(model);
            } else if resp.status() == 403 {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: cloned_state.territory.clone(),
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    error_message: "Forbidden".to_string(),
                    ..TerritoryEditorModel::default()
                };
    
                cloned_state.set(model);
            } else {
                let model: TerritoryEditorModel = TerritoryEditorModel {
                    territory: cloned_state.territory.clone(),
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    error_message: format!("{}", resp.status()),
                    ..TerritoryEditorModel::default()
                };
    
                cloned_state.set(model);
            }
        });
    });

    let selected_stage_id: i32 = state.territory.stage_id.unwrap_or_default();
    //let selected_status_id: i32 = state.territory.status_id;
    
    let assigner_state = assigner_state.clone();
    let assignee_onchange = {
        let assigner_state = assigner_state.clone();
        Callback::from(move |assignee: String| {
            let mut assigner = assigner_state.deref().clone();
            assigner.assignee = assignee;
            assigner_state.set(assigner);
        })
    };
   
    let cloned_state = state.clone();
    let assigner_state_clone = assigner_state.clone();
    let assignment_result_state_clone = assignment_result_state.clone();
    let unassignment_result_state_clone = unassignment_result_state.clone();
    let assigner_onsubmit = Callback::from(move |_: i32| { 
        //event.prevent_default();
        
        let cloned_state = cloned_state.clone();
        let assigner_state_clone = assigner_state_clone.clone();
        let assignment_result_state_clone = assignment_result_state_clone.clone();
        let unassignment_result_state_clone = unassignment_result_state_clone.clone();
        spawn_local(async move {
            log!("Posting Assignment or Reassignment...");
            let uri_string: String = format!("{path}?territoryNumber={number}&assignee={assignee}&assigner=check-session", 
                path = "/api/territory-assignment/assignments",
                number = cloned_state.territory.number.clone(),
                assignee = assigner_state_clone.assignee.clone()
            );

            let uri: &str = uri_string.as_str();
            let resp = Request::new(uri)
                .method(Method::POST)
                //.header("Content-Type", "application/json")
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
   
    let cloned_state = state.clone();
    let assigner_state_clone = assigner_state.clone();
    let assignment_result_state_clone = assignment_result_state.clone();
    let unassignment_result_state_clone = unassignment_result_state.clone();
    //let unassign_onclick = Callback::from(move |event: MouseEvent| { 
    let unassign_onclick = Callback::from(move |_| { 
        //event.prevent_default();
        
        let cloned_state = cloned_state.clone();
        let _assigner_state_clone = assigner_state_clone.clone();
        let assignment_result_state_clone = assignment_result_state_clone.clone();
        let unassignment_result_state_clone = unassignment_result_state_clone.clone();
        spawn_local(async move {
            log!("Deleting Assignment (Unassigning)...");
            let uri_string: String = format!("{path}?territoryNumber={number}&assignerEmail=check-session", 
                path = "/api/territory-assignment/assignments",
                number = cloned_state.territory.number.clone()
            );

            let uri: &str = uri_string.as_str();
            let resp = Request::new(uri)
                .method(Method::DELETE)
                //.header("Content-Type", "application/json")
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
                modified_state.territory.signed_out_to = None;
                modified_state.territory.signed_out = None;
                modified_state.territory.stage_id = Some(1000); // TODO: Get a value from the return body
                modified_state.show_reassign = false;
                cloned_state.set(modified_state);
            }
        });
    });

    let cloned_state = state.clone();
    let assigner_state_clone = assigner_state.clone();

    let stage_change_result_state_clone = stage_change_result_state.clone();
    let save_stage_onclick = Callback::from(move |_: i32| { 
        //event.prevent_default();
        
        let cloned_state = cloned_state.clone();
        let assigner_state_clone = assigner_state_clone.clone();
        //let assignment_result_state_clone = assignment_result_state_clone.clone();
        let stage_change_result_state_clone = stage_change_result_state_clone.clone();
        let territory_id = cloned_state.territory.id.unwrap_or_default();
        let to_stage_id = cloned_state.territory.stage_id.unwrap_or_default();
        spawn_local(async move {
            let _model = assigner_state_clone.clone();
            let uri_string: String = format!("{path}?territoryId={territory_id}&stageId={to_stage_id}&assignee={assignee}", 
                path = "/api/territory-marking/stages",
                assignee = cloned_state.territory.signed_out_to.clone().unwrap_or_default(),
            );

            let uri: &str = uri_string.as_str();
            let resp = Request::new(uri)
                .method(Method::POST)
                //.header("Content-Type", "application/json")
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

            stage_change_result_state_clone.set(result);

            if resp.status() == 200 {
                // let mut modified_state = cloned_state.deref().clone();
                // modified_state.territory.signed_out_to = Some(link_contract.clone().assignee_name);
                // modified_state.territory.signed_out = link_contract.clone().assigned_date_utc;
                // modified_state.territory.stage_id = Some(link_contract.clone().stage_id); 
                // modified_state.territory.last_completed_by = link_contract.clone().territory_last_completed_by;
                // modified_state.territory.last_completed_date = link_contract.clone().territory_last_completed_date;
                // cloned_state.set(modified_state);
            }
        });
    });

    let cloned_state = state.clone();
    let assigner_state_clone = assigner_state.clone();
    let _assignment_result_state_clone = assignment_result_state.clone();
    let stage_change_result_state_clone = stage_change_result_state.clone();
    let _confirm_complete_click = Callback::from(move |_: i32| { 
        //event.prevent_default();
        
        let cloned_state = cloned_state.clone();
        let _assigner_state_clone = assigner_state_clone.clone();
        //let assignment_result_state_clone = assignment_result_state_clone.clone();
        let _stage_change_result_state_clone = stage_change_result_state_clone.clone();
        let _territory_id = cloned_state.territory.id.unwrap_or_default();
        let _to_stage_id = cloned_state.territory.stage_id.unwrap_or_default();
        spawn_local(async move {
            /*
            let _model = assigner_state_clone.clone();
            let uri_string: String = format!("{path}?territoryId={territory_id}&stageId={to_stage_id}&assignee={assignee}", 
                path = "/api/territory-marking/stages",
                assignee = cloned_state.territory.signed_out_to.clone().unwrap_or_default(),
            );

            let uri: &str = uri_string.as_str();
            let resp = Request::new(uri)
                .method(Method::POST)
                //.header("Content-Type", "application/json")
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

            stage_change_result_state_clone.set(result);

            if resp.status() == 200 {
                // let mut modified_state = cloned_state.deref().clone();
                // modified_state.territory.signed_out_to = None;
                // modified_state.territory.signed_out = None;
                // cloned_state.set(modified_state);
            }
            */
        });
    });

    let cloned_state = state.clone();
    let clipboard_clone = clipboard.clone();
    let _copy_link_onclick = Callback::from(move |event: MouseEvent| { 
        event.prevent_default();
        
        let cloned_state = cloned_state.clone();
        let clipboard_clone = clipboard_clone.clone();
        spawn_local(async move {
           
            let active_link = format!("/mtk/{}", cloned_state.territory.assignee_link_key.clone().unwrap_or_default());
            //stage_change_result_state_clone.set(result);

            clipboard_clone.write_text(active_link.to_owned());
    
        });
    });

    let cloned_state = state.clone();
    let show_status_section_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        let mut modification = cloned_state.deref().clone();
        modification.show_status_section = !cloned_state.show_status_section;
        cloned_state.set(modification);
    });

    let cloned_state = state.clone();
    let show_details_section_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        let mut modification = cloned_state.deref().clone();
        modification.show_details_section = !cloned_state.show_details_section;
        cloned_state.set(modification);
    });

    let cloned_state = state.clone();
    let show_history_section_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        let mut modification = cloned_state.deref().clone();
        modification.show_history_section = !cloned_state.show_history_section;
        cloned_state.set(modification);
    });

    let cloned_state = state.clone();
    let show_changes_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        let mut modification = cloned_state.deref().clone();
        modification.show_changes = !cloned_state.show_changes;
        cloned_state.set(modification);
    });

    let cloned_state = state.clone();
    let show_reassign_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        let mut modification = cloned_state.deref().clone();
        modification.show_reassign = !cloned_state.show_reassign;
        cloned_state.set(modification);
    });

    let cloned_state = state.clone();
    let full_signed_out = cloned_state.territory.signed_out.clone().unwrap_or_default();
    let _signed_out_date = if full_signed_out.is_empty() || full_signed_out.len() < 10 { 
        "".to_string() 
    } else {
        full_signed_out.clone().chars().take(10).collect::<String>()
    };

    let _assignment_result_state = assignment_result_state.clone();
    let is_assigned: bool = !state.territory.signed_out_to.clone().unwrap_or_default().is_empty();
    
    log!(format!("teditor:signed_out (date): {}", state.territory.signed_out.clone().unwrap_or_default()));

    //let assigned_date = format_date_only(state.territory.signed_out.clone());
    let assigned_date = if state.territory.signed_out.clone().is_some() {
        state.territory.signed_out.clone().unwrap_or_default().as_str()[0..10].to_string()
    } else {
        "".to_string()
    };
    
    let assigned_to = format!("{}   {}",
        state.territory.signed_out_to.clone().unwrap_or_default(),
        assigned_date.clone());

    let last_completed_date = if state.territory.last_completed.clone().is_some() {
        state.territory.last_completed.clone().unwrap_or_default().as_str()[0..10].to_string()
    } else {
        "".to_string()
    };
    let last_completed_by = format!("{}   {}",
        state.territory.last_completed_by.clone().unwrap_or_default(),
        last_completed_date.clone());

    let _assign_uri = format!("/app/assign/{territory_number}/{description}/Current+Assignee", 
        territory_number = state.territory.number.clone(),
        description = state.territory.description.clone().unwrap_or("".to_string()));
    
    let link_key = state.territory.assignee_link_key.clone().unwrap_or_default();
    let active_link = format!("/mtk/{}", state.territory.assignee_link_key.clone().unwrap_or_default());
    let has_active_link = !link_key.is_empty();

    let addresses_unvisited = state.territory.addresses_unvisited.unwrap_or_default();
    let addresses_active = state.territory.addresses_active.unwrap_or_default();

    let stage_id = state.territory.stage_id.unwrap_or_default();
    // let (dtd_stage, letter_stage) = if (2000..3000).contains(&stage_id) {
    //     String::from("Completed")
    // } else { 
    //     state.territory.stage.clone().unwrap_or_default() 
    // };

    let (dtd_stage, 
        letter_stage,
        show_letter_check_out_button,
        show_letter_complete_button,
        show_dtd_check_out_button,
        show_dtd_complete_button) = 
    // Available
    if (0..2000).contains(&stage_id) {
        (state.territory.stage.clone().unwrap_or_default(), 
        String::from("Unvailable"), 
        false,
        false,
        true,
        false)
    // Letter
    } else if (2000..3000).contains(&stage_id) {
        (String::from("Completed"), 
        state.territory.stage.clone().unwrap_or_default(), 
        false,
        true,
        false,
        false)
    // Phone, Visiting Started
    } else if (3000..=4010).contains(&stage_id) {
        (state.territory.stage.clone().unwrap_or_default(), 
        String::from("Unvailable"), 
        false,
        false,
        false,
        true)
    // Visiting Done, Cooling Off
    } else if (4011..=5000).contains(&stage_id) { // There is no 4011
        (state.territory.stage.clone().unwrap_or_default(), 
        String::from("Available"), 
        true,
        false,
        false,
        false)
    // Reserved, Ready to Visit
    } else { 
        (state.territory.stage.clone().unwrap_or_default(), 
        String::from("Unavailable"), 
        false,
        false,
        false,
        false)
    };

    html! {
        <>
        <MenuBarV2>
            <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                <li class={"nav-item"}>
                    <MapPageLink />
                </li> 
            </ul>
        </MenuBarV2>
        <div class="container p-1 pt-3">
            <div class="row g-3 pb-2">
                <div class="col-12">
                    if has_active_link {
                        <a href={active_link} class="btn btn-outline-primary">{"Open"}</a>
                    } else {
                        <button class="btn btn-outline-secondary">{"Open"}</button>
                    }
                    <span class="fs-5 p-2 pt-3">{"Territory "}{state.territory.number.clone()}</span>
                </div>
            </div>
            <CollapsibleSection hidden={true} text="委派给 Territory Assignment Status (v2)" show_section_default={true}>
                <div class="row p-0 px-1">
                    <div class="col-12 col-sm-6 col-md-3 border-top-primary pb-2">
                        <div style="width:100%;background-color:lightgray;"><strong>{"Door-to-door"}</strong></div>
                        <br/>
                        <span>{"Status:"}</span>
                        <span class="mx-2 badge bg-dark">{dtd_stage.clone()}</span>
                        <span>{format!("{}/{}", addresses_unvisited, addresses_active)}</span>
                        if show_dtd_check_out_button {
                            <br/>
                            <button class="btn btn-primary mt-2">{"Check Out"}</button>
                        }
                        if show_dtd_complete_button {
                            <br/>
                            <button class="btn btn-primary mt-2">{"Complete"}</button>
                        }
                    </div>
                    <div class="col-12 col-sm-6 col-md-3 border-top-1 border-dark pb-2">
                        <div style="width:100%;background-color:lightgray;"><strong>{"Letter Writing"}</strong></div>
                        <br/>
                        <span>{"Status:"}</span>
                        <span class="mx-2 badge bg-secondary">{letter_stage.clone()}</span>
                        if show_letter_check_out_button {
                            <br/>
                            <button class="btn btn-primary mt-2">{"Write Letters"}</button>
                        }
                        if show_letter_complete_button {
                            <br/>
                            <button class="btn btn-primary mt-2">{"Complete"}</button>
                        }
                    </div>
                </div>
            </CollapsibleSection>
            <CollapsibleSection text="委派给 Territory Assignment Status" show_section_default={true}>
                <div class="row g-3 pt-3">    
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
                            <div class="row">
                                <div class="col">
                                    <span class="mx-1 badge bg-danger">{"Unassignment Error"}</span> 
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
                    <div class="row">
                        <div class="col">
                            <span class="mx-1 badge bg-danger">{"Assignment Error"}</span> 
                            <span class="mx-1" style="color:red;">{assignment_result_state.load_failed_message.clone()}</span>
                            <span class="mx-1 badge bg-danger">{assignment_result_state.status}</span>
                        </div>
                    </div>
                } //
                if assignment_result_state.success {
                    <div class="col-12 col-sm-8 col-md-6 col-lg-4">
                        //<p style="color:blue;">{"Success"}</p>
                        <span class="mx-1 badge bg-success">{"Success"}</span><br/>
                        <a 
                            style="color:blue;margin-bottom:10px;"
                            href={assignment_result_state.link_contract.territory_uri.clone()}>
                            {assignment_result_state.link_contract.territory_uri.clone()}
                        </a>
                        //<button onclick={copy_link_onclick} class="btn btn-outline-primary">{"Copy"}</button>
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
                <div class="row g-3 pt-3">
                    <div class="col-12 col-sm-8 col-md-6 col-lg-4">
                        <label for="input-stage" class="form-label">{"Stage"}</label>
                        // TODO: Load this dynamically, it has already changed
                        <div class="input-group">
                            <select 
                                id="input-stage" 
                                onchange={stage_id_onchange} 
                                class="form-select shadow-sm">
                                <EnglishChineseIdOption id={1} english="None" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={1000} english="Available for Check Out" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={2000} english="Letter: Writing" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={2100} english="Letter: Sent" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={2200} english="Letter: Returned (Done)" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={3000} english="Phone: Calling" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={3100} english="Phone: Done" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={4000} english="Visiting" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={4005} english="Visiting Started" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={4010} english="Visiting Not-at-Homes" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={4020} english="Visiting Done" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={4100} english="Territory Done" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={5000} english="Territory Cooling Off" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={6000} english="Reserved" chinese="" selected={selected_stage_id} />
                                <EnglishChineseIdOption id={6500} english="Ready to Visit" chinese="" selected={selected_stage_id} />
                            </select>
                            <ButtonWithConfirm 
                                id="save-stage-button" 
                                button_text="Save" 
                                on_confirm={save_stage_onclick.clone()} 
                            />
                        </div>
                        if stage_change_result_state.success {
                            <div class="row">
                                <div class="col">
                                    <span class="mx-1 badge bg-success">{"Stage Change Saved"}</span> 
                                </div>
                            </div>
                        }
                        if stage_change_result_state.load_failed {
                            <div class="row">
                                <div class="col">
                                    <span class="mx-1 badge bg-danger">{"Stage Change Error"}</span> 
                                    <span class="mx-1" style="color:red;">{stage_change_result_state.load_failed_message.clone()}</span>
                                    <span class="mx-1 badge bg-danger">{stage_change_result_state.status}</span>
                                </div>
                            </div>
                        }
                    </div>
                </div>
            </CollapsibleSection>
            <CollapsibleSection text="Edit Territory Details">
            
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
                <div class="row g-3 pt-3">
                    <div class="col-6 col-sm-6 col-md-4 col-lg-3">
                        <label for="inputNumber" class="form-label">{"区域号码 Territory No."}</label>
                        <input 
                            id="territory-number-input" 
                            readonly=true 
                            value={state.territory.number.clone()} 
                            onchange={number_onchange} 
                            type="text" 
                            class="form-control shadow-sm" 
                            placeholder="Number"/>
                    </div>
                    <div class="col-6 col-sm-6 col-md-4 col-lg-3">
                        <label for="input-group-id" class="form-label">{"Group ID"}</label>
                        <input value={state.territory.group_id.clone()} onchange={group_id_onchange} type="text" rows="2" cols="30" class="form-control shadow-sm" id="input-group_id" placeholder="Group ID"/>
                    </div>                
                    <div class="col-12 col-sm-6 col-md-4 col-lg-3">               
                </div>                
                    <div class="col-12 col-sm-9 col-md-6">
                        <label for="inputDescription" class="form-label">{"区域名称 Description"}</label>
                        <input value={state.territory.description.clone()} onchange={description_onchange} type="text" class="form-control shadow-sm" id="inputDescription" placeholder="Description"/>
                    </div>
                    <div class="col-12">
                        <label for="input-notes" class="form-label">{"笔记 Notes"}</label>
                        <textarea value={state.territory.notes.clone()} onchange={notes_onchange} type="text" rows="2" cols="30" class="form-control shadow-sm" id="input-notes" placeholder="Notes"/>
                    </div>
                    <div class="col-12">
                        <ButtonWithConfirm 
                            id="save-details-button" 
                            button_text="Save Details" 
                            on_confirm={onsubmit.clone()} 
                        />
                        <a href="/app/address-search" class="mx-1 btn btn-secondary shadow-sm">{"Close"}</a>
                        if state.save_success { 
                            <span class="mx-1 badge bg-success">{"Saved"}</span> 
                        }
                        if state.save_error { 
                            <span class="mx-1 badge bg-danger">{"Save Error"}</span> 
                            <span class="mx-1" style="color:red;">{state.error_message.clone()}</span>
                        }
                        <br/><span><small>{"TID: "}{state.territory.id}</small></span>
                        <span><small>{" ATID: "}{state.territory.id}</small></span>
                    </div>
                </div>
            </CollapsibleSection>
            <CollapsibleSection text="History">
                <div class="row g-3 pt-3">
                    // <hr/>
                    // <span><strong>{"History"}</strong></span>
                    <div class="col-12 col-sm-8 col-md-6 col-lg-4">
                        <label class="form-label">{"Last Completed By"}</label>
                        <input readonly={true} value={last_completed_by.clone()} type="text" class="form-control shadow-sm" id="last-completed-by-input" placeholder="Name"/>
                    </div>
                </div>
                <div class="row g-3 pt-3">
                    <div class="col-12">
                        <button onclick={show_changes_onclick} class="btn btn-outline-primary">
                        if state.show_changes {
                            {"Hide Changes"}
                        } else {
                            {"Show Changes"}
                        }
                        </button>
                    </div>
                </div>
                if state.show_changes {
                    <div class="row g-3 pt-3">
                        <div class="col-12 table-responsive">
                            <table class="table">
                                <thead>
                                    <tr>
                                        <th scope="col">{"Changed"}</th>
                                        <th scope="col">{"Stage"}</th>
                                        <th scope="col">{"Assignee Name"}</th>
                                        <th scope="col">{"Changed by"}</th>
                                    </tr>
                                </thead>
                                <tbody>
                                {
                                    state.territory.stage_changes.iter().map(|change| {   
                                        let _change_date = change.change_date_utc.clone().chars().take(10).collect::<String>();
                                        let _change_time = change.change_date_utc.clone().as_str()[11..16].to_string();
                                        let assignee = if change.assignee_name.clone() == None {
                                            change.assignee_normalized_email.clone()
                                        } else {
                                            change.assignee_name.clone() 
                                        };

                                        let formatted_date_time = format_date(Some(change.change_date_utc.clone()));

                                        html! {
                                                <tr>
                                                    //<td scope="row">{change_date}{" "}{change_time}</td>
                                                    <td>{formatted_date_time}</td>
                                                    //<td>{change.stage_id}</td>
                                                    <td>{change.stage.clone()}</td>
                                                    <td>{assignee}</td>
                                                    //<td>{change.assignee_normalized_email.clone()}</td>
                                                    <td>{change.created_by_user_id.clone()}</td>
                                                </tr>
                                                
                                        }
                                    }).collect::<Html>()
                                }
                                </tbody>
                            </table>                        
                        </div>
                    </div>
                }
            </CollapsibleSection>
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

pub fn format_date(text: Option<String>) -> String {
    if text.is_none() {
        "".to_string()
    } else {
        let utc = DateTime::parse_from_rfc3339( 
            format!("{}Z", text.expect("String date"))
            //.parse::<DateTime<Utc>>()
            .to_string().as_str()
        )
        .expect("DateTime");
        
        let local = Local.from_utc_datetime(&utc.naive_utc());

        local
        .format("%Y-%m-%d %H:%M:%S")
        .to_string()
    }
}


pub fn format_date_only(text: Option<String>) -> String {
    if text.is_none() {
        "".to_string()
    } else {
        let utc = DateTime::parse_from_rfc3339( 
            //format!("{}Z", text.expect("String date"))
            //.parse::<DateTime<Utc>>()
            text
            .unwrap_or_default()
            .as_str()
        )
        .expect("DateTime");
        
        let local = Local.from_utc_datetime(&utc.naive_utc());

        local
        .format("%Y-%m-%d")
        .to_string()
    }
}