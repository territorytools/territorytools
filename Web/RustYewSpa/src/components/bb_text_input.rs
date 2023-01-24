use std::ops::Deref;
use stylist::{yew::styled_component};
use gloo::console::log;
use web_sys::HtmlInputElement;
use wasm_bindgen::JsCast;
use yew::prelude::*;

#[derive(PartialEq, Clone)]
pub enum InputType {
    Text,
}

impl ToString for InputType {
    fn to_string(&self) -> String {
        match self {
            InputType::Text => "text".to_owned(),
        }
    }
}

#[derive(Properties, PartialEq, Clone)]
pub struct Props {
    pub data_test: String,
    pub label: String,
    pub placeholder: Option<String>,
    pub class: Option<String>,
    pub input_type: InputType,
    pub onchange: Callback<String>,
    pub value: Option<String>,
}

#[styled_component(BBTextInput)]
pub fn bb_text_input(props: &Props) -> Html {
    // let stylesheet = Style::new(css!(
    //     r#"
    //     margin-top:5px;
    // "#
    // ))
    // .unwrap();
    let placeholder = props.placeholder.clone().unwrap_or_default();
    let id = props.label.to_lowercase().replace(' ', "-");
    //let class = props.class.clone().unwrap_or_default();
    let state = use_state(String::new);
    let initial_load = use_state(|| false);
    let onchange = {
        let emit_onchange = props.onchange.clone();
        let state = state.clone();
        Callback::from(move |event: Event| {
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();
            emit_onchange.emit(value.clone());
            state.set(value);
        })
    };

    {
        let state = state.clone();
        let props_value = props.value.clone();
        let initial_load = initial_load;
        log!(*initial_load);
        use_effect(move || {
            if let Some(props_value) = props_value {
                if !*initial_load && !props_value.is_empty() {
                    state.set(props_value);
                    initial_load.set(true);
                }
            }
            || {}
        })
    }

    html! {
        <>
          <label 
            for={id.clone()}>
            {&props.label}
          </label>
          <input
            type={props.input_type.to_string()}
            id={id}
            class={"form-control"} 
            style={"margin-bottom:15px;background-color:white;"}
            {placeholder}
            data-test={props.data_test.clone()}
            {onchange}
            value={state.deref().clone()}
            readonly=true
          />
        </>
    }
}