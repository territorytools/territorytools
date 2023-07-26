use crate::models::addresses::Address;
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, PartialEq, Clone)]
pub struct Territories {
    pub list: Vec<Territory>,
}

#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct Territory {
    #[serde(default)]
    pub id: Option<i32>,
    pub number: String,
    pub status: String,
    pub stage_id: Option<i32>,
    pub description: Option<String>,
    pub notes: Option<String>,
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

#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct TerritoryEditRequest {
    #[serde(default)]
    pub id: i32,
    pub territory_number: String,
    //pub status: String,
    pub stage_id: i32,
    pub description: Option<String>,
    pub notes: Option<String>,
    // pub area_code: Option<String>,
    pub group_id: Option<String>,
    // pub sub_group_id: Option<String>,
    // #[serde(default)]
    // pub is_active: bool,
    // #[serde(default)]
    // pub is_hidden: bool,
    // pub border: Vec<Vec<f32>>,
    pub modification_type: String,
}

// TODO: This is good: https://yew.rs/docs/0.18.0/concepts/wasm-bindgen/web-sys

#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct TerritorySummary {
    #[serde(default)]
    pub id: Option<i32>,
    pub number: String,
    pub group_id: Option<String>,
    pub description: Option<String>,
    pub area_description: Option<String>,
    pub area_code: Option<String>,
    pub status: Option<String>,
    pub status_date: Option<String>,
    pub publisher: Option<String>,
    pub view_link: Option<String>,
    // pub stage_id: Option<i32>,
    // pub notes: Option<String>,
    // pub address_count: i32,
    // pub last_completed_by: Option<String>,
    // pub signed_out_to: Option<String>,
    // pub sub_group_id: Option<String>,
    // #[serde(default)]
    // pub is_active: bool,
}
