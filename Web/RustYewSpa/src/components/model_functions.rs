use crate::components::{
    map_component::{MapModel},
};
use crate::models::territories::{Territory,BorderFilteredResult};

use reqwasm::http::Request;
use gloo_console::log;
use regex::Regex;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-filtered.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders-filtered";

fn find_center(territories: &Vec<Territory>) -> (f64, f64) {    
    let filtered_territories = territories.iter().filter(|&t| territory_filter(t)).collect::<Vec<_>>();
    let latitude_sum: f64 = filtered_territories.iter().map(|t| find_border_center(&t.border).0).sum();
    let longitude_sum: f64 = filtered_territories.iter().map(|t| find_border_center(&t.border).1).sum();
    let count: f64 = filtered_territories.len() as f64;
    let lat_avg = latitude_sum/count;
    let lon_avg = longitude_sum/count;
    (lat_avg, lon_avg)
}

fn find_border_center(border: &Vec<Vec<f32>>) -> (f64, f64) {
    let latitude_sum: f64 = border.iter().filter(|&v| v.len() == 2).map(|v| v[0] as f64).sum();
    let longitude_sum: f64 = border.iter().filter(|&v| v.len() == 2).map(|v| v[1] as f64).sum();
    let count: f64 = border.len() as f64; 
    let lat_avg = latitude_sum/count;
    let lon_avg = longitude_sum/count;
    (lat_avg, lon_avg)
}

fn territory_filter(t: &Territory) -> bool {
    t.border.len() > 2
}

pub async fn fetch_territory_map_w_key(access_key: &String) -> MapModel {
    log!(format!("fetch_territory_map_w_key: access_key: {access_key}"));
    let mut fetched_result: BorderFilteredResult = fetch_territories_w_key(&access_key).await;       
    let map_center = find_center(&fetched_result.territories);

    // edit-territory-button-enabled Section
    let regex = Regex::new(r"(^|;)edit\-territory\-button\-enabled=([^;]+?)($|;)").expect("Valid RegEx");
    let link_grants_clone = fetched_result.link_grants.clone().unwrap_or("".to_string());
    let caps = regex.captures(link_grants_clone.as_str());
    let mut edit_territory_button_enabled: String = "".to_string();
    if caps.is_some() && caps.as_ref().unwrap().len() > 0usize {
        edit_territory_button_enabled = caps.as_ref().expect("description-contains in link_grants").get(2).map_or("".to_string(), |m| m.as_str().to_string());
        //self.search = description_contains.clone();
        //  fetched_result.edit_territory_button_enabled 
        //      = edit_territory_button_enabled.parse().unwrap_or(true);
    }
    log!(format!("model:update: LoadBorderPath: edit_territory_button_enabled: {}", edit_territory_button_enabled.parse().unwrap_or(true)));

    //owner big-map-territory-open-enabled=true Section
    let regex = Regex::new(r"(^|;|\s+)big\-map\-territory\-open\-enabled=([^;]+?)($|;)").expect("Valid RegEx");
    let user_roles_clone = fetched_result.user_roles.clone().unwrap_or("".to_string());
    let caps = regex.captures(user_roles_clone.as_str());
    let mut territory_open_enabled: String = "".to_string();
    if caps.is_some() && caps.as_ref().unwrap().len() > 0usize {
        territory_open_enabled = caps.as_ref().expect("big-map-territory-open-enabled in user_roles").get(2).map_or("".to_string(), |m| m.as_str().to_string());
    }
    log!(format!("model:update: LoadBorderPath: territory_open_enabled: {}", territory_open_enabled.parse().unwrap_or(false)));

    MapModel {
        territories: fetched_result.territories.clone(),
        // TODO: add search enabled
        territories_is_loaded: true,
        local_load: false,
        lat: map_center.0,
        lon: map_center.1,
        zoom: 10.0,
        group_visible: String::from("*"),
        link_grants: fetched_result.link_grants.clone(),
        user_roles: fetched_result.user_roles.clone(),
        edit_territory_button_enabled: edit_territory_button_enabled.parse().unwrap_or(true),
        territory_open_enabled: territory_open_enabled.parse().unwrap_or(false),
    }
}

pub async fn fetch_territories_w_key(access_key: &String) ->  BorderFilteredResult {
    let uri: String = format!("{DATA_API_PATH}?mtk={access_key}");
    Request::get(uri.as_str())
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a BorderFilteredResult")
}