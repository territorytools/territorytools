use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct User {
    pub id: i32,
    pub alba_full_name: Option<String>,
    pub alba_user_id: Option<String>,
    pub normalized_email: Option<String>,
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

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug, Default)]
#[serde(rename_all = "camelCase")]
pub struct UserResponse {
    pub user: UserChanges,
    pub requested_by_user: UserSummary,
    pub roles_visible: bool,
    pub email_visible: bool,
    pub user_can_edit: bool,
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