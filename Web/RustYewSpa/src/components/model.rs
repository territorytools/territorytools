use crate::components::{
    control::{
        Cities, 
        //Control,
    },
    map_component::{City, MapComponent, PixelPoint, MapModel},
    model_functions::*,
    map_component_functions::{tpoly_from_territory,TerritoryPolygon},
};
use crate::components::menu_bar_v2::MenuBarV2;

use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use gloo_console::log;

pub enum Msg {
    SelectCity(City),
    LoadBorders(MapModel),
    Search(String),
}

pub struct Model {
    city: City,
    cities: Cities,
    territory_map: MapModel,
    tpolygons: Vec<TerritoryPolygon>,
    search: String,
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

        Self { city, cities, territory_map, search: "".to_string(), tpolygons: vec![] }
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

                self.tpolygons.clear();
                let mut id_list: Vec<i32> = vec![];
                for t in self.territory_map.territories.iter() {
                    if t.group_id != Some("outer".to_string()) && t.number != "OUTER".to_string() {
                        let tp = tpoly_from_territory(t);
                        self.tpolygons.push(tp);
                    }            
                }

                log!(format!("model: Msg::LoadBorders: tpolygons len: {}", self.tpolygons.len()));
            },
            Msg::Search(search) => {
                self.search = search;

                self.tpolygons.clear();
                let mut id_list: Vec<i32> = vec![];
                for t in self.territory_map.territories.iter() {
                    if t.group_id == Some(self.search.clone()) 
                      || t.number == self.search.clone() {
                        let tp = tpoly_from_territory(t);
                        self.tpolygons.push(tp);
                    }            
                }

                log!(format!("model: Msg::Search: tpolygons len: {} territories len: {}", self.tpolygons.len(), self.territory_map.territories.len()));
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

        let search_text_onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
        });

        let link = ctx.link().clone();
        let search_text_onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();
                
                log!(format!("model: search_text_onchange: value: {}", value));

                link.send_message(Msg::Search(value));
            })
        };

        let search_clear_onclick = {
            Callback::from(move |_event: MouseEvent| {
                
            })
        };

        html! {
           <div style="background-color:yellow;height:100%;">
            <div id="menu-bar-header" style="height:57px;background-color:red;">
                    <MenuBarV2>
                        <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                            // <li class="nav-item">
                            //     <TerritorySearchLink />
                            // </li>
                            <li class="nav-item">
                                <div class="d-flex flex-colum shadow-sm">
                                    <div class="input-group">
                                        <form onsubmit={search_text_onsubmit} id="search-form" style="max-width:150px;">
                                            <input onchange={search_text_onchange}
                                                value="temp" //value={search_state.search_text.clone()}
                                                type="text"
                                                class="form-control"
                                                placeholder="Search"  />
                                        </form>
                                        <button onclick={search_clear_onclick} class="btn btn-outline-primary">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-lg" viewBox="0 0 16 16">
                                                <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                                            </svg>
                                        </button>
                                        <button form="search-form" class="btn btn-primary" type="submit">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-search" viewBox="0 0 16 16">
                                                <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
                                            </svg>
                                        </button>
                                        // <span>{"  Mouse: "}{mouse_click_model.mouse_click_x}{","}{mouse_click_model.mouse_click_y}</span>
                                        // <span>{"  LatLng: "}{format!("{:.4},{:.4}",latLng.lat(),latLng.lng())}</span>
                                    </div>
                                </div>
                            </li>
                        </ul>
                    </MenuBarV2>
                </div>
                <MapComponent city={&self.city} territory_map={&self.territory_map} tpolygons={self.tpolygons.clone()} search={self.search.clone()}/>
                //<Control select_city={cb} border_loader={tcb} cities={&self.cities}/>
            </div>
        }
    }
}
