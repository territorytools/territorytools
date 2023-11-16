use serde::Serialize;
use yew::prelude::*;
use std::ops::Deref;


#[derive(Properties, Default, Clone, PartialEq)]
pub struct Props {
    #[prop_or_default]
    pub show_section_default: bool,
    pub text: String,
    #[prop_or_default]
    pub children: Html,
}

#[derive(Properties, PartialEq, Clone, Serialize, Default)]
pub struct CollapsibleSectionModel {
    #[prop_or_default]
    pub show_section: bool,
}

#[function_component(CollapsibleSection)]
pub fn collapsible_section(props: &Props) -> Html {
    let state: yew::UseStateHandle<CollapsibleSectionModel> = use_state(
        || CollapsibleSectionModel{
            show_section: props.show_section_default,
        });

    let cloned_state = state.clone();
    let show_section_onclick = Callback::from(move |event: MouseEvent| {
        event.prevent_default();
        let mut modification = cloned_state.deref().clone();
        modification.show_section = !cloned_state.show_section;
        cloned_state.set(modification);
    });

    html!{
        <>
            <div onclick={show_section_onclick} 
                class={format!("bg-secondary mt-1 mb-0 p-1 {}", (if state.show_section {" rounded-top"} else {" rounded"}))}>
                if state.show_section {
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="white" class="bi bi-chevron-down" viewBox="0 0 16 16">
                        <path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z"/>
                    </svg>
                } else {
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="white" class="bi bi-chevron-right" viewBox="0 0 16 16">
                        <path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/>
                    </svg>
                }
                <span class="text-white ms-2">{props.text.clone()}</span>
            </div>
            if state.show_section {
                <div class="container mb-1 border border-secondary rounded-bottom mt-0 p-1 pb-3">
                    {props.children.clone()}
                </div>
            }
        </>
    }
}