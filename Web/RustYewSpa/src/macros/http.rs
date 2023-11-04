use reqwasm::http::Response;
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

            let status = $crate::macros::http::response_status(&response);
            let mut modification = state.deref().clone();
            modification.$($field_path).+ = response.json().await.expect("Valid JSON");
            modification.error_message = status.error_message;
            modification.load_error = status.load_error;
            
            state.set(modification);
        });
    }};
}

#[macro_export]
macro_rules! http_put {
    ($state:ident.$($field_path:ident).+, $uri:ident) => {{
        Callback::from(move |_: i32| { 
            let state = $state.clone();
            let uri = $uri.clone();
            spawn_local(async move {
                let uri: &str = uri.as_str();
                let model = state.$($field_path).+.clone();
                let model_serialized = serde_json::to_string_pretty(&model)
                    .expect("Cannot serialize model into JSON");
                let method = Method::PUT;
                let resp = Request::new(uri)
                    .method(method)
                    .header("Content-Type", "application/json")
                    .body(model_serialized)
                    .send()
                    .await
                    .expect("A result from the endpoint");        
                
                let mut modified = state.deref().clone();

                if resp.status() == 200 {
                    modified.save_success = true;
                    modified.save_error = false;
                } else {
                    let errors = if (401..403).contains(&resp.status()) { 
                        Some("Unauthorized".to_string()) 
                    } else {
                        Some(resp.status().to_string())
                    };
                    modified.error_message = errors.unwrap_or_default();
                    modified.save_error = true;
                    modified.save_success = false;
                };

                state.set(modified);
            });
        })
    }}
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ResponseStatus {
    // pub success: bool,
    // pub errors: Option<String>,
    // pub status: u16,
    // pub completed: bool,
    pub load_error: bool,
    pub error_message: String,
}

pub fn response_status(response: &Response) -> ResponseStatus {
    if response.status() == 200 {
        //$crate::macros::http::GetResult::default()
        ResponseStatus::default()
    } else if response.status() == 401 {
        ResponseStatus {
            load_error: true,
            error_message: "Unauthorized".to_string(),
        }
    } else {
        ResponseStatus {
            load_error: true,
            error_message: format!("Error: {}", response.status()),
        }
    }
}