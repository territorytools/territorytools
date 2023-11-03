use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::territory_links::TerritoryLinkContract;
use crate::modals::unauthorized::UnauthorizedModal;

use chrono::DateTime;
use reqwasm::http::Request;
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone)]
pub struct TerritoryLinkPageProps {
}

#[derive(Properties, PartialEq, Clone)]
pub struct TerritoryLinkCollection {
    pub links: Vec<TerritoryLinkContract>,
    pub error_message: String,        
}

#[function_component(TerritoryLinkPage)]
pub fn territory_link_page(_props: &TerritoryLinkPageProps) -> Html {        
    let links = use_state(|| TerritoryLinkCollection { links: vec![], error_message: "".to_string() });
    {
        let links = links.clone();
        use_effect_with((), move |_| {
            let links = links.clone();
            wasm_bindgen_futures::spawn_local(async move {
                
                let uri = "/api/territory-links?activeOnly=false";

                let response = Request::get(uri)
                    .send()
                    .await
                    .expect("A valid Vec<TerritoryLinkContract>");

                if response.status() == 200 {
                    let fetched_links: Vec<TerritoryLinkContract> = response
                        .json()
                        .await
                        .expect("Not a valid JSON of Vec<TerritoryLinkContract>");

                    links.set(TerritoryLinkCollection { links: fetched_links, error_message: "".to_string() });
                } else if response.status() == 401 {
                    links.set(
                        TerritoryLinkCollection 
                        { 
                            links: vec![],
                            error_message: "Unauthorized".to_string()
                        });
                } else {
                    links.set(
                        TerritoryLinkCollection 
                        { 
                            links: vec![],
                            error_message: "Error".to_string(),
                        });
                }
            });
            || ()
        });
    }

    let show_unauthorized_modal = links.error_message == "Unauthorized";
    let return_url = "%2Fapp%2Flinks";
    
    html! {
        <>
            <MenuBarV2>
                <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                    <li class={"nav-item"}>
                        <MapPageLink />
                    </li>  
                </ul>
            </MenuBarV2>
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
                links.links.iter().map(|link| 
                    {
                        let uri = format!("/mtk/{id}", id = link.id.clone());
                        let link_edit_uri = format!("/app/link-edit?link_id={}", link.id.clone());

                        html! {
                            <a href={link_edit_uri} style="text-decoration:none;color:black;">
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
                                    <div class={"col-6 col-sm-4 col-lg-3"} style="text-overflow:clip;overflow:hidden;">
                                        {link.territory_description.clone()}
                                    </div>
                                    <div class={"col-6 col-sm-4 col-lg-2"}>
                                        <a href={format!("mailto:{}",link.assignee_email.clone().expect("string"))}>
                                            {link.assignee_name.clone()}
                                        </a>
                                    </div>
                                </div>
                            </a>
                        }
                    }).collect::<Html>()
            }
            </div>
            if show_unauthorized_modal {
                <UnauthorizedModal return_url={return_url} />              
            }
        </>
    }
}

pub fn format_date(text: Option<String>) -> String {
    if text.is_none() {
        "".to_string()
    } else {
        DateTime::parse_from_rfc3339( 
            format!("{}Z", text.expect("String date"))
            .to_string().as_str()
        )
        .expect("DateTime")
        .format("%Y-%m-%d %H:%M:%S")
        .to_string()
    }
}
