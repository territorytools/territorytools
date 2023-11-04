use crate::components::button_with_confirm::ButtonWithConfirm;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::components::text_box::{InputCell, StringCell};
use crate::functions::document_functions::set_document_title;
use crate::macros::http::{SaveStatus, LoadStatus};
use crate::macros::input_callback_macros::GridInput;
use crate::models::territory_links::{LinkChanges, TerritoryLinkContract};
use crate::{field, field_string, http_get_set, save_callback};

use reqwasm::http::{Request, Method};
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen_futures::spawn_local;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::hooks::use_location;

#[derive(Default, Properties, PartialEq, Clone, Serialize)]
pub struct LinkEditorModel {
    pub link: TerritoryLinkContract,
    pub save_status: SaveStatus,
    pub load_status: LoadStatus,
}

#[derive(Clone, Debug, PartialEq, Deserialize, Serialize)]
pub struct LinkEditorParameters {
    pub link_id: Option<String>,
    pub features: Option<String>,
}

#[function_component(LinkEditPage)]
pub fn user_editor_page() -> Html {
    set_document_title("Link Editor");

    let state = use_state(LinkEditorModel::default);
    let location = use_location().expect("Should be a location to get query string");
    let parameters: LinkEditorParameters = location.query::<LinkEditorParameters>().expect("An object");
    let link_id: String = parameters.link_id.clone().unwrap_or_default();

    let cloned_state = state.clone();
    let save_uri: String = "/api/territory-links".to_string();
    let save_onclick = save_callback!(cloned_state.link, save_uri);
   
    let cloned_state = state.clone();
    let uri: String = format!("/api/territory-links/{}", link_id.clone());
    use_effect_with((), move |_| {
        http_get_set!(cloned_state.link, uri);
    });

    let state = state.clone();

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
                    <StringCell label="Key (mkt)" field={field_string!(state.link.id)} /> 
                    <StringCell label="Territory Number" field={field_string!(state.link.territory_number)} />
                    <InputCell label="Territory Description" class="col-12" field={field!(state.link.territory_description)} /> 
                    <InputCell label="Created" field={field!(state.link.created)} /> 
                    <InputCell label="Created By" field={field!(state.link.created_by)} /> 
                    <InputCell label="Expires" field={field!(state.link.expires)} /> 
                    <InputCell label="Assignee Id" field={field!(state.link.assignee_id)} /> 
                    <StringCell label="Assignee" field={field_string!(state.link.assignee_name)} /> 
                    <InputCell label="Assignee Email" field={field!(state.link.assignee_email)} /> 
                    <InputCell label="Assignee Phone" field={field!(state.link.assignee_phone)} /> 
                </div>
                <div class="row g-3 my-2">
                    if true { //state.user_response.user_can_edit {
                        <div class="col-12 p-3">
                            <ButtonWithConfirm id="save-button" button_text="Save" on_confirm={save_onclick.clone()} />
                            if state.save_status.save_error {
                                <span class="mx-1 badge bg-danger">{"Error"}</span>
                                <span class="mx-1 fw-bold text-danger">{state.save_status.error_message.clone()}</span>
                            }
                            if state.save_status.save_success {
                                <span class="mx-1 badge bg-success">{"Saved"}</span>
                            }
                        </div>
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

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct LinkSaveRequest {
    pub created_by: Option<String>,
    pub link: LinkChanges,
}