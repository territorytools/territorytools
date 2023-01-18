use crate::components::territory_map::TerritoryMap;
use crate::components::territory_summary::TerritorySummary;
use yew::prelude::*;
mod components;
mod models;

enum Msg {
}

struct Model {
}

impl Component for Model {
    type Message = Msg;
    type Properties = ();

    fn create(_ctx: &Context<Self>) -> Self {
        Self { 
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, _msg: Self::Message) -> bool {
        true
    }

    fn changed(&mut self, _ctx: &Context<Self>) -> bool {
        false
    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
        html! {
            <div style={"width:100%;height:90vh;"}>
                <TerritoryMap />
            </div>
        }
    }
}

fn main() {
    yew::start_app::<Model>();
}

