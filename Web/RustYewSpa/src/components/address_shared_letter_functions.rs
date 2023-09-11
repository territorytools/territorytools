use crate::components::address_shared_letter_page::AddressSharedLetterResult;

use serde::{Deserialize};
use reqwasm::http::{Request,Method};

#[cfg(debug_assertions)]
const API_SUFFIX: &str = ".json";

#[cfg(not(debug_assertions))]
const API_SUFFIX: &str = "";

#[cfg(debug_assertions)]
const API_PREFIX: &str = "/data";

#[cfg(not(debug_assertions))]
const API_PREFIX: &str = "/api";

#[cfg(debug_assertions)]
const API_SEPARATOR: &str = "-";

#[cfg(not(debug_assertions))]
const API_SEPARATOR: &str = "/";

const API_SHARED_LETTER_PATH: &str = "addresses-shared-letter";

#[cfg(not(debug_assertions))]
const POST_METHOD: Method = Method::POST;
#[cfg(debug_assertions)]
const POST_METHOD: Method = Method::GET;

pub async fn fetch_shared_letter_addresses(session_id: String, mtk: String) ->  AddressSharedLetterResult {    
    let uri: String = format!("{API_PREFIX}/{API_SHARED_LETTER_PATH}{API_SUFFIX}?sessionId={session_id}&mtk={mtk}");
    Request::get(uri.as_str())
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a AddressSharedLetterResult")
}

// TODO: Need to get session_id in the main list too

#[derive(PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct CheckoutStartResult {
    pub success: bool,
    pub address_id: i32,
    pub alba_address_id: i32,
    pub checkout_start_utc: Option<String>,
    pub session_id: Option<String>,
}

pub async fn post_address_checkout_start(alba_address_id: i32, session_id: String, mtk: String) ->  CheckoutStartResult {
    let uri: String = format!("{API_PREFIX}/{API_SHARED_LETTER_PATH}{API_SEPARATOR}address-checkout-start{API_SUFFIX}?albaAddressId={alba_address_id}&sessionId={session_id}&?mtk={mtk}");

    Request::new(uri.as_str())
        .method(POST_METHOD)
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a CheckoutStartResult")
}

#[derive(PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct CheckoutFinishResult {
    pub success: bool,
    pub address_id: i32,
    pub alba_address_id: i32,
    pub checkout_start_utc: Option<String>,
    pub publisher: Option<String>,
}

pub async fn post_address_checkout_finish(alba_address_id: i32, publisher: String, mtk: String) ->  CheckoutFinishResult {
    let uri: String = format!("{API_PREFIX}/{API_SHARED_LETTER_PATH}{API_SEPARATOR}address-checkout-finish{API_SUFFIX}?albaAddressId={alba_address_id}&publisher={publisher}&mtk={mtk}"); 
        //?mtk={access_key}");

    Request::new(uri.as_str())
        .method(POST_METHOD)
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a CheckoutFinishResult")
}

#[derive(PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct LetterSentResult {
    pub success: bool,
    pub address_id: i32,
    pub alba_address_id: i32,
    pub checkout_start_utc: Option<String>,
    pub letter_sent_utc: Option<String>,
    pub delivery_status_id: i32,
    pub delivery_status: Option<String>,

    pub publisher: Option<String>,
}

pub async fn post_address_leter_sent(alba_address_id: i32, publisher: String, mtk: String) ->  LetterSentResult {
    let uri: String = format!("{API_PREFIX}/{API_SHARED_LETTER_PATH}{API_SEPARATOR}letter-sent{API_SUFFIX}?albaAddressId={alba_address_id}&publisher={publisher}&mtk={mtk}");

    Request::new(uri.as_str())
        .method(POST_METHOD)
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a LetterSentResult")
}