use serde::{Serialize, Deserialize};

use crate::macros::{save_callback::SaveStatus, http::LoadStatus};

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug, Default)]
#[serde(rename_all = "camelCase")]
pub struct User {
    pub id: i32,
    pub alba_full_name: Option<String>,
    pub alba_user_id: Option<String>,
    pub normalized_email: Option<String>,
}

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug, Default)]
#[serde(rename_all = "camelCase")]
pub struct SessionUser {
    pub id: i32,
    pub alba_full_name: Option<String>,
    pub alba_user_id: Option<String>,
    pub normalized_email: Option<String>,
    pub can_assign_territories: bool,
    pub can_impersonate_users: bool,
    pub can_edit_territories: bool,
    pub roles: String,
    pub is_active: bool,
}


// TODO: This is good: https://yew.rs/docs/0.18.0/concepts/wasm-bindgen/web-sys

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug, Default)]
#[serde(rename_all = "camelCase")]
pub struct UserSummary {
    pub id: i32,
    pub alba_full_name: Option<String>,
    pub given_name: Option<String>,
    pub surname: Option<String>,
    pub notes: Option<String>,
    pub alba_user_id: Option<String>,
    pub normalized_email: Option<String>,
    pub is_active: bool,
    pub group_id: Option<String>,
    pub roles: Option<String>,
    pub territory_summary: Option<String>,
}

#[derive(Serialize, Deserialize, PartialEq, Clone, Default)]
#[serde(rename_all = "camelCase")]
pub struct UserLoadResult {
    pub user: UserChanges,
    pub requested_by_user: UserSummary,
    pub roles_visible: bool,
    pub email_visible: bool,
    pub user_can_edit: bool,
    //pub status: LoadStatus,
}

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug, Default)]
#[serde(rename_all = "camelCase")]
pub struct UserChanges {
    pub id: i32,
    pub alba_full_name: Option<String>,
    pub given_name: Option<String>,
    pub surname: Option<String>,
    pub phone: Option<String>,
    pub notes: Option<String>,
    pub alba_user_id: Option<String>,
    pub normalized_email: Option<String>,
    pub group_id: Option<String>,
    pub roles: Option<String>,
    pub territory_summary: Option<String>,
    pub is_active: bool,
    pub can_assign_territories: bool,
    pub can_edit_territories: bool,
    pub can_impersonate_users: bool,
}