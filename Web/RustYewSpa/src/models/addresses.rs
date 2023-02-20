use serde::{Serialize, Deserialize};
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Address {
    pub alba_address_id: i32,
    pub territory_number: Option<String>,
    pub language: Option<String>,
    pub language_id : i32,
    pub status: Option<String>,
    pub status_id: i32,
    pub delivery_status_id: i32,
    pub name: Option<String>,
    pub street: Option<String>,
    pub unit: Option<String>,
    pub city: Option<String>,
    pub state: Option<String>,
    pub postal_code: Option<String>,
    pub latitude: f32,
    pub longitude: f32,
    pub phone: Option<String>,
    pub notes: Option<String>,
}

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct AddressDeliveryStatus {
    pub id: i32,
    pub name: Option<String>,
    pub description: Option<String>,
    pub is_active: bool,
}

// #[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize)]
// #[serde(rename_all = "camelCase")]
// pub struct AddressSaveRequest {
//     pub albaTerritoryKey: Option<String>,
//     pub albaAddressId: Option<String>,
//     pub latitude: Option<String>,
//     pub longitude: Option<String>,
//     pub albaTerritoryId: Option<String>,
//     pub albaStatusId: Option<String>,
//     pub albaLanguageId: Option<String>,
//     pub fullName: Option<String>,
//     pub unit: Option<String>,
//     pub street: Option<String>,
//     pub city: Option<String>,
//     pub state: Option<String>,
//     pub country: Option<String>,
//     pub postalCode: Option<String>,
//     pub telephone: Option<String>,
//     pub notes: Option<String>,
//     pub notesPrivate: Option<String>,
// }