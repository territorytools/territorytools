use serde::Serialize;
use std::ops::Deref;
use yew::prelude::*;
use stdweb::js;

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct ButtonWithConfirmModel {
    pub is_confirming: bool,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct ButtonWithConfirmProps {
    pub id: String,
    pub button_text: String,
    pub on_confirm: Callback<i32>,
    #[prop_or_default]
    pub class: Option<String>,
}

#[function_component]
pub fn ButtonWithConfirm(props: &ButtonWithConfirmProps) -> Html {
    let state: yew::UseStateHandle<ButtonWithConfirmModel> = use_state(|| ButtonWithConfirmModel::default());

    let cloned_state = state.clone();
    let action_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        //let cloned_state = cloned_state.clone();
        let mut modification = cloned_state.deref().clone();
        modification.is_confirming = true;
        cloned_state.set(modification);
    });

    let cloned_state = state.clone();
    let cancel_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        //let cloned_state = cloned_state.clone();
        let mut modification = cloned_state.deref().clone();
        modification.is_confirming = false;
        cloned_state.set(modification);
    });
    
    let cloned_state = state.clone();
    let props_clone = props.clone();
    let confirm_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        
        props_clone.on_confirm.emit(0);

        let mut modification = cloned_state.deref().clone();
        modification.is_confirming = false;
        cloned_state.set(modification);
    });

    let confirm_button_id = format!("{}-confirm", props.id.clone());
    let cancel_button_id = format!("{}-cancel", props.id.clone());
    let button_class = if props.class.clone().is_none() {
        "me-1 btn btn-primary shadow-sm".to_string()
    } else {
        props.class.clone().unwrap_or_default()
    };

    html! {
    <>
       if state.is_confirming {
            <span class="px-3 pt-2">{"Confirm:"}</span>
            <button 
                id={confirm_button_id}
                //onclick={props.on_confirm.clone()} 
                onclick={confirm_onclick}
                class="me-1 btn btn-success shadow-sm">
                {"Yes"}
            </button>
            <button 
                id={cancel_button_id}
                onclick={cancel_onclick} 
                class="me-1 btn btn-outline-secondary shadow-sm">
                {"Cancel"}
            </button>
        } else {
            <button 
                id={props.id.clone()}
                onclick={action_onclick} 
                class={button_class}>
                // TODO: Put a call back here that sets the is_confirming back to false, and then calls the callback
                {props.button_text.clone()}
            </button>
        }
       </>
    }
}


#[function_component]
pub fn SomeJsThing() -> Html {
    let something = "Hello".to_string();
    js! {
        var why = "hello";
    }
}