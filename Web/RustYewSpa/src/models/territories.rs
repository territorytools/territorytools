use crate::models::addresses::Address;
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, PartialEq, Clone)]
pub struct Territories {
    pub list: Vec<Territory>,
}

#[derive(Serialize, Deserialize,  Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct Territory {
    #[serde(default)]
    pub id: i32,
    pub number: String,
    pub status: String,
    pub stage_id: i32,
    pub description: Option<String>,
    pub address_count: i32,
    pub area_code: Option<String>,
    pub last_completed_by: Option<String>,
    pub signed_out_to: Option<String>,
    pub group_id: Option<String>,
    pub sub_group_id: Option<String>,
    #[serde(default)]
    pub is_active: bool,
    #[serde(default)]
    pub is_hidden: bool,
    pub border: Vec<Vec<f32>>,
    #[serde(default)]
    pub addresses: Vec<Address>,
}

// TODO: This is good: https://yew.rs/docs/0.18.0/concepts/wasm-bindgen/web-sys
