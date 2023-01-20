use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct User {
    pub id: i32,
    pub alba_full_name: Option<String>,
    pub alba_user_id: Option<String>,
}

// TODO: This is good: https://yew.rs/docs/0.18.0/concepts/wasm-bindgen/web-sys