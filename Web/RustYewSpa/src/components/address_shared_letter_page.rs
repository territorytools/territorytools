use crate::components::address_shared_letter_functions::*;
use crate::components::address_shared_letter_row::AddressSharedLetterRow;
use crate::components::address_shared_letter_row::SharedLetterAddress;
use serde::{Deserialize, Serialize};
use crate::Route;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::addresses::Address;
use gloo_console::log;
use gloo::timers::callback::{Interval};
use rand::Rng;
use yew::prelude::*;
//use yew_router::prelude::LocationHandle;
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
    session_id: String,
    _timer: Interval,
    //_listener: LocationHandle,
    query: LetterQuery,
}

impl Component for AddressSharedLetter {
    type Message = Msg;
    type Properties = Props;

    fn create(ctx: &Context<Self>) -> Self {
        let mut rng = rand::thread_rng();
        let session_id: u32 = rng.gen();
        let session_id = format!("{}", session_id);
        log!(format!("session_id: {session_id}"));
        
        // let listener = ctx.link()
        // .add_location_listener(ctx.link().callback(
        //     // handle event
        // ))
        // .unwrap();

        let location = ctx.link().location().expect("Location or URI");
        let query = location.query().unwrap_or(LetterQuery::default());

        let session_id_clone = session_id.clone();
        let mtk = query.mtk.clone().unwrap_or_default();
        ctx.link().send_future(async move {
            Msg::Load(fetch_shared_letter_addresses(session_id_clone, mtk).await)
        });

        let session_id_clone = session_id.clone();
        let link_clone = ctx.link().clone();
        let mtk = query.mtk.clone().unwrap_or_default();
        let standalone_handle = Interval::new(15000, move ||
        { 
            let session_id_clone = session_id_clone.clone();
            let mtk = mtk.clone();
            link_clone.send_future(async move {
                Msg::Load(fetch_shared_letter_addresses(session_id_clone, mtk).await)
            });
        });
        
        let session_id_clone = session_id.clone();
        AddressSharedLetter {
            result: AddressSharedLetterResult {
                count: 0,
                addresses: vec![],
            },
            current_publisher: query.current_publisher.clone(),
            session_id: session_id_clone,
            _timer: standalone_handle,
           // _listener: listener,
            query: query,
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.result = result.clone();
                true
            },
            Msg::SetCurrentPublisher(publisher) => {
                self.current_publisher = Some(publisher.clone());
                

                let location = ctx.link().location().expect("Location or URI");
                let mut query = location.query().unwrap_or(LetterQuery::default());
                query.current_publisher = Some(publisher);
                    
                let navigator = ctx.link().navigator().unwrap();
                let _ = navigator.replace_with_query(&Route::AddressSharedLetter, &query);

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
        
        // let navigator = ctx.link().navigator().unwrap();
        // let query = LetterQuery { 
        //     current_publisher: Some("joe publisher".to_string()), //self.current_publisher.clone(), 
        //     ..Default::default() };

        // navigator.replace_with_query(&Route::AddressSharedLetter, &query);

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
                            if self.query.debug.unwrap_or_default() {
                                <span>{", Publisher: "}{self.current_publisher.clone()}</span>
                                <span>{", Session ID: "}{self.session_id.clone()}</span>
                            }
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
#[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
pub struct LetterQuery {
    pub campaign_name: Option<String>,
    pub current_publisher: Option<String>,
    pub mtk: Option<String>,
    pub debug: Option<bool>,
}