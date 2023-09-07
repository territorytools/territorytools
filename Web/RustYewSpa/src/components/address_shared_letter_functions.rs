use crate::components::address_shared_letter_page::AddressSharedLetterResult;

use serde::{Deserialize};
use reqwasm::http::{Request,Method};

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/addresses-shared-letter.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses-shared-letter";

#[cfg(debug_assertions)]
const CHECKOUT_START_API_PATH: &str = "/data/addresses-shared-letter-checkout-start.json";

#[cfg(not(debug_assertions))]
const CHECKOUT_START_API_PATH: &str = "/api/addresses-shared-letter/address-checkout-start";

#[cfg(not(debug_assertions))]
const POST_METHOD: Method = Method::POST;
#[cfg(debug_assertions)]
const POST_METHOD: Method = Method::GET;

pub async fn fetch_shared_letter_addresses() ->  AddressSharedLetterResult {
    let _access_key: String = "TEST-ACCESS-KEY".to_string();
    let uri: String = format!("{DATA_API_PATH}"); //?mtk={access_key}");
    Request::get(uri.as_str())
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a AddressSharedLetterResult")
}


#[derive(PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct CheckoutStartResult {
    pub success: bool,
    pub address_id: i32,
    pub alba_address_id: i32,
    pub checkout_start_utc: Option<String>,
}

pub async fn post_address_checkout_start(alba_address_id: i32) ->  CheckoutStartResult {
    let _access_key: String = "TEST-ACCESS-KEY".to_string();
    let uri: String = format!("{CHECKOUT_START_API_PATH}?albaAddressId={alba_address_id}"); //?mtk={access_key}");

    Request::new(uri.as_str())
        .method(POST_METHOD)
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a CheckoutStartResult")
}