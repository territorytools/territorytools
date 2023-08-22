use crate::components::address_edit_page::AddressEditPage;
use crate::components::address_search_page::AddressSearch;
use crate::components::assign_page::AssignPage;
use crate::components::link_page::TerritoryLinkPage;
use crate::components::route_stuff::Route;
use crate::components::route_stuff::switch;
use crate::components::territory_editor::*;
use crate::components::svg_map::*;
use crate::components::canvas_map::*;
use crate::components::territory_edit_page::*;
use crate::components::territory_edit_page_example::*;
use crate::components::territory_search_page::*;
use crate::components::territory_map::TerritoryMap;
use yew::prelude::*;
use yew_router::prelude::*;
mod components;
mod models;
mod functions;
mod libs;

fn main() {
    yew::Renderer::<App>::new().render();
}

#[function_component(App)]
fn app() -> Html {
    html! {
        <BrowserRouter>
            <Switch<Route> render={switch} /> // <- must be child of <BrowserRouter>
                //<canvas id="canvas" style="width:100%;height:100%;"></canvas>
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
