use super::map_component::City;
use gloo_console::log;
use yew::{html::ImplicitClone, prelude::*};
use crate::components::map_component::MapModel;

pub enum Msg {
    CityChosen(City),
    LoadBorders(MapModel),
}

pub struct Control {
    cities: Vec<City>,
}

#[derive(PartialEq, Clone)]
pub struct Cities {
    pub list: Vec<City>,
}

impl ImplicitClone for Cities {}

#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    pub cities: Cities,
    pub select_city: Callback<City>,
    //pub territory_map: Callback<TerritoryMapModel>,
    pub border_loader: Callback<MapModel>, 
}

impl Control {
    fn button(&self, ctx: &Context<Self>, city: City) -> Html {
        let name = city.name.clone();
        let cb = ctx.link().callback(move |_| Msg::CityChosen(city.clone()));
        let tcb = ctx.link().callback(move |_| Msg::LoadBorders(MapModel::default()));
        html! {
            <>
                <button onclick={cb}>{name.clone()}</button>
                <button onclick={tcb}>{name.clone()}{" map"}</button>
            </>
        }
    }
}

impl Component for Control {
    type Message = Msg;
    type Properties = Props;

    fn create(ctx: &Context<Self>) -> Self {
        Control {
            cities: ctx.props().cities.list.clone(),
            //territory_map: TerritoryMapModel::default(),
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::CityChosen(city) => {
                log!(format!("Update: {:?}", city.name));
                // This is a relay from the button to ... the map_component?
                ctx.props().select_city.emit(city);
            },
            Msg::LoadBorders(map_model) => {
                log!("Updated MapModel.");
                // emit() is for callbacks, this is all wrongf
                ctx.props().border_loader.emit(map_model);
            }
        }
        true
    }

    fn changed(&mut self, _ctx: &Context<Self>, old_props: &Self::Properties) -> bool {
        false
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        html! {
            <div class="control component-container">
                <h1>{"Choose a city"}</h1>
                <div>
                    {for self.cities.iter().map(|city| Self::button(self, ctx, city.clone()))}
                </div>
            </div>
        }
    }
}