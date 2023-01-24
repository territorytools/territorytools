use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, PartialEq, Clone)]
pub struct Territories {
    pub list: Vec<Territory>,
}

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct Territory {
    pub number: String,
    pub status: String,
    pub description: Option<String>,
    pub address_count: i32,
    pub area_code: Option<String>,
    pub last_completed_by: Option<String>,
    pub signed_out_to: Option<String>,
    pub group_id: Option<String>,
    pub border: Vec<Vec<f32>>,
}

// TODO: This is good: https://yew.rs/docs/0.18.0/concepts/wasm-bindgen/web-sys