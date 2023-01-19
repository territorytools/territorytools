use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
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

#[function_component(MenuBar)]
pub fn menu_bar() -> Html {
    // let navigator = use_navigator().unwrap();

    // let onclick = Callback::from(move |_| navigator.push(&Route::Assign));
    html! {
        <header>
            <nav class={"navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"}>
                <div class={"container"}>
                    <a class={"navbar-brand"} href={"/"}>
                        <img src={"/favicon-32x32.png"} alt={"Logo"} style={"width:20px;"} />
                        {"Territory Tools"}
                    </a>
                    <button class={"navbar-toggler"} type={"button"} data-toggle={"collapse"} data-target={".navbar-collapse"}
                        aria-controls={"navbarSupportedContent"} aria-expanded={"false"} aria-label={"Toggle navigation"}>
                        <span class={"navbar-toggler-icon"}></span>
                    </button>
                    <div class={"navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse"}>
                        <ul class={"navbar-nav flex-grow"}>
                            // Dropdown 
                            <li class={"nav-item dropdown"}>
                                <a class={"nav-link dropdown-toggle"} href={"#"} id={"navbardrop"} data-toggle={"dropdown"}>
                                    <i class={"fas fa-language fa-lg text-body"}></i>
                                </a>
                                <div class={"dropdown-menu dropdown-menu-right"}>
                                </div>
                            </li>
                            <li>
                                <ul class={"navbar-nav"}>
                                    <li class={"nav-item dropdown"}>
                                        <a class={"nav-link dropdown-toggle"} href={"#"} id={"navbardrop"} data-toggle={"dropdown"}>
                                            <i class={"fas fa-user-circle fa-lg text-body"}></i>
                                            <span style={"color:black;"}>{"User"}</span>
                                        </a>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                        <ul class={"navbar-nav flex-grow-1"}>
                            <li class={"nav-item"}>
                                <a class={"nav-link text-dark"} href={"/"}>{"Home"}</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
        </header>   
    }
}
