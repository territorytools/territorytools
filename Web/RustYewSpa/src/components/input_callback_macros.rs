use yew::Callback;
use yew::prelude::*;

#[macro_export]
macro_rules! callback_value {
    // This awesome stack overflow comment made it work: https://stackoverflow.com/questions/65451484/passing-nested-struct-field-path-as-macro-parameter/65451718#65451718
    ($cloner:ident, $($field_path:ident).+) => (
        {
            let state = $cloner.clone();
            Callback::from(move |event: Event| {
                let mut modification = state.deref().clone();
                let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

                //log!(format!("callback_value!!!: {}", value.clone()));
                modification.$($field_path).+ = Some(value.to_string());
                state.set(modification);
            })
        }
    )
}

#[macro_export]
macro_rules! callback_string {
    // This awesome stack overflow comment made it work: https://stackoverflow.com/questions/65451484/passing-nested-struct-field-path-as-macro-parameter/65451718#65451718
    ($cloner:ident, $($field_path:ident).*) => (
        {
            let state = $cloner.clone();
            Callback::from(move |event: Event| {
                let mut modification = state.deref().clone();
                let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

                //log!(format!("callback_value!!!: {}", value.clone()));
                modification.$($field_path).* = value.to_string();
                state.set(modification);
            })
        }
    )
}

#[macro_export]
macro_rules! callback_checked {
    // This awesome stack overflow comment made it work: https://stackoverflow.com/questions/65451484/passing-nested-struct-field-path-as-macro-parameter/65451718#65451718
    ($cloner:ident, $($field_path:ident).+) => (
        {
            let state = $cloner.clone();
            Callback::from(move |event: Event| {
                let mut modification = state.deref().clone();
                let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .checked();

                //log!(format!("checked!!!: {}", value.clone()));
                modification.$($field_path).+ = value;
                state.set(modification);
            })
        }
    )
}

#[macro_export]
macro_rules! grid_input_box {
    // This awesome stack overflow comment made it work: https://stackoverflow.com/questions/65451484/passing-nested-struct-field-path-as-macro-parameter/65451718#65451718
    ($cloner:ident.$($field_path:ident).+) => (
        {
            let state = $cloner.clone();
            Callback::from(move |event: Event| {
                let mut modification = state.deref().clone();
                let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

                //log!(format!("checked!!!: {}", value.clone()));
                modification.$($field_path).+ = Some(value);
                state.set(modification);
            })
        }
    )
}

#[derive(Properties, PartialEq, Clone)]
pub struct GridInput {
    pub callback: Callback<Event>,
    pub value: Option<String>,
}

#[macro_export]
macro_rules! field {
    // This awesome stack overflow comment made it work: https://stackoverflow.com/questions/65451484/passing-nested-struct-field-path-as-macro-parameter/65451718#65451718
    ($cloner:ident.$($field_path:ident).+) => (
        {
            let state = $cloner.clone();
            let callback = Callback::from(move |event: Event| {
                let mut modification = state.deref().clone();
                let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

                //log!(format!("checked!!!: {}", value.clone()));
                modification.$($field_path).+ = Some(value);
                state.set(modification);
            });
            let value = $cloner.$($field_path).+.clone();
            GridInput {
                callback,
                value,
            }
        }
    )
}


#[derive(Properties, PartialEq, Clone)]
pub struct CheckboxCellInput {
    pub callback: Callback<Event>,
    pub checked: bool
}

#[macro_export]
macro_rules! field_checked {
    ($cloner:ident.$($field_path:ident).+) => (
        {
            let state = $cloner.clone();
            let callback = Callback::from(move |event: Event| {
                let mut modification = state.deref().clone();
                let checked = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .checked();

                modification.$($field_path).+ = checked;
                state.set(modification);
            });
            let state = $cloner.clone();
            let checked = state.$($field_path).+;
            $crate::components::input_callback_macros::CheckboxCellInput {
                callback,
                checked,
            }
        }
    )
}