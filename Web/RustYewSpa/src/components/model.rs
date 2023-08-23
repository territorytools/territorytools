use crate::components::{
    control::{Cities, Control},
    map_component::{City, MapComponent, Point},
};
use crate::components::territory_map::TerritoryMapModel;
use crate::models::territories::Territory;
use gloo_console::log;
use reqwasm::http::Request;
use yew::prelude::*;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

pub enum Msg {
    SelectCity(City),
}

pub struct Model {
    city: City,
    cities: Cities,
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
        Self { city, cities }
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
            }
        }
        true
    }

    fn changed(&mut self, _ctx: &Context<Self>, old_props: &Self::Properties) -> bool {
        false
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let cb = ctx.link().callback(Msg::SelectCity);
        log!("Model view started");
        //let model_clone = model.clone();
        //use_effect_with_deps(
        //    move |_| {
                //let model_clone = model_clone.clone();
                wasm_bindgen_futures::spawn_local(async move {
                    let group_id: String = "2".to_string();//group_id;
                    let uri: String =
                        format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);
    
                    //if !model_clone.territories_is_loaded {
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
                            group_visible: String::from("*"),
                        };

                        log!("Got territory borders!");

                        //model_clone.set(m);
                    //}
                });
                //|| ()
            //},
            //(),
        //);

        html! {
            <>
                <MapComponent city={&self.city}  />
                <Control select_city={cb} cities={&self.cities}/>
            </>
        }
    }
}
