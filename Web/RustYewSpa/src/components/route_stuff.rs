use crate::AddressEditPage;
use crate::AddressSearchPage;
use crate::components::address_shared_letter_page::AddressSharedLetter;
use crate::AssignPage;
use crate::Secure;
use crate::TerritoryEditPage;
use crate::TerritoryEditorPage;
use crate::TerritoryEditPageExample;
use crate::TerritorySearchPage;
use crate::TerritorySearchOld;
use crate::TerritoryLinkPage;
use crate::TerritoryMap;
use crate::SvgMap;
use crate::CanvasMap;
use crate::Model;

use yew_router::prelude::*;
use yew::html;
use yew::Html;
use gloo_console::log;

#[derive(Clone, Routable, PartialEq)]
pub enum Route {
    #[at("/app")]
    Root,
    #[at("/wasm/index.html")]
    Start,
    #[at("/app/map-old")]
    Map,
    #[at("/key/:mtk")]
    MapComponentKey { mtk: String },
    #[at("/app/map")]
    MapComponent,
    #[at("/app/svg-map")]
    SvgMap,
    #[at("/app/canvas-map")]
    CanvasMap,
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
    #[at("/app/territories/:territory_number/edit")]
    Edit {
        territory_number: String,
    },
    #[at("/app/territories/:territory_number/example")]
    EditExample { territory_number: String },
    #[at("/app/address-search")]
    AddressSearch,
    #[at("/app/shared-letter")]
    AddressSharedLetter,
    #[at("/app/address-edit")]
    AddressEdit,
    #[at("/app/territory-edit")]
    TerritoryEditor,
    #[at("/app/territory-search")]
    TerritorySearch,    
    #[at("/app/territory-search-old")]
    TerritorySearchOld,    
    #[at("/app/secure")]
    Secure,
    #[at("/app/territory/:id")]
    TerritoryView { id: String },
    //#[at("/app/testmap")]
    // TestMap,
    #[at("/app/links")]
    Links,
    #[at("/app/*path")]
    Misc { path: String },
    #[not_found]
    #[at("/404")]
    NotFound,
}

pub fn switch(route: Route) -> Html {
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
        } =>
        //territory_edit_page(TerritoryEditPageProps { territory_number: territory_number}),
        {
            html! {
            <TerritoryEditPage
                territory_number={territory_number}
            /> }
        }
        Route::EditExample { territory_number } => html! {
        <TerritoryEditPageExample
            territory_number={territory_number}
        /> },
        Route::Map => html! { <TerritoryMap /> },
        Route::MapComponentKey { mtk } => {
            log!(format!("route_stuff:MapCompKey:mtk:{}", mtk.clone()));
            html! { <Model mtk={mtk} /> }
        },
        Route::MapComponent => html! { <Model /> },
        Route::SvgMap => html! { <SvgMap /> },
        Route::CanvasMap => html! { <CanvasMap /> },
        Route::AddressSearch => html! { <AddressSearchPage /> },
        Route::AddressSharedLetter => html! { <AddressSharedLetter /> },
        Route::AddressEdit => html! { <AddressEditPage /> },
        Route::TerritoryEditor => html! { <TerritoryEditorPage /> },
        Route::TerritorySearch => html! { <TerritorySearchPage /> },
        Route::TerritorySearchOld => html! { <TerritorySearchOld /> },
        Route::Secure => html! { // TODO: Delete this
            <Secure />
        },
        Route::NotFound => html! { <div><h1>{ "404" }</h1><h2>{"Not Found"}</h2></div> },
        //Route::NotFound => html! {<Redirect<Route> to={"/"}/>}
        Route::TerritoryView { id } => {
            html! {<p>{format!("You are looking at Territory {}", id)}</p>}
        },
        //Route::TestMap => html! { <TestMap /> },
        Route::Links => html! { <TerritoryLinkPage /> },
        Route::Misc { path } => html! {<p>{format!("Cannot find path: {}", path)}</p>},
    }
}