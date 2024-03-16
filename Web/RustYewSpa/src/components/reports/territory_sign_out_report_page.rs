// Uncomment for debugging without an API server
//const DATA_API_PATH: &str = "/data/territory-stage-reports-monthly-completion.json";

const DATA_API_PATH: &str = "/api/territory-stage-reports/sign-out";

use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::components::reports::sign_out_summary::SignOutSummary;

use gloo_console::log;
use reqwasm::http::Request;
use yew::prelude::*;
use yew_router::prelude::LocationHandle;
use yew_router::scope_ext::RouterScopeExt;

pub enum Msg {
    Load(SignOutResult),
    Refresh(),
}

pub struct TerritorySignOutReportPage {
    _listener: LocationHandle,
    territories: Vec<SignOutSummary>,
    _result: SignOutResult,
}

impl Component for TerritorySignOutReportPage {
    type Message = Msg;
    type Properties = ();
    
    fn create(ctx: &Context<Self>) -> Self {
        let link = ctx.link().clone();      
        let listener = ctx.link()
            .add_location_listener(
                Callback::from(move |_| {
                    link.send_message(Msg::Refresh());
                })
            )
            .unwrap();

        Self {
            _listener: listener,
            territories: vec![],
            _result: SignOutResult::default(),
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.territories = result.territories.clone();
                true
            },
            Msg::Refresh() => {
                ctx.link().send_future(async move {
                    Msg::Load(get_report().await)
                });
                false
            }
        }
    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
        //set_document_title("Territory Search");
       
        let _count = self.territories.len();
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
                    <span><strong>{"Territory Reports - Sign Out Summary"}</strong></span>
                    <div class="row py-1" style="border-top: 1px solid gray;">
                        <div class="col-2 col-md-1"><strong>{"Number"}</strong></div>
                        <div class="col-4 col-md-2"><strong>{"Publisher"}</strong></div>
                        <div class="col-2 col-md-2"><strong>{"Signed Out"}</strong></div>
                        <div class="col-2 col-md-2"><strong>{"Completed"}</strong></div>
                    </div>
                    {
                        self.territories.iter().map(|territory| {   
                            html! {
                                 <a style="text-decoration:none;color:black;">
                                     <div class="row py-1" style="border-top: 1px solid lightgray;">
                                         <div class="col-2 col-md-1" style="font-weight:bold;">
                                             {territory.number.clone()}
                                         </div>
                                         <div class="col-4 col-md-2">
                                             {territory.publisher.clone()}
                                         </div>
                                         <div class="col-2 col-md-2">
                                            {territory.signed_out.clone()}
                                        </div>
                                        <div class="col-2 col-md-2">
                                            {territory.completed.clone()}
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

#[derive(Properties, PartialEq, Clone, Default)]
pub struct SignOutResult {
    pub success: bool,
    pub count: i32,
    pub territories: Vec<SignOutSummary>,
    pub load_error: bool,
    pub load_error_message: String,
}

async fn get_report() -> SignOutResult {
    let uri_string: String = format!("{path}", path = DATA_API_PATH);
    let uri: &str = uri_string.as_str();
    let resp = Request::get(uri)
        .header("Content-Type", "application/json")
        .send()
        .await
        .expect("A result from the reports endpoint");
    
    log!(format!("load territory report from result code: {}", resp.status().to_string()));

    let report_result: Vec<SignOutSummary> = if resp.status() == 200 {
        resp
        .json()
        .await
        .expect("Valid territory report result in JSON format")
    } else {
        vec![]
    };
    
    let result = SignOutResult {
        success: (resp.status() == 200),
        count: report_result.len() as i32,
        territories: report_result,
        load_error: resp.status() != 200,
        load_error_message: if resp.status() == 401 {
                "Unauthorized".to_string()
            } else if resp.status() == 403 {
                "Forbidden".to_string()
            } else {
                format!("Error {:?}", resp.status())
            }
    };

    result
}
