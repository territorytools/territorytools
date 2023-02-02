#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-links.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territory-links";

use crate::components::assign_form::*;
use crate::components::email_section::EmailSection;
use crate::components::menu_bar::MenuBar;
use crate::components::sms_section::SmsSection;

use crate::models::territory_links::TerritoryLinkContract;
use reqwasm::http::{Request, Method};
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone)]
pub struct TerritoryLinkPageProps {
}

#[derive(Properties, PartialEq, Clone)]
pub struct TerritoryLinkCollection {
}

#[function_component(TerritoryLinkPage)]
pub fn territory_link_page(props: &TerritoryLinkPageProps) -> Html {        
    let state = use_state(|| TerritoryLinkCollection {});
    let cloned_state = state.clone();
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
