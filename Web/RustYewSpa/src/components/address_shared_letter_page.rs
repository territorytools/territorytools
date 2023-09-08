use crate::components::address_shared_letter_functions::*;
use crate::components::address_shared_letter_row::AddressSharedLetterRow;
use crate::components::address_shared_letter_row::SharedLetterAddress;
use serde::{Deserialize, Serialize};

use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::addresses::Address;
use gloo_console::log;
use gloo::timers::callback::{Interval};
use rand::Rng;
use yew::prelude::*;

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
    //Search(String),
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

//#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressSharedLetter {
    result: AddressSharedLetterResult,
    current_publisher: Option<String>,
    // search: String,
    _timer: Interval,
    session_id: String,
}

impl Component for AddressSharedLetter {
    type Message = Msg;
    type Properties = Props;

    fn create(ctx: &Context<Self>) -> Self {
        let mut rng = rand::thread_rng();
        let session_id: u32 = rng.gen();
        let session_id = format!("{}", session_id);
        log!(format!("session_id: {session_id}"));
        
        let session_id_clone = session_id.clone();
        ctx.link().send_future(async move {
            Msg::Load(fetch_shared_letter_addresses(session_id_clone).await)
        });

        let session_id_clone = session_id.clone();
        let link_clone = ctx.link().clone();
        let standalone_handle = Interval::new(15000, move ||
        { 
            let session_id_clone = session_id_clone.clone();
            link_clone.send_future(async move {
                Msg::Load(fetch_shared_letter_addresses(session_id_clone).await)
            });
        });
        
        let session_id_clone = session_id.clone();
        AddressSharedLetter {
            result: AddressSharedLetterResult {
                count: 0,
                addresses: vec![],
            },
            current_publisher: None,
            _timer: standalone_handle,
            session_id: session_id_clone,
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.result = result.clone();
                log!("ASLP:update: Msg::Load: loading...");
                true
            },
            // Msg::Search(_text) => {

            // },
            Msg::SetCurrentPublisher(publisher) => {
                self.current_publisher = Some(publisher.clone());
                true
            }
        }
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
                                    session_id={self.session_id.clone()}
                                    on_publisher_change={publisher_change.clone()}
                                    />                                  
                            }
                    }).collect::<Html>()
                }                
                </div>
            </>
        }
    }
}
