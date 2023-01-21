use crate::components::assign_page::AssignPage;
use crate::components::territory_map::TerritoryMap;
use crate::components::territory_summary::TerritorySummary;
use yew::prelude::*;
use yew_router::prelude::*;
//mod components;
//mod models;

#[derive(Clone, Routable, PartialEq)]
pub enum Route {
    #[at("/app")]
    Root,
    #[at("/wasm/index.html")]
    Start,
    #[at("/app/map")]
    Map,
    #[at("/app/home")]
    Home,
    // #[at("/other")]
    // Other,
    #[at("/app/assign/:territory_number/:description/:assignee_name")]
    Assign {
        territory_number: String,
        description: String,
        assignee_name: String,
    },
    #[at("/app/secure")]
    Secure,
    #[at("/app/territory/:id")]
    TerritoryView { id: String },
    #[at("/app/*path")]
    Misc { path: String },
    #[not_found]
    #[at("/404")]
    NotFound,
}
