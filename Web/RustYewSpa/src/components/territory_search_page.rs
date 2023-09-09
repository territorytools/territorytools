#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-list.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/list";

use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::territories::TerritorySummary;
use crate::functions::document_functions::set_document_title;
use crate::Route;

use gloo_console::log;
use std::ops::Deref;
use reqwasm::http::{Request};
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use yew_router::hooks::use_location;
use yew_router::prelude::use_navigator;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use serde::{Serialize, Deserialize};

#[derive(Properties, PartialEq, Clone, Default)]
pub struct Props {
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct TerritorySearchPage {
    pub search_text: String,
}

pub enum Msg {
    Search(String),
}

impl Component for TerritorySearchPage {
    type Message = Msg;
    type Properties = Props;
    fn create(_ctx: &Context<Self>) -> Self {
        return TerritorySearchPage::default()
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Search(text) => {
                true
            }
        }
    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
        //set_document_title("Territory Search");
        html!{
            <div></div>
        }
    }
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct TerritorySearchResult {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub territories: Vec<TerritorySummary>,
    pub load_error: bool,
    pub load_error_message: String,
}

async fn get_territories(search_text: String) -> TerritorySearchResult {
    let uri_string: String = format!("{path}?filter={search_text}", path = DATA_API_PATH);
    let uri: &str = uri_string.as_str();
    let resp = Request::get(uri)
        .header("Content-Type", "application/json")
        .send()
        .await
        .expect("A result from the /api/territories/list endpoint");
    
    log!(format!("load territory from search result code: {}", resp.status().to_string()));

    let territory_result: Vec<TerritorySummary> = if resp.status() == 200 {
        resp
        .json()
        .await
        .expect("Valid territory search result in JSON format")
    } else {
        vec![]
    };
    
    let result = TerritorySearchResult {
        success: (resp.status() == 200),
        count: territory_result.len() as i32,
        territories: territory_result,
        search_text: "".to_string(),
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

#[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
pub struct TerritorySearchQuery {
    pub search_text: Option<String>,
}