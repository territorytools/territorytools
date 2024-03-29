use reqwasm::http::Response;
use yew::prelude::*;
use serde::{Deserialize,Serialize};
extern crate jsonpath;
extern crate serde_json;

#[macro_export]
macro_rules! get_result_by_id {
     ($uri:expr, $state:ident.$result:ident.$entity:ident.$id:ident) => {{
        let uri = format!($uri);
        let state = $state.clone();
        spawn_local(async move {
            let response = Request::get(uri.as_str())
                .send()
                .await
                .expect("Response (raw) from API");
            
            let mut modification = state.deref().clone();
            modification.$result = response.json().await.expect("Valid JSON"); 
            modification.$entity = modification.$result.$entity.clone();
            state.set(modification);
        });
    }};
}

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
            // TODO: copy the result.entity to the ui_model.entity
            // TODO: copy the satus to the result
            // TODO: Select the right method POST/PUT
            // ...maybe a macro is more complicated that just doing the code...
            state.set(modification);
        });
    }};
     // More simple for user_editor
     (
        result_entity: $result_state:ident.$($result_entity_path:ident).+,
        uri: $uri:ident,
        body: $body:block
    ) => {{
        let result_state = $result_state.clone();
        let uri = $uri.clone();
        spawn_local(async move {
            let response = Request::get(uri.as_str())
                .send()
                .await
                .expect("Response (raw) from API");

            $body

        });
    }};
    // More complicated for user_editor
    (
        entity: $entity_state:ident.$($entity_path:ident).+, 
        result: $result_state:ident.$($result_path:ident).+, 
        result_entity: $result_state2:ident.$($result_entity_path:ident).+,
        uri: $uri:ident
    ) => {{
        let entity_state = $entity_state.clone();
        let result_state = $result_state.clone();
        let uri = $uri.clone();
        spawn_local(async move {
            let response = Request::get(uri.as_str())
                .send()
                .await
                .expect("Response (raw) from API");

                //response.bo

            //let mut modification = result_state.deref().clone();
            let mut modification = entity_state.deref().clone();

            gloo_console::log!("modification.$($result_path).+    {}", stringify!( modification.$($result_path).+));
            gloo_console::log!("modification.$($result_path).+.status {}", stringify!(modification.$($result_path).+.status));
            gloo_console::log!("modification.$($entity_path).+ {}", stringify!(modification.$($entity_path).+));
            gloo_console::log!("modification.$($result_entity_path).+ {}", stringify!(modification.$($result_entity_path).+));
            
            modification.$($result_path).+ = response.json().await.unwrap_or_default();
            gloo_console::log!("after unwrap", stringify!(modification.$($result_path).+.user.id), modification.$($result_path).+.user.id);

            modification.$($result_path).+.status = $crate::macros::http::load_status(&response);
            //result_state.set(modification);

            //let mut modification = entity_state.deref().clone();
            let result_entity_id = modification.$($result_entity_path).+.id;
            let entity_id = modification.$($entity_path).+.id;

            modification.$($entity_path).+ = modification.$($result_entity_path).+.clone();
            entity_state.set(modification);            

            gloo_console::log!(stringify!(modification.$($result_entity_path).+.id), result_entity_id);
            gloo_console::log!(stringify!(modification.$($entity_path).+.id), entity_id);
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
