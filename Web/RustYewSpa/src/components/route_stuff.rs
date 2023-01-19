use crate::components::territory_map::TerritoryMap;
use crate::components::territory_summary::TerritorySummary;
use crate::components::assign_page::AssignPage;
use yew_router::prelude::*;
use yew::prelude::*;
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
    #[at("/app/assign/:id")]
    Assign { id: String },
    #[at("/app/secure")]
    Secure,
    #[at("/app/territory/:id")]
    TerritoryView { id: String },    
    #[at("/app/*path")]
    Misc { path: String },
    #[not_found]
    #[at("/app/404")]
    NotFound,
}
