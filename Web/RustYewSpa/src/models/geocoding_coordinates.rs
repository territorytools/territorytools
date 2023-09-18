use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, PartialEq, Clone)]
pub struct GeocodingCoordinates {
    pub score: f64,
    pub latitude: f64,
    pub longitude: f64,
}

#[derive(Serialize, Deserialize, PartialEq, Clone)]
pub struct AddressToGeocode {
    pub unit: Option<String>,
    pub address: Option<String>,
    pub city: Option<String>,
    pub state: Option<String>,
    pub postal_code: Option<String>,
    pub country: Option<String>,
}