use crate::models::addresses::{AddressDeliveryStatus};
use wasm_bindgen::JsCast;
use reqwasm::http::{Request};
use yew::prelude::*;
use web_sys::HtmlSelectElement;

#[cfg(debug_assertions)]
const STATUSES_API_PATH: &str = "/data/address_delivery_statuses.json";

#[cfg(not(debug_assertions))]
const STATUSES_API_PATH: &str = "/api/addresses/delivery-statuses";


#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    pub id: i32,
    pub onchange: Callback<String>,
}

#[function_component(AddressDeliveryStatusSelector)]
pub fn address_delivery_status_selector(props: &Props) -> Html {
    let statuses = use_state(|| vec![]);
    {
        let statuses = statuses.clone();
        use_effect_with_deps(move |_| {
            let statuses = statuses.clone();
            wasm_bindgen_futures::spawn_local(async move {
                let uri: &str = STATUSES_API_PATH;

                let fetched_statuses: Vec<AddressDeliveryStatus> = Request::get(uri)
                    .send()
                    .await
                    .unwrap()
                    .json()
                    .await
                    .unwrap();

                statuses.set(fetched_statuses);
            });
            || ()
        }, ());
    }

    let onchange = {
        let props_onchange = props.onchange.clone();
        Callback::from(move |event: Event| {
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlSelectElement>()
                .value();

            props_onchange.emit(value);
        })
    };

    html! {
        <select id={"deliver-status-menu"} name={"delivery-status-id"} class={"form-select shadow-sm"} {onchange}>
            <option value={"0"}>{"Select Delivery Status"}</option>
            {      
                statuses.iter().map(|status| {   
                    let status = status.clone();     
                    html!{
                        <option value={status.id.to_string()} selected={status.id == props.id}>
                            <span>{status.name.unwrap_or_default().to_string()}</span>
                        </option>
                    }
                }).collect::<Html>()
            }
        </select>
    }
}