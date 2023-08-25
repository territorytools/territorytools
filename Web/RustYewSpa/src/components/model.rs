use crate::components::{
    control::{
        Cities, 
        //Control,
    },
    map_component::{City, MapComponent, PixelPoint, MapModel},
    model_functions::*,
};

use yew::prelude::*;

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
                self.territory_map = territory_map.clone();
            },
        }
        true
    }

    fn changed(&mut self, _ctx: &Context<Self>, _old_props: &Self::Properties) -> bool {
        false
    }

    fn rendered(&mut self, ctx: &Context<Self>, first_render: bool) {
        if first_render {
            let group_id: String = "23".to_string();
            ctx.link().send_future(async move {
                Msg::LoadBorders(fetch_territory_map(&group_id).await)
            });
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let _cb = ctx.link().callback(Msg::SelectCity); // Call self back with this message
        let _tcb = ctx.link().callback(Msg::LoadBorders); // Call self back with this message
        html! {
            <>
                <MapComponent city={&self.city} territory_map={&self.territory_map} />
                //<Control select_city={cb} border_loader={tcb} cities={&self.cities}/>
            </>
        }
    }
}
