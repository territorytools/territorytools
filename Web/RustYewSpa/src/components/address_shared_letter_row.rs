use crate::components::address_shared_letter_functions::*;
use serde::{Deserialize, Serialize};

// #[cfg(debug_assertions)]
// const DATA_API_PATH: &str = "/data/addresses_search.json";

// #[cfg(not(debug_assertions))]
// const DATA_API_PATH: &str = "/api/addresses/search";

use crate::models::addresses::Address;
use gloo_console::log;
use yew::prelude::*;
use wasm_bindgen::JsCast;
use web_sys::{
    HtmlInputElement,
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
    // Load(AddressSharedLetterResult),
    // Search(String),
    // SetCurrentPublisher(String),
    // LoadAddress(CheckoutStartResult),
    PublisherClick(CheckoutStartResult),
    UpdatePublisher(String),
    LetterSent(),
    CheckoutFinish(),
    CheckoutCancel(),
}


#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    pub address: SharedLetterAddress,
    pub on_publisher_change: Callback<String>,
    pub current_publisher: Option<String>,
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

#[derive(Properties, PartialEq, Clone, Default, Serialize)]
pub struct AddressSharedLetterRow {
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
}

impl Component for AddressSharedLetterRow {
    type Message = Msg;
    type Properties = Props;

    fn create(ctx: &Context<Self>) -> Self {
        return Self {
            address: ctx.props().address.clone(),
            check_out_button_visible: false,
            publisher_input_visible: true, 
            publisher_input_error: false,
            publisher_input_readonly: false, // is_sent || publisher_is_entered || is_checking_out,
            final_check_out_button_visible: false,
            sent_button_visible: false, //publisher_is_entered && !is_sent,
            cancel_button_visible: false,
            is_sent: false, //is_sent,
            status_pills_visible: true, //status_pills_visible,
            address_visible: true,
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            // Msg::LoadAddress(_result) => {
            //     true
            // },
            Msg::PublisherClick(result) => {
                log!(format!("aslp: PublisherClick: post result: aid: {}, success: {}", result.alba_address_id, result.success));
                if self.address.sent_date.clone().unwrap_or_default().is_empty() 
                    && self.address.check_out_started.clone().unwrap_or_default().is_empty() {
                    if self.address.publisher.clone().unwrap_or_default().is_empty() {
                        if result.success {
                            log!("aslr: result.success: Setting button visibilities...");
                            self.publisher_input_visible = true;
                            self.check_out_button_visible = false;
                            self.final_check_out_button_visible = true;
                            self.cancel_button_visible = true;
                            self.sent_button_visible = false;
                            self.is_sent = false;
                            self.status_pills_visible = false;
                            self.address.publisher = ctx.props().current_publisher.clone();
                        } else {
                            self.publisher_input_readonly = true;  
                            self.cancel_button_visible = true;
                            self.address.check_out_started = Some("9999-12-31T23:59:59".to_string());
                        }
                    } 
                } 
                true
            },
            Msg::UpdatePublisher(value) => {
                self.address.publisher = Some(value);
                true
            },
            Msg::LetterSent() => {
                if self.address.publisher.clone().unwrap_or_default().is_empty() {
                    self.publisher_input_visible = true;
                    self.publisher_input_error = true;
                    self.publisher_input_readonly = false;
                    self.sent_button_visible = true;
                    self.is_sent = false;
                    self.status_pills_visible = true;
                } else {
                    self.publisher_input_visible = true;
                    self.publisher_input_error = false;
                    self.publisher_input_readonly = true;
                    self.sent_button_visible = false;
                    self.address.delivery_status = Some("Sent".to_string());
                    self.address.sent_date = Some("Just now".to_string());
                    self.is_sent = true;
                    self.status_pills_visible = false;
                }
                true
            },
            Msg::CheckoutFinish() => {
            
                if self.address.publisher.clone().unwrap_or_default().is_empty() {
                    self.publisher_input_visible = true;
                    self.publisher_input_error = true;
                    self.publisher_input_readonly = false;                
                    self.final_check_out_button_visible = true;
                    self.cancel_button_visible = true;
                
                } else {
                    self.publisher_input_visible = true;
                    self.publisher_input_error = false;
                    self.publisher_input_readonly = true;                
                    self.final_check_out_button_visible = false;
                    self.sent_button_visible = true;
                    self.cancel_button_visible = false;
                    //////self.address.publisher = ctx.props().address.publisher.clone();
                ////on_publisher_change.emit(address_clone.publisher.clone().unwrap_or_default());
                }
                true
            },
            Msg::CheckoutCancel() => {
                self.publisher_input_visible = true; // should always be true
                self.publisher_input_error = false;
                self.cancel_button_visible = false;
                self.final_check_out_button_visible = false;
                self.sent_button_visible = false;
                self.status_pills_visible = true;
                self.address.publisher = Some("".to_string());
                true
            }
        }
    }

    //pub fn address_shared_letter_row(props: &AddressSharedLetterRowProperties) -> Html {
    fn changed(&mut self, ctx: &Context<Self>, _old_props: &Self::Properties) -> bool {
        self.address = ctx.props().address.clone();
        
        //let publisher_is_entered =  !ctx.props().address.publisher.clone().unwrap_or_default().is_empty();
        //let is_checking_out = !ctx.props().address.check_out_started.clone().unwrap_or_default().is_empty();
        let _is_sent = !ctx.props().address.sent_date.clone().unwrap_or_default().is_empty();
        // // //let status_pills_visible = !is_sent;

        //self.publisher_input_readonly =  is_sent || publisher_is_entered || is_checking_out;
        //self.cancel_button_visible = publisher_is_entered && !is_sent;
        // // //self.status_pills_visible = status_pills_visible;
        true
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
     
        }
    }
    fn view(&self, ctx: &Context<Self>) -> Html {
    
        let _publisher_is_entered =  !self.address.publisher.clone().unwrap_or_default().is_empty();
        let _is_sent = !self.address.sent_date.clone().unwrap_or_default().is_empty();
        let _status_pills_visible = !_is_sent;

        let link = ctx.link().clone();
        let alba_address_id = self.address.alba_address_id;
        let publisher_click = {
            Callback::from(move |_event: MouseEvent| {
                link.send_future(async move {
                    Msg::PublisherClick(post_address_checkout_start(alba_address_id).await)
                });
            })
        };
        
        let link = ctx.link().clone();
        let publisher_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an Publisher HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();
                
                log!(format!("model: publisher_text_onchange: value: {}", value));

                link.send_message(Msg::UpdatePublisher(value));
            })
        };

        let link = ctx.link().clone();
        let final_check_out_click = {
            Callback::from(move |_event: MouseEvent| {
                link.send_message(Msg::CheckoutFinish());
            })
        };

        let link = ctx.link().clone();
        let sent_click = {
            Callback::from(move |_event: MouseEvent| {
                link.send_message(Msg::LetterSent());                
            })
        };

        let link = ctx.link().clone();
        let cancel_click = {
            Callback::from(move |_event: MouseEvent| {
                link.send_message(Msg::CheckoutCancel());
            })
        };

        let _edit_uri = format!(
            "/app/address-edit?alba_address_id={}",
            self.address.alba_address_id);

        let unit_text: String = match &self.address.unit {
            Some(v) => if v == "" { "".to_string() } else { format!(", {}", v.clone()) },
            None => "".to_string()
        };

        let has_publisher: bool = !self.address.publisher.clone().unwrap_or("".to_string()).is_empty();
        let publisher_style = if self.clone().publisher_input_error {
            "border-width:4px;border-color:red;color:black;"
        } else if self.clone().is_sent {
            "color:#090;border-color:#090;"
        } else if !self.address.check_out_started.clone().unwrap_or_default().is_empty() {
            "color:gray;border-color:gray;"
        } else if has_publisher { 
            "color:black;border-color:black;"
        } else { 
            "border-color:blue;color:black;" 
        };

        let is_checking_out = !self.address.check_out_started.clone().unwrap_or_default().is_empty();
        let publisher_value: Option<String> = if is_checking_out {
            Some("Unavailable...".to_string())
        } else {
            self.address.publisher.clone()
        };

        html!{
            <>
                <div class="row" style="border-top: 1px solid gray;padding-top:8px;margin-bottom:8px;">
                    <div class="col-5 col-sm-5 col-md-3 col-lg-2 col-xl-2">
                        if self.publisher_input_visible.clone() {
                            <input 
                                value={publisher_value} 
                                id={format!("publisher-for-address-id-{}", self.address.address_id)} 
                                onchange={publisher_text_onchange.clone()}
                                onclick={publisher_click.clone()}
                                type="text" 
                                style={publisher_style}
                                class="form-control shadow-sm m-1 letter-writing-shared-input" 
                                readonly={self.publisher_input_readonly.clone()}
                                placeholder="你的名字"/>
                        } 
            
                        if self.check_out_button_visible.clone() {
                            <button
                                id={format!("check-out-button-for-address-id-{}", self.address.address_id)} 
                                class="btn btn-outline-primary m-1"
                                data-address-id={self.address.address_id.to_string()} 
                                onclick={publisher_click.clone()}>
                                {"Check Out"}
                            </button>
                        }
                        if self.final_check_out_button_visible.clone() {
                            <button
                                id={format!("final-check-out-button-for-address-id-{}", self.address.address_id)} 
                                //style="display:none;" 
                                class="btn btn-primary m-1"
                                data-address-id={self.address.address_id.to_string()} 
                                onclick={final_check_out_click.clone()}>
                                {"Check Out"}
                            </button>
                        }

                        if self.sent_button_visible.clone() {
                            <button 
                                id={format!("sent-button-for-address-id-{}", self.address.address_id)} 
                                //style="border-color:#090;" 
                                class="btn btn-outline-primary m-1" 
                                data-address-id={self.address.address_id.to_string()} 
                                onclick={sent_click.clone()}>
                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-send" viewBox="0 0 16 16">
                                    <path d="M15.854.146a.5.5 0 0 1 .11.54l-5.819 14.547a.75.75 0 0 1-1.329.124l-3.178-4.995L.643 7.184a.75.75 0 0 1 .124-1.33L15.314.037a.5.5 0 0 1 .54.11ZM6.636 10.07l2.761 4.338L14.13 2.576 6.636 10.07Zm6.787-8.201L1.591 6.602l4.339 2.76 7.494-7.493Z"/>
                                </svg>
                                {" Send Letter"}
                            </button>
                        }

                        if self.cancel_button_visible.clone() {
                            <button 
                                id={format!("cancel-button-for-address-id-{}", self.address.address_id)} 
                                //style="display:none;" 
                                class="btn btn-secondary m-1" 
                                data-address-id={self.address.address_id.to_string()} 
                                onclick={cancel_click.clone()}>
                                {"Cancel"}
                            </button>
                        }
                        if self.is_sent {
                            <span class="ms-2 badge rounded-pill" style="background-color:#090;">{"Sent"}</span> 
                            <span>{" "}</span>
                            <span style="color:#090">{self.address.sent_date.clone().unwrap_or_default().chars().take(10).collect::<String>()}</span>
                        }
                    </div>
                    <div class="col-7 col-sm-6 col-md-4 col-lg-4 col-xl-3">
                        <span style="font-weight:bold;">{self.address.name.clone()}</span>
                        <br/>
                        {self.address.street.clone().unwrap_or("".to_string())}
                        {unit_text}
                        <br/>
                        {self.address.city.clone()}
                        {", "}
                        {self.address.state.clone()}
                        {"  "}
                        {self.address.postal_code.clone()}
                    </div> 
                                        
                    <div class="col-12 col-sm-12 col-md-3 col-lg-2 col-xl-2">
                        
                        if self.status_pills_visible {
                            if self.address.status.clone() == Some("Valid".to_string()) {
                                <span class="ms-2 badge rounded-pill bg-secondary">{""}{self.address.language.clone()}</span> 
                            }
                            //<br/>
                            if self.address.status.clone() == Some("New".to_string()) {
                            } else if self.address.status.clone() == Some("Valid".to_string()) {
                            } else if self.address.status.clone() == Some("Do not call".to_string()) {
                                <span class="ms-2 badge rounded-pill bg-danger">{self.address.status.clone()}</span> 
                            } else if self.address.status.clone() == Some("Moved".to_string()) {
                                <span class="ms-2 badge rounded-pill bg-warning">{self.address.status.clone()}</span> 
                            } 
                            else {
                                <span class="ms-2 badge rounded-pill bg-dark">{self.address.status.clone()}</span> 
                            }
                            // <br/>
                            if self.address.delivery_status.clone() == Some("None".to_string()) {
                            
                            } else if self.address.delivery_status.clone() == Some("Assigned".to_string()) {
                                <span class="ms-2 badge rounded-pill bg-info">{self.address.delivery_status.clone()}</span> 
                            } else if self.address.delivery_status.clone() == Some("Sent".to_string()) {
                            // <span class="ms-2 badge rounded-pill bg-success">{address.delivery_status.clone()}</span> 
                            } else if self.address.delivery_status.clone() == Some("Returned".to_string()) {
                                <span class="ms-2 badge rounded-pill bg-warning">{self.address.delivery_status.clone()}</span> 
                            } else if self.address.delivery_status.clone() == Some("Undeliverable".to_string()) {
                                <span class="ms-2 badge rounded-pill bg-warning">{self.address.delivery_status.clone()}</span> 
                            } else {
                                    <span class="ms-2 badge rounded-pill bg-dark">{self.address.delivery_status.clone()}</span> 
                            }     
                        }
                    </div>
                </div>
            </>
        }
    }
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

