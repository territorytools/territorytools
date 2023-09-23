#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-stage-reports-monthly-completion.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territory-stage-reports/monthly-completion";

use crate::{components::menu_bar_v2::MenuBarV2, models::territory_reports::MonthCompletionSummary};
use crate::components::menu_bar::MapPageLink;
use crate::models::territories::TerritorySummary;
use crate::Route;

use gloo_console::log;
use reqwasm::http::Request;
use yew::prelude::*;
use yew_router::prelude::LocationHandle;
use yew_router::scope_ext::RouterScopeExt;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use serde::{Serialize, Deserialize};

pub enum Msg {
    Load(TerritoryStageReportResult),
    Refresh(),
}

pub struct TerritoryStageReportPage {
    _listener: LocationHandle,
    months: Vec<MonthCompletionSummary>,
    result: TerritoryStageReportResult,
}

impl Component for TerritoryStageReportPage {
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

        return Self {
            _listener: listener,
            months: vec![],
            result: TerritoryStageReportResult::default(),
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.months = result.months.clone();
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

    fn view(&self, ctx: &Context<Self>) -> Html {
        //set_document_title("Territory Search");
       
        let count = self.months.len();
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
                    <span><strong>{"Territory Stage Report - Monthly Completion"}</strong></span>
        
                    <hr/>
                    <div class="row py-1" style="border-top: 1px solid gray;">
                        <div class="col-2 col-md-1"><strong>{"Month"}</strong></div>
                        <div class="col-6 col-md-3"><strong>{"Visiting Done"}</strong></div>
                        <div class="col-4 col-md-2"><strong>{"Done"}</strong></div>
                        <div class="col-8 col-md-3"><strong>{"Cooling Off"}</strong></div>
                        <div class="col-4 col-md-2"><strong>{"Total"}</strong></div>
                    </div>
                    {
                        self.months.iter().map(|month| {   
                            html! {
                                 <a style="text-decoration:none;color:black;">
                                     <div class="row py-1" style="border-top: 1px solid lightgray;">
                                         <div class="col-2 col-md-1" style="font-weight:bold;">
                                             {month.month.clone()}
                                         </div>
                                         <div class="col-6 col-md-3">
                                             {month.visiting_done}
                                         </div>
                                         <div class="col-4 col-md-2">
                                            {month.done}
                                        </div>
                                        <div class="col-8 col-md-3">
                                            {month.cooling_off}
                                        </div>
                                        <div class="col-4 col-md-2">
                                            {month.total}
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
pub struct TerritoryStageReportResult {
    pub success: bool,
    pub count: i32,
    pub months: Vec<MonthCompletionSummary>,
    pub load_error: bool,
    pub load_error_message: String,
}

async fn get_report() -> TerritoryStageReportResult {
    let uri_string: String = format!("{path}", path = DATA_API_PATH);
    let uri: &str = uri_string.as_str();
    let resp = Request::get(uri)
        .header("Content-Type", "application/json")
        .send()
        .await
        .expect("A result from the reports endpoint");
    
    log!(format!("load territory report from result code: {}", resp.status().to_string()));

    let report_result: Vec<MonthCompletionSummary> = if resp.status() == 200 {
        resp
        .json()
        .await
        .expect("Valid territory report result in JSON format")
    } else {
        vec![]
    };
    
    let result = TerritoryStageReportResult {
        success: (resp.status() == 200),
        count: report_result.len() as i32,
        months: report_result,
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

// #[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
// pub struct TerritorySearchQuery {
//     pub search_text: Option<String>,
// }

// pub trait SearchQuery {
//     fn search_query(&self) -> TerritorySearchQuery;
// }

// impl SearchQuery for &Context<TerritorySearchPage> {
//     fn search_query(&self) -> TerritorySearchQuery {
//         let location = self.link().location().expect("Location or URI");
//         location.query().unwrap_or(TerritorySearchQuery::default())    
//     }
// }
