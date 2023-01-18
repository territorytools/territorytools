use crate::components::territory_map::TerritoryMap;
use crate::components::territory_summary::TerritorySummary;
use yew_router::prelude::*;
use yew::prelude::*;
mod components;
mod models;

enum Msg {
}

struct Model {
}

impl Component for Model {
    type Message = Msg;
    type Properties = ();

    fn create(_ctx: &Context<Self>) -> Self {
        Self { 
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, _msg: Self::Message) -> bool {
        true
    }

    fn changed(&mut self, _ctx: &Context<Self>) -> bool {
        false
    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
        // let render = Router::render(|switch: Route| match switch {
        //     Route::Home => html! { <TerritoryMap /> },
        // });

        html! {
            <Router render={switch} />
        }
    }
}

fn main() {
    yew::start_app::<Model>();
}

#[derive(Clone, Routable, PartialEq)]
enum Route {
    #[at("/")]
    Home,
    #[at("/map")]
    Map,
}

fn switch(route: Route) -> Html {
    match route {
        Route::Home => html! { <h1>{ "Home" }</h1> },
        Route::Map => html! { <TerritoryMap /> },
        //Route::Post { id } => html! {<p>{format!("You are looking at Post {}", id)}</p>},
        //Route::Misc { path } => html! {<p>{format!("Matched some other path: {}", path)}</p>},
    }
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
