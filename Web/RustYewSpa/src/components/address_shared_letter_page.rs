use crate::components::address_shared_letter_functions::*;
use crate::components::address_shared_letter_row::AddressSharedLetterRow;
use crate::components::address_shared_letter_row::SharedLetterAddress;
use serde::{Deserialize, Serialize};

use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::addresses::Address;
use crate::functions::document_functions::set_document_title;
use gloo_console::log;
use gloo_utils::document;
use std::ops::Deref;
use wasm_bindgen_futures::spawn_local;
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
    Load(AddressSharedLetterResult),
    Search(String),
    SetCurrentPublisher(String),
}


#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    //pub path: Option<String>,
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
                count: 0,
                addresses: vec![],
            },
            current_publisher: None,
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.result = result.clone();
                return true;
            },
            Msg::Search(text) => {

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
        let publisher_change =
            Callback::from(move |publisher_name: String| {
                link.send_message(Msg::SetCurrentPublisher(publisher_name.clone()));
            });

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
                    <div class="row">
                        <div class="col">
                            <span>{"Addresses: "}{self.result.addresses.len()}</span>
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
