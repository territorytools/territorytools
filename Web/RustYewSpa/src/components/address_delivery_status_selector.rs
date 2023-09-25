use crate::models::addresses::AddressDeliveryStatus;
use wasm_bindgen::JsCast;
use reqwasm::http::Request;
use yew::prelude::*;
use web_sys::HtmlSelectElement;

// Uncomment for debugging without an API server
//const STATUSES_API_PATH: &str = "/data/address_delivery_statuses.json";
const STATUSES_API_PATH: &str = "/api/addresses/delivery-statuses";

#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    pub id: i32,
    pub onchange: Callback<String>,
    pub disabled: bool,
    pub mtk: Option<String>,
}

#[function_component(AddressDeliveryStatusSelector)]
pub fn address_delivery_status_selector(props: &Props) -> Html {
    let statuses = use_state(|| vec![]);
    {
        let statuses = statuses.clone();
        let mtk = props.mtk.clone().unwrap_or_default();
        use_effect_with_deps(move |_| {
            let statuses = statuses.clone();
            let mtk = mtk.clone();
            wasm_bindgen_futures::spawn_local(async move {
                let uri: String = format!(
                    "{base_path}?mtk={mtk}", 
                    base_path = STATUSES_API_PATH);

                let fetched_statuses: Vec<AddressDeliveryStatus> = Request::get(&uri)
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
        <select disabled={props.disabled} id={"deliver-status-menu"} name={"delivery-status-id"} class={"form-select shadow-sm"} {onchange}>
            <option value={"0"}>{"For letter writing only"}</option>
            {      
                statuses.iter().map(|status| {   
                    let status_clone = status.clone();
                    let status_description = status_clone.description.unwrap_or_default();
                    let status_text = format!("{name}{description}",
                        name = status_clone.name.unwrap_or_default(),
                        description = 
                            if status_description.is_empty()
                            { 
                                "".to_string() 
                            } else {
                                format!(" ({})", status_description)
                            }
                    );

                    let status_clone = status.clone(); 
                    html!{
                        <option value={status_clone.id.to_string()} selected={status_clone.id == props.id}>
                            {status_text}
                        </option>
                    }
                }).collect::<Html>()
            }
        </select>
    }
}