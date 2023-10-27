//use crate::components::menu_bar::MenuBar;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::button_with_confirm::ButtonWithConfirm;
use crate::components::menu_bar::MapPageLink;
use crate::models::users::UserSummary;
use crate::functions::document_functions::set_document_title;

//use gloo_console::log;
use reqwasm::http::Request;
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_hooks::use_clipboard;
use yew_router::hooks::use_location;
//use serde_json::ser::to_string;

#[derive(Properties, PartialEq, Clone, Serialize)]
pub struct UserEditorModel {
    pub user: UserSummary,
    pub save_success: bool,
    pub save_error: bool,
    #[prop_or_default]
    pub load_error: bool,
    pub error_message: String,
}

impl Default for UserEditorModel {
    fn default() -> Self {
        UserEditorModel {
            user: UserSummary::default(),
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

    let state: yew::UseStateHandle<UserEditorModel> = use_state(|| UserEditorModel::default());
    let clipboard = use_clipboard();
    let cloned_state = state.clone();
    let location = use_location().expect("Should be a location to get query string");
    let parameters: UserEditorParameters = location.query::<UserEditorParameters>().expect("An object");
    let user_id: i32 = match parameters.user_id {
        Some(v) => v,
        _ => 0,
    };

    let email_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.user.normalized_email = Some(value.to_string());
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

            modification.user.group_id = Some(value);   
            state.set(modification);
        })
    };

    /*
    let cloned_state = state.clone();
    let save_onclick = Callback::from(move |_: i32| { 
        //event.prevent_default();
        let cloned_state = cloned_state.clone();
        spawn_local(async move {
            let uri_string: String = format!("{path}?territoryId={territory_id}&stageId={to_stage_id}&assignee={assignee}", 
                path = "/api/users",
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
    */
   
    let cloned_state = state.clone();
    use_effect_with((), move |_| {
        let cloned_state = cloned_state.clone();
        let user_id = parameters.user_id.clone().unwrap_or_default();
        wasm_bindgen_futures::spawn_local(async move {
            let uri: String = format!("/api/users?userId={user_id}");

            let user_response = Request::get(uri.as_str())
                .send()
                .await
                .expect("User response (raw) from API");
            
            if user_response.status() == 200 {
                let fetched_user: UserSummary = user_response
                    .json()
                    .await
                    .expect("Valid territory JSON from API");

                let model: UserEditorModel = UserEditorModel {
                    user: fetched_user,
                    ..UserEditorModel::default()
                };

                cloned_state.set(model);
            } else if user_response.status() == 401 {
                let model: UserEditorModel = UserEditorModel {
                    user: UserSummary::default(),
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
            <div class="container pt-3">
                <div class="row g-3 pt-3">
                    <div class="col-12 col-sm-6 col-md-4">
                        <span><strong>{"User Editor"}</strong></span>
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <a href={my_territories_link} class="btn btn-primary">{"View My Territories Page"}</a>
                    </div>                    
                </div>
                <div class="row g-3 pt-3">    
                    <div class="col-12 col-sm-6 col-md-4">
                        <label class="form-label">{"Full Name"}</label>
                        <input 
                            id="full-name-to-input" 
                            value={cloned_state.user.alba_full_name.clone()} 
                            type="text" 
                            class="form-control shadow-sm" />                       
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <label class="form-label">{"Email"}</label>
                        <div class="input-group">
                            <input 
                                id="email-to-input" 
                                value={cloned_state.user.normalized_email.clone()} 
                                onchange={group_id_onchange}
                                type="text" 
                                class="form-control shadow-sm" />                       
                        </div>
                    </div>
                    <div class="col-6 col-sm-4 col-md-3">
                        <label class="form-label">{"Group ID"}</label>
                        <input 
                            id="group-id-input" 
                            value={cloned_state.user.group_id.clone()} 
                            type="text" 
                            class="form-control shadow-sm" />                       
                    </div>
                    <div class="col-3 col-sm-3 col-md-2">
                        <input 
                            id="is-active-input" 
                            checked={cloned_state.user.is_active} 
                            type="checkbox" 
                            class="form-check-input shadow-sm" />                       
                        <label class="form-check-label">{"Active"}</label>
                    </div>
                    <div class="col-12">
                        <label class="form-label">{"Notes"}</label>
                        <textarea 
                            id="group-id-input" 
                            value={cloned_state.user.notes.clone()} 
                            rows="3"
                            class="form-control shadow-sm" />                       
                    </div>
                    <div class="col-12">
                        // <ButtonWithConfirm 
                        //     id="save-button" 
                        //     button_text="Save" 
                        //     on_confirm={save_onclick.clone()} 
                        // />
                    </div>
                </div>
            </div>
        </>
    }
}

