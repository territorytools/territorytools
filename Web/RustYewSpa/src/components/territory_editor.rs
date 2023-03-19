//use crate::components::menu_bar::MenuBar;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::models::territories::Territory;
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

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "PUT";

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

#[derive(Clone, Debug, PartialEq, Deserialize)]
pub struct TerritoryEditorParameters {
    pub id: Option<i32>,
}

// pub fn add_field(state: yew::UseStateHandle<AddressEditModel>, field: &str) {
//     state.
// }

#[function_component(TerritoryEditorPage)]
pub fn territory_editor_page() -> Html {
    set_document_title("Territory Editor");

    let state: yew::UseStateHandle<TerritoryEditorModel> = use_state(|| TerritoryEditorModel::default());
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

            modification.territory.stage_id = value.parse().unwrap();
            
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
        wasm_bindgen_futures::spawn_local(async move {
            log!("Loading territory...");
            let territory_id: i32 = territory_id;
            let uri: String = format!(
                "{base_path}?id={territory_id}", 
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
    

    // let onsubmit = Callback::from(move |event: SubmitEvent| {
    //     event.prevent_default();
    // });
    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |event: SubmitEvent| {   //model: AddressEditModel| { //
        event.prevent_default();
        let cloned_state = cloned_state.clone();
        //let navigator = navigator.clone();
        spawn_local(async move {
            log!("Spawing request for territory change...");
            let model = cloned_state.clone();
            let uri_string: String = format!("{path}", 
                path = PUT_TERRITORY_API_PATH);

            let uri: &str = uri_string.as_str();
            
            let _method: Method = match ASSIGN_METHOD {
                "PUT" => Method::PUT,
                "GET" => Method::PUT,
                &_ =>  Method::GET,
            };

            let body_model = &model.deref();
            let data_serialized = serde_json::to_string_pretty(&body_model.territory)
                .expect("Should be able to serialize address edit form into JSON");

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

    let selected_stage_id: i32 = state.territory.stage_id;
    //let selected_status_id: i32 = state.territory.status_id;

    html! {
        <>
        <MenuBarV2>
            <ul class="navbar-nav ms-2 me-auto mb-05 mb-lg-0">
                <li class="nav-item">
                    <a class="nav-link active" aria-current="page" href="/app/address-search">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-search" viewBox="0 0 16 16">
                            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                        </svg>
                        <span class="ms-1">{"Search"}</span>
                    </a>
                </li> 
            </ul>
        </MenuBarV2>
        <div class="container">
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
                <div class="col-12 col-sm-6 col-md-4">
                    <label for="input-stage" class="form-label">{"Stage"}</label>
                    <select onchange={stage_id_onchange} id="input-stage" class="form-select shadow-sm">
                        <EnglishChineseIdOption id={1} english="None" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={10} english="Available" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={20} english="Writing Letters" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={21} english="Letters Sent" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={22} english="Letters Returned" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={30} english="Calling" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={31} english="Called" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={40} english="Visiting" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={41} english="Done" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={50} english="Cooling Off" chinese="" selected={selected_stage_id} />
                        <EnglishChineseIdOption id={60} english="Reserved" chinese="" selected={selected_stage_id} />
                    </select>
                </div>
                // <div class="col-12 col-sm-6 col-md-4">
                //     <label for="input-delivery-status" class="form-label">{"Mail Delivery Status"}</label>
                //     <AddressDeliveryStatusSelector 
                //         onchange={delivery_status_onchange} 
                //         id={state.address.delivery_status_id} />
                // </div>
                <div class="col-12">
                    <label for="inputNumber" class="form-label">{"Number"}</label>
                    <input value={state.territory.number.clone()} onchange={number_onchange} type="text" class="form-control shadow-sm" id="inputNumber" placeholder="Number"/>
                </div>
                <div class="col-12">
                    <label for="inputDescription" class="form-label">{"Description"}</label>
                    <input value={state.territory.description.clone()} onchange={description_onchange} type="text" class="form-control shadow-sm" id="inputDescription" placeholder="Description"/>
                </div>
                // <div class="col-12">
                //     <label for="input-notes" class="form-label">{"笔记 Notes"}</label>
                //     <textarea value={state.address.notes.clone()} onchange={notes_onchange} type="text" rows="2" cols="30" class="form-control shadow-sm" id="input-notes" placeholder="Notes"/>
                // </div>

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
                </div>
                <div class="col-12">
                    <span><small>{"TID: "}{state.territory.id}</small></span>
                    <span><small>{"ATID: "}{state.territory.id}</small></span>
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