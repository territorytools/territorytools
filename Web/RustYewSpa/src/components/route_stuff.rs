use crate::components::address_shared_letter_page::AddressSharedLetter;
use crate::components::territory_stage_report_page::TerritoryReportsPage;
use crate::components::user_search_page::UserSearchPage;
use crate::components::user_editor::UserEditorPage;

use crate::AddressEditPage;
use crate::AddressSearchPage;
use crate::AssignPage;
use crate::CanvasMap;
use crate::LinkEditPage;
use crate::Model;
use crate::Secure;
use crate::SvgMap;
use crate::TerritoryEditPage;
use crate::TerritoryEditorPage;
use crate::TerritoryEditPageExample;
use crate::TerritorySearchPage;
use crate::TerritorySearchOld;
use crate::TerritoryLinkPage;
use crate::TerritoryMap;

use yew_router::prelude::*;
use yew::html;
use yew::Html;

use super::app_menu::AppMenuPage;
use super::my_territories_page::MyTerritoriesPage;
//use gloo_console::log;

#[derive(Clone, Routable, PartialEq)]
pub enum Route {
    #[at("/app")]
    Root,
    #[at("/app/menu")]
    AppMenu,
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
    #[at("/app/user-search")]
    UserSearch,
    #[at("/app/shared-letter")]
    AddressSharedLetter,
    #[at("/app/address-edit")]
    AddressEdit,
    #[at("/app/link-edit")]
    LinkEdit,
    #[at("/app/territory-edit")]
    TerritoryEditor,
    #[at("/app/user-edit")]
    UserEditor,
    #[at("/app/territory-search")]
    TerritorySearch,    
    #[at("/app/my-territories")]
    MyTerritoriesPage,    
    #[at("/app/territory-search-old")]
    TerritorySearchOld,    
    #[at("/app/secure")]
    Secure,
    #[at("/app/territory/:id")]
    TerritoryView { id: String },
    #[at("/app/territory-reports/monthly-completion")]
    TerritoryReportsMonthlyCompletion,
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
        Route::AppMenu => html! { <AppMenuPage /> },
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
            html! { <Model mtk={mtk} /> }
        },
        Route::MapComponent => html! { <Model mtk={""} /> },
        Route::SvgMap => html! { <SvgMap /> },        // Experiment
        Route::CanvasMap => html! { <CanvasMap /> },  // Experiment
        Route::AddressSearch => html! { <AddressSearchPage /> },
        Route::UserSearch => html! { <UserSearchPage /> },
        Route::AddressSharedLetter => html! { <AddressSharedLetter /> },
        Route::AddressEdit => html! { <AddressEditPage /> },
        Route::LinkEdit => html! { <LinkEditPage /> },
        Route::TerritoryEditor => html! { <TerritoryEditorPage /> },
        Route::UserEditor => html! { <UserEditorPage /> },
        Route::TerritorySearch => html! { <TerritorySearchPage /> },
        Route::MyTerritoriesPage => html! { <MyTerritoriesPage /> },
        Route::TerritorySearchOld => html! { <TerritorySearchOld /> },
        Route::TerritoryReportsMonthlyCompletion => html! { <TerritoryReportsPage /> },
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