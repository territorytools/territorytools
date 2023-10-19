use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct EnglishChineseIdOptionProps {
    pub id: i32,
    pub english: String,
    pub chinese: String,
    pub selected: i32,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct EnglishChineseValueOptionProps {
    pub value: String,
    pub english: String,
    pub chinese: String,
    pub selected: String,
}

#[function_component]
pub fn EnglishChineseIdOption(props: &EnglishChineseIdOptionProps) -> Html {
    html! {
        <option value={props.id.to_string()} selected={props.id == props.selected}>
            {props.chinese.clone()}{" "}{props.english.clone()}
        </option>
    }
}

#[function_component]
pub fn EnglishChineseValueOption(props: &EnglishChineseValueOptionProps) -> Html {
    html! {
        <option value={props.value.to_string()} selected={props.value.clone() == props.selected.clone()}>
            {props.chinese.clone()}{" "}{props.english.clone()}
        </option>
    }
}
