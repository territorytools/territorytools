use crate::components::{
    control::{Cities, Control},
    map_component::{City, MapComponent, PixelPoint, MapModel},
};
use crate::models::territories::Territory;

use gloo_console::log;
use reqwasm::http::Request;
use reqwasm::http::Response;
use wasm_bindgen_futures::JsFuture;
use yew::prelude::*;
use yew::html::Scope;
use yew_hooks::use_async;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

pub enum Msg {
    SelectCity(City),
    LoadBorders(MapModel),
}

pub struct Model {
    city: City,
    cities: Cities,
    territory_map: MapModel
}

impl Component for Model {
    type Message = Msg;
    type Properties = ();

    fn create(_ctx: &Context<Self>) -> Self {
        let aachen = City {
            name: "Aachen".to_string(),
            lat: PixelPoint(50.7597f64, 6.0967f64),
        };
        let stuttgart = City {
            name: "Stuttgart".to_string(),
            lat: PixelPoint(48.7784f64, 9.1742f64),
        };
        let seattle = City {
            name: "Seattle".to_string(),
            lat: PixelPoint(47.7784f64, -122.1742f64),
        };
        let cities: Cities = Cities {
            list: vec![aachen, seattle, stuttgart],
        };
        let city = cities.list[0].clone();
        let territory_map: MapModel = MapModel::default();

        Self { city, cities, territory_map }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        // Calling this update(Message) calls map_components.change(Properties)
        match msg {
            Msg::SelectCity(city) => {
                self.city = self
                    .cities
                    .list
                    .iter()
                    .find(|c| c.name == city.name)
                    .unwrap()
                    .clone();
            },
            Msg::LoadBorders(territory_map) => {
                log!("model.LoadBorders(MapModel) message is running! (from the load_data_3 function maybe?)");
               log!(format!("model.LoadBorders: territoryMap.lat: {:.4},{:.4}", territory_map.lat, territory_map.lon));
 
                self.territory_map = territory_map.clone();
            },
        }
        true
    }

    fn changed(&mut self, _ctx: &Context<Self>, old_props: &Self::Properties) -> bool {
        false
    }

    fn rendered(&mut self, ctx: &Context<Self>, first_render: bool) {
        if first_render {
            let group_id: String = "23".to_string();
            ctx.link().send_future(async move {
                let m = fetch_territory_map(&group_id).await;
                log!("model:rendered:send_future: Sending load borders message");
                Msg::LoadBorders(m)
            });
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let cb = ctx.link().callback(Msg::SelectCity); // Call self back with this message
        let tcb = ctx.link().callback(Msg::LoadBorders); // Call self back with this message
        html! {
            <>
                <MapComponent city={&self.city} territory_map={&self.territory_map} />
                <Control select_city={cb} border_loader={tcb} cities={&self.cities}/>
            </>
        }
    }
}


fn find_center(territories: &Vec<Territory>) -> (f64, f64) {    
    let filtered_territories = territories.iter().filter(|&t| territory_filter(t)).collect::<Vec<_>>();
    let latitude_sum: f64 = filtered_territories.iter().map(|t| find_border_center(&t.border).0).sum();
    let longitude_sum: f64 = filtered_territories.iter().map(|t| find_border_center(&t.border).1).sum();
    let count: f64 = filtered_territories.len() as f64;
    let lat_avg = latitude_sum/count;
    let lon_avg = longitude_sum/count;
    log!(format!("Territory Center: {:.4}, {:.4}", lat_avg, lon_avg));
    (lat_avg, lon_avg)
}

fn find_border_center(border: &Vec<Vec<f32>>) -> (f64, f64) {
    
    let latitude_sum: f64 = border.iter().filter(|&v| v.len() == 2).map(|v| v[0] as f64).sum();
    let longitude_sum: f64 = border.iter().filter(|&v| v.len() == 2).map(|v| v[1] as f64).sum();
    let count: f64 = border.len() as f64; 
    let lat_avg = latitude_sum/count;
    let lon_avg = longitude_sum/count;
    //log!(format!("Border Center: {:.4}, {:.4}", lat_avg, lon_avg));
    (lat_avg, lon_avg)
}

fn territory_filter(t: &Territory) -> bool {
    //t.border.len() > 2 && (if t.group_id.is_none() { true } else {t.group_id.clone().expect("Must have a string here").to_string() != "outer".to_string() })
    t.border.len() > 2
        // && !t.group_id.is_none() 
        // && t.group_id.clone().expect("Must have a string here").to_string() == "borders".to_string() 
}


pub async fn fetch_territory_map(group_id: &String) -> MapModel {
    let fetched_territories: Vec<Territory> = fetch_territories(&group_id).await;

        log!(format!("model:rendered: territories.len: {}", fetched_territories.len()));
        
        let mut filtered_territories = fetched_territories.iter().filter(|t| t.number > "10000".to_string()).map(|t| t.clone()).collect::<Vec<_>>();                    
        //filtered_territories.truncate(10);
        log!(format!("model:rendered: filtered_territories.len: {}", filtered_territories.len()));

        let map_center = find_center(&filtered_territories);

        log!(format!("model: map_center: {:.4}, {:.4}", map_center.0, map_center.1));

        let m = MapModel {
            territories: filtered_territories.clone(),
            territories_is_loaded: true,
            local_load: false,
            lat: map_center.0,
            lon: map_center.1,
            zoom: 10.0,
            group_visible: String::from("*"),
        };

        m
}

pub async fn fetch_territories(group_id: &String) ->  Vec<Territory> {
    let uri: String =
                format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

    let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
        .send()
        .await
        .unwrap()
        .json()
        .await
        .unwrap();

    fetched_territories
}