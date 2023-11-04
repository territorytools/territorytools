use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::button_with_confirm::ButtonWithConfirm;
use crate::components::menu_bar::MapPageLink;
use crate::components::text_box::{InputCell, CheckboxCell, TextAreaCell};
use crate::macros::input_callback_macros::GridInput;
use crate::functions::document_functions::set_document_title;
use crate::models::users::{UserChanges,UserResponse};
use crate::field_checked;
use crate::field;

use reqwasm::http::Request;
use reqwasm::http::Method;
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::hooks::use_location;

#[derive(Properties, PartialEq, Clone, Serialize)]
pub struct UserEditorModel {
    pub user: UserChanges,
    pub user_response: UserResponse,
    pub save_success: bool,
    pub save_error: bool,
    #[prop_or_default]
    pub load_error: bool,
    pub error_message: String,
}

impl Default for UserEditorModel {
    fn default() -> Self {
        UserEditorModel {
            user: UserChanges::default(),
            user_response: UserResponse::default(),
            save_success: false,
            save_error: false,
            load_error: false,
            error_message: "".to_string(),
        }
    }
}

#[derive(Clone, Debug, PartialEq, Deserialize, Serialize)]
pub struct UserEditorParameters {
    pub user_id: Option<i32>,
    pub features: Option<String>,
}

#[function_component(UserEditorPage)]
pub fn user_editor_page() -> Html {
    set_document_title("User Editor");

    let state: yew::UseStateHandle<UserEditorModel> = use_state(UserEditorModel::default);
    let location = use_location().expect("Should be a location to get query string");
    let parameters: UserEditorParameters = location.query::<UserEditorParameters>().expect("An object");
    let user_id: i32 = match parameters.user_id { Some(v) => v, _ => 0 };

    let cloned_state = state.clone();
    let save_onclick = Callback::from(move |_: i32| { 
        //event.prevent_default();
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            let uri_string: String = "/api/users".to_string();

            let uri: &str = uri_string.as_str();

            let body_model = UserSaveRequest {
                created_by: Some("unkown".to_string()),
                user: UserChanges {
                    id: cloned_state.user.id,
                    alba_full_name: cloned_state.user.alba_full_name.clone(),
                    given_name: cloned_state.user.given_name.clone(),
                    surname: cloned_state.user.surname.clone(),
                    phone: cloned_state.user.phone.clone(),
                    notes: cloned_state.user.notes.clone(),
                    alba_user_id: cloned_state.user.alba_user_id.clone(),
                    normalized_email: cloned_state.user.normalized_email.clone(),
                    group_id: cloned_state.user.group_id.clone(),
                    roles: cloned_state.user.roles.clone(),
                    territory_summary: cloned_state.user.territory_summary.clone(), 
                    is_active: cloned_state.user.is_active,
                    can_assign_territories: cloned_state.user.can_assign_territories,
                    can_edit_territories: cloned_state.user.can_edit_territories,
                    can_impersonate_users: cloned_state.user.can_impersonate_users,                    
                }
            };

            let data_serialized = serde_json::to_string_pretty(&body_model)
                .expect("Should be able to serialize address for geocoding into JSON");

            let method = if user_id == 0 { Method::POST } else { Method::PUT };

            let resp = Request::new(uri)
                .method(method)
                .header("Content-Type", "application/json")
                .body(data_serialized)
                .send()
                .await
                .expect("A result from the endpoint");
            
            let _result = UserSaveResult {
                success: (resp.status() == 200),
                errors: Some("".to_string()),
                status: resp.status(),
                completed: true,
            };

            //stage_change_result_state_clone.set(result);
            let mut modified = cloned_state.deref().clone();

            if resp.status() == 200 {
                modified.save_success = true;
                modified.save_error = false;
                // UserSaveResult {
                //     success: true,
                //     errors: Some("".to_string()),
                //     status: resp.status(),
                //     completed: true,
                // }
            } else {
                let errors = if (401..403).contains(&resp.status()) { 
                    Some("Unauthorized".to_string()) 
                } else {
                    Some(resp.status().to_string())
                };
                modified.error_message = errors.unwrap_or_default();
                modified.save_error = true;
                modified.save_success = false;
                // UserSaveResult {
                //     success: false,
                //     errors,
                //     status: resp.status(),
                //     completed: true,
                // }
            };

            cloned_state.set(modified);
        });
    });
   
    let cloned_state = state.clone();
    use_effect_with((), move |_| {
        let cloned_state = cloned_state.clone();
        let user_id = parameters.user_id.unwrap_or_default();
        wasm_bindgen_futures::spawn_local(async move {
            let uri: String = format!("/api/users?userId={user_id}");

            let response = Request::get(uri.as_str())
                .send()
                .await
                .expect("User response (raw) from API");
            
            if response.status() == 200 {
                let user_response: UserResponse = response
                    .json()
                    .await
                    .expect("Valid UserResponse JSON from API");
  
                let model: UserEditorModel = UserEditorModel {
                    user: user_response.clone().user.unwrap_or_default(),
                    user_response: user_response.clone(),
                    ..UserEditorModel::default()
                };

                cloned_state.set(model);
            } else if response.status() == 401 {
                let model: UserEditorModel = UserEditorModel {
                    user: UserChanges::default(),
                    user_response: UserResponse::default(),
                    load_error: true,
                    error_message: "Unauthorized".to_string(),
                    ..UserEditorModel::default()
                };

                cloned_state.set(model);
            }
        });
        || ()
    });

    let full_name = state.user.alba_full_name.clone().unwrap_or_default();
    let my_territories_link = format!("/app/my-territories?impersonate={full_name}");
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
                        <span><strong>{"User Editor"}</strong></span>
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <a href={my_territories_link} class="btn btn-primary">{"View My Territories Page"}</a>
                    </div>                    
                </div>
                <div class="row g-3 my-2">    
                    <InputCell label="Full Name" field={field!(cloned_state.user.alba_full_name)} /> 
                    <InputCell label="Surname (family name)" field={field!(cloned_state.user.surname)} /> 
                    <InputCell label="Given Name" field={field!(cloned_state.user.given_name)} />                      
                    if state.user_response.email_visible {
                         <InputCell label="Email" field={field!(cloned_state.user.normalized_email)} />  
                         <InputCell label="Phone" field={field!(cloned_state.user.phone)} />  
                    }
                    <InputCell label="Group ID" field={field!(cloned_state.user.group_id)} />  
                </div>
                <div class="row g-3 my-2">
                    <CheckboxCell label="Active" field={field_checked!(cloned_state.user.is_active)} />  
                    if state.user_response.roles_visible {
                        <CheckboxCell label="Can Impersonate Users"  field={field_checked!(cloned_state.user.can_impersonate_users)} /> 
                        <CheckboxCell label="Can Assign Territories" field={field_checked!(cloned_state.user.can_assign_territories)} /> 
                        <CheckboxCell label="Can Edit Territories"   field={field_checked!(cloned_state.user.can_edit_territories)} /> 
                    }
                    <TextAreaCell label="Notes" field={field!(cloned_state.user.notes)} />
                    if state.user_response.roles_visible {
                        <InputCell 
                            class="col-12 col-sm-12 col-md-12" 
                            label="Roles" 
                            field={field!(cloned_state.user.roles)} />  
                    }
                    if state.user_response.user_can_edit {
                        <div class="col-12 p-3">
                            <ButtonWithConfirm id="save-button" button_text="Save" on_confirm={save_onclick.clone()} />
                        </div>
                    }
                </div>
            </div>
        </>
    }
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct UserSaveResult {
    pub success: bool,
    pub errors: Option<String>,
    pub status: u16,
    pub completed: bool,
}

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct UserSaveRequest {
    pub created_by: Option<String>,
    pub user: UserChanges,
}