//use crate::components::menu_bar::MenuBar;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::state_selector::SelectAddressState;
use crate::components::address_delivery_status_selector::AddressDeliveryStatusSelector;
use crate::components::territory_editor::TerritoryEditorParameters;
use crate::models::addresses::Address;
use crate::models::geocoding_coordinates::AddressToGeocode;
use crate::models::geocoding_coordinates::GeocodingCoordinates;
use crate::functions::document_functions::set_document_title;
use crate::Route;
//use std::fmt::Display;
//use crate::models::territories::Territory;
//use serde::{Deserialize, Serialize};
use gloo_console::log;
use reqwasm::http::{Request, Method};
use serde::{Serialize, Deserialize};
use std::ops::Deref;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::hooks::use_location;
use yew_router::prelude::use_navigator;
//use serde_json::ser::to_string;

#[cfg(debug_assertions)]
const DEBUG_IS_ON: bool = true;

#[cfg(not(debug_assertions))]
const DEBUG_IS_ON: bool = false;

// Uncomment for debugging without an API server
// const DATA_API_PATH: &str = "/data/put_address.json";
// const GET_ADDRESSES_API_PATH: &str = "/data/get_address.json?slash=";
// const GET_GEOCODING_API_PATH: &str = "/data/geocoding.json?slash=";

const DATA_API_PATH: &str = "/api/addresses/save";
const GET_ADDRESSES_API_PATH: &str = "/api/addresses/address-id";
const GET_GEOCODING_API_PATH: &str = "/api/geocoding";
const ASSIGN_METHOD: &str = "PUT";

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct AddressEditModel {
    pub address: Address,
    // TODO: OriginalAddress (Used to compare by server)
    pub address_id: i32,
    pub save_success: bool,
    pub save_error: bool,
    #[prop_or_default]
    pub load_error: bool,
    pub error_message: String,
    pub geocoding_error: String,
    pub geocoding_success: String,
    pub territory_was_changed: bool,
    pub moved_to_territory_number: String,
    pub show_visits: bool,
    pub show_address_marker: bool,
    pub address_marking_error: String,
    pub new_visit_id: i32,
}


#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct AddressMarkModel {
    pub address_id: i32,
    pub mark_type: String,
    pub mark_date_utc: String,
}


// #[derive(Properties, PartialEq, Clone, Default, Deserialize, Serialize)]
// pub struct AddressResponse {
//     pub addresses: Vec<Address>,
// }

// #[derive(Properties, PartialEq, Clone, Default)]
// pub struct AddressEditProps {
//     pub address_id: i32,
// }

#[derive(Clone, Debug, PartialEq, Deserialize)]
pub struct AddressEditParameters {
    pub address_id: Option<i32>,
    pub mtk: Option<String>,
    pub territory_number: Option<String>,
    pub features: Option<String>,
}

// pub fn add_field(state: yew::UseStateHandle<AddressEditModel>, field: &str) {
//     state.
// }

#[function_component(AddressEditPage)]
pub fn address_edit_page() -> Html {
    set_document_title("Address Edit");

    let state: yew::UseStateHandle<AddressEditModel> = use_state(|| AddressEditModel::default());
    let address_mark_model: yew::UseStateHandle<AddressMarkModel> = use_state(|| AddressMarkModel::default());
    let cloned_state = state.clone();
    let location = use_location().expect("Should be a location to get query string");
    let parameters: AddressEditParameters = location.query().expect("An object");
    let address_id: i32 = match parameters.address_id {
        Some(v) => v,
        _ => 0,
    };

    let navigator = use_navigator().unwrap();
    let close_onclick = {
        Callback::from(move |_| {
            navigator.back();
        })
    };

    let language_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.language_id = value.parse().unwrap();
            // Clear the status string to prevent confusion about which one is right
            modification.address.language = Some("".to_string());

            log!(format!("Address language id set to {name:?}", name = modification.address.language_id));

            state.set(modification);
        })
    };

    let status_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.status_id = value.parse().unwrap();
            // Clear the status string to prevent confusion about which one is right
            modification.address.status = Some("".to_string());

            log!(format!("Address status id set to {name:?}", name = modification.address.status_id));

            state.set(modification);
        })
    };
   
    let delivery_status_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |value: String| {
            let mut modification = state.deref().clone();
            modification.address.delivery_status_id = value.parse().unwrap();
            state.set(modification);
        })
    };

    //last_delivery_status_date_utc
    let last_delivery_status_date_utc_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.last_delivery_status_date_utc = Some(value);
            state.set(modification);
        })
    };

    let territory_number_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.territory_number = Some(value);

            log!(format!("Territory number to {number:?}", number = modification.address.territory_number.clone()));

            state.set(modification);
        })
    };

    let territory_number = state.address.territory_number.clone().unwrap_or_default();
    let navigator = use_navigator().unwrap();
    let territory_open_onclick = {
        let navigator = navigator.clone();
        //let territory_number = territory_number.clone();
        Callback::from(move |_| {
            let query = TerritoryEditorParameters {
                id: None,
                number: Some(territory_number.clone()),
            };
            let _ = navigator.push_with_query(&Route::TerritoryEditor, &query);
        })
    };
    
    let name_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.name = Some(value);

            log!(format!("Address name set to {name:?}", name = modification.address.name.clone()));

            state.set(modification);
        })
    };

    let street_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.street = Some(value);

            log!(format!("Address Address set to {street:?}", street = modification.address.street));

            state.set(modification);
        })
    };

    let unit_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.unit = Some(value);

            log!(format!("Address unit set to {street:?}", street = modification.address.unit));

            state.set(modification);
        })
    };

    let city_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.city = Some(value);

            log!(format!("Address City set to {name:?}", name = modification.address.city.clone()));

            state.set(modification);
        })
    };
 
    let state_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |value: String| {
            let mut modification = state.deref().clone();
            modification.address.state = Some(value);

            log!(format!("Address State set to {name:?}", name = modification.address.state.clone()));

            state.set(modification);
        })
    };

    // let state_onchange = {
    //     let state = cloned_state.clone();
    //     Callback::from(move |event: Event| {
    //         let mut modification = state.deref().clone();
    //         let value = event
    //             .target()
    //             .unwrap()
    //             .unchecked_into::<HtmlInputElement>()
    //             .value();

    //         modification.address.state = Some(value);

    //         log!(format!("Address State set to {name:?}", name = modification.address.state.clone()));

    //         state.set(modification);
    //     })
    // };
 
    let postal_code_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.postal_code = Some(value);

            log!(format!("Address Postal Code set to {name:?}", name = modification.address.postal_code.clone()));

            state.set(modification);
        })
    };
    
    let phone_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.phone = Some(value);

            log!(format!("Address phone set to {name:?}", name = modification.address.phone.clone()));

            state.set(modification);
        })
    };

    let longitude_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.longitude = value.parse().unwrap();

            log!(format!("Address longitude set to {name:?}", name = modification.address.longitude.clone()));

            state.set(modification);
        })
    };

    let latitude_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.latitude = value.parse().unwrap();

            log!(format!("Address latitude set to {name:?}", name = modification.address.latitude.clone()));

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

            modification.address.notes = Some(value);

            log!(format!("Address notes set to {name:?}", name = modification.address.notes.clone()));

            state.set(modification);
        })
    };

    let mark_type_onchange = {
        let address_mark_model_clone = address_mark_model.clone();
        Callback::from(move |event: Event| {
            let mut modification = address_mark_model_clone.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.mark_type = value.clone();

            log!(format!("Mark type set to {:?}", value.clone()));

            address_mark_model_clone.set(modification);
        })
    };

    let mark_date_onchange = {
        let address_mark_model_clone = address_mark_model.clone();
        Callback::from(move |event: Event| {
            let mut modification = address_mark_model_clone.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.mark_date_utc = value.clone();

            log!(format!("Mark date set to {:?}", value.clone()));

            address_mark_model_clone.set(modification);
        })
    };

    let mtk = parameters.mtk.clone().unwrap_or_default();
    let address_mark_model_clone = address_mark_model.clone();
    let cloned_state = state.clone();
    let mark_onclick = {
        Callback::from(move |event: MouseEvent| {
            event.prevent_default();
            let mtk = mtk.clone();
            let mark_type = address_mark_model_clone.mark_type.to_string();
            let mark_date_utc = address_mark_model_clone.mark_date_utc.to_string();
            let cloned_state = cloned_state.clone();
            spawn_local(async move {
                let uri_string: String = format!("/api/address-marking?mtk={mtk}&addressId={address_id}&markType={mark_type}&dateTimeUtc={mark_date_utc}");
    
                let uri: &str = uri_string.as_str();
                
                let resp = Request::new(uri)
                        .method(Method::POST)
                        .send()
                        .await
                        .expect("A result from the endpoint");

                if resp.status() == 200 {
                    let mark_address_result: MarkAddressResult = resp
                        .json()
                        .await
                        .expect("Valid MarkAddressResult JSON from API");

                    let mut modification = cloned_state.deref().clone();
        
                    modification.address.visits = mark_address_result.address.unwrap_or_default().visits.clone();
                    modification.address_marking_error = "".to_string();
                    modification.new_visit_id = mark_address_result.new_visit_id;
                    modification.show_visits = true;

                    cloned_state.set(modification);
                } else {
                    let mut modification = cloned_state.deref().clone();
        
                    modification.address_marking_error = 
                        format!("Error {}",
                            resp.status());
        
                    cloned_state.set(modification);
                }
            })
        })
    };

    let cloned_state = state.clone();
    let visits_onclick = {
        Callback::from(move |event: MouseEvent| {
            event.prevent_default();
            let mut modification = cloned_state.deref().clone();
            
            modification.show_visits = !cloned_state.show_visits;

            cloned_state.set(modification);
        })
    };
   
    let cloned_state = state.clone();
    let show_address_marker_onclick = {
        Callback::from(move |event: MouseEvent| {
            event.prevent_default();
            let mut modification = cloned_state.deref().clone();
            
            modification.show_address_marker = !cloned_state.show_address_marker;

            cloned_state.set(modification);
        })
    };
   
    let no_visits_onclick = {
        Callback::from(move |event: MouseEvent| {
            event.prevent_default();
        })
    };

    //let territories = use_state(|| vec![]);

    let mtk = parameters.mtk.clone().unwrap_or_default();
    let cloned_state = state.clone();
    use_effect_with_deps(move |_| {
        let cloned_state = cloned_state.clone();
        let mtk = mtk.clone();
        wasm_bindgen_futures::spawn_local(async move {
            if address_id == 0 {            
                log!("AddressId is zero, loading new empty address...");
                cloned_state.set(AddressEditModel {
                    address: Address {
                        state: Some("WA".to_string()),
                        country: Some("us".to_string()),
                        ..Address::default()
                    },
                    address_id: 0,
                    save_success: false,
                    save_error: false,
                    load_error: false,
                    error_message: "".to_string(),
                    ..AddressEditModel::default()
                });
            } else {
                log!("Loading address...");
                let uri: String = format!(
                    "{base_path}/{address_id}?address_id={address_id}&mtk={mtk}", 
                    base_path = GET_ADDRESSES_API_PATH);

                let address_response = Request::get(uri.as_str())
                    .send()
                    .await
                    .expect("Address response (raw) from API");
                
                if address_response.status() == 200 {
                    let fetched_address: Address = address_response
                        .json()
                        .await
                        .expect("Valid address JSON from API");

                    log!(format!(
                        "Fetched address 1, street: {street:?}",
                        street = fetched_address.street
                    ));

                    //let fetched_address_clone = fetched_address.clone();

                    let model: AddressEditModel = AddressEditModel {
                        address: fetched_address,
                        address_id,
                        save_success: false,
                        save_error: false,
                        load_error: false,
                        error_message: "".to_string(),
                        ..AddressEditModel::default()
                    };

                    log!(format!(
                        "Fetched address 2, street: {street:?}",
                        street = model.address.street
                    ));

                    cloned_state.set(model);
                } else if address_response.status() == 401 {
                    let model: AddressEditModel = AddressEditModel {
                        address: Address::default(),
                        address_id,
                        save_success: false,
                        save_error: false,
                        load_error: true,
                        error_message: "Unauthorized".to_string(),
                        ..AddressEditModel::default()
                    };

                    cloned_state.set(model);
                }
            }
        });
        || ()
    }, ());

    let cloned_state = state.clone();
    let mtk = parameters.mtk.clone().unwrap_or_default();
    let save_onclick = Callback::from(move |event: MouseEvent| {   //model: AddressEditModel| { //
        event.prevent_default();
        let cloned_state = cloned_state.clone();
        //let navigator = navigator.clone();
        let mtk = mtk.clone();
        spawn_local(async move {
            log!("Spawing request for address change...");
            let model = cloned_state.clone();
            let uri_string: String = format!("{path}?mtk={mtk}", 
                path = DATA_API_PATH);

            let uri: &str = uri_string.as_str();
            let body_model = &model.deref();
            let data_serialized = serde_json::to_string_pretty(&body_model.address)
                .expect("Should be able to serialize address edit form into JSON");

            // TODO: FetchService::fetch accepts two parameters: a Request object and a Callback.
            // https://yew.rs/docs/0.18.0/concepts/services/fetch
            let resp = if ASSIGN_METHOD == "PUT" {
                Request::new(uri)
                    .method(Method::PUT)
                    .header("Content-Type", "application/json")
                    .body(data_serialized)
                    .send()
                    .await
                    .expect("A result from the endpoint")
            } else {
                Request::new(uri)
                    .method(Method::GET)
                    .header("Content-Type", "application/json")
                    .send()
                    .await
                    .expect("A result from the fake endpoint for debugging")
            };

         

            // TODO: Can I just set this? make it mut?
            //&model.saved_success = true;

            // let link_contract: TerritoryLinkContract = if resp.status() == 200 {
            //     resp.json().await.unwrap()
            // } else {
            //     TerritoryLinkContract::default()
            // };
            
            // let result = TerritoryEditResult {
            //     success: (resp.status() == 200),
            //     status: resp.status(),
            //     completed: true,
            // };

            // // // //cloned_state.set(result);
            
            // TODO: Check for errors
            if resp.status() == 200 {
                let save_result: AddressSaveResult = resp.json().await
                .expect("A valid AddressSaveResult JSON");

                let model: AddressEditModel = AddressEditModel {
                    address: cloned_state.address.clone(), //Address::default(),
                    address_id,
                    save_success: true,
                    save_error: false,
                    load_error: false,
                    territory_was_changed: save_result.territory_was_changed,
                    moved_to_territory_number: save_result.territory_number.unwrap_or_default(),
                    error_message: "".to_string(),
                    ..AddressEditModel::default()
                };
    
                cloned_state.set(model);
            } else if resp.status() == 401 {
                let model: AddressEditModel = AddressEditModel {
                    address: cloned_state.address.clone(), //Address::default(),
                    address_id,
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    territory_was_changed: false,
                    moved_to_territory_number: "".to_string(),
                    error_message: "Unauthorized".to_string(),
                    ..AddressEditModel::default()
                };
    
                cloned_state.set(model);
            } else if resp.status() == 403 {
                let model: AddressEditModel = AddressEditModel {
                    address: cloned_state.address.clone(), //Address::default(),
                    address_id,
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    territory_was_changed: false,
                    moved_to_territory_number: "".to_string(),
                    error_message: "Forbidden".to_string(),
                    ..AddressEditModel::default()
                };
    
                cloned_state.set(model);
            } else {
                let model: AddressEditModel = AddressEditModel {
                    address: cloned_state.address.clone(), //Address::default(),
                    address_id,
                    save_success: false,
                    save_error: true,
                    load_error: false,
                    territory_was_changed: false,
                    moved_to_territory_number: "".to_string(),
                    error_message: format!("{}", resp.status()),
                    ..AddressEditModel::default()
                };
    
                cloned_state.set(model);
            }
        });
    });

    let cloned_state = state.clone();
    let mtk = parameters.mtk.clone().unwrap_or_default();
    let geocode_click = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        let cloned_state = cloned_state.clone();
        let mtk = mtk.clone();
        spawn_local(async move {
            log!("Spawing request for geocoding...");
            let model = cloned_state.clone();
            let uri_string: String = format!("{path}?mtk={mtk}", 
                path = GET_GEOCODING_API_PATH);
                
            let uri: &str = uri_string.as_str();

            let address = &model.address;
            let body_model = AddressToGeocode {
                unit: address.unit.clone(),
                address: address.street.clone(),
                city: address.city.clone(),
                state: address.state.clone(),
                postal_code: address.postal_code.clone(),
                country: address.country.clone(),
            };

            let data_serialized = serde_json::to_string_pretty(&body_model)
                .expect("Should be able to serialize address for geocoding into JSON");

            // TODO: FetchService::fetch accepts two parameters: a Request object and a Callback.
            // https://yew.rs/docs/0.18.0/concepts/services/fetch
            let resp = if DEBUG_IS_ON {
                Request::new(uri)
                    .method(Method::POST)
                    .header("Content-Type", "application/json")
                    .body(data_serialized)
                    .send()
                    .await
                    .expect("A result from the fake endpoint for debugging")
            } else {
                Request::new(uri)
                    .method(Method::POST)
                    .header("Content-Type", "application/json")
                    .body(data_serialized)
                    .send()
                    .await
                    .expect("A result from the endpoint")                
            };
            
            // let mut latitude = cloned_state.address.latitude;
            // let mut longitude =  cloned_state.address.longitude;
            // let mut geocoding_success = "".to_string();
            // let mut geocoding_error = "".to_string();

            let geocoding_result =
            if resp.status() == 200 {
                let geocoding_result: GeocodingCoordinates = resp
                    .json()
                    .await
                    .expect("Valid GeocodingCoordinates JSON from API");

                if geocoding_result.score >= 4.0 {
                    GeocodingResultMessages {
                        success: "Geocoding Successful".to_string(),
                        error: "".to_string(),
                        latitude: geocoding_result.latitude as f32,
                        longitude: geocoding_result.longitude as f32,
                    }
                } else {
                    GeocodingResultMessages {
                        success: "".to_string(),
                        error: format!("Geocode Failure Score: {:0.1}", geocoding_result.score),
                        ..Default::default()
                    }
                }
            } else if resp.status() == 401 {
                GeocodingResultMessages {
                    success: "".to_string(),
                    error: "Authentication Error".to_string(),
                    ..Default::default()
                }
            } else if resp.status() == 403 {
                GeocodingResultMessages {
                    success: "".to_string(),
                    error: "Unauthorized".to_string(),
                    ..Default::default()
                }
            } else if resp.status() == 404 {
                GeocodingResultMessages {
                    success: "".to_string(),
                    error: "Cannot find address!".to_string(),
                    ..Default::default()
                }
            } else {
                GeocodingResultMessages {
                    success: "".to_string(),
                    error: format!("Error {}", resp.status()),
                    ..Default::default()
                }
            };

            let address_model = Address {
                latitude: geocoding_result.latitude,
                longitude: geocoding_result.longitude,
                ..cloned_state.address.clone()   
            };

            let cloned_state = cloned_state.clone();
            let model: AddressEditModel = AddressEditModel {
                address: address_model, //cloned_state.address.clone(), 
                address_id: cloned_state.address_id,
                save_success: cloned_state.save_success,
                save_error: cloned_state.save_error,
                load_error: cloned_state.load_error,
                territory_was_changed: false,
                moved_to_territory_number: "".to_string(),
                error_message: cloned_state.error_message.clone(),
                geocoding_success: geocoding_result.success.clone(),
                geocoding_error: geocoding_result.error.clone(),
                show_visits: cloned_state.show_visits,
                show_address_marker: cloned_state.show_address_marker,
                address_marking_error: cloned_state.address_marking_error.clone(),
                new_visit_id: cloned_state.new_visit_id,
            };

            cloned_state.set(model);
        });
    });

    let features = parameters.features.clone().unwrap_or_default();
    let features: Vec<_> = features.split(',').collect();
    //let show_alba_address_id = features.clone().iter().any(|&i| i=="show-alba-address-id");
    let show_alba_address_id = features.clone().contains(&"show-alba-address-id");
    let show_delivery_status_date = features.clone().contains(&"show-delivery-status-date");
    
    // TODO: This language_id is a hack, this should be in some sort of configuration
    let selected_language_id: i32 = if state.address.language_id == 0 { 83 } else { state.address.language_id };
    let selected_status_id: i32 = if state.address.status_id == 0 { 1 } else { state.address.status_id };
    log!(format!("selected_language_id: {}, selected_status_id: {}", selected_language_id, selected_status_id));


    let mtk = parameters.mtk.clone().unwrap_or_default();
    let is_new_address: bool = state.address_id == 0;
    let territory_number_param = parameters.territory_number.clone().unwrap_or_default();
    let new_address_uri = format!("?address_id=0&mtk={mtk}&territory_number={territory_number_param}");
    let territory_number = state.address.territory_number.clone().unwrap_or_default();
    let territory_number = if territory_number.is_empty() {
        territory_number_param
    } else {
        territory_number.clone()
    };

    let map_uri = format!("https://www.google.com/maps/search/{lat},{lon}",
        lat = state.address.latitude,
        lon = state.address.longitude);
    
    let phone = state.address.phone.clone().unwrap_or_default();
    let phone_uri = format!("tel:{}", phone.clone());
    let show_phone_button = !phone.is_empty();

    let visit_count = state.address.visits.len();

    let mtk = parameters.mtk.clone().unwrap_or_default();

    html! {
        <>
        <MenuBarV2>
            <div class="nav ms-2 me-auto mb-05 mb-lg-0">
                <li class="nav-item">
                    <a class="nav-link active" aria-current="page" href="/app/address-search">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-search" viewBox="0 0 16 16">
                            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                        </svg>
                        <span class="ms-1">{"Search"}</span>
                    </a>
                </li> 
                <li class="nav-item">
                    <a class="nav-link active" href={new_address_uri.clone()}>
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-house-add" viewBox="0 0 16 16">
                            <path d="M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L2 8.207V13.5A1.5 1.5 0 0 0 3.5 15h4a.5.5 0 1 0 0-1h-4a.5.5 0 0 1-.5-.5V7.207l5-5 6.646 6.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.707 1.5Z"/>
                            <path d="M16 12.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Zm-3.5-2a.5.5 0 0 0-.5.5v1h-1a.5.5 0 0 0 0 1h1v1a.5.5 0 1 0 1 0v-1h1a.5.5 0 1 0 0-1h-1v-1a.5.5 0 0 0-.5-.5Z"/>
                        </svg>
                        <span class="ms-1">{"New"}</span>
                    </a>
                </li> 
            </div>
        </MenuBarV2>
        <div class="container">
            <form class="row g-3">
                <div class="col-12">
                    <strong>{"地址 Address Editor"}</strong><br/>
                    // <span>{state.address.name.clone()}</span><br/>
                    // <span>{state.address.street.clone()}</span><br/>
                    // if !state.address.unit.clone().unwrap_or_default().is_empty() {
                    //     <span>{state.address.unit.clone()}<br/></span>
                    // } 
                    // <span>{state.address.city.clone()}{", "}{state.address.state.clone()}{" "}{state.address.postal_code.clone()}</span>
                </div>
                if state.show_address_marker {
                    <div class="col-12 col-md-6">
                        <label for="input-mark-address" class="form-label"><strong>{"Mark Address"}</strong></label>
                        <div class="input-group">
                            <select 
                                onchange={mark_type_onchange} 
                                id="input-mark-address" 
                                class="form-select shadow-sm" 
                                style="max-width:300px;">
                                <option selected={true} value="">{"Select result"}</option>
                                <option value="nothome">{"Not Home"}</option>
                                <option value="home-cc">{"Home Confirmed Chinese"}</option>
                                <option value="home-nc">{"Home Not Chinese"}</option>
                                <option value="business-office">{"Business Office"}</option>
                                <option value="business-shop">{"Business Shop"}</option>
                                <option value="business-other">{"Business Other"}</option>
                                <option value="inaccessible">{"Inaccessible"}</option>
                                <option value="inaccessible-other">{"Inaccessible Other"}</option>
                                <option value="locked-gate">{"Locked Gate"}</option>
                                <option value="no-trespassing">{"No Trespassing"}</option>
                                <option value="delivery-returned">{"Delivery Returned"}</option>
                                <option value="delivery-sent">{"Delivery Sent"}</option>                                
                            </select>
                            <input value={address_mark_model.mark_date_utc.clone()} 
                                onchange={mark_date_onchange} 
                                type="text" 
                                class="form-control shadow-sm" 
                                id="input-mark-date" 
                                placeholder="Date (optional)" 
                                style="max-width:200px;"/>
                            <button onclick={mark_onclick} class="me-1 btn btn-primary shadow-sm">{"Mark"}</button>
                        </div>
                        <span class="mx-1 badge bg-danger">{state.address_marking_error.clone()}</span>
                    </div>
                }
                <div class="col-12 col-md-6">
                    if state.show_address_marker {
                        <button onclick={show_address_marker_onclick} class="me-1 btn btn-outline-secondary shadow-sm">{"Hide Marker"}</button>
                    } else {
                        <button onclick={show_address_marker_onclick} class="me-1 btn btn-outline-primary shadow-sm">{"Mark"}</button>
                    }
                    if visit_count > 0 {
                        if state.show_visits {
                            <button onclick={visits_onclick} class="me-1 btn btn-outline-primary shadow-sm">{"Hide Visits..."}</button>
                        } else {
                            <button onclick={visits_onclick} class="me-1 btn btn-outline-primary shadow-sm">{visit_count}{" Visits..."}</button>
                        }
                    } else {
                        <button onclick={no_visits_onclick} class="me-1 btn btn-outline-secondary shadow-sm">{"No Visits"}</button>
                    }
                </div>
                if state.show_visits {
                    <div class="col-12">
                        <ul>
                        {
                            state.address.visits.iter().map(|visit| {   
                            let is_new_visit = state.new_visit_id == visit.id;
                            html! {
                                <li>
                                    {visit.date_utc.clone().chars().take(10).collect::<String>()}
                                    {" "}
                                    {visit.result.clone()}
                                    <span class="mx-1 badge bg-success">{if is_new_visit {"New"} else {""}}</span>
                                  
                                </li>
                            }
                            }).collect::<Html>()
                        }
                        </ul>
                    </div>
                }
                <div>
                    <hr/>
                    <span>
                        <strong>
                            if is_new_address {
                                <span>{"地址 New Address"}</span>
                                <span class="badge rounded-pill text-bg-success ms-3" style="background-color:green;">{"NEW"}</span>
                            } else {
                                <span>{" 地址 Edit Address"}</span>
                            }
                        </strong>
                    </span>         
                </div>
                <div class="col-12">
                    // <button onclick={save_onclick} class="me-1 btn btn-primary shadow-sm">{"Save"}</button>
                    // <a onclick={close_onclick.clone()} href="#" class="mx-1 btn btn-secondary shadow-sm">{"Close"}</a>
                    // if address_id != 0 {
                    //     <a class="mx-1 btn btn-outline-primary" href={new_address_uri.clone()}>
                    //         <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-house-add" viewBox="0 0 16 16">
                    //             <path d="M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L2 8.207V13.5A1.5 1.5 0 0 0 3.5 15h4a.5.5 0 1 0 0-1h-4a.5.5 0 0 1-.5-.5V7.207l5-5 6.646 6.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.707 1.5Z"/>
                    //             <path d="M16 12.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Zm-3.5-2a.5.5 0 0 0-.5.5v1h-1a.5.5 0 0 0 0 1h1v1a.5.5 0 1 0 1 0v-1h1a.5.5 0 1 0 0-1h-1v-1a.5.5 0 0 0-.5-.5Z"/>
                    //         </svg>
                    //         <span class="mx-1">{"New"}</span>
                    //     </a>
                    // }
                    if state.save_success { 
                        <span class="mx-1 badge bg-success">{"Saved"}</span> 
                    }
                    if state.save_error { 
                        <span class="mx-1 badge bg-danger">{"Save Error"}</span> 
                        <span class="mx-1" style="color:red;">{state.error_message.clone()}</span>
                    }
                    if state.territory_was_changed { 
                        <span class="mx-1 badge bg-warning">{"Moved"}</span> 
                        <span class="mx-1" style="color:blue;">{format!("Moved to territory {}", state.moved_to_territory_number.clone())}</span> 
                    }         
                </div>
                <div class="col-12">
                    <label for="inputName" class="form-label">{"姓名 Name"}</label>
                    <input value={state.address.name.clone()} onchange={name_onchange} type="text" class="form-control shadow-sm" id="inputName" placeholder="Name"/>
                </div>
                <div class="col-12 col-md-9">
                    <label for="inputAddress" class="form-label">{"地址 Address"}</label>
                    <input value={state.address.street.clone()} onchange={street_onchange} type="text" class="form-control shadow-sm" id="inputAddress" placeholder="1234 Main St"/>
                </div>
                <div class="col-12 col-md-3">
                    <label for="inputUnit" class="form-label">{"单元号 Unit"}</label>
                    <input value={state.address.unit.clone()} onchange={unit_onchange} type="text" class="form-control shadow-sm" id="inputUnit" placeholder="Apartment, studio, or floor"/>
                </div>
                <div class="col-md-6">
                    <label for="inputCity" class="form-label">{"城市 City"}</label>
                    <input value={state.address.city.clone()} onchange={city_onchange} type="text" class="form-control shadow-sm" id="inputCity"/>
                </div>
                <div class="col-md-4">
                    <SelectAddressState 
                        value={state.address.state.clone()}
                        onchange={state_onchange}/>
                </div>
                <div class="col-md-2">
                    <label for="input-postal-code" class="form-label">{"邮政编码 Zip"}</label>
                    <input value={state.address.postal_code.clone()} onchange={postal_code_onchange} type="text" class="form-control shadow-sm" id="input-postal-code"/>
                </div>              
                <div class="col-6 col-sm-5 col-md-4 col-lg-3">
                    <label for="input-phone" class="form-label">{"电话 Phone"}</label>
                    <div class="input-group">
                        <input value={state.address.phone.clone()} onchange={phone_onchange} type="text" class="form-control shadow-sm" id="input-phone" placeholder="000-000-0000"/>
                        if show_phone_button {
                            <a href={phone_uri} class="btn btn-primary">
                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-telephone-fill" viewBox="0 0 16 16">
                                    <path fill-rule="evenodd" d="M1.885.511a1.745 1.745 0 0 1 2.61.163L6.29 2.98c.329.423.445.974.315 1.494l-.547 2.19a.678.678 0 0 0 .178.643l2.457 2.457a.678.678 0 0 0 .644.178l2.189-.547a1.745 1.745 0 0 1 1.494.315l2.306 1.794c.829.645.905 1.87.163 2.611l-1.034 1.034c-.74.74-1.846 1.065-2.877.702a18.634 18.634 0 0 1-7.01-4.42 18.634 18.634 0 0 1-4.42-7.009c-.362-1.03-.037-2.137.703-2.877L1.885.511z"/>
                                </svg>
                            </a>
                        }
                    </div>
                </div>
                <div class="col-12">
                    <label for="input-notes" class="form-label">{"笔记 Notes"}</label>
                    <textarea value={state.address.notes.clone()} onchange={notes_onchange} type="text" rows="2" cols="30" class="form-control shadow-sm" id="input-notes" placeholder="Notes"/>
                </div>
                <div class="col-12 col-sm-6 col-md-4">
                    <label for="input-language" class="form-label">{"Language"}</label>
                    <select onchange={language_onchange} id="input-language" class="form-select shadow-sm">
                        <option value="83">{"Select language"}</option>
                        <EnglishChineseIdOption id={83} english="Chinese" chinese="中文" selected={selected_language_id} />
                        <EnglishChineseIdOption id={5} english="Cantonese" chinese="广东话" selected={selected_language_id} />
                        <EnglishChineseIdOption id={188} english="Fukien" chinese="福建话" selected={selected_language_id} />
                        <EnglishChineseIdOption id={258} english="Fuzhounese" chinese="福州话" selected={selected_language_id} />
                        <EnglishChineseIdOption id={190} english="Hakka" chinese="客家话" selected={selected_language_id} />
                        <EnglishChineseIdOption id={4} english="Mandarin" chinese="普通话" selected={selected_language_id} />
                        <EnglishChineseIdOption id={189} english="Teochew" chinese="潮州话" selected={selected_language_id} />
                        <EnglishChineseIdOption id={73} english="Toisan" chinese="台山话" selected={selected_language_id} />
                        <EnglishChineseIdOption id={259} english="Wenzhounese" chinese="温州话" selected={selected_language_id} />
                    </select>
                </div>
                <div class="col-12 col-sm-6 col-md-4">
                    <label for="input-status" class="form-label">{"Visit Status"}</label>
                    <select onchange={status_onchange} id="input-status" class="form-select shadow-sm">
                        <EnglishChineseIdOption id={1} english="New" chinese="不确定" selected={selected_status_id} />
                        <EnglishChineseIdOption id={2} english="Valid" chinese="确定" selected={selected_status_id} />
                        <EnglishChineseIdOption id={3} english="Do not call" chinese="不要拜访" selected={selected_status_id} />
                        <EnglishChineseIdOption id={4} english="Moved" chinese="搬家" selected={selected_status_id} />
                        <EnglishChineseIdOption id={5} english="Duplicate" chinese="地址重复" selected={selected_status_id} />
                        <EnglishChineseIdOption id={6} english="Not valid" chinese="不说中文" selected={selected_status_id} />
                    </select>
                </div>
                <div class="col-12 col-sm-6 col-md-4">
                    <label for="input-delivery-status" class="form-label">{"Mail Delivery Status"}</label>
                    <AddressDeliveryStatusSelector
                        {mtk}
                        disabled={false}
                        onchange={delivery_status_onchange} 
                        id={state.address.delivery_status_id} />
                </div>
                if show_delivery_status_date {
                    <div class="col-12 col-sm-6 col-md-4">
                        <label for="input-last-delivery-status-date-utc" class="form-label">{"Last Delivery Status Date"}</label>
                        <input value={state.address.last_delivery_status_date_utc.clone()} onchange={last_delivery_status_date_utc_onchange} type="text" class="form-control shadow-sm" id="input-last-delivery-status-date-utc"/>
                    </div>
                }
                <div class="col-6 col-sm-4 col-md-3">
                    <label for="inputTerritoryNumber" class="form-label">{"Territory Number"}</label>
                    <div class="input-group">
                        <input value={territory_number} onchange={territory_number_onchange} type="text" class="form-control shadow-sm" id="inputTerritoryNumber" placeholder="Territory Number"/>
                        <button onclick={territory_open_onclick} class="btn btn-outline-primary">
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16">
                                <path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/>
                            </svg>
                        </button>
                    </div>
                    if state.territory_was_changed { 
                        <span class="mx-1 badge bg-warning">{"Moved"}</span> 
                        <span class="mx-1" style="color:blue;">{format!("Moved to territory {}", state.moved_to_territory_number.clone())}</span> 
                    }         
                </div>
                // <div class="col-6 col-sm-4 col-md-4">
                //     <label for="input-latitude" class="form-label">{"纬度 Latitude"}</label>
                //     <input readonly={true} value={state.address.latitude.to_string()} onchange={latitude_onchange.clone()} type="text" class="form-control shadow-sm" id="input-latitude" placeholder="纬度 Latitude"/>
                // </div>                
                // <div class="col-6 col-sm-4 col-md-4">
                //     <label for="input-longitude" class="form-label">{"经度 Longitude"}</label>
                //     <input readonly={true} value={state.address.longitude.to_string()} onchange={longitude_onchange.clone()} type="text" class="form-control shadow-sm" id="input-longitude" placeholder="经度 Longitude"/>
                // </div>
                
                <div class="col-12 col-sm-8 col-md-6 col-lg-5">
                    <label for="input-longitude" class="form-label">{"纬度,经度 Latitude,Longitude"}</label>
                    <div class="input-group">
                        <a href={map_uri} class="btn btn-outline-primary">
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-geo-alt" viewBox="0 0 16 16">
                                <path d="M12.166 8.94c-.524 1.062-1.234 2.12-1.96 3.07A31.493 31.493 0 0 1 8 14.58a31.481 31.481 0 0 1-2.206-2.57c-.726-.95-1.436-2.008-1.96-3.07C3.304 7.867 3 6.862 3 6a5 5 0 0 1 10 0c0 .862-.305 1.867-.834 2.94zM8 16s6-5.686 6-10A6 6 0 0 0 2 6c0 4.314 6 10 6 10z"/>
                                <path d="M8 8a2 2 0 1 1 0-4 2 2 0 0 1 0 4zm0 1a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>
                            </svg>
                        </a>
                        <input value={state.address.latitude.to_string()} onchange={latitude_onchange} type="text" class="form-control shadow-sm" id="input-latitude" placeholder="纬度 Latitude"/>
                        <input value={state.address.longitude.to_string()} onchange={longitude_onchange} type="text" class="form-control shadow-sm" id="input-longitude" placeholder="经度 Longitude"/>
                        <button onclick={geocode_click} class="btn btn-primary">{"Geocode"}</button>
                    </div>
                    //if geocoding_success { 
                        <span class="mx-1 badge bg-success">{state.geocoding_success.clone()}</span> 
                    //}
                    //if geocoding_error { 
                        <span class="mx-1 badge bg-danger">{state.geocoding_error.clone()}</span> 
                    //}      
                </div>                
                if show_alba_address_id {
                    <div class="col-12 col-sm-6 col-md-4">
                        <label for="inputAlbaAddressId" class="form-label">{"Alba Address Id"}</label>
                        <input value={format!("{}",state.address.alba_address_id)} /*onchange={alba_address_id_onchange}*/ type="text" class="form-control shadow-sm" id="inputAlbaAddressId" placeholder="Alba Address Id" readonly={true} />
                    </div>
                }               
                <div class="col-12">
                    <button type="submit" class="me-1 btn btn-primary shadow-sm">{"Save"}</button>
                    <a onclick={close_onclick} href="#" class="mx-1 btn btn-secondary shadow-sm">{"Close"}</a>
                    if address_id != 0 {
                        <a class="mx-1 btn btn-outline-primary" href={new_address_uri.clone()}>
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-house-add" viewBox="0 0 16 16">
                                <path d="M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L2 8.207V13.5A1.5 1.5 0 0 0 3.5 15h4a.5.5 0 1 0 0-1h-4a.5.5 0 0 1-.5-.5V7.207l5-5 6.646 6.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.707 1.5Z"/>
                                <path d="M16 12.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Zm-3.5-2a.5.5 0 0 0-.5.5v1h-1a.5.5 0 0 0 0 1h1v1a.5.5 0 1 0 1 0v-1h1a.5.5 0 1 0 0-1h-1v-1a.5.5 0 0 0-.5-.5Z"/>
                            </svg>
                            <span class="mx-1">{"New"}</span>
                        </a>
                    }                    
                    if state.save_success { 
                        <span class="mx-1 badge bg-success">{"Saved"}</span> 
                    }
                    if state.save_error { 
                        <span class="mx-1 badge bg-danger">{"Save Error"}</span> 
                        <span class="mx-1" style="color:red;">{state.error_message.clone()}</span>
                    }
                </div>               
                <div class="col-12">
                    <span><small>{"AAID: "}{state.address.alba_address_id}{" AID: "}{state.address.address_id}</small></span>
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


#[derive(Default)]
pub struct GeocodingResultMessages {
    pub success: String,
    pub error: String,
    pub latitude: f32,
    pub longitude: f32,
}

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize, Debug)]
#[serde(rename_all = "camelCase")]
pub struct AddressSaveResult {
     pub previous_territory_id: Option<i32>,
     pub previous_territory_number: Option<String>,
     pub territory_was_changed: bool,
     pub territory_id: Option<i32>,
     pub territory_number: Option<String>,
     pub address: Option<Address>,
}

// impl Default for GeocodingResultMessages {
//     fn default() -> GeocodingResultMessages {
//         GeocodingResultMessages {
//             success: "".to_string(),
//             error: "".to_string(),
//             latitude: 0.0,
//             longitude: 0.0,
//         }
//     }
// }

#[function_component]
pub fn EnglishChineseIdOption(props: &EnglishChineseIdOptionProps) -> Html {
    html! {
        <option value={props.id.to_string()} selected={props.id == props.selected}>
            {props.chinese.clone()}{" "}{props.english.clone()}
        </option>
    }
}

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize, Debug)]
#[serde(rename_all = "camelCase")]
pub struct MarkAddressResult {
    pub id: i32,
    pub new_visit_id: i32,
    pub message: String,
    pub address: Option<Address>,
}