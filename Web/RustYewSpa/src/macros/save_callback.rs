use serde::{Serialize, Deserialize};
use yew::Properties;
use reqwasm::http::Response;

#[macro_export]
macro_rules! save_callback {
    ($state:ident.$($field_path:ident).+, $uri:ident) => {{
        Callback::from(move |_: i32| { 
            let state = $state.clone();
            let uri = $uri.clone();
            spawn_local(async move {
                let model = state.$($field_path).+.clone();
                let model_serialized = serde_json::to_string_pretty(&model)
                    .expect("Cannot serialize model into JSON");
                let method = Method::POST;
                let response = Request::new(uri.as_str())
                    .method(method)
                    .header("Content-Type", "application/json")
                    .body(model_serialized)
                    .send()
                    .await
                    .expect("A result from the endpoint");        
                
                let status = $crate::macros::save_callback::save_status(&response);

                let mut modified = state.deref().clone();
                modified.save_status = status;
                state.set(modified);
            });
        })
    }};
    // TODO: Finish or delete this, this should copy the model to the request.model property...
    ($state:ident.$($model_path:ident).*$model:ident, $request_state:ident.$($request_path:ident).+, $uri:ident) => {{
        Callback::from(move |_: i32| { 
            let state = $state.clone();
            let request_state = $request_state.clone();
            let uri = $uri.clone();
            spawn_local(async move {
                let model = state.$($model_path).*$model.clone();

                let mut request = request_state.deref().clone();
                // TODO: request.user = model ?
                // state.save_request.user = model
                request.$($request_path:ident).+ = model.clone();
                
                let request_serialized = serde_json::to_string_pretty(&request)
                    .expect("Cannot serialize request into JSON");
                let method = Method::POST;
                let response = Request::new(uri.as_str())
                    .method(method)
                    .header("Content-Type", "application/json")
                    .body(model_serialized)
                    .send()
                    .await
                    .expect("A result from the endpoint");        
                
                let status = $crate::macros::http::save_status(&response);

                let mut modified = state.deref().clone();
                modified.save_status = status;
                state.set(modified);
            });
        })
    }}
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct SaveStatus {
    pub error_message: String,
    pub save_error: bool,
    pub save_success: bool,
}

pub fn save_status(response: &Response) -> SaveStatus {
    if response.status() == 200 {
        SaveStatus {
            save_success: true,
            ..SaveStatus::default()
        }
    } else if (401..403).contains(&response.status()) {
        SaveStatus {
            save_error: true,
            error_message: "Unauthorized".to_string(),
            ..SaveStatus::default()
        }
    } else {
        SaveStatus {
            save_error: true,
            error_message: format!("Error: {}", response.status()),
            ..SaveStatus::default()
        }
    }
}