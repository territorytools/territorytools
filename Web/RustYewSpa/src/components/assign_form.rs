use crate::components::{
    //bb_button::BBButton,
    bb_text_input::{BBTextInput, InputType},
    user_selector::UserSelector,
};

use std::ops::Deref;
use yew::prelude::*;
use urlencoding::decode;

#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    pub onsubmit: Callback<TerritoryAssignment>,
    pub territory_number: String,
    pub description: String,
    pub assignee_alba_id: String,
}

#[derive(Default, Clone)]
pub struct TerritoryAssignment {
    pub territory_number: String,
    pub description: String,
    pub assignee: String,
}

#[function_component(AssignForm)]
pub fn assign_form(props: &Props) -> Html {
    let state = use_state(||TerritoryAssignment {
        territory_number: props.territory_number.clone(),
        description: props.description.clone(),
        assignee: props.assignee_alba_id.clone(),
    });

    let description: String = format!("{}", decode(&props.description).expect("UTF-8"));

    let territory_number_onchange = {
        let state = state.clone();
        Callback::from(move |territory_number: String| {
            let mut assignment = state.deref().clone();
            assignment.territory_number = territory_number;
            state.set(assignment);
        })
    };

    let description_onchange = {
        let state = state.clone();
        Callback::from(move |description: String| {
            let mut assignment = state.deref().clone();
            assignment.description = description;
            state.set(assignment);
        })
    };

    let assignee_onchange = {
        let state = state.clone();
        Callback::from(move |assignee: String| {
            let mut assignment = state.deref().clone();
            assignment.assignee = assignee;
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
        <>
            <div class={"container"}>
                <form {onsubmit}>
                    <div class={"form-group"}>
                        <BBTextInput 
                            value={props.territory_number.clone()} 
                            data_test="territory_number" 
                            label="区域号码 Territory Number" 
                            placeholder="Number" 
                            class="form-control" 
                            input_type={InputType::Text}
                            onchange={territory_number_onchange} />
                        <BBTextInput 
                            value={description} 
                            data_test="description" 
                            label="区域名称 Description" 
                            placeholder="What description do you want?" 
                            class="form-control" 
                            input_type={InputType::Text} 
                            onchange={description_onchange} />
                        <label>{"委派给 Assign to"}</label>
                        <div class={"input-group-append"}>
                            <UserSelector id="user-selector" onchange={assignee_onchange} />
                            //<BBButton label={"Assign"} data_test="submit" class={"btn btn-primary"} />
                            <button id="assign-button" type="submit" class="btn btn-primary">{"Assign"}</button>
                        </div>
                    </div>
                </form>
            </div>

        </>
    }
}
