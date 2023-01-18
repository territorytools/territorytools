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

    fn changed(&mut self, _: &yew::Context<Self>, _: &<Self as yew::Component>::Properties) -> bool {
        false
    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
        // let render = Router::render(|switch: Route| match switch {
        //     Route::Home => html! { <TerritoryMap /> },
        // });

        html! {
            <p>{"Hi"}</p>
        }
    }
}

fn main() {
    //yew::Renderer::<Main>();
    yew::Renderer::<Main>::new().render();
}

#[function_component(Main)]
fn app() -> Html {
    html! {
        // <TerritoryMap/>
        <BrowserRouter>
            <Switch<Route> render={switch} /> // <- must be child of <BrowserRouter>
        </BrowserRouter>
    }
}

#[derive(Clone, Routable, PartialEq)]
enum Route {
    #[at("/")]
    Home,
    #[at("/secure")]
    Secure,
    #[at("/map")]
    Map,
    #[not_found]
    #[at("/404")]
    NotFound,
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
        Route::Home => html! { <h1>{ "Home" }</h1> },
        Route::Map => html! { <TerritoryMap /> },
        Route::Secure => html! {
            <Secure />
        },
        Route::NotFound => html! { <h1>{ "404" }</h1> },
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
