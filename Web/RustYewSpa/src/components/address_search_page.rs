#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/addresses_search.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses/search";

use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::addresses::Address;
use crate::Route;

use gloo_console::log;
use reqwasm::http::{Request};
use serde::Deserialize;
use serde::Serialize;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::prelude::LocationHandle;
use yew_router::scope_ext::RouterScopeExt;

pub enum Msg {
    Load(AddressSearchPageResult),
    RefreshFromSearchText(),
}

//#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressSearchPage {
    _listener: LocationHandle,
    // pub success: bool,
    // pub count: i32,
    // pub search_text: String,
    pub addresses: Vec<Address>,
    pub result: AddressSearchPageResult
    // pub load_error: bool,
    // pub load_error_message: String,
}

impl Component for AddressSearchPage {
    type Message = Msg;
    type Properties = ();
    
    fn create(ctx: &Context<Self>) -> Self {
        let link = ctx.link().clone();      
        let listener = ctx.link()
            .add_location_listener(
                Callback::from(move |_| {
                    link.send_message(Msg::RefreshFromSearchText());
                })
            )
            .unwrap();

        return Self {
            _listener: listener,
            addresses: vec![],
            result: AddressSearchPageResult::default(),
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.addresses = result.addresses.clone();
                true
            },
            Msg::RefreshFromSearchText() => {
                let search_text = ctx.search_query().search_text.clone().unwrap_or_default();  
                ctx.link().send_future(async move {
                    Msg::Load(get_addresses(search_text.clone()).await)
                });
                false
            }
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        //set_document_title("Territory Search");
        
        let onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            // If we don't prevent_default() it will clear the box and search again
        });

        let navigator = ctx.link().navigator().unwrap();
        let onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = AddressSearchQuery {
                    search_text: Some(value.clone()),
                };
                let _ = navigator.push_with_query(&Route::AddressSearch, &query);
            })
        };
    
        let count = self.addresses.len();
        let search_text = ctx.search_query().search_text.clone().unwrap_or_default();  
      
        html! {
            <>
                <MenuBarV2>
                    <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                        <li class={"nav-item"}>
                            <MapPageLink />
                        </li>  
                    </ul>
                </MenuBarV2>
                <div class="container">
                    <span><strong>{"Address Search"}</strong></span>
        
                    <hr/>
                    <form {onsubmit} >
                    <div class="d-flex flex-row">
                        <div class="d-flex flex-colum mb-2 shadow-sm">
                            <input {onchange} type="text" value={search_text} style="max-width:400px;" placeholder="Enter part of address" class="form-control" />
                            <button type="submit" class="btn btn-primary">{"Search"}</button>
                            if self.result.load_error { 
                                <span class="mx-1 badge bg-danger">{"Error"}</span> 
                                <span class="mx-1" style="color:red;">{self.result.load_error_message.clone()}</span>
                            }    
                        </div>
                    </div>
                    </form>
                    <div class="row">
                        <div class="col">
                            <span>{"Count: "}{self.result.count}</span>
                            <span class="ms-2 badge mb-2 bg-secondary">{"Language"}</span> 
                            <span class="ms-2 badge mb-2 bg-secondary">{"Visit Status"}</span> 
                            <span class="ms-2 badge mb-2 bg-secondary">{"Mail Status"}</span> 
                        </div>
                    </div>
                    {
                        self.addresses.iter().map(|address| {   
                            let alba_address_id = address.alba_address_id;
                            let edit_uri = format!("/app/address-edit?alba_address_id={alba_address_id}");
                            let unit_text: String = match &address.unit {
                                Some(v) => if v == "" { "".to_string() } else { format!(", {}", v.clone()) },
                                None => "".to_string()
                            };
        
                            html! {
                                <a href={edit_uri} style="text-decoration:none;color:black;">
                                    <div class="row" style="border-top: 1px solid lightgray;">
                                        <div class="col-2 col-md-1">
                                            {address.territory_number.clone()}
                                        </div>
                                        <div class="col-10 col-md-11" style="font-weight:bold;">
                                            {address.name.clone()}
                                            <span class="ms-2 badge bg-secondary">{address.language.clone()}</span> 
                                            if address.status.clone() == Some("New".to_string()) {
                                                <span class="ms-2 badge bg-info">{address.status.clone()}</span> 
                                            } else if address.status.clone() == Some("Valid".to_string()) {
                                                <span class="ms-2 badge bg-success">{address.status.clone()}</span> 
                                            } else if address.status.clone() == Some("Do not call".to_string()) {
                                                <span class="ms-2 badge bg-danger">{address.status.clone()}</span> 
                                            } else if address.status.clone() == Some("Moved".to_string()) {
                                                <span class="ms-2 badge bg-warning">{address.status.clone()}</span> 
                                            } else {
                                                <span class="ms-2 badge bg-dark">{address.status.clone()}</span> 
                                            }
                                            if address.delivery_status.clone() == Some("None".to_string()) {
                                                <span class="ms-2 badge bg-secondary">{address.delivery_status.clone()}</span> 
                                            } else if address.delivery_status.clone() == Some("Assigned".to_string()) {
                                                <span class="ms-2 badge bg-info">{address.delivery_status.clone()}</span> 
                                            } else if address.delivery_status.clone() == Some("Sent".to_string()) {
                                                <span class="ms-2 badge bg-success">{address.delivery_status.clone()}</span> 
                                            } else if address.delivery_status.clone() == Some("Returned".to_string()) {
                                                <span class="ms-2 badge bg-warning">{address.delivery_status.clone()}</span> 
                                            } else if address.delivery_status.clone() == Some("Undeliverable".to_string()) {
                                                <span class="ms-2 badge bg-warning">{address.delivery_status.clone()}</span> 
                                            } else {
                                                <span class="ms-2 badge bg-dark">{address.delivery_status.clone()}</span> 
                                            }
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-2 col-md-1">
                                            <small style="color:lightgray;">{address.alba_address_id}</small>
                                        </div>
                                        <div class="col-10 col-md-11">
                                            {address.street.clone()}
                                            {unit_text}
                                            {", "}
                                            {address.city.clone()}
                                            {", "}
                                            {address.postal_code.clone()}
                                        </div>
                                    </div>
                                </a>
                            }
                        }).collect::<Html>()
                    }
                </div>
            </>
        }
    }
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct AddressSearchResults {
    pub count: i32,
    pub addresses: Vec<Address>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressSearchPageResult {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub addresses: Vec<Address>,
    pub load_error: bool,
    pub load_error_message: String,
}

async fn get_addresses(search_text: String) -> AddressSearchPageResult {
    let uri_string: String = format!("{path}?text={search_text}", path = DATA_API_PATH);
    let uri: &str = uri_string.as_str();
    let resp = Request::get(uri)
        .header("Content-Type", "application/json")
        .send()
        .await
        .expect("A result from the /api/addresses/search endpoint");
    
    log!(format!("load addresses from search result code: {}", resp.status().to_string()));

    let address_result: AddressSearchResults = if resp.status() == 200 {
        resp
        .json()
        .await
        .expect("Valid address search result in JSON format")
    } else {
        AddressSearchResults {
            count: 0,
            addresses: vec![],
        }
    };
    
    AddressSearchPageResult {
        success: (resp.status() == 200),
        count: address_result.count,
        addresses: address_result.addresses,
        search_text: search_text.to_string(),
        load_error: resp.status() != 200,
        load_error_message: if resp.status() == 401 {
                "Unauthorized".to_string()
            } else if resp.status() == 403 {
                "Forbidden".to_string()
            } else {
                format!("Error {:?}", resp.status())
            }
    }
}

#[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
pub struct AddressSearchQuery {
    pub search_text: Option<String>,
}

pub trait SearchQuery {
    fn search_query(&self) -> AddressSearchQuery;
}

impl SearchQuery for &Context<AddressSearchPage> {
    fn search_query(&self) -> AddressSearchQuery {
        let location = self.link().location().expect("Location or URI");
        location.query().unwrap_or(AddressSearchQuery::default())    
    }
}
