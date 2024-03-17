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
                <table class="table">
                    <thead>
                    <tr>
                        <th scope="col"><strong>{"Number"}</strong></th>
                        <th scope="col"><strong>{"Publisher"}</strong></th>
                        <th scope="col"><strong>{"Signed Out"}</strong></th>
                        <th scope="col"><strong>{"Completed"}</strong></th>
                    </tr>
                    </thead>
                    <tbody>
                    {
                        self.territories.iter().map(|territory| {  
                            let territory_uri = format!("/app/territory-edit?id={}",territory.territory_id);
                            html! {
                                <tr>
                                    <th scope="row">
                                        <a href={territory_uri} style="text-decoration:none;color:blue;">
                                             {territory.number.clone()}
                                             </a>
                                         </th>
                                         <td >
                                             {territory.publisher.clone()}
                                         </td>
                                         <td>
                                            {territory.signed_out.clone()}
                                        </td>
                                        <td>
                                            {territory.completed.clone()}
                                        </td>
                                    </tr>
                           }
                         }).collect::<Html>()
                    }
                    </tbody>
                </table>
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
