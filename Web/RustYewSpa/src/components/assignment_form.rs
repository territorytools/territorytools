use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
use crate::components::user_selector::UserSelector;
use crate::models::territories::{Territory};
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use leaflet::{LatLng, Map, TileLayer, Polygon, Polyline, Control};
use reqwasm::http::{Request};
use yew::prelude::*;
use gloo_utils::document;
use gloo_console::log;
use gloo_timers::callback::Timeout;
use serde::{Serialize, Deserialize};
//use js_sys::{Array, Date};
use web_sys::{
    Document,
    Element,
    HtmlElement,
    Window,
    Node
};

#[function_component(AssignmentForm)]
pub fn menu_bar() -> Html {
    html! {
        <div class={"container"}>
            <div id={"assignment-buttons"}>
                <form>
                    <div class={"form-group"}>
                        <label for={"modal-number"}>{"Current Assignee:"}</label>
                        <input name={"territoryNumber"} type={"hidden"} class={"form-control"} id={"modal-territory-number"}/> // readonly
                        <div class={"input-group-append"}>
                            <input name={"assignee"} type={"text"} class={"form-control"} placeholder={"Nobody assigned"} id={"modal-current-assignee"}/> // readonly
                            <button id={"unassign-button"} type={"button"} class={"btn btn-primary"}>{"Unassign"}</button>
                        </div>
                    </div>
                    <div id={"unassign-status"}></div>
                </form>
                <form>
                    <div class={"form-group"}>
                        <label for={"user-menu"}>{"New Assignee:"}</label>
                        <div class={"input-group-append"}>
                            //<select id={"user-menu"} name={"albaUserId"} class={"custom-select"}></select>
                            <UserSelector />
                            <button id={"assign-button"} type={"button"} class={"btn btn-primary"}>{"Assign"}</button>
                        </div>
                    </div>
                    <div id={"assign-status"}></div>
                    <div><a id={"assigned-link"} href={"#"}></a></div>
                    <div id={"phone-number-section"} class={"form-group"} style={"display:none;"}>
                        <label for={"phone-number"}>{"SMS Phone Number:"}</label>
                        <div class={"input-group-append"}>
                            <input id={"phone-number"} name={"phoneNumber"} type={"text"} class={"form-control"}/>
                            <button id={"send-sms-button"} type={"button"} class={"btn btn-primary"}>{"Send"}</button>
                        </div>
                        <div id={"send-sms-status"}></div>
                    </div>
                </form>
            </div>
        </div>
    }
}