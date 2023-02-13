use serde::{Serialize, Deserialize};
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Address {
    pub alba_address_id: i32,
    pub territory_number: Option<String>,
    pub name: Option<String>,
    pub street: Option<String>,
    pub city: Option<String>,
    pub postal_code: Option<String>,
}
