use gloo_console::log;
use yew::function_component;
use yew::prelude::*;

use super::input_callback_macros::CheckboxCellInput;
use super::input_callback_macros::FieldStringInput;
use super::input_callback_macros::GridInput;

#[derive(Properties, PartialEq, Clone)]
pub struct Props {
    pub id: String,
    pub class: String,
    pub value: Option<String>,
    pub text: String,
    pub onchange: Callback<Event>,
}

#[function_component(TextBox)]
pub fn text_box(props: &Props) -> Html {
    log!(format!("text_box::props::value::{}", props.value.clone().unwrap_or_default()));
    html!{
        <div class={props.class.clone()}>
            <label class="form-label">{props.text.clone()}</label>
            <input 
                id={props.id.clone()}
                value={props.value.clone()} 
                type="text"
                onchange={props.onchange.clone()}
                class="form-control shadow-sm" />                       
        </div>  
    }
}

#[derive(Properties, PartialEq, Clone)]
pub struct InputCellProps {
    #[prop_or_default]
    pub id: Option<String>,
    #[prop_or_default]
    pub class: Option<String>,
    pub label: String,
    pub field: GridInput,
}

#[function_component(InputCell)]
pub fn grid_input_box(props: &InputCellProps) -> Html {
    let id = props.id.clone();
    let class = if props.class.is_some() {
        props.class.clone()
    } else {
        Some("col-12 col-sm-6 col-md-4".to_string())
    };

    html!{
        <div class={class.clone()}>
            <label class="form-label">{props.label.clone()}</label>
            <input 
                {id}
                value={props.field.value.clone()} 
                type="text"
                onchange={props.field.callback.clone()}
                class="form-control shadow-sm" />                       
        </div>  
    }
}

#[derive(Properties, PartialEq, Clone)]
pub struct StringCellProps {
    #[prop_or_default]
    pub id: Option<String>,
    #[prop_or_default]
    pub class: Option<String>,
    pub label: String,
    pub field: FieldStringInput,
}

#[function_component(StringCell)]
pub fn grid_input_box(props: &StringCellProps) -> Html {
    let id = props.id.clone();
    let class = if props.class.is_some() {
        props.class.clone()
    } else {
        Some("col-12 col-sm-6 col-md-4".to_string())
    };

    html!{
        <div class={class.clone()}>
            <label class="form-label">{props.label.clone()}</label>
            <input 
                {id}
                value={props.field.value.clone()} 
                type="text"
                onchange={props.field.callback.clone()}
                class="form-control shadow-sm" />                       
        </div>  
    }
}

#[derive(Properties, PartialEq, Clone)]
pub struct TextAreaCellProps {
    #[prop_or_default]
    pub id: Option<String>,
    #[prop_or_default]
    pub class: Option<String>,
    pub label: String,
    pub field: GridInput,
    #[prop_or_default]
    pub rows: Option<i32>,
}

#[function_component(TextAreaCell)]
pub fn text_area_cell(props: &TextAreaCellProps) -> Html {
    let id = props.id.clone();
    let class = if props.class.is_some() {
        props.class.clone()
    } else {
        Some("col-12".to_string())
    };

    let rows = format!("{}", props.rows.unwrap_or(3));
    let class = format!("{:?} mt-2", class);

    html!{
        <div {class}>
            <label class="form-label">{"Notes"}</label>
            <textarea 
                {id}
                value={props.field.value.clone()} 
                {rows}
                onchange={props.field.callback.clone()}
                class="form-control shadow-sm" />                         
        </div>
    }
}

#[derive(Properties, PartialEq, Clone)]
pub struct CheckboxCellProps {
    #[prop_or_default]
    pub id: Option<String>,
    #[prop_or_default]
    pub class: Option<String>,
    pub label: String,
    pub field: CheckboxCellInput,
}

#[function_component(CheckboxCell)]
pub fn checkbox_cell(props: &CheckboxCellProps) -> Html {
    let id = props.id.clone();
    let class = if props.class.is_some() {
        props.class.clone()
    } else {
        Some("col-auto".to_string())
    };

    let class = format!("{} mt-3 mb-2", class.unwrap_or_default());

    html!{
        <div {class}>
            <input 
                {id}
                checked={props.field.checked} 
                type="checkbox"
                onchange={props.field.callback.clone()}
                class="form-check-input shadow-sm mx-1" />
            <label class="form-check-label mx-1">{props.label.clone()}</label>                     
        </div>  
    }
}

