#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-links.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territory-links";

use crate::components::menu_bar::MenuBar;
use crate::models::territory_links::TerritoryLinkContract;

use chrono::DateTime;
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
            <div class={"container text-nowrap"}>
                <div class={"row border-bottom"} style="font-weight:bold;">
                    <div class={"col-6 col-sm-4 col-lg-2"}>{"Created"}</div>
                    <div class={"col-6 col-sm-4 col-lg-2"}>{"Expires"}</div>
                    <div class={"col-6 col-sm-4 col-lg-2"}>{"Key"}</div>
                    <div class={"col-6 col-sm-4 col-lg-1"}>{"Number"}</div>
                    <div class={"col-6 col-sm-4 col-lg-3"}>{"Description"}</div>
                    <div class={"col-6 col-sm-4 col-lg-2"}>{"Publisher"}</div>
                </div>
            { 
                links.iter().map(|link| 
                    {
                        let uri = format!("https://mobile.territorytools.org/mtk/{id}", id = link.id.clone());

                        html! {
                        <div class={"row border-bottom"}>
                            <div class={"col-6 col-sm-4 col-lg-2"}>
                                {format_date(link.created.clone())}
                            </div>
                            <div class={"col-6 col-sm-4 col-lg-2"}>
                            <span class={if link.expired { "text-decoration-line-through"} else {""}}>
                                 {format_date(link.expires.clone())}</span>
                            </div>
                            <div class={"col-6 col-sm-4 col-lg-2"}><a href={uri}>{link.id.clone()}</a></div>
                            <div class={"col-6 col-sm-4 col-lg-1"}>
                                {link.territory_number.clone()}
                            </div>
                            <div class={"col-6 col-sm-4 col-lg-3"}>
                                {link.territory_description.clone()}
                            </div>
                            <div class={"col-6 col-sm-4 col-lg-2"}>
                                <a href={format!("mailto:{}",link.assignee_email.clone().expect("string").to_string())}>
                                    {link.assignee_name.clone()}
                                </a>
                            </div>
                           
                        </div>
                }}).collect::<Html>()
            }
            </div>
        </>
    }
}

pub fn format_date(text: Option<String>) -> String {
    DateTime::parse_from_rfc3339( 
        format!("{}Z", text.expect("String date"))
        .to_string().as_str()
    )
    .expect("DateTime")
    .format("%Y-%m-%d %H:%M:%S")
    .to_string()
}
