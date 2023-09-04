use crate::components::address_shared_letter_functions::fetch_shared_letter_addresses;
use serde::{Deserialize, Serialize};

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/addresses_search.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses/search";

//use crate::components::menu_bar::MenuBar;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::addresses::Address;
use crate::functions::document_functions::set_document_title;
use gloo_console::log;
use gloo_utils::document;
use std::ops::Deref;
use reqwasm::http::{Request};
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use wasm_bindgen::JsCast;
use web_sys::{
    Element,
    HtmlInputElement,
    HtmlButtonElement,
};

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct AddressSearchResults {
    pub count: i32,
    pub addresses: Vec<Address>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressSearchPage {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub addresses: Vec<Address>,
    pub load_error: bool,
    pub load_error_message: String,
}

pub enum Msg {
    Load(AddressSharedLetterResult),
    Search(String),
}


#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    //pub path: Option<String>,
}
//#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize, Debug)]
#[serde(rename_all = "camelCase")]
pub struct SharedLetterAddress {
    pub address_id: i32,
    pub alba_address_id: i32,
    pub territory_number: String,
    pub name: Option<String>,
    pub street: Option<String>,
    pub unit: Option<String>,
    pub city: Option<String>,
    pub state: Option<String>,
    pub postal_code: Option<String>,
    pub delivery_status: Option<String>,
    pub language: Option<String>,
    pub status: Option<String>,
}

#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct AddressSharedLetterResult {
    count: i32,
    addresses: Vec<SharedLetterAddress>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressSharedLetter {
    result: AddressSharedLetterResult,
    // search: String,
}

impl Component for AddressSharedLetter {
    type Message = Msg;
    type Properties = Props;

    fn create(ctx: &Context<Self>) -> Self {
        ctx.link().send_future(async move {
            Msg::Load(fetch_shared_letter_addresses().await)
        });
        AddressSharedLetter {
            result: AddressSharedLetterResult {
                count: 99,
                addresses: vec![],
            }
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.result = result.clone();
                log!(format!("aslp:update: addresses.len(): {}", self.result.addresses.len()));
                return true;
            },
            Msg::Search(String) => {

            }
        }
        false
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let link = ctx.link().clone();
        let publisher_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an Publisher HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();
                
                log!(format!("model: publisher_text_onchange: value: {}", value));

                //link.send_message(Msg::Search(value));
            })
        };

        let check_out_click = {
            Callback::from(move |event: MouseEvent| {
                let address_id = event
                    .target()
                    .expect("An input value for an Publisher HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .get_attribute("data-address-id");
                
                //log!(format!("model: publisher_text_onchange: value: {}", value));
                log!(format!("check out clicked: address_id: {}", address_id.clone().unwrap_or("null".to_string())));
                
                let publisher_box: Element = document().get_element_by_id(format!("publisher-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                        .expect(format!("Cannot find input box with id: publisher-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let publisher_box: HtmlInputElement = publisher_box.dyn_into().unwrap();
                publisher_box
                    .style()
                    .set_property("display", "block")
                    .expect("'display' should have been set to 'block'");

                let sent_button: Element = document().get_element_by_id(format!("sent-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                    .expect(format!("Cannot find button with id: sent-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let sent_button: HtmlButtonElement = sent_button.dyn_into().unwrap();
                sent_button
                    .style()
                    .set_property("display", "none")
                    .expect("'display' should have been set to 'none'");

                let check_out_button: Element = document().get_element_by_id(format!("check-out-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                    .expect(format!("Cannot find button with id: check-out-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let check_out_button: HtmlButtonElement = check_out_button.dyn_into().unwrap();
                check_out_button
                    .style()
                    .set_property("display", "none")
                    .expect("'display' should have been set to 'none'");

                let final_check_out_button: Element = document().get_element_by_id(format!("final-check-out-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                    .expect(format!("Cannot find button with id: check-out-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let final_check_out_button: HtmlButtonElement = final_check_out_button.dyn_into().unwrap();
                final_check_out_button
                    .style()
                    .set_property("display", "block")
                    .expect("'display' should have been set to 'block'");

                let cancel_button: Element = document().get_element_by_id(format!("cancel-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                    .expect(format!("Cannot find button with id: cancel-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let cancel_button: HtmlButtonElement = cancel_button.dyn_into().unwrap();
                cancel_button
                    .style()
                    .set_property("display", "block")
                    .expect("'display' should have been set to 'block'");

                //link.send_message(Msg::Search(value));
            })
        };

        let final_check_out_click = {
            Callback::from(move |event: MouseEvent| {
                let address_id = event
                    .target()
                    .expect("An input value for an Publisher HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .get_attribute("data-address-id");
                
                //log!(format!("model: publisher_text_onchange: value: {}", value));
                log!(format!("check out clicked: address_id: {}", address_id.clone().unwrap_or("null".to_string())));
                
                // let publisher_box: Element = document().get_element_by_id(format!("publisher-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                //         .expect(format!("Cannot find input box with id: publisher-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                // let publisher_box: HtmlInputElement = publisher_box.dyn_into().unwrap();
                // publisher_box
                //     .style()
                //     .set_property("display", "block")
                //     .expect("'display' should have been set to 'block'");

                let sent_button: Element = document().get_element_by_id(format!("sent-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                    .expect(format!("Cannot find button with id: sent-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let sent_button: HtmlButtonElement = sent_button.dyn_into().unwrap();
                sent_button
                    .style()
                    .set_property("display", "block")
                    .expect("'display' should have been set to 'block'");

                let check_out_button: Element = document().get_element_by_id(format!("check-out-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                    .expect(format!("Cannot find button with id: check-out-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let check_out_button: HtmlButtonElement = check_out_button.dyn_into().unwrap();
                check_out_button
                    .style()
                    .set_property("display", "none")
                    .expect("'display' should have been set to 'none'");

                let final_check_out_button: Element = document().get_element_by_id(format!("final-check-out-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                    .expect(format!("Cannot find button with id: check-out-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let final_check_out_button: HtmlButtonElement = final_check_out_button.dyn_into().unwrap();
                final_check_out_button
                    .style()
                    .set_property("display", "none")
                    .expect("'display' should have been set to 'none'");

                let cancel_button: Element = document().get_element_by_id(format!("cancel-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                    .expect(format!("Cannot find button with id: cancel-button-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let cancel_button: HtmlButtonElement = cancel_button.dyn_into().unwrap();
                cancel_button
                    .style()
                    .set_property("display", "none")
                    .expect("'display' should have been set to 'none'");

                //link.send_message(Msg::Search(value));
            })
        };

        let sent_click = {
            Callback::from(move |event: MouseEvent| {
                let address_id = event
                    .target()
                    .expect("An input value for an Publisher HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .get_attribute("data-address-id");
                
                //log!(format!("model: publisher_text_onchange: value: {}", value));
                log!(format!("check out clicked: address_id: {}", address_id.clone().unwrap_or("null".to_string())));
                
                let publisher_box: Element = document().get_element_by_id(format!("publisher-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str())
                        .expect(format!("Cannot find input box with id: publisher-for-address-id-{}", address_id.clone().unwrap_or("null".to_string())).as_str());
                let publisher_box: HtmlInputElement = publisher_box.dyn_into().unwrap();
                let publisher_name: String = publisher_box
                    // .expect("An input value for an Publisher HtmlInputElement")
                    // .unchecked_into::<HtmlInputElement>()
                    .value();

                log!(format!("aslp:sent_click:publisher_name: {}", publisher_name.clone()));
                
                if publisher_name.clone().is_empty() {
                    log!("aslp:sent_click:publisher_name: EMPTY");
                } else {
                    log!("aslp:sent_click:publisher_name: READY");
                }

                //link.send_message(Msg::Search(value));
            })
        };

        html!{
            <>
                <MenuBarV2>
                    <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                        <li class={"nav-item"}>
                            <MapPageLink />
                        </li>  
                    </ul>
                </MenuBarV2>
                <div class="container">
                    <span><strong>{"Letter Writing - Shared"}</strong></span>
                    <hr/>
                    <div class="d-flex flex-row">
                        <div class="d-flex flex-colum mb-2 shadow-sm">
                            // <input /*{onchange}*/ type="text" value="" style="max-width:400px;" placeholder="Enter part of address" class="form-control" />
                            // <button type="submit" class="btn btn-primary">{"Search"}</button>

                            // if state.load_error { 
                            //     <span class="mx-1 badge bg-danger">{"Error"}</span> 
                            //     <span class="mx-1" style="color:red;">{state.load_error_message.clone()}</span>
                            // }    
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <span>{"Addresses: "}{self.result.count}</span>
                        </div>
                    </div>
                    {
                        self.result.addresses.iter().map(|address| {
                            let alba_address_id = address.alba_address_id;
                            let edit_uri = format!("/app/address-edit?alba_address_id={alba_address_id}");
                            let unit_text: String = match &address.unit {
                                Some(v) => if v == "" { "".to_string() } else { format!(", {}", v.clone()) },
                                None => "".to_string()
                            };
                            let mut checked_out: bool = false;
                            html!{
                                <>
                                    <div class="row" style="border-top: 1px solid gray;padding-top:8px;margin-bottom:8px;">
                                        <div class="col-5 col-sm-3 col-md-3 col-lg-2 col-xl-2">
                                            <div>
                                            <input value="" /*{state.address.name.clone()}*/ 
                                                id={format!("publisher-for-address-id-{}", address.address_id)} 
                                                onchange={publisher_text_onchange.clone()} 
                                                type="text" 
                                                style="display:none;" 
                                                class="form-control shadow-sm" 
                                                placeholder="Publisher"/>
                                            
                                            <button
                                                id={format!("check-out-button-for-address-id-{}", address.address_id)} 
                                                style="display:block;" 
                                                class="btn btn-outline-primary m-1"
                                                data-address-id={address.address_id.to_string()} 
                                                onclick={check_out_click.clone()}>
                                                {"Check Out"}
                                            </button>
                                            <button
                                                id={format!("final-check-out-button-for-address-id-{}", address.address_id)} 
                                                style="display:none;" 
                                                class="btn btn-primary m-1"
                                                data-address-id={address.address_id.to_string()} 
                                                onclick={final_check_out_click.clone()}>
                                                {"Check Out"}
                                            </button>

                                            <button 
                                                id={format!("sent-button-for-address-id-{}", address.address_id)} 
                                                style="display:none;" 
                                                class="btn btn-primary m-1" 
                                                data-address-id={address.address_id.to_string()} 
                                                onclick={sent_click.clone()}>
                                                {"Letter Sent"}
                                            </button>

                                            <button 
                                                id={format!("cancel-button-for-address-id-{}", address.address_id)} 
                                                style="display:none;" 
                                                class="btn btn-secondary m-1" 
                                                data-address-id={address.address_id.to_string()} 
                                                //onclick={sent_click.clone()}
                                                >
                                                {"Cancel"}
                                            </button>
                                            </div>
                                        </div>
                                        // <div class="col-6 col-md-3">
                                        //     //<input value="" /*{state.address.name.clone()}*/ onchange={publisher_text_onchange.clone()} type="text" class="form-control shadow-sm" id="input_sent_date" placeholder="Date Letter Sent"/>
                                        //     // <button class="btn btn-primary">{"Letter Sent"}</button>
                                        // </div>
                                        <div class="col-7 col-sm-6 col-md-4 col-lg-4 col-xl-3">
                                            <span style="font-weight:bold;">{address.name.clone()}</span>
                                            <br/>
                                            {address.street.clone().unwrap_or("".to_string())}
                                            {unit_text}
                                            <br/>
                                            {address.city.clone()}
                                            {", "}
                                            {address.state.clone()}
                                            {"  "}
                                            {address.postal_code.clone()}
                                        </div>                                        
                                        <div class="col-12 col-sm-3 col-md-3 col-lg-2 col-xl-2">
                                            if address.status.clone() == Some("Valid".to_string()) {
                                                <span class="ms-2 badge rounded-pill bg-success">{""}{address.language.clone()}</span> 
                                            }
                                            //<br/>
                                            if address.status.clone() == Some("New".to_string()) {
                                            } else if address.status.clone() == Some("Valid".to_string()) {
                                            } else if address.status.clone() == Some("Do not call".to_string()) {
                                                <span class="ms-2 badge rounded-pill bg-danger">{address.status.clone()}</span> 
                                            } else if address.status.clone() == Some("Moved".to_string()) {
                                                <span class="ms-2 badge rounded-pill bg-warning">{address.status.clone()}</span> 
                                            } 
                                            else {
                                                <span class="ms-2 badge rounded-pill bg-dark">{address.status.clone()}</span> 
                                            }
                                            // <br/>
                                            if address.delivery_status.clone() == Some("None".to_string()) {
                                            
                                            } else if address.delivery_status.clone() == Some("Assigned".to_string()) {
                                                <span class="ms-2 badge rounded-pill bg-info">{address.delivery_status.clone()}</span> 
                                            } else if address.delivery_status.clone() == Some("Sent".to_string()) {
                                                <span class="ms-2 badge rounded-pill bg-success">{address.delivery_status.clone()}</span> 
                                            } else if address.delivery_status.clone() == Some("Returned".to_string()) {
                                                <span class="ms-2 badge rounded-pill bg-warning">{address.delivery_status.clone()}</span> 
                                            } else if address.delivery_status.clone() == Some("Undeliverable".to_string()) {
                                                <span class="ms-2 badge rounded-pill bg-warning">{address.delivery_status.clone()}</span> 
                                            } else {
                                                 <span class="ms-2 badge rounded-pill bg-dark">{address.delivery_status.clone()}</span> 
                                            }               
                                        </div>                                      
                                        // <div class="col-2 col-md-1">
                                        //     {address.territory_number.clone()}
                                        //     <br/>
                                        //     <small style="color:lightgray;">{address.alba_address_id}</small>
                                        // </div>
                                    </div>
                                </>
                            }
                    }).collect::<Html>()
                }                
                </div>
            </>
        }
    }
}
