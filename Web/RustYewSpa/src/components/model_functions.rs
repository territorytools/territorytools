use crate::components::{
    map_component::{MapModel},
};
use crate::models::territories::Territory;

use reqwasm::http::Request;
use gloo_console::log;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

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

pub async fn fetch_territory_map(group_id: &String) -> MapModel {
    let fetched_territories: Vec<Territory> = fetch_territories(&group_id).await;       
    let map_center = find_center(&fetched_territories);
    MapModel {
        territories: fetched_territories.clone(),
        territories_is_loaded: true,
        local_load: false,
        lat: map_center.0,
        lon: map_center.1,
        zoom: 10.0,
        group_visible: String::from("*"),
    }
}

pub async fn fetch_territory_map_w_key(group_id: &String, access_key: &String) -> MapModel {
    log!(format!("fetch_territory_map_w_key: access_key: {access_key}"));
    let fetched_territories: Vec<Territory> = fetch_territories_w_key(&group_id, &access_key).await;       
    let map_center = find_center(&fetched_territories);
    MapModel {
        territories: fetched_territories.clone(),
        territories_is_loaded: true,
        local_load: false,
        lat: map_center.0,
        lon: map_center.1,
        zoom: 10.0,
        group_visible: String::from("*"),
    }
}

pub async fn fetch_territories(group_id: &String) ->  Vec<Territory> {
    let uri: String = format!("{DATA_API_PATH}?groupId={group_id}&accessKey=");
    Request::get(uri.as_str())
        .send()
        .await
        .unwrap()
        .json()
        .await
        .unwrap()
}

pub async fn fetch_territories_w_key(group_id: &String, access_key: &String) ->  Vec<Territory> {
    let uri: String = format!("{DATA_API_PATH}?groupId={group_id}&mtk={access_key}");
    Request::get(uri.as_str())
        .send()
        .await
        .unwrap()
        .json()
        .await
        .unwrap()
}