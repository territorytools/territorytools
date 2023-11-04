use yew::prelude::*;
use serde::Deserialize;

#[macro_export]
macro_rules! http_get_set {
    ($state:ident.$($field_path:ident).+, $uri:ident) => {{
        let state = $state.clone();
        let uri = $uri.clone();
        wasm_bindgen_futures::spawn_local(async move {
            let response = Request::get(uri.as_str())
                .send()
                .await
                .expect("Response (raw) from API");
            //let status = response_status(&response);
            let status = if response.status() == 200 {
                $crate::macros::http::GetResult::default()
            } else if response.status() == 401 {
                $crate::macros::http::GetResult {
                    load_error: true,
                    error_message: "Unauthorized".to_string(),
                }
            } else {
                $crate::macros::http::GetResult {
                    load_error: true,
                    error_message: format!("Error: {}", response.status()),
                }
            };
            
            let mut modification = state.deref().clone();
            modification.$($field_path).+ = response.json().await.expect("Valid JSON");
            modification.error_message = status.error_message;
            modification.load_error = status.load_error;
            
            state.set(modification);
        });
    }};
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct GetResult {
    // pub success: bool,
    // pub errors: Option<String>,
    // pub status: u16,
    // pub completed: bool,
    pub load_error: bool,
    pub error_message: String,
}