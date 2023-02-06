#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-links.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territory-links";

use crate::components::menu_bar::MenuBar;
use crate::models::territory_links::TerritoryLinkContract;
use reqwasm::http::{Request};
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone)]
pub struct TerritoryLinkPageProps {
}

#[derive(Properties, PartialEq, Clone)]
pub struct TerritoryLinkCollection {
}

#[function_component(TerritoryLinkPage)]
pub fn territory_link_page(_props: &TerritoryLinkPageProps) -> Html {        
    
    let links = use_state(|| vec![]);
    {
        let links = links.clone();
        use_effect_with_deps(move |_| {
            let links = links.clone();
            wasm_bindgen_futures::spawn_local(async move {
                
                let uri: &str = DATA_API_PATH;

                let fetched_links: Vec<TerritoryLinkContract> = Request::get(uri)
                    .send()
                    .await
                    .unwrap()
                    .json()
                    .await
                    //.expect("Territory Link JSON not parsing");
                    .unwrap();

                    links.set(fetched_links);                    
            });
            || ()
        }, ());
    }

    html! {
        <>
            <MenuBar/>
            <div class={"container"}>
            { 
                links.iter().map(|link| html! {
                    <div class={"row"}>
                        <div class={"col col-lg-1"}>{link.territory_number.clone()}</div>
                        <div class={"col col-lg-2"}>{link.territory_description.clone()}</div>
                        <div class={"col col-lg-2"}>{link.assignee_name.clone()}</div>
                        <div class={"col"}><a href={"mailto:you@gmail.com"}>{link.assignee_email.clone()}</a></div>
                        <div class={"col"}>{link.assignee_phone.clone()}</div>
                        <div class={"col"}>{link.territory_uri.clone()}</div>
                    </div>
                }).collect::<Html>()
            }
            </div>
        </>
    }
}
