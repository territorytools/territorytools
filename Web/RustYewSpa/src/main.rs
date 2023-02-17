use crate::components::address_edit_page::AddressEditPage;
use crate::components::address_search_page::AddressSearch;
use crate::components::assign_page::AssignPage;
use crate::components::link_page::TerritoryLinkPage;
use crate::components::route_stuff::Route;
use crate::components::route_stuff::switch;
use crate::components::territory_edit_page::*;
use crate::components::territory_edit_page_example::*;
use crate::components::territory_map::TerritoryMap;
use gloo_console::log;
use wasm_bindgen::prelude::wasm_bindgen;
use yew::prelude::*;
use yew_router::prelude::*;
mod components;
mod models;
mod functions;

fn main() {
    yew::Renderer::<App>::new().render();
}

#[function_component(App)]
fn app() -> Html {
    html! {
        <BrowserRouter>
            <Switch<Route> render={switch} /> // <- must be child of <BrowserRouter>
        </BrowserRouter>
    }
}

#[function_component(Secure)]
fn secure() -> Html {
    let navigator = use_navigator().unwrap();

    let onclick = Callback::from(move |_| navigator.push(&Route::Home));
    html! {
        <div>
            <h1>{ "Secure" }</h1>
            <button {onclick}>{ "Go Home" }</button>
        </div>
    }
}

#[wasm_bindgen]
pub fn test_log() {
    log!("You just called from Rust from JavaScript!");
}

// #[derive(Clone, Routable, PartialEq)]
// enum Route {
//     #[at("/")]
//     Home,
//     #[at("/post/:id")]
//     Post { id: String },
//     #[at("/*path")]
//     Misc { path: String },
// }

// fn switch(route: Route) -> Html {
//     match route {
//         Route::Home => html! { <h1>{ "Home" }</h1> },
//         Route::Post { id } => html! {<p>{format!("You are looking at Post {}", id)}</p>},
//         Route::Misc { path } => html! {<p>{format!("Matched some other path: {}", path)}</p>},
//     }
// }

#[wasm_bindgen]
pub fn try_it() {
    // do something
    log!("tried it");
}

// // export a Rust function called `bar`
// #[no_mangle]
// pub extern fn bar() {
//     log!("tried bar");
// }
