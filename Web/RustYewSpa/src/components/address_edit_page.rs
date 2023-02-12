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
            // <div class="d-flex flex-row">
            //     <div class="m-1 d-flex flex-colum">
            <div class="mb-3">
                    <label for="street" class="form-label">{"Street"}</label>
                    <input id="street" type="text" value="" style="max-width:400px;" placeholder="123 Some St" class="form-control" />
                    </div>
                    <div class="mb-3">
            //     </div>
            // </div>
            // <div class="d-flex flex-row">
            //     <div class="m-1 d-flex flex-colum">
                    <label for="city" class="form-label">{"City"}</label>
                    <input id="city" type="text" value="" style="max-width:400px;" placeholder="City" class="form-control" />
                    </div>
                    <div class="mb-3">
            //     </div>
            // </div>
            // <div class="d-flex flex-row">
            //     <div class="m-1 d-flex flex-colum">
                    <label for="postal-code" class="form-label">{"Postal Code"}</label>
                    <input id="post-code" type="text" value="" style="max-width:400px;" placeholder="Postal Code" class="form-control" />
                    </div>
                    <div class="mb-3">
            //     </div>
            // </div>
            // <div class="d-flex flex-row">
            //     <div class="m-1 d-flex flex-colum">
                    <button type="submit" class={"btn btn-primary"}>{"Save"}</button>
                    </div>
            //     </div>
            // </div>
            </form>
        </div>
        </>
    }
}
