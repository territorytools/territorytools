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
    SetCurrentPublisher(String),
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
    pub sent_date: Option<String>,    
    pub publisher: Option<String>,    
    pub check_out_started: Option<String>,
    //pub check_out_completed: Option<String>,
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
    current_publisher: Option<String>,
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
            },
            current_publisher: None,
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

            },
            Msg::SetCurrentPublisher(publisher) => {
                self.current_publisher = Some(publisher.clone());
                return true;
            }
        }
        false
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let link = ctx.link().clone();
        // let publisher_text_onchange = {
        //     Callback::from(move |event: Event| {
        //         let value = event
        //             .target()
        //             .expect("An input value for an Publisher HtmlInputElement")
        //             .unchecked_into::<HtmlInputElement>()
        //             .value();
        //         log!(format!("model: publisher_text_onchange: value: {}", value));
        //         //link.send_message(Msg::Search(value));
        //     })
        // };
        let publisher_change = //{
            Callback::from(move |publisher_name: String| {
                log!(format!("ASLP: publisher_change: publisher_name: value: {}", publisher_name.clone()));
                //self.current_publisher = Some(publisher_name.clone());
                link.send_message(Msg::SetCurrentPublisher(publisher_name.clone()));
            });
        //};

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
                    //<div class="d-flex flex-row">
                    //     <div class="d-flex flex-colum mb-2 shadow-sm">
                    //         // <input /*{onchange}*/ type="text" value="" style="max-width:400px;" placeholder="Enter part of address" class="form-control" />
                    //         // <button type="submit" class="btn btn-primary">{"Search"}</button>

                    //         // if state.load_error { 
                    //         //     <span class="mx-1 badge bg-danger">{"Error"}</span> 
                    //         //     <span class="mx-1" style="color:red;">{state.load_error_message.clone()}</span>
                    //         // }    
                    //     </div>
                    // </div>
                    <div class="row">
                        <div class="col">
                            <span>{"Addresses: "}{self.result.count}</span>
                        </div>
                    </div>
                    {
                        self.result.addresses.iter().map(|address| {
                            html!{
                                <AddressSharedLetterRow     
                                    address={address.clone()} 
                                    current_publisher={self.current_publisher.clone()}
                                    on_publisher_change={publisher_change.clone()} />                                  
                            }
                    }).collect::<Html>()
                }                
                </div>
            </>
        }
    }
}


#[function_component(AddressSharedLetterRow)]
pub fn address_shared_letter_row(props: &AddressSharedLetterRowProperties) -> Html {
    let publisher_is_entered =  !props.address.publisher.clone().unwrap_or_default().is_empty();
    let is_checking_out = !props.address.check_out_started.clone().unwrap_or_default().is_empty();
    let is_sent = !props.address.sent_date.clone().unwrap_or_default().is_empty();
    let status_pills_visible = !is_sent;
    let state = use_state(|| AddressSharedLetterRowModel {
        address: props.address.clone(),
        check_out_button_visible: false, // !publisher_is_entered && !is_checking_out,
        publisher_input_visible: true, // publisher_is_entered,
        publisher_input_error: false,
        publisher_input_readonly: is_sent,
        final_check_out_button_visible: false,
        sent_button_visible: publisher_is_entered && !is_sent,
        cancel_button_visible: false,
        is_sent: is_sent,
        status_pills_visible: status_pills_visible,
        address_visible: true,
    });

    let state_clone = state.clone();
    let props_clone = props.clone();
    let publisher_click = {
        Callback::from(move |event: MouseEvent| {
            if state_clone.address.sent_date.clone().unwrap_or_default().is_empty() {
                let mut modification = state_clone.deref().clone();
                modification.publisher_input_visible = true;
                modification.check_out_button_visible = false;
                modification.final_check_out_button_visible = true;
                modification.cancel_button_visible = true;
                modification.sent_button_visible = false;
                modification.is_sent = false;
                modification.status_pills_visible = false;

                if modification.address.publisher.clone().unwrap_or_default().is_empty() {
                    log!(format!("address: current_publisher: {}", props_clone.current_publisher.clone().unwrap_or_default()));
                    modification.address.publisher = props_clone.current_publisher.clone();
                }

                state_clone.set(modification);
            } // or do nothing
        })
    };
    
    let state_clone = state.clone();
    let publisher_text_onchange = {
        Callback::from(move |event: Event| {
            let value = event
                .target()
                .expect("An input value for an Publisher HtmlInputElement")
                .unchecked_into::<HtmlInputElement>()
                .value();
            
            log!(format!("model: publisher_text_onchange: value: {}", value));

            let mut modification = state_clone.deref().clone();
            modification.address.publisher = Some(value);
            state_clone.set(modification);
        })
    };

    let state_clone = state.clone();
    let on_publisher_change = props.on_publisher_change.clone();
    let final_check_out_click = {
        Callback::from(move |event: MouseEvent| {
            let mut modification = state_clone.deref().clone();
            if state_clone.address.publisher.clone().unwrap_or_default().is_empty() {
                modification.publisher_input_visible = true;
                modification.publisher_input_error = true;
                modification.final_check_out_button_visible = true;
                modification.cancel_button_visible = true;
                
            } else {
                modification.publisher_input_visible = true;
                modification.publisher_input_error = false;
                modification.final_check_out_button_visible = false;
                modification.sent_button_visible = true;
                modification.cancel_button_visible = false;
                on_publisher_change.emit(state_clone.address.publisher.clone().unwrap_or_default());
            }
            state_clone.set(modification);
        })
    };

    let state_clone = state.clone();
    let sent_click = {
        Callback::from(move |event: MouseEvent| {
            let mut modification = state_clone.deref().clone();
            if state_clone.address.publisher.clone().unwrap_or_default().is_empty() {
                modification.publisher_input_visible = true;
                modification.publisher_input_error = true;
                modification.publisher_input_readonly = false;
                modification.sent_button_visible = true;
                modification.is_sent = false;
                modification.status_pills_visible = true;
            } else {
                modification.publisher_input_visible = true;
                modification.publisher_input_error = false;
                modification.publisher_input_readonly = true;
                modification.sent_button_visible = false;
                modification.address.delivery_status = Some("Sent".to_string());
                modification.address.sent_date = Some("Just now".to_string());
                modification.is_sent = true;
                modification.status_pills_visible = false;
            }
            state_clone.set(modification);
        })
    };

    let state_clone = state.clone();
    let cancel_click = {
        Callback::from(move |event: MouseEvent| {
            let mut modification = state_clone.deref().clone();
            modification.publisher_input_visible = true; // should always be true
            modification.publisher_input_error = false;
            modification.cancel_button_visible = false;
           // modification.check_out_button_visible = true;
            modification.final_check_out_button_visible = false;
            modification.sent_button_visible = false;
            modification.status_pills_visible = true;
            modification.address.publisher = Some("".to_string());
            state_clone.set(modification);
        })
    };

    //let state = state.clone();
    let address = state.address.clone();

    let alba_address_id = address.alba_address_id;
    let edit_uri = format!("/app/address-edit?alba_address_id={alba_address_id}");
    let unit_text: String = match &address.unit {
        Some(v) => if v == "" { "".to_string() } else { format!(", {}", v.clone()) },
        None => "".to_string()
    };
    //let mut checked_out: bool = false;
    let has_publisher: bool = !address.publisher.clone().unwrap_or("".to_string()).is_empty();
    let publisher_style = if state.clone().publisher_input_error {
        "border-width:4px;border-color:red;color:black;"
    } else if state.clone().is_sent {
        "color:#090;border-color:#090;" //"border-width:4px;border-color:#090;" //background-color:#090;color:white;"
    } else if has_publisher { 
        "color:black;border-color:black;"
    } else { 
        "border-color:blue;color:black;" 
    };

    let state_clone = state.clone();
    log!(format!("publisher_input_error: {}",state_clone.publisher_input_error.clone()));

    html!{
        <>
            <div class="row" style="border-top: 1px solid gray;padding-top:8px;margin-bottom:8px;">
                <div class="col-5 col-sm-5 col-md-3 col-lg-2 col-xl-2">
                    if state_clone.publisher_input_visible.clone() {
                        <input 
                            value={address.publisher.clone()} 
                            id={format!("publisher-for-address-id-{}", address.address_id)} 
                            onchange={publisher_text_onchange.clone()}
                            onclick={publisher_click.clone()}
                            type="text" 
                            style={publisher_style}
                            class="form-control shadow-sm m-1 letter-writing-shared-input" 
                            readonly={state_clone.publisher_input_readonly.clone()}
                            placeholder="你的名字"/>
                    } 
         
                    if state_clone.check_out_button_visible.clone() {
                        <button
                            id={format!("check-out-button-for-address-id-{}", address.address_id)} 
                            class="btn btn-outline-primary m-1"
                            data-address-id={address.address_id.to_string()} 
                            onclick={publisher_click.clone()}>
                            {"Check Out"}
                        </button>
                    }
                    if state_clone.final_check_out_button_visible.clone() {
                        <button
                            id={format!("final-check-out-button-for-address-id-{}", address.address_id)} 
                            //style="display:none;" 
                            class="btn btn-primary m-1"
                            data-address-id={address.address_id.to_string()} 
                            onclick={final_check_out_click.clone()}>
                            {"Check Out"}
                        </button>
                    }

                    if state_clone.sent_button_visible.clone() {
                        <button 
                            id={format!("sent-button-for-address-id-{}", address.address_id)} 
                            style="border-color:#090;" 
                            class="btn btn-outline-success m-1" 
                            data-address-id={address.address_id.to_string()} 
                            onclick={sent_click.clone()}>
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-send" viewBox="0 0 16 16">
                                <path d="M15.854.146a.5.5 0 0 1 .11.54l-5.819 14.547a.75.75 0 0 1-1.329.124l-3.178-4.995L.643 7.184a.75.75 0 0 1 .124-1.33L15.314.037a.5.5 0 0 1 .54.11ZM6.636 10.07l2.761 4.338L14.13 2.576 6.636 10.07Zm6.787-8.201L1.591 6.602l4.339 2.76 7.494-7.493Z"/>
                            </svg>
                            {" Send Letter"}
                        </button>
                    }

                    if state_clone.cancel_button_visible.clone() {
                        <button 
                            id={format!("cancel-button-for-address-id-{}", address.address_id)} 
                            //style="display:none;" 
                            class="btn btn-secondary m-1" 
                            data-address-id={address.address_id.to_string()} 
                            onclick={cancel_click.clone()}>
                            {"Cancel"}
                        </button>
                    }
                    if state.is_sent {
                        <span class="ms-2 badge rounded-pill" style="background-color:#090;">{"Sent"}</span> 
                        <span>{" "}</span>
                        <span style="color:#090">{state.address.sent_date.clone().unwrap_or_default().chars().take(10).collect::<String>()}</span>
                    }
                </div>
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
                                    
                <div class="col-12 col-sm-12 col-md-3 col-lg-2 col-xl-2">
                    
                    if state.status_pills_visible {
                        if address.status.clone() == Some("Valid".to_string()) {
                            <span class="ms-2 badge rounded-pill bg-secondary">{""}{address.language.clone()}</span> 
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
                        // <span class="ms-2 badge rounded-pill bg-success">{address.delivery_status.clone()}</span> 
                        } else if address.delivery_status.clone() == Some("Returned".to_string()) {
                            <span class="ms-2 badge rounded-pill bg-warning">{address.delivery_status.clone()}</span> 
                        } else if address.delivery_status.clone() == Some("Undeliverable".to_string()) {
                            <span class="ms-2 badge rounded-pill bg-warning">{address.delivery_status.clone()}</span> 
                        } else {
                                <span class="ms-2 badge rounded-pill bg-dark">{address.delivery_status.clone()}</span> 
                        }     
                    }
                </div>
            </div>
        </>
    }
}

#[derive(Properties, PartialEq, Clone, Default)] //, Serialize)]
pub struct AddressSharedLetterRowProperties {
    pub address: SharedLetterAddress,
    pub on_publisher_change: Callback<String>,
    pub current_publisher: Option<String>,
    // #[prop_or_default]
    // pub error_message: String,
}


#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct AddressSharedLetterRowModel {
    pub address: SharedLetterAddress,
    pub check_out_button_visible: bool,
    pub publisher_input_visible: bool,
    pub publisher_input_error: bool,
    pub publisher_input_readonly: bool,
    pub final_check_out_button_visible: bool,
    pub sent_button_visible: bool,
    pub cancel_button_visible: bool,
    pub is_sent: bool,
    pub status_pills_visible: bool,
    pub address_visible: bool,
    //pub current_publisher: Option<String>,
}

