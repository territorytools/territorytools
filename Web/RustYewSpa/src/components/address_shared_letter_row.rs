use crate::components::address_shared_letter_functions::*;
use crate::components::address_shared_letter_page::LetterQuery;
use serde::{Deserialize, Serialize};

// #[cfg(debug_assertions)]
// const DATA_API_PATH: &str = "/data/addresses_search.json";

// #[cfg(not(debug_assertions))]
// const DATA_API_PATH: &str = "/api/addresses/search";

use crate::models::addresses::Address;
use gloo_console::log;
use wasm_bindgen::JsCast;
use web_sys::{
    HtmlInputElement,
};
use yew::prelude::*;
use yew_router::scope_ext::RouterScopeExt;
use yew_router::history::HistoryError;

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
    HoldForCheckout(CheckoutStartResult),
    UpdatePublisher(String),
    LetterSent(LetterSentResult),
    CheckoutFinish(CheckoutFinishResult),
    CheckoutCancel(),
}


#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    pub address: SharedLetterAddress,
    pub on_publisher_change: Callback<String>,
    pub current_publisher: Option<String>,
    pub session_id: String,
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
    pub session_id: Option<String>,
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
    pub session_id: String,
    pub me_checking_out: bool,
    pub pending_publisher: Option<String>,
    pub query: LetterQuery
}

impl Component for AddressSharedLetterRow {
    type Message = Msg;
    type Properties = Props;

    fn create(ctx: &Context<Self>) -> Self {
        let address = ctx.props().address.clone();
        let me_checking_out = address.session_id == Some(ctx.props().session_id.clone()) || address.session_id == Some("session-1".to_string());
        //if me_checking_out {
        //    log!(format!("aslr:create: me_checking_out: {} aaid: {}, name: {}, street: {}",
        //        me_checking_out, address.alba_address_id, address.name.clone().unwrap_or_default(), address.street.clone().unwrap_or_default()));
        //}

        let location = ctx.link().location().expect("Location or URI");
        let query: LetterQuery = location.query().unwrap_or(LetterQuery::default());

        return Self {
            address: ctx.props().address.clone(),
            check_out_button_visible: false,
            publisher_input_visible: true, 
            publisher_input_error: false,
            publisher_input_readonly: false,
            final_check_out_button_visible: false,
            sent_button_visible: false,
            cancel_button_visible: false,
            is_sent: false,
            status_pills_visible: true,
            address_visible: true,
            session_id: ctx.props().session_id.clone(),
            me_checking_out: me_checking_out,
            pending_publisher: None,
            query: query,
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::HoldForCheckout(result) => {
                //log!(format!("aslp:update: HoldForCheckout: post result: aid: {}, success: {}", result.alba_address_id, result.success));
                // if result.success {
                    self.address.check_out_started = result.checkout_start_utc;
                    self.address.session_id = result.session_id;                
                //}
                true
            },
            Msg::UpdatePublisher(value) => {
                //log!(format!("aslr:update:UpdatePublisher:publisher: {}", value));
                //self.address.publisher = Some(value);
                          
                log!(format!("row:UpdatePublisher:value: {}", value.clone()));
                ctx.props().on_publisher_change.emit(value.clone());

                self.pending_publisher = Some(value);
                true
            },
            Msg::CheckoutCancel() => {
                //log!(format!("aslr:update:CheckoutCancel:publisher: {}", self.address.publisher.clone().unwrap_or_default()));
                self.address.check_out_started = None;
                self.address.session_id = None;
                self.address.publisher = None;
                self.pending_publisher = None;
                //log!(format!("aslr:update:CheckoutCancel:pending_publisher: {}", self.pending_publisher.clone().unwrap_or_default()));
                true
            }
            Msg::CheckoutFinish(result) => {
                //log!(format!("aslr:update:CheckoutFinish:publisher: {}", result.publisher.clone().unwrap_or_default()));
                
                // log!(format!("row:CheckoutFinish:address:publisher: {}", self.address.publisher.clone().unwrap_or_default()));
                // ctx.props().on_publisher_change.emit(result.publisher.clone().unwrap_or_default());

                if result.success {
                    self.address.publisher = result.publisher.clone();
                    //log!(format!("row:CheckoutFinish:result:publisher: {}", result.publisher.clone().unwrap_or_default()));
                    //ctx.props().on_publisher_change.emit(result.publisher.clone().unwrap_or_default());
                }

                true
            },
            Msg::LetterSent(result) => {
                if result.success {
                    self.address.sent_date = result.letter_sent_utc;
                    self.address.publisher = result.publisher;
                }
                true
            },
        }
    }

    fn changed(&mut self, ctx: &Context<Self>, _old_props: &Self::Properties) -> bool {
        self.address = ctx.props().address.clone();
        true
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
     
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let alba_address_id = self.address.alba_address_id;
        let session_id = self.session_id.clone();
        let is_sent = !self.address.sent_date.clone().unwrap_or_default().is_empty();
        let is_checking_out = !self.address.check_out_started.clone().unwrap_or_default().is_empty();
        let is_checked_out = !self.address.publisher.clone().unwrap_or_default().is_empty();
        let has_publisher = !self.address.publisher.clone().unwrap_or_default().is_empty();
        let already_mine = is_checking_out && self.address.session_id.clone().unwrap_or_default() == session_id
            && !session_id.is_empty()
            || self.address.session_id == Some("session-1".to_string());

        let others_checking_out = !already_mine && is_checking_out;
        //let me_checking_out = self.me_checking_out;
        // let is_checked_out = !self.address.publisher.clone().unwrap_or("".to_string()).is_empty();
        // let is_sent = !self.address.sent_date.clone().unwrap_or_default().is_empty();

        // let link = ctx.link().clone();
        // let session_id_clone = session_id.clone();
        // let publisher_click = {
        //     Callback::from(move |_: MouseEvent| {
        //         let session_id_clone = session_id_clone.clone();
        //         if !already_mine && !is_sent && !is_checked_out && !is_checking_out {
        //             link.send_future(async move {
        //                 Msg::HoldForCheckout(post_address_checkout_start(alba_address_id, session_id_clone).await)
        //             });
        //         }
        //     })
        // };

        //let link = ctx.link().clone();
        //let session_id_clone = session_id.clone();
        // let publisher_onfocus = {
        //     Callback::from(move |_: FocusEvent| {
        //         //let session_id_clone = session_id_clone.clone();
        //         // if !already_mine && !is_sent && !is_checked_out && !is_checking_out {
        //         //     link.send_future(async move {
        //         //         Msg::HoldForCheckout(post_address_checkout_start(alba_address_id, session_id_clone).await)
        //         //     });
        //         // }
        //     })
        // };
        
        let link = ctx.link().clone();
        let session_id_clone = session_id.clone();
        let mtk = self.query.mtk.clone().unwrap_or_default();
        let publisher_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an Publisher HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();
                
                event
                    .target()
                    .expect("An input value for an Publisher HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .set_value("");

                link.send_message(Msg::UpdatePublisher(value.clone()));

                let session_id_clone = session_id_clone.clone();
                let mtk = mtk.clone();
                if value.len() > 0 {
                    link.send_future(async move {
                        Msg::HoldForCheckout(post_address_checkout_start(alba_address_id, session_id_clone, mtk).await)
                    });
                }
            })
        };

        let link = ctx.link().clone();
        let session_id_clone = session_id.clone();
        let mtk = self.query.mtk.clone().unwrap_or_default();
        let publisher_onkeypress = {
            Callback::from(move |event: KeyboardEvent| {
                let value = event
                    .target()
                    .expect("An input value for an Publisher HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();
                
                log!(format!("aslr:publisher_onkeypress:value: {}", value.clone()));
                // event
                //     .target()
                //     .expect("An input value for an Publisher HtmlInputElement")
                //     .unchecked_into::<HtmlInputElement>()
                //     .set_value("");

                // removed for queries // link.send_message(Msg::UpdatePublisher(value.clone()));

                let session_id_clone = session_id_clone.clone();
                let mtk = mtk.clone();
                //if value.len() > 0 {
                    link.send_future(async move {
                        Msg::HoldForCheckout(post_address_checkout_start(alba_address_id, session_id_clone, mtk).await)
                    });
                //}
            })
        };

        let link = ctx.link().clone();
        let alba_address_id = self.address.alba_address_id.clone();
        //let publisher = self.address.publisher.clone().unwrap_or_default();
        let pending_publisher = self.pending_publisher.clone().unwrap_or_default();
        let mtk = self.query.mtk.clone().unwrap_or_default();
        let final_check_out_click = {
            let pending_publisher = pending_publisher.clone();
            Callback::from(move |_: MouseEvent| {
                let pending_publisher = pending_publisher.clone();
                let mtk = mtk.clone();
                link.send_future(async move {
                    Msg::CheckoutFinish(post_address_checkout_finish(alba_address_id, pending_publisher, mtk).await)
                });
            })
        };
        
        let link = ctx.link().clone();
        let cancel_click = {
            Callback::from(move |_: MouseEvent| {
                link.send_message(Msg::CheckoutCancel());
            })
        };

        let link = ctx.link().clone();
        let alba_address_id = self.address.alba_address_id.clone();
        let publisher = self.address.publisher.clone().unwrap_or_default();
        let mtk = self.query.mtk.clone().unwrap_or_default();
        let sent_click = {
            let publisher = publisher.clone();
            Callback::from(move |_: MouseEvent| {
                let publisher = publisher.clone();
                let mtk = mtk.clone();
                link.send_future(async move {
                    Msg::LetterSent(post_address_leter_sent(alba_address_id, publisher, mtk).await)
                });
            })
        };

        let unit_text: String = match &self.address.unit {
            Some(v) => if v == "" { "".to_string() } else { format!(", {}", v.clone()) },
            None => "".to_string()
        };

        let publisher_style = if self.clone().publisher_input_error {
            "border-width:4px;border-color:red;color:black;"
        } else if is_sent {
            "color:#090;border-color:#090;"
        } else if is_checked_out { 
            "color:black;border-color:black;"
        } else if already_mine && is_checking_out {
            "color:blue;border-color:blue;" // TODO: change this color
        // } else if others_checking_out {
        //     "color:magenta;border-color:magenta;"
        } else { 
            "border-color:blue;color:black;" 
        };

        // let publisher_value: Option<String> = 
        //     if already_mine && is_checking_out && !others_checking_out && !is_sent && !is_checked_out {
        //         self.pending_publisher.clone()
        //     } else if is_sent {
        //         self.address.publisher.clone()
        //     } else if is_checked_out {
        //         self.address.publisher.clone()
        //     // } else if others_checking_out {
        //     //     //Some("Unavailable...".to_string())
        //     //     self.address.publisher.clone()
        //     } else {
        //         self.address.publisher.clone()
        //     };
        
        let publisher_readonly = others_checking_out || is_checked_out || is_sent;
        let checkout_button_visible = !is_sent && !is_checked_out && already_mine && is_checking_out;
        let send_button_visible = !is_sent && is_checked_out;

        html!{
            <>
                <div class="row" style="border-top: 1px solid gray;padding-top:8px;margin-bottom:8px;">
                    <div class="col-5 col-sm-5 col-md-3 col-lg-2 col-xl-2">
                        if others_checking_out && !has_publisher {
                            <span 
                                style="color:magenta;border-color:magenta;"
                                class="form-control shadow-sm m-1 letter-writing-shared-input">
                                {"Not available"}
                            </span>
                        } else {
                            <input 
                                value={self.address.publisher.clone()} 
                                //id={format!("publisher-for-address-id-{}", self.address.address_id)} 
                                onchange={publisher_text_onchange.clone()}
                                // TODO: onleave ? (for tab keys and enter)
                                //onclick={publisher_click.clone()}
                                //onfocus={publisher_onfocus.clone()}
                                onkeypress={publisher_onkeypress.clone()}
                                type="text" 
                                style={publisher_style}
                                class="form-control shadow-sm m-1 letter-writing-shared-input" 
                                readonly={publisher_readonly}
                                placeholder="你的名字"/>
                        } 
            
                        // if checkout_button_visible {
                        //     <button
                        //         id={format!("check-out-button-for-address-id-{}", self.address.address_id)} 
                        //         class="btn btn-outline-primary m-1"
                        //         data-address-id={self.address.address_id.to_string()} 
                        //         onclick={publisher_click.clone()}>
                        //         {"Check Out"}
                        //     </button>
                        // }

                        if checkout_button_visible {
                            <button
                                id={format!("final-check-out-button-for-address-id-{}", self.address.address_id)} 
                                //style="display:none;" 
                                class="btn btn-primary m-1"
                                data-address-id={self.address.address_id.to_string()} 
                                onclick={final_check_out_click.clone()}>
                                {"Check Out"}
                            </button>
                            <button 
                                id={format!("cancel-button-for-address-id-{}", self.address.address_id)} 
                                //style="display:none;" 
                                class="btn btn-secondary m-1" 
                                data-address-id={self.address.address_id.to_string()} 
                                onclick={cancel_click.clone()}>
                                {"Cancel"}
                            </button>
                        }

                        //if self.sent_button_visible.clone() {
                        if send_button_visible {
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

                        // if self.is_sent was probably better
                        if is_sent {
                            <span class="ms-2 badge rounded-pill" style="background-color:#090;">{"Sent"}</span> 
                            <span>{" "}</span>
                            <span style="color:#090">{self.address.sent_date.clone().unwrap_or_default().chars().take(10).collect::<String>()}</span>
                        }
                    </div>
                    if self.query.debug.unwrap_or_default() {
                        <div class="col-5 col-sm-5 col-md-3 col-lg-2 col-xl-2">
                            {"error:"}{self.clone().publisher_input_error}
                            {",sid:"}{self.session_id.clone()}<br/>
                            {",held:"}{is_checking_out}
                            {",mine:"}{already_mine}<br/>
                            {",others:"}{others_checking_out}
                            {",out:"}{is_checked_out}<br/>
                            {",sent:"}{is_sent}<br/>
                        </div>
                    } else {
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
                    }            
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

// #[derive(Properties, PartialEq, Clone, Default, Serialize)]
// pub struct AddressSharedLetterRowModel {
//     pub address: SharedLetterAddress,
//     pub check_out_button_visible: bool,
//     pub publisher_input_visible: bool,
//     pub publisher_input_error: bool,
//     pub publisher_input_readonly: bool,
//     pub final_check_out_button_visible: bool,
//     pub sent_button_visible: bool,
//     pub cancel_button_visible: bool,
//     pub is_sent: bool,
//     pub status_pills_visible: bool,
//     pub address_visible: bool,
//     //pub current_publisher: Option<String>,
// }

