use crate::models::addresses::Address;
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Default, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct Area {
    pub area_id: i32,
    pub account_id: i32,
    #[serde(default)]
    pub parent_id: Option<i32>,
    pub number: String,
    pub name: Option<String>,
    pub description: Option<String>,
    pub border: Vec<Vec<f32>>,
    #[serde(default)]
    pub is_active: bool,
    pub updated_by_user_id: String,
    pub updated_date_utc: String,
    pub created_by_user_id: String,
    pub created_date_utc: String,
}    












