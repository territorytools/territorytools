use crate::components::map_component::MapModel;
use crate::models::territories::{Territory,BorderFilteredResult};
use crate::components::popup_content::PopupContentOptions;

use gloo_net::http::QueryParams;
use reqwasm::http::Request;
use regex::Regex;

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

pub async fn fetch_territory_map_w_mtk(mtk: &str, as_of_date: Option<String>, show_areas: bool) -> MapModel {
    let fetched_result: BorderFilteredResult = fetch_territories_w_mtk(
        mtk, 
        as_of_date.clone().unwrap_or_default().as_str()).await;       

    let map_center = find_center(&fetched_result.territories);

    // edit-territory-button-enabled Section
    let regex = Regex::new(r"(^|;)edit\-territory\-button\-enabled=([^;]+?)($|;)").expect("Valid RegEx");
    let mut edit_territory_button_enabled: bool = false; //String = "".to_string();
    // link_grants
    let link_grants_clone = fetched_result.link_grants.clone().unwrap_or("".to_string());
    let caps = regex.captures(link_grants_clone.as_str());
    if caps.is_some() && caps.as_ref().unwrap().len() > 0usize {
        edit_territory_button_enabled = caps.as_ref().expect("edit_territory_button_enabled in link_grants").get(2).map_or("".to_string(), |m| m.as_str().to_string()).parse().unwrap_or(true);
        //log!(format!("model:update: LoadBorderPath: edit_territory_button_enabled: (link_grants) {}", edit_territory_button_enabled));
    }
    // user_roles (override link_grants)
    let user_roles_clone = fetched_result.user_roles.clone().unwrap_or("".to_string());
    let caps2 = regex.captures(user_roles_clone.as_str());
    if caps2.is_some() && caps2.as_ref().unwrap().len() > 0usize {
        edit_territory_button_enabled = caps2.as_ref().expect("edit_territory_button_enabled in user_roles_clone").get(2).map_or("".to_string(), |m| m.as_str().to_string()).parse().unwrap_or(true);
        //log!(format!("model:update: LoadBorderPath: edit_territory_button_enabled: (user_roles) {}", edit_territory_button_enabled));
    }
    //log!(format!("model:update: LoadBorderPath: edit_territory_button_enabled: {}", edit_territory_button_enabled.parse().unwrap_or(true)));

    //owner big-map-territory-open-enabled=true Section
    let regex = Regex::new(r"(^|;|\s+)big\-map\-territory\-open\-enabled=([^;]+?)($|;)").expect("Valid RegEx");
    let user_roles_clone = fetched_result.user_roles.clone().unwrap_or("".to_string());
    let caps = regex.captures(user_roles_clone.as_str());
    let mut territory_open_enabled: String = "".to_string();
    if caps.is_some() && caps.as_ref().unwrap().len() > 0usize {
        territory_open_enabled = caps.as_ref().expect("big-map-territory-open-enabled in user_roles").get(2).map_or("".to_string(), |m| m.as_str().to_string());
    }
    //log!(format!("model:update: LoadBorderPath: territory_open_enabled: {}", territory_open_enabled.parse().unwrap_or(false)));

    // big-map-show-stage Section
    let regex = Regex::new(r"(^|;|\s+)big\-map\-show\-stage=([^;]+?)($|;)").expect("Valid RegEx");
    let user_roles_clone = fetched_result.user_roles.clone().unwrap_or("".to_string());
    let caps = regex.captures(user_roles_clone.as_str());
    let mut show_stage: String = "".to_string();
    if caps.is_some() && caps.as_ref().unwrap().len() > 0usize {
        show_stage = caps.as_ref().expect("big-map-show-stage in user_roles").get(2).map_or("".to_string(), |m| m.as_str().to_string());
    }
    //log!(format!("model:update: LoadBorderPath: show_stage: {}", show_stage.parse().unwrap_or(false)));

    MapModel {
        territories: fetched_result.territories.clone(),
        areas: if show_areas { fetched_result.areas.clone() } else { vec![] },
        // TODO: add search enabled
        territories_is_loaded: true,
        local_load: false,
        lat: map_center.0,
        lon: map_center.1,
        zoom: 10.0,
        group_visible: String::from("*"),
        link_grants: fetched_result.link_grants.clone(),
        user_roles: fetched_result.user_roles.clone(),
        popup_content_options: PopupContentOptions {
            //edit_territory_button_enabled: edit_territory_button_enabled.parse().unwrap_or(true),
            edit_territory_button_enabled,
            territory_open_enabled: territory_open_enabled.parse().unwrap_or(false),
            show_stage: show_stage.parse().unwrap_or(false),
            as_of_date: as_of_date.clone(),
        }
        // TODO: Try default()?
    }
}

pub async fn fetch_territories_w_mtk(mtk: &str, as_of_date: &str) ->  BorderFilteredResult {
    let params = QueryParams::new();
    params.append("mtk", mtk);
    params.append("asOfDate", as_of_date);
    let query_string = params.to_string();    
    let uri: String = format!("/api/territories/borders-filtered?{query_string}");
    Request::get(uri.as_str())
        .send()
        .await
        .unwrap()
        .json()
        .await
        .expect("Valid JSON for a BorderFilteredResult")
}
