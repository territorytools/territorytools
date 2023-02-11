use crate::components::assign_page::AssignPage;
use crate::components::territory_edit_page::*;
use crate::components::territory_edit_page_example::*;
use crate::components::route_stuff::Route;
use crate::components::territory_map::TerritoryMap;
use crate::components::link_page::TerritoryLinkPage;
use crate::components::address_search_page::AddressSearch;
use gloo_console::log;
use wasm_bindgen::prelude::wasm_bindgen;
use yew::prelude::*;
use yew_router::prelude::*;
mod components;
mod models;

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

fn switch(route: Route) -> Html {
    match route {
        //Route::Home => html! { <Redirect<Route> to={"/"} },
        Route::Home => html! { <h3>{"Home"}</h3> },
        Route::Root => html! { <Redirect<Route> to={Route::Map}/> },
        Route::Start => html! { <Redirect<Route> to={Route::Map}/> },
        //Route::Other => html! { <Redirect<Route> to={"https://google.com"}/> },
        Route::Assign {
            territory_number,
            description,
            assignee_name,
        } => {
            html! { <AssignPage territory_number={territory_number} description={description} assignee_name={assignee_name}/> }
        }
        Route::Edit {
            territory_number,
            //description,
            //group_id,
        } => //territory_edit_page(TerritoryEditPageProps { territory_number: territory_number}),
            html! { 
                <TerritoryEditPage 
                    territory_number={territory_number}
                /> },
        Route::EditExample {
                    territory_number,
                } => 
                    html! { 
                        <TerritoryEditPageExample
                            territory_number={territory_number}
                        /> },                
        Route::Map => html! { <TerritoryMap /> },
        Route::AddressSearch => html! { <AddressSearch /> },
        Route::Secure => html! { // TODO: Delete this
            <Secure />
        },
        Route::NotFound => html! { <div><h1>{ "404" }</h1><h2>{"Not Found"}</h2></div> },
        //Route::NotFound => html! {<Redirect<Route> to={"/"}/>}
        Route::TerritoryView { id } => {
            html! {<p>{format!("You are looking at Territory {}", id)}</p>}
        },
        Route::Links => 
            html! { <TerritoryLinkPage /> },
        Route::Misc { path } => html! {<p>{format!("Matched some other path: {}", path)}</p>},
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