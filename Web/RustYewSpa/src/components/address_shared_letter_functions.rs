use crate::components::address_shared_letter_page::AddressSharedLetterResult;

use reqwasm::http::Request;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/addresses-shared-letter.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses-shared-letter";

pub async fn fetch_shared_letter_addresses() ->  AddressSharedLetterResult {
    let access_key: String = "TEST-ACCESS-KEY".to_string();
    let uri: String = format!("{DATA_API_PATH}"); //?mtk={access_key}");
    Request::get(uri.as_str())
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a AddressSharedLetterResult")
}