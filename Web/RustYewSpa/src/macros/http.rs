use reqwasm::http::Response;
use yew::prelude::*;
use serde::{Deserialize,Serialize};

#[macro_export]
macro_rules! http_get_set {
    ($state:ident.$($field_path:ident).+, $uri:ident) => {{
        let state = $state.clone();
        let uri = $uri.clone();
        spawn_local(async move {
            let response = Request::get(uri.as_str())
                .send()
                .await
                .expect("Response (raw) from API");

            let status = $crate::macros::http::load_status(&response);
            
            let mut modification = state.deref().clone();
            modification.$($field_path).+ = response.json().await.unwrap_or_default();
            modification.load_status = status.clone();
            
            state.set(modification);
        });
    }};
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct LoadStatus {
    pub load_error: bool,
    pub error_message: String,
}

pub fn load_status(response: &Response) -> LoadStatus {
    if response.status() == 200 {
        //$crate::macros::http::GetResult::default()
        LoadStatus::default()
    } else if (401..403).contains(&response.status()) {
        LoadStatus {
            load_error: true,
            error_message: "Unauthorized".to_string(),
        }
    } else {
        LoadStatus {
            load_error: true,
            error_message: format!("Error: {}", response.status()),
        }
    }
}
