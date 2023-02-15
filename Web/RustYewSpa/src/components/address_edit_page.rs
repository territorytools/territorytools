use crate::components::menu_bar::MenuBar;
use crate::components::state_selector::SelectAddressState;
use crate::models::addresses::Address;
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
//use serde_json::ser::to_string;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/put_address.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses/save";

#[cfg(debug_assertions)]
const GET_ADDRESSES_API_PATH: &str = "/data/get_address.json?id=";

#[cfg(not(debug_assertions))]
const GET_ADDRESSES_API_PATH: &str = "/api/addresses/addresses/alba-address-id";

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "PUT";

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct AddressEditModel {
    pub address: Address,
    // TODO: OriginalAddress (Used to compare by server)
    pub alba_address_id: i32,
    pub territory_number: Option<String>,
    pub name: Option<String>,
    pub street: Option<String>,
    pub city: Option<String>,
    pub postal_code: Option<String>,
}


// #[derive(Properties, PartialEq, Clone, Default, Deserialize, Serialize)]
// pub struct AddressResponse {
//     pub addresses: Vec<Address>,
// }

// #[derive(Properties, PartialEq, Clone, Default)]
// pub struct AddressEditProps {
//     pub alba_address_id: i32,
// }

#[derive(Clone, Debug, PartialEq, Deserialize)]
pub struct AddressEditParameters {
    pub alba_address_id: Option<i32>,
}

#[function_component(AddressEditPage)]
pub fn address_edit_page() -> Html {
    let state = use_state(|| AddressEditModel::default());
    let cloned_state = state.clone();
    let location = use_location().expect("Should be a location to get query string");
    let parameters: AddressEditParameters = location.query().expect("An object");
    let alba_address_id: i32 = match parameters.alba_address_id {
        Some(v) => v,
        _ => 0,
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

    //let territories = use_state(|| vec![]);    
    let cloned_state = state.clone();
    use_effect_with_deps(move |_| {
        let cloned_state = cloned_state.clone();
        wasm_bindgen_futures::spawn_local(async move {
            let alba_address_id: i32 = alba_address_id;
            let uri: String = format!(
                "{base_path}/{alba_address_id}", 
                base_path = GET_ADDRESSES_API_PATH);

            let fetched_address: Address = Request::get(uri.as_str())
                .send()
                .await
                .expect("Address JSON from API")
                .json()
                .await
                .expect("Valid address JSON from API");

            log!(format!(
                "Fetched address 1, street: {street:?}",
                street = fetched_address.street
            ));

            let model: AddressEditModel = AddressEditModel {
                address: fetched_address,
                alba_address_id: alba_address_id,
                city: None,
                name: None,
                postal_code: None,
                street: None,
                territory_number: None,
            };

            log!(format!(
                "Fetched address 2, street: {street:?}",
                street = model.address.street
            ));

            cloned_state.set(model);
        });
        || ()
    }, ());
    

    // let onsubmit = Callback::from(move |event: SubmitEvent| {
    //     event.prevent_default();
    // });
    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |_event: SubmitEvent| {   //model: AddressEditModel| { //
        let cloned_state = cloned_state.clone();
        //let navigator = navigator.clone();
        spawn_local(async move {
            log!("Spawing request for address change...");
            let model = cloned_state.clone();
            let uri_string: String = format!("{path}", 
                path = DATA_API_PATH);

            let uri: &str = uri_string.as_str();
            
            let _method: Method = match ASSIGN_METHOD {
                "PUT" => Method::PUT,
                "GET" => Method::PUT,
                &_ =>  Method::GET,
            };

            // let name = match &model.name {
            //     Some(v) => v.to_string(),
            //     _ => "".to_string(),
            // };

            // let street = match &model.street {
            //     Some(v) => v.to_string(),
            //     _ => "".to_string(),
            // };

            let body_model = &model.deref();

            let data_serialized = serde_json::to_string_pretty(&body_model.address)
                .expect("Should be able to serialize address edit form into JSON");

            // TODO: FetchService::fetch accepts two parameters: a Request object and a Callback.
            // https://yew.rs/docs/0.18.0/concepts/services/fetch
            let _resp = Request::new(uri)
                .method(Method::PUT)
                .header("Content-Type", "application/json")
                .body(data_serialized)
                .send()
                .await
                .expect("A result from the endpoint");

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
            // if resp.status() == 200 {
            //     navigator.push(&Route::Map);
            // }
        });
    });

    // let selected_language: String = state.address.language.clone().unwrap_or_default();
    // let selected_status: String = state.address.status.clone().unwrap_or_default();

    let selected_language_id: i32 = state.address.language_id;
    let selected_status_id: i32 = state.address.status_id;

    html! {
        <>
        <MenuBar/>
        <div class="container">
            <span><strong>{"Edit 地址 Address"}</strong></span>
            <a class="mx-3 btn btn-outline-primary" href="/app/address-search">{"Search"}</a>
            <hr/>
            <form {onsubmit} class="row g-3">
                <div class="col-12 col-sm-6 col-md-4">
                    <label for="input-language" class="form-label">{"Language"}</label>
                    <select onchange={language_onchange} id="input-language" class="form-select">
                        <option value="0">{"Select language"}</option>
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
                    <label for="input-status" class="form-label">{"Status"}</label>
                    <select onchange={status_onchange} id="input-status" class="form-select">
                        <EnglishChineseIdOption id={1} english="New" chinese="不确定" selected={selected_status_id} />
                        <EnglishChineseIdOption id={2} english="Valid" chinese="确定" selected={selected_status_id} />
                        <EnglishChineseIdOption id={3} english="Do not call" chinese="不要拜访" selected={selected_status_id} />
                        <EnglishChineseIdOption id={4} english="Moved" chinese="搬家" selected={selected_status_id} />
                        <EnglishChineseIdOption id={5} english="Duplicate" chinese="地址重复" selected={selected_status_id} />
                        <EnglishChineseIdOption id={6} english="Not valid" chinese="不说中文" selected={selected_status_id} />
                    </select>
                </div>
                <div class="col-12">
                    <label for="inputName" class="form-label">{"姓名 Name"}</label>
                    <input value={state.address.name.clone()} onchange={name_onchange} type="text" class="form-control" id="inputName" placeholder="Name"/>
                </div>
                <div class="col-12 col-md-9">
                    <label for="inputAddress" class="form-label">{"地址 Address"}</label>
                    <input value={state.address.street.clone()} onchange={street_onchange} type="text" class="form-control" id="inputAddress" placeholder="1234 Main St"/>
                </div>
                <div class="col-12 col-md-3">
                    <label for="inputUnit" class="form-label">{"单元号 Unit"}</label>
                    <input value={state.address.unit.clone()} onchange={unit_onchange} type="text" class="form-control" id="inputUnit" placeholder="Apartment, studio, or floor"/>
                </div>
                <div class="col-md-6">
                    <label for="inputCity" class="form-label">{"城市 City"}</label>
                    <input value={state.address.city.clone()} onchange={city_onchange} type="text" class="form-control" id="inputCity"/>
                </div>
                <div class="col-md-4">
                    <SelectAddressState onchange={state_onchange}/>
                </div>
                <div class="col-md-2">
                    <label for="input-postal-code" class="form-label">{"邮政编码 Zip"}</label>
                    <input value={state.address.postal_code.clone()} onchange={postal_code_onchange} type="text" class="form-control" id="input-postal-code"/>
                </div>
                <div class="col-6 col-sm-4 col-md-4">
                    <label for="input-latitude" class="form-label">{"纬度 Latitude"}</label>
                    <input value={state.address.latitude.to_string()} onchange={latitude_onchange} type="text" class="form-control" id="input-latitude" placeholder="纬度 Latitude"/>
                </div>
                <div class="col-6 col-sm-4 col-md-4">
                    <label for="input-longitude" class="form-label">{"经度 Longitude"}</label>
                    <input value={state.address.longitude.to_string()} onchange={longitude_onchange} type="text" class="form-control" id="input-longitude" placeholder="经度 Longitude"/>
                </div>
                <div class="col-12">
                    <label for="input-phone" class="form-label">{"电话 Phone"}</label>
                    <input value={state.address.phone.clone()} onchange={phone_onchange} type="text" class="form-control" id="input-phone" placeholder="000-000-0000"/>
                </div>
                <div class="col-12">
                    <label for="input-notes" class="form-label">{"笔记 Notes"}</label>
                    <textarea value={state.address.notes.clone()} onchange={notes_onchange} type="text" rows="2" cols="30" class="form-control" id="input-notes" placeholder="Notes"/>
                </div>
                // <div class="col-12">
                //     <div class="form-check">
                //     <input class="form-check-input" type="checkbox" id="gridCheck"/>
                //     <label class="form-check-label" for="gridCheck">
                //         {"Check me out"}
                //     </label>
                //     </div>
                // </div>
                <div class="col-12">
                    <button type="submit" class="me-1 btn btn-primary">{"Save"}</button>
                    <button class="me-1 btn btn-secondary">{"Close"}</button>
                </div>
                <div class="col-12">
                    <span><small>{"AAID: "}{state.address.alba_address_id}</small></span>
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