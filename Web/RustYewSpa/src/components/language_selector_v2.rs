use crate::models::users::User;
use js_sys::Boolean;
use reqwasm::http::Request;
use serde::{Deserialize, Serialize};
use wasm_bindgen::JsCast;
use web_sys::HtmlSelectElement;
use yew::prelude::*;

#[derive(Properties, Default, Clone, PartialEq)]
pub struct Props {
    pub id: String,
    pub onchange: Callback<String>,
    pub value: i32,
}

#[function_component(LanguageSelectorV2)]
pub fn language_selector_v2(props: &Props) -> Html {
    
    let languages = use_state(|| vec![]);
    {
        let languages = languages.clone();
        use_effect_with((), move |_| {
            let languages = languages.clone();
            wasm_bindgen_futures::spawn_local(async move {
                let uri: &str = "/api/languages";

                let fetched: Vec<Language> = Request::get(uri)
                    .send()
                    .await
                    .unwrap()
                    .json()
                    .await
                    .unwrap();
                languages.set(fetched);
            });
            || ()
        });
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
        <select id={props.id.clone()} name="languageId" class={"form-select shadow-sm"} {onchange}>
            <option value={"0"}>{"Select Language"}</option>
            {                
                languages.iter().map(|language| {
                    let language_id = format!("{}", language.language_id);

                    html!{
                        <option value={language_id.clone()}>{language.name.clone()}</option>
                    }
                }).collect::<Html>()
            }
        </select>
    }
}


#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct Language {
    #[serde(default)]
    pub language_id: i32,
    pub code: Option<String>,
    pub name: String,
    #[serde(default)]
    pub alba_language_id: Option<i32>,
    pub alba_langauge_name: Option<String>,
    #[serde(default)]
    pub is_active: bool,
}