use crate::models::addresses::Address;
use crate::models::areas::Area;
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, PartialEq, Clone)]
pub struct Territories {
    pub list: Vec<Territory>,
}


#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct BorderFilteredResult {
    pub user_roles: Option<String>,
    pub link_grants: Option<String>,
    pub territories: Vec<Territory>,
    pub areas: Vec<Area>,
}

#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct Territory {
    #[serde(default)]
    pub id: i32,
    pub number: String,
    pub status: Option<String>,
    pub stage_id: Option<i32>,
    pub stage: Option<String>,
    #[serde(default)]
    pub stage_changes: Vec<TerritoryStageChange>,
    pub last_visiting_started: Option<String>,
    pub last_visiting_done: Option<String>,
    pub description: Option<String>,
    pub notes: Option<String>,
    pub address_count: i32,
    pub area_code: Option<String>,
    pub last_completed_by: Option<String>,
    pub last_completed: Option<String>,
    pub signed_out_to: Option<String>,
    pub signed_out: Option<String>,
    pub assignee_link_key: Option<String>,
    pub group_id: Option<String>,
    pub sub_group_id: Option<String>,
    #[serde(default)]
    pub is_active: bool,
    #[serde(default)]
    pub is_hidden: bool,
    pub border: Vec<Vec<f32>>,
    #[serde(default)]
    pub language_group_id: i32,
    #[serde(default)]
    pub addresses: Vec<Address>,
    #[serde(default)]
    pub addresses_total: Option<i32>,
    #[serde(default)]
    pub addresses_active:  Option<i32>,
    #[serde(default)]
    pub addresses_unvisited:  Option<i32>,
}

#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct TerritoryEditRequest {
    #[serde(default)]
    pub territory_id: i32,
    pub territory_number: String,
    //pub status: String,
    pub stage_id: i32,
    pub language_group_id: i32,
    pub description: Option<String>,
    pub notes: Option<String>,
    // pub area_code: Option<String>,
    pub group_id: Option<String>,
    // pub sub_group_id: Option<String>,
    // #[serde(default)]
    // pub is_active: bool,
    // #[serde(default)]
    // pub is_hidden: bool,
    pub border: Vec<Vec<f32>>,
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
    pub stage: Option<String>,
    pub publisher: Option<String>,
    pub view_link: Option<String>,
    #[serde(default)]
    pub addresses_unvisited: i32,
    #[serde(default)]
    pub addresses_active: i32,
    // pub stage_id: Option<i32>,
    // pub notes: Option<String>,
    // pub address_count: i32,
    // pub last_completed_by: Option<String>,
    // pub signed_out_to: Option<String>,
    // pub sub_group_id: Option<String>,
    // #[serde(default)]
    // pub is_active: bool,
}

#[derive(Ord, Clone, Debug, Default, Deserialize, Eq, PartialEq, PartialOrd, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct TerritoryStageChange {
    pub change_date_utc: String,
    pub stage_id: i32,
    pub stage: Option<String>,
    pub assignee_normalized_email: Option<String>,
    pub assignee_name: Option<String>,
    pub created_by_user_id: Option<String>,
}
