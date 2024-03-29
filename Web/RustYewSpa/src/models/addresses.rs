use serde::{Serialize, Deserialize};
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize, Debug)]
#[serde(rename_all = "camelCase")]
pub struct Address {
    pub address_id: i32,
    pub alba_address_id: i32,
    pub territory_id: Option<i32>,
    pub territory_number: Option<String>,
    pub language: Option<String>,
    pub language_id: i32,
    pub alba_language_id : Option<i32>, // TODO: Deprecate
    pub status: Option<String>,
    pub status_id: i32,
    pub delivery_status_id: i32,
    pub delivery_status: Option<String>,
    pub last_delivery_status_date_utc: Option<String>,
    pub name: Option<String>,
    pub street: Option<String>,
    pub unit: Option<String>,
    pub city: Option<String>,
    pub state: Option<String>,
    pub country: Option<String>,
    pub postal_code: Option<String>,
    pub latitude: f32,
    pub longitude: f32,
    pub phone: Option<String>,
    pub notes: Option<String>,
    pub notes_private: Option<String>,
    pub visits: Vec<AddressVisit>,
}

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize, Debug)]
#[serde(rename_all = "camelCase")]
pub struct AddressVisit {
    pub id: i32,
    pub address_id: i32,
    pub alba_address_id: i32,
    pub date_utc: String,
    pub result: String,
    pub mobile_territory_key: Option<String>,
}

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct AddressDeliveryStatus {
    pub id: i32,
    pub name: Option<String>,
    pub description: Option<String>,
    pub is_active: bool,
}
