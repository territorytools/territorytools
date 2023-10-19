use crate::models::users::User;
use crate::components::selector_option_bilingual::EnglishChineseValueOption;
use reqwasm::http::Request;
use wasm_bindgen::JsCast;
use web_sys::HtmlSelectElement;
use yew::prelude::*;

#[derive(Properties, Default, Clone, PartialEq)]
pub struct Props {
    pub id: String,
    pub onchange: Callback<String>,
    pub value: String,
}


#[function_component(AddressStatusSelector)]
pub fn address_status_selector(props: &Props) -> Html {

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

    let selected = props.value.clone();

    html! {
        <select 
            onchange={onchange} 
            id={props.id.clone()}
            class="form-select shadow-sm" 
        >
            <EnglishChineseValueOption value="" english="Select new status" chinese="" selected={selected.clone()} />
            <EnglishChineseValueOption value="nothome" english="Not Home" chinese="不在家" selected={selected.clone()} />
            <EnglishChineseValueOption value="home-cc" english="Home Confirmed Chinese" chinese="在家（说中文）" selected={selected.clone()} />
            <EnglishChineseValueOption value="home-nc" english="Home Not Chinese" chinese="在家（不说中文）" selected={selected.clone()} />           
            <EnglishChineseValueOption value="business-office" english="Business Office" chinese="办公室" selected={selected.clone()} />
            <EnglishChineseValueOption value="business-shop" english="Business Shop" chinese="商家" selected={selected.clone()} />
            <EnglishChineseValueOption value="business-other" english="Business Other" chinese="其他行业" selected={selected.clone()} />
            
            <EnglishChineseValueOption value="inaccessible" english="Inaccessible" chinese="" selected={selected.clone()} />
            <EnglishChineseValueOption value="inaccessible-other" english="Inaccessible Other" chinese="" selected={selected.clone()} />
            <EnglishChineseValueOption value="locked-gate" english="Locked Gate" chinese="" selected={selected.clone()} />
            <EnglishChineseValueOption value="no-trespassing" english="No Trespassing" chinese="" selected={selected.clone()} />
            <EnglishChineseValueOption value="delivery-returned" english="Delivery Returned" chinese="" selected={selected.clone()} />
            <EnglishChineseValueOption value="delivery-sent" english="Delivery Sent" chinese="" selected={selected.clone()} />

            // TODO: If this is selected, select another language
            // <option value="duplicate">{"Duplicate"}</option>                                
            // <option value="moved">{"Moved"}</option>                                
            // <option value="do-not-call" chinese="不要拜访">{"Do Not Call"}</option>  
                         
        </select>
    }
}