use crate::components::menu_bar::MenuBar;
use crate::models::addresses::Address;
//use crate::models::territories::Territory;
//use serde::{Deserialize, Serialize};
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressEditModel {
    pub addresses: Address,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressEditProps {
    pub alba_address_id: i32,
}

#[function_component(AddressEditPage)]
pub fn address_edit_page() -> Html {
    let state = use_state(|| AddressEditModel::default());
    let cloned_state = state.clone();
    let onsubmit = Callback::from(move |event: SubmitEvent| {
        event.prevent_default();
    });

    html! {
        <>
        <MenuBar/>
        <div class="container">
            <span>{"Edit Address"}</span>
            <form {onsubmit} >
            <div class="d-flex flex-row">
                <div class="m-1 d-flex flex-colum">
                    <input    type="text" value="" style="max-width:400px;" placeholder="Street" class="form-control" />
                </div>
            </div>
            <div class="d-flex flex-row">
                <div class="m-1 d-flex flex-colum">
                    <input    type="text" value="" style="max-width:400px;" placeholder="City" class="form-control" />
                </div>
            </div>
            <div class="d-flex flex-row">
                <div class="m-1 d-flex flex-colum">
                    <input    type="text" value="" style="max-width:400px;" placeholder="Postal Code" class="form-control" />
                </div>
            </div>
            <div class="d-flex flex-row">
                <div class="m-1 d-flex flex-colum">
                    <button type="submit" class={"btn btn-primary"}>{"Save"}</button>
                </div>
            </div>
            </form>
        </div>
        </>
    }
}
