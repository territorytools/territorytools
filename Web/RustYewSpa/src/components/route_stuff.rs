use crate::AddressEditPage;
use crate::AddressSearch;
use crate::AssignPage;
use crate::Secure;
use crate::TerritoryEditPage;
use crate::TerritoryEditorPage;
use crate::TerritoryEditPageExample;
use crate::TerritorySearch;
use crate::TerritoryLinkPage;
use crate::TerritoryMap;
//use crate::TestMap;
//use crate::components::test_map::TestMap;

use yew_router::prelude::*;
use yew::html;
use yew::Html;

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
    #[at("/app/territories/:territory_number/edit")]
    Edit {
        territory_number: String,
        //description: String,
        //group_id: String,
    },
    #[at("/app/territories/:territory_number/example")]
    EditExample { territory_number: String },
    #[at("/app/address-search")]
    AddressSearch,
    #[at("/app/address-edit")]
    AddressEdit,
    #[at("/app/territory-edit")]
    TerritoryEditor,
    #[at("/app/territory-search")]
    TerritorySearch,    
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
        Route::AddressSearch => html! { <AddressSearch /> },
        Route::AddressEdit => html! { <AddressEditPage /> },
        Route::TerritoryEditor => html! { <TerritoryEditorPage /> },
        Route::TerritorySearch => html! { <TerritorySearch /> },
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