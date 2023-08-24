use crate::components::{
    control::{Cities, Control},
    map_component::{City, MapComponent, Point},
};
use crate::components::territory_map::TerritoryMapModel;
use crate::models::territories::Territory;
use gloo_console::log;
use yew::prelude::*;
use reqwasm::http::Request;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

pub enum Msg {
    SelectCity(City),
    LoadBorders(TerritoryMapModel),
}

pub struct Model {
    city: City,
    cities: Cities,
    territory_map: TerritoryMapModel
}

impl Component for Model {
    type Message = Msg;
    type Properties = ();

    fn create(_ctx: &Context<Self>) -> Self {
        let aachen = City {
            name: "Aachen".to_string(),
            lat: Point(50.7597f64, 6.0967f64),
        };
        let stuttgart = City {
            name: "Stuttgart".to_string(),
            lat: Point(48.7784f64, 9.1742f64),
        };
        let cities: Cities = Cities {
            list: vec![aachen, stuttgart],
        };
        let city = cities.list[0].clone();
        let territory_map: TerritoryMapModel = TerritoryMapModel::default();

        wasm_bindgen_futures::spawn_local(async move {
            let group_id: String = "2".to_string();//group_id;
            let uri: String =
                format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

            let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
                .send()
                .await
                .unwrap()
                .json()
                .await
                .unwrap();

            let m = TerritoryMapModel {
                territories: fetched_territories,
                territories_is_loaded: true,
                local_load: false,
                lat: 47.66,
                lon: -122.20,
                zoom: 10.0,
                group_visible: "*".to_string(),
            };

            log!("Model got territory borders! triggered on create.");

            //self.territory_map = m;
            //ctx.props().select_city.emit(city);
            //ctx.link().callback(Msg::LoadBorders)
        });

        Self { city, cities, territory_map }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
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
            Msg::LoadBorders(territoryMap) => {
                log!("LoadBorders message is running!");
                load_data();
            }
        }
        true
    }

    fn changed(&mut self, _ctx: &Context<Self>, old_props: &Self::Properties) -> bool {
        false
    }

    fn rendered(&mut self, _ctx: &Context<Self>, first_render: bool) {
        if first_render {
            
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

pub fn load_data() {
    wasm_bindgen_futures::spawn_local(async move {
        let group_id: String = "2".to_string();//group_id;
        let uri: String =
            format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

        let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
            .send()
            .await
            .unwrap()
            .json()
            .await
            .unwrap();

        let m = TerritoryMapModel {
            territories: fetched_territories,
            territories_is_loaded: true,
            local_load: false,
            lat: 47.66,
            lon: -122.20,
            zoom: 10.0,
            group_visible: "*".to_string(),
        };

        log!("Model got territory borders! the one you're looking for.");

        //self.territory_map = m;
    });
}