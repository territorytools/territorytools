use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::button_with_confirm::ButtonWithConfirm;
use crate::components::menu_bar::MapPageLink;
use crate::components::text_box::{InputCell, CheckboxCell, TextAreaCell};
use crate::macros::http::LoadStatus;
use crate::macros::save_callback::SaveStatus;
use crate::macros::input_callback_macros::GridInput;
use crate::functions::document_functions::set_document_title;
use crate::models::users::{UserChanges,UserLoadResult};
use crate::{field, field_checked, save_callback, get_result_by_id};

use reqwasm::http::Request;
use reqwasm::http::Method;
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::hooks::use_location;


#[derive(Properties, PartialEq, Clone, Default, Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct UserSaveResult {
    pub user: UserChanges,
    pub success: bool,
    pub errors: Option<String>,
    pub status: SaveStatus,
    pub completed: bool,
}

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct UserSaveRequest {
    pub created_by: Option<String>,
    pub user: UserChanges, // <-- Can I add this later?
}

// #[derive(Properties, PartialEq, Clone, Serialize)]
// pub struct UserEditorModelTest {
//     pub user: UserChanges,
//     pub load_response: UserResponse,
//     // pub save_response: SaveStatus,
//     // pub save_request: SaveUserRequest,

//     pub load_status: LoadStatus,
//     pub save_status: SaveStatus,
// }

#[derive(Properties, PartialEq, Clone, Serialize)]
pub struct UserEditorModel {
    pub user: UserChanges,
    pub load_result: UserLoadResult,
    pub save_result: UserSaveResult,
    pub save_request: UserSaveRequest,
    pub save_success: bool,
    pub save_error: bool,
    #[prop_or_default]
    pub load_error: bool,
    pub error_message: String,
    pub load_status: LoadStatus,
    pub save_status: SaveStatus,
    pub wrapper: Wrapper<UserChanges>,
}

impl Default for UserEditorModel {
    fn default() -> Self {
        UserEditorModel {
            user: UserChanges::default(),
            load_result: UserLoadResult::default(),
            save_result: UserSaveResult::default(),
            save_request: UserSaveRequest::default(),
            save_success: false,
            save_error: false,
            load_error: false,
            error_message: "".to_string(),
            load_status: LoadStatus::default(),
            save_status: SaveStatus::default(),
            wrapper: Wrapper::<UserChanges>::default(),
        }
    }
}

#[derive(Clone, Debug, PartialEq, Deserialize, Serialize, Default)]
#[serde(rename_all = "camelCase")]
pub struct Wrapper<E> {
    pub entity: E,
    pub timestamp: String,
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

    let cloned_state = state.clone();
    let save_onclick = save_callback!(
        "/api/users",
        id: cloned_state.save_request.user.id,
        status: cloned_state.save_status);

    let cloned_state = state.clone();
    let user_id = parameters.user_id.unwrap_or_default();
    use_effect_with((), move |_| {
        get_result_by_id!(
            "/api/users?userId={user_id}", 
            cloned_state.load_result.user.id)
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
                        <a href={my_territories_link} class="btn btn-primary">{"My Territories"}</a>
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <a href="/app/user-edit?user_id=0" class="btn btn-primary">{"New User"}</a>
                    </div>                    
                </div>
                <div class="row g-3 my-2">    
                    <InputCell label="Full Name" field={field!(cloned_state.user.alba_full_name)} /> 
                    <InputCell label="Surname (family name)" field={field!(cloned_state.user.surname)} /> 
                    <InputCell label="Given Name" field={field!(cloned_state.user.given_name)} />                      
                    if state.load_result.email_visible {
                         <InputCell label="Email" field={field!(cloned_state.user.normalized_email)} />  
                         <InputCell label="Phone" field={field!(cloned_state.user.phone)} />  
                    }
                    <InputCell label="Group ID" field={field!(cloned_state.user.group_id)} />  
                </div>
                <div class="row g-3 my-2">
                    <CheckboxCell label="Active" field={field_checked!(cloned_state.user.is_active)} />  
                    if state.load_result.roles_visible {
                        <CheckboxCell label="Can Impersonate Users"  field={field_checked!(cloned_state.user.can_impersonate_users)} /> 
                        <CheckboxCell label="Can Assign Territories" field={field_checked!(cloned_state.user.can_assign_territories)} /> 
                        <CheckboxCell label="Can Edit Territories"   field={field_checked!(cloned_state.user.can_edit_territories)} /> 
                    }
                    <TextAreaCell label="Notes" field={field!(cloned_state.user.notes)} />
                    if state.load_result.roles_visible {
                        <InputCell 
                            class="col-12 col-sm-12 col-md-12" 
                            label="Roles" 
                            field={field!(cloned_state.user.roles)} />  
                    }
                    if state.load_result.user_can_edit {
                        <div class="col-12 p-3">
                            <ButtonWithConfirm id="save-button" button_text="Save" on_confirm={save_onclick.clone()} />
                        </div>
                    }
                    <div class="col-12"><span><small>{"UID:"}{cloned_state.user.id}</small></span></div>
                </div>
            </div>
        </>
    }
}
