use crate::components::{
    bb_button::BBButton,
    bb_text_input::{BBTextInput, InputType},
};

use std::ops::Deref;
use yew::prelude::*;
use urlencoding::decode;

#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    pub onsubmit: Callback<TerritoryModification>,
    pub territory_number: String,
    pub description: String,
    pub group_id: String,
}

#[derive(Default, Clone)]
pub struct TerritoryModification {
    pub territory_number: String,
    pub description: String,
    pub group_id: String,
}

#[function_component(TerritoryEditForm)]
pub fn territory_edit_form(props: &Props) -> Html {
    let state = use_state(||TerritoryModification {
        territory_number: props.territory_number.clone(),
        description: props.description.clone(),
        group_id: props.group_id.clone(),
    });

    let _territory_number: String = format!("{}", decode(&props.territory_number).expect("UTF-8"));
    let description: String = format!("{}", decode(&props.description).expect("UTF-8"));
    let group_id: String = format!("{}", decode(&props.group_id).expect("UTF-8"));

    let territory_number_onchange = {
        let state = state.clone();
        Callback::from(move |territory_number: String| {
            let mut modification = state.deref().clone();
            modification.territory_number = territory_number;
            state.set(modification);
        })
    };

    let description_onchange = {
        let state = state.clone();
        Callback::from(move |description: String| {
            let mut modification = state.deref().clone();
            modification.description = description;
            state.set(modification);
        })
    };

    let group_id_onchange = {
        let state = state.clone();
        Callback::from(move |group_id: String| {
            let mut modification = state.deref().clone();
            modification.group_id = group_id;
            state.set(modification);
        })
    };

    let onsubmit = {
        let onsubmit_prop = props.onsubmit.clone();
        let state = state;
        Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            let modification = state.deref().clone();
            onsubmit_prop.emit(modification);
        })
    };

    html! {
        <div class={"container"}>
            <form {onsubmit}>
                <div class={"form-group"}>
                    <BBTextInput value={props.territory_number.clone()} data_test="territory_number" label="区域号码 Territory Number" placeholder="Number" class="form-control" input_type={InputType::Text} onchange={territory_number_onchange} readonly={true} />
                    <BBTextInput value={description} data_test="description" label="区域名称 Description" placeholder="What description do you want?" class="form-control" input_type={InputType::Text} onchange={description_onchange} />
                    <BBTextInput value={group_id} data_test="group_id" label="Group ID" placeholder="Group" class="form-control" input_type={InputType::Text} onchange={group_id_onchange} />
                    // <label>{"委派给 Assign to"}</label>
                    // <div class={"input-group-append"}>
                    // <UserSelector onchange={assignee_onchange} />
                    // <BBButton label={"Assign"} data_test="submit" />
                    // </div>
                    <BBButton label={"Save"} data_test="submit" class={"btn btn-primary"}/>
                    <a class="btn btn-secondary" href="/app/map">{"Close"}</a>
                </div>
            </form>
        </div>
    }
}
