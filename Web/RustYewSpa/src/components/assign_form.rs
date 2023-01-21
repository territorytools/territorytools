use yew::prelude::*;
use std::ops::Deref;

use crate::components::{
    //bb_button::BBButton,
    bb_text_input::{BBTextInput, InputType},
};

use stylist::yew::styled_component;

#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    pub onsubmit: Callback<TerritoryAssignment>,
    //pub action: Action,
}

// #[derive(Clone, PartialEq)]
// pub enum Action {
//     CreateAccount,
//     Login,
// }

#[derive(Default, Clone)]
pub struct TerritoryAssignment {
    pub number: String,
    pub description: String,
    //pub assignee: String,
}

#[function_component(AssignForm)]
pub fn assign_form(props: &Props) -> Html {
    let state = use_state(TerritoryAssignment::default);

    let description_onchange = {
        let state = state.clone();
        Callback::from(move |description: String| {
            let mut assignment = state.deref().clone();
            assignment.description = description;
            state.set(assignment);
        })
    };

    let onsubmit = {
        let onsubmit_prop = props.onsubmit.clone();
        let state = state;
        Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            let assignment = state.deref().clone();
            onsubmit_prop.emit(assignment);
        })
    };

    html! {
        <div class={"container"}>
        <div id={"assignment-buttons"}>
        <form {onsubmit}>
            <div class={"form-group"}>
                <BBTextInput data_test="description" label="Description" placeholder="What description do you want?" class="form-control" input_type={InputType::Text} onchange={description_onchange} />
                // <BBButton label={props.action.to_string()} data_test="submit" />
            </div>
        </form>
        </div>
        </div>
    }
}