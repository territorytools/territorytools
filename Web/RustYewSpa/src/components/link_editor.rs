use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::button_with_confirm::ButtonWithConfirm;
use crate::components::menu_bar::MapPageLink;
use crate::components::text_box::{InputCell, CheckboxCell, StringCell};
use crate::components::input_callback_macros::GridInput;
use crate::functions::document_functions::set_document_title;
use crate::models::territory_links::{LinkChanges, LinkResponse, TerritoryLinkContract};
use crate::field_string;
use crate::field;

use reqwasm::http::{Request,Response};
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen::JsCast;
//use wasm_bindgen_futures::spawn_local;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::hooks::use_location;

#[macro_export]
macro_rules! http_get_set {
    ($state:ident.$($field_path:ident).+, $uri:ident) => {{
        let state = $state.clone();
        let uri = $uri.clone();
        wasm_bindgen_futures::spawn_local(async move {
            let response = Request::get(uri.as_str())
                .send()
                .await
                .expect("Response (raw) from API");
            let status = response_status(&response);
            
            let mut modification = state.deref().clone();
            modification.$($field_path).+ = response.json().await.expect("Valid JSON");
            modification.error_message = status.error_message;
            modification.load_error = status.load_error;
            
            state.set(modification);
        });
    }};
}

#[derive(Properties, PartialEq, Clone, Serialize)]
pub struct LinkEditorModel {
    pub link: TerritoryLinkContract,
    pub save_success: bool,
    pub save_error: bool,
    #[prop_or_default]
    pub load_error: bool,
    pub error_message: String,
}

impl Default for LinkEditorModel {
    fn default() -> Self {
        LinkEditorModel {
            link: TerritoryLinkContract::default(),
            save_success: false,
            save_error: false,
            load_error: false,
            error_message: "".to_string(),
        }
    }
}

#[derive(Clone, Debug, PartialEq, Deserialize, Serialize)]
pub struct LinkEditorParameters {
    pub link_id: Option<String>,
    pub features: Option<String>,
}

#[function_component(LinkEditPage)]
pub fn user_editor_page() -> Html {
    set_document_title("Link Editor");

    let state: yew::UseStateHandle<LinkEditorModel> = use_state(LinkEditorModel::default);
    let location = use_location().expect("Should be a location to get query string");
    let parameters: LinkEditorParameters = location.query::<LinkEditorParameters>().expect("An object");
    let link_id: String = match parameters.link_id { Some(v) => v, _ => "".to_string() };

    let cloned_state = state.clone();
    let link_id = link_id.clone();
    // let save_onclick = Callback::from(move |_: i32| { 
    //     //event.prevent_default();
    //     let cloned_state = cloned_state.clone();
    //     spawn_local(async move {
    //         let uri_string: String = "/api/territory-links".to_string();
    //         let uri: &str = uri_string.as_str();
    //         let body_model = LinkSaveRequest {
    //             created_by: Some("unkown".to_string()),
    //             link: LinkChanges {
    //                 id: cloned_state.link.id.clone(),
    //                 territory_uri: cloned_state.link.territory_uri.clone(),
    //                 alba_mobile_territory_key: cloned_state.link.alba_mobile_territory_key.clone(),
    //                 territory_number: cloned_state.link.territory_number.clone(),
    //                 territory_description: cloned_state.link.territory_description.clone(),
    //                 created: cloned_state.link.created.clone(),
    //                 expires: cloned_state.link.expires.clone(),
    //                 expired: cloned_state.link.expired.clone(),
    //                 assigned_date_utc: cloned_state.link.assigned_date_utc.clone(),
    //                 assignee_name: cloned_state.link.assignee_name.clone(),
    //                 assignee_email: cloned_state.link.assignee_email.clone(),
    //                 assignee_phone: cloned_state.link.assignee_phone.clone(),
    //                 territory_last_completed_by: cloned_state.link.territory_last_completed_by.clone(),
    //                 territory_last_completed_date: cloned_state.link.territory_last_completed_date.clone(),
    //                 stage_id: cloned_state.link.stage_id,
    //                 successful: cloned_state.link.successful.clone(),
    //             }
    //         };
    //         let data_serialized = serde_json::to_string_pretty(&body_model)
    //             .expect("Should be able to serialize address for geocoding into JSON");
    //         let method = if link_id.is_empty() { Method::POST } else { Method::PUT };
    //         let resp = Request::new(uri)
    //             .method(method)
    //             .header("Content-Type", "application/json")
    //             .body(data_serialized)
    //             .send()
    //             .await
    //             .expect("A result from the endpoint");        
    //         let _result = LinkSaveResult {
    //             success: (resp.status() == 200),
    //             errors: Some("".to_string()),
    //             status: resp.status(),
    //             completed: true,
    //         };
    //         //stage_change_result_state_clone.set(result);
    //         let mut modified = cloned_state.deref().clone();
    //         if resp.status() == 200 {
    //             modified.save_success = true;
    //             modified.save_error = false;
    //             // UserSaveResult {
    //             //     success: true,
    //             //     errors: Some("".to_string()),
    //             //     status: resp.status(),
    //             //     completed: true,
    //             // }
    //         } else {
    //             let errors = if (401..403).contains(&resp.status()) { 
    //                 Some("Unauthorized".to_string()) 
    //             } else {
    //                 Some(resp.status().to_string())
    //             };
    //             modified.error_message = errors.unwrap_or_default();
    //             modified.save_error = true;
    //             modified.save_success = false;
    //             // UserSaveResult {
    //             //     success: false,
    //             //     errors,
    //             //     status: resp.status(),
    //             //     completed: true,
    //             // }
    //         };
    //         cloned_state.set(modified);
    //     });
    // });
    //let parameters = parameters.clone();
    let cloned_state = state.clone();
    let uri: String = format!("/api/territory-links/{link_id}");
    use_effect_with((), move |_| {
        http_get_set!(cloned_state.link, uri);
    });

    let cloned_state = state.clone();

    html! {
        <>
            <MenuBarV2>
                <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                    <li class={"nav-item"}>
                        <MapPageLink />
                    </li> 
                </ul>
            </MenuBarV2>
            <div class="container my-2">
                <div class="row g-3 my-3">
                    <div class="col-12 col-sm-6 col-md-4">
                        <span><strong>{"Link Editor"}</strong></span>
                    </div>
                </div>
                <div class="row g-3 my-2">
                    <StringCell label="Key (mkt)" field={field_string!(cloned_state.link.id)} /> 
                    <StringCell label="Territory Number" field={field_string!(cloned_state.link.territory_number)} />
                    <InputCell label="Territory Description" class="col-12" field={field!(cloned_state.link.territory_description)} /> 
                    <InputCell label="Created" field={field!(cloned_state.link.created)} /> 
                    <InputCell label="Created By" field={field!(cloned_state.link.created_by)} /> 
                    <InputCell label="Expires" field={field!(cloned_state.link.expires)} /> 
                    <InputCell label="Assignee Id" field={field!(cloned_state.link.assignee_id)} /> 
                    <StringCell label="Assignee" field={field_string!(cloned_state.link.assignee_name)} /> 
                    <InputCell label="Assignee Email" field={field!(cloned_state.link.assignee_email)} /> 
                    <InputCell label="Assignee Phone" field={field!(cloned_state.link.assignee_phone)} /> 
                </div>
                <div class="row g-3 my-2">
                    if true { //state.user_response.user_can_edit {
                        // <div class="col-12 p-3">
                        //     <ButtonWithConfirm id="save-button" button_text="Save" on_confirm={save_onclick.clone()} />
                        // </div>
                    }
                </div>
            </div>
        </>
    }
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct LinkSaveResult {
    pub success: bool,
    pub errors: Option<String>,
    pub status: u16,
    pub completed: bool,
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct GetResult {
    // pub success: bool,
    // pub errors: Option<String>,
    // pub status: u16,
    // pub completed: bool,
    pub load_error: bool,
    pub error_message: String,
}

pub fn response_status(response: &Response) -> GetResult {
    if response.status() == 200 {
        GetResult::default()
    } else if response.status() == 401 {
        GetResult {
            load_error: true,
            error_message: "Unauthorized".to_string(),
        }
    } else {
        GetResult {
            load_error: true,
            error_message: format!("Error: {}", response.status()),
        }
    }
}

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct LinkSaveRequest {
    pub created_by: Option<String>,
    pub link: LinkChanges,
}