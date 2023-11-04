use crate::components::map_menu::MapMenu;
use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::popup_content::popup_content_w_button;
use crate::components::popup_content::PopupContentOptions;
use crate::functions::document_functions::set_document_title;
use crate::models::territories::Territory;
use crate::libs::leaflet::{LatLng, LatLngBounds, Map, Polygon, Polyline, TileLayer, Point};
use crate::html::ImplicitClone;
use gloo_console::log;
use gloo_timers::callback::Timeout;
use gloo_utils::document;
use reqwasm::http::Request;
use serde::{Deserialize, Serialize};
use std::ops::Deref;
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use web_sys::{Element, HtmlElement, HtmlInputElement, Node};
use yew_router::hooks::use_location;

// Uncomment for debugging without an API server
//const DATA_API_PATH: &str = "/data/territory-borders-all.json";

const DATA_API_PATH: &str = "/api/territories/borders";

#[derive(Clone, Debug, PartialEq, Deserialize)]
pub struct TerritoryMapParameters {
    pub group_id: Option<String>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct TerritoryMapModel {
    pub territories: Vec<Territory>,
    pub territories_is_loaded: bool,
    pub local_load: bool,
    pub zoom: f64,
    pub lat: f64,
    pub lon: f64,
    pub group_visible: String,
}

impl ImplicitClone for TerritoryMapModel {}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct MouseClickModel {
    pub mouse_click_x: i32,
    pub mouse_click_y: i32,
}
#[derive(Properties, PartialEq, Clone, Default)]
pub struct TerritorySearchPage {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub territories: Vec<Territory>,
    pub load_error: bool,
    pub load_error_message: String,
}

#[function_component(TerritoryMap)]
pub fn territory_map() -> Html {
    set_document_title("Territory Map");

    let search_state = use_state(|| TerritorySearchPage::default());

    let mouse_click_model: UseStateHandle<MouseClickModel> =
        use_state(|| MouseClickModel::default());

    let model: UseStateHandle<TerritoryMapModel> = use_state(|| TerritoryMapModel::default());
    let location = use_location().expect("Should be a location to get query string");
    //log!("territory_map Query: {}", location.query_str());
    let parameters = location.query::<TerritoryMapParameters>().expect("An object");
    let group_id: String = match &parameters.group_id {
        Some(v) => v.to_string(),
        _ => "".to_string(),
    };
    log!("territory_map Query.group_id: {}", group_id.clone());

    let container: Element = document().create_element("div").unwrap();
    let container: HtmlElement = container.dyn_into().unwrap();
    let map_container = render_map(&container);
    // From render_map
    //let node: &Node = container.clone().into();
    // from create
    container.set_class_name("map");
    let leaflet_map: Map = Map::new_with_element(&container, &JsValue::NULL);

    add_tile_layer2(&leaflet_map);

    // TODO: FetchService::fetch accepts two parameters: a Request object and a Callback.
    // https://yew.rs/docs/0.18.0/concepts/services/fetch
    //let territories = use_state(|| vec![]);

    let model_clone = model.clone();
    use_effect_with((),
        move |_| {
            let model_clone = model_clone.clone();
            wasm_bindgen_futures::spawn_local(async move {
                //let uri: &str = "/data/territory-borders-all.json";
                //let uri: &str = "/api/territories/borders";

                let group_id: String = group_id;
                // TODO: Try activeGroupId instead of groupId, needs to be set up in the API too
                let uri: String =
                    format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

                if !model_clone.territories_is_loaded {
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

                    model_clone.set(m);
                }
            });
            || ()
        }
    );

    let model_clone = model.clone();
    let tcount: usize = model_clone.territories.len();
    log!("Map Comp Loaded Territories 2: ", tcount);
    // Figure out how to show when there is no data
    if tcount == 0 {
        return html! {
            <div style={"width:100%;"}>
            {
                {map_container}
            }
            </div>
        };
    }

    // TerritorySummary // let mut available_count = 0;
    // TerritorySummary // let mut signed_out_count = 0;
    // TerritorySummary // let mut completed_count = 0;
    // TerritorySummary // let mut total_count = 0;
    // TerritorySummary // let mut hidden_count = 0;

    let _bounds: LatLngBounds =
        LatLngBounds::new(&LatLng::new(47.66, -122.20), &LatLng::new(47.76, -122.30));

    for t in model.territories.iter() {
        // TerritorySummary // total_count += 1;

        let mut polygon: Vec<LatLng> = Vec::new();

        for v in &t.border {
            if v.len() > 1 {
                //log!("Vertex: {},{}", v[0],v[1]);
                polygon.push(LatLng::new(v[0].into(), v[1].into()));
            }
        }

        let completed_by: String = {
            match t.last_completed_by {
                Some(_) => "yes".to_string(),
                None => "no".to_string(),
            }
        };

        let _group_id: String = {
            match &t.group_id {
                Some(v) => v.to_string(),
                None => "".to_string(),
            }
        };

        let area_code: String = {
            match t.area_code {
                Some(_) => t.area_code.clone().unwrap(),
                None => "".to_string(),
            }
        };

        let territory_color: String = {
            if area_code == "TER" {
                "red".to_string()
            } else if t.status == "Signed-out" {
                // TerritorySummary // signed_out_count += 1;
                "magenta".to_string()
            } else if t.status == "Completed" || t.status == "Available" && completed_by == "yes" {
                // TerritorySummary // completed_count += 1;
                "blue".to_string() // Completed
            } else if t.status == "Available" {
                // TerritorySummary // available_count += 1;
                "black".to_string()
            } else {
                "#090".to_string()
            }
        };

        let opacity: f32 = 1.0;

        if area_code == "TER" {
            let polyline = Polyline::new_with_options(
                polygon.iter().map(JsValue::from).collect(),
                &serde_wasm_bindgen::to_value(&PolylineOptions {
                    color: territory_color,
                    opacity: 1.0,
                })
                .expect("Unable to serialize polygon options"),
            );

            // bounds = polyline.getBounds();
            // leaflet_map.fitBounds(&bounds);

            polyline.addTo(&leaflet_map);
            // TerritorySummary // hidden_count += 1;
        } else {
            let poly = Polygon::new_with_options(
                polygon.iter().map(JsValue::from).collect(),
                &serde_wasm_bindgen::to_value(&PolylineOptions {
                    color: territory_color,
                    opacity: opacity,
                })
                .expect("Unable to serialize polygon options"),
            );

            if t.number == *"10001" {
                let bounds = poly.getBounds();
                leaflet_map.fitBounds(&bounds);
            }

            let tooltip_text: String = t.number.clone().to_string();

            let popup_options = PopupContentOptions {
                edit_territory_button_enabled: true,
                territory_open_enabled: false,
                show_stage: false,
                as_of_date: None,
            };

            let popup_text = popup_content_w_button(&t, popup_options); //true, false);

            if t.border.len() > 2 {
                poly.bindTooltip(
                    &JsValue::from_str(&tooltip_text),
                    &serde_wasm_bindgen::to_value(&TooltipOptions {
                        sticky: true,
                        permanent: false,
                        opacity: 0.9,
                    })
                    .expect("Unable to serialize tooltip options"),
                );
            }

            poly.bindPopup(
                &JsValue::from_str(&popup_text),
                &serde_wasm_bindgen::to_value(&PopupOptions { auto_close: true })
                    .expect("Unable to serialize popup options"),
            );

            if !t.is_hidden && t.group_id.clone().unwrap_or("".to_string()) != "outer".to_string() {
                poly.addTo(&leaflet_map);
            }
        }
    }

    // from rendered
    //if !model_clone.local_load {
        
    leaflet_map.setView(&LatLng::new(47.66, -122.20), 11.0);
    
    //let clickedLatLng = leaflet_map.layerPointToLatLng(&Point::new(50,50));
    //log!(format!("clickedLatLng: {},{}", clickedLatLng.lat(), clickedLatLng.lng()));

    // // // //leaflet_map.setView(&LatLng::new(model_clone.lat, model_clone.lon), model_clone.zoom);
    //}
    ////leaflet_map.on("onclick", clicked_map);

    // move |event: MouseEvent| {
    //     println!("Map clicked at {:?}", event.latlng());
    // });

    // let legendBoxProps = leaflet::Control::extend(
    //     &serde_wasm_bindgen::to_value(&LegendBoxOptions {
    //         position: "bottomright".to_string()
    //     }).expect("Unable to serialize legend box options")
    //     );

    // let legendBox = leaflet::Control {};
    // legendBox.addTo(&leaflet_map);

    // We must wait for 1/10th of a second for the browser to be ready
    // let leaflet_map_clone = leaflet_map.clone();
    // Timeout::new(
    //     100,
    //     move || {
    //         let _ = &leaflet_map.invalidateSize(false); // Parameter name: animate
    //     }).forget();

    // TODO: Work on this later
    // let leaflet_map_clone = leaflet_map.clone();
    // let group_4_onclick = {
    //     //let props_onclick = props.onclick.clone();
    //     let model_clone = model.clone();
    //     Callback::from(move |_event: MouseEvent| {
    //         // if let Some(props_onclick) = props_onclick.clone() {
    //         //     props_onclick.emit(event);
    //         // }
    //         log!("Popup click works");

    //         let popupLatLng = &LatLng::new(47.66,  -122.20);
    //         let newLayer = Layer::default();
    //         newLayer.addTo(&leaflet_map);
    //         newLayer.openPopup_with_latlng(popupLatLng);
    //     })
    // };

    let group_inner_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "*");
        })
    };

    let _group_all_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "all");
        })
    };

    let group_core_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "core");
        })
    };

    let group_1_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "1");
        })
    };

    let group_2_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "2");
        })
    };

    let group_3_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "3");
        })
    };

    let group_4_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "4");
        })
    };

    let group_5_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "5");
        })
    };

    let group_6_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "6");
        })
    };

    let group_7_onclick = {
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            setup_filter(model_clone.clone(), "7");
        })
    };

    
    let mouse_click_model_clone = mouse_click_model.clone();
    let leaflet_map_clone = leaflet_map.clone();
    let _map_cover_click = {
        let mouse_click_model_clone = mouse_click_model_clone.clone();
        let _leaflet_map_clone = leaflet_map_clone.clone();
        //let model_clone = model.clone();
        Callback::from(move |event: MouseEvent| {
            //print_click_lat_lng(leaflet_map.clone());
            log!(format!("Map cover clicked {}, {}", event.x(), event.y()));
            let mouse_thing = MouseClickModel {
                mouse_click_x: event.x(),
                mouse_click_y: event.y(),
            };
            mouse_click_model_clone.set(mouse_thing);
            
            // TODO: Figure this out, leafelet_map gets moved here
            // let latLng = &leaflet_map.layerPointToLatLng(
            //     &Point::new(
            //         event.x() as u32, 
            //         event.y() as u32));
        })
    };
    
    let bnds = LatLngBounds::new(&LatLng::new(47.66, -122.00), &LatLng::new(47.46, -122.20));

    // leaflet_map.fitBounds(
    //     &LatLngBounds::new(
    //         &LatLng::new(47.66, -122.00),
    //         &LatLng::new(47.46, -122.20)));

    let bounds_lat_ne = leaflet_map.getBounds().getNorthEast().lat();
    let bounds_lng_ne = leaflet_map.getBounds().getNorthEast().lng();
    let bounds_lat_sw = leaflet_map.getBounds().getSouthWest().lat();
    let bounds_lng_sw = leaflet_map.getBounds().getSouthWest().lng();

    log!(format!(
        "Bounds Last {},{}  {},{} -- {},{} {},{}",
        bounds_lat_ne.to_string(),
        bounds_lng_ne.to_string(),
        bounds_lat_sw.to_string(),
        bounds_lng_sw.to_string(),
        bnds.getNorthEast().lat(),
        bnds.getNorthEast().lng(),
        bnds.getSouthWest().lat(),
        bnds.getSouthWest().lng()
    ));

    // let mouse_click_model = mouse_click_model.clone();
    // let map_box_click = {
    //     let mouse_click_model = mouse_click_model.clone();
    //     Callback::from(move |event: MouseEvent| {
    //         let x = event.client_x();
    //         let y = event.client_y();
    //         let mcm = MouseClickModel {
    //             mouse_click_x: x,
    //             mouse_click_y: y,
    //         };
    //         mouse_click_model.set(mcm);
    //         log!(format!("Mouse click x,y: {},{}", x, y));
    //     })
    // };

    let search_state_clone = search_state.clone();
    let search_text_onchange = {
        let search_state_clone = search_state_clone.clone();
        Callback::from(move |event: Event| {
            let mut modification = search_state_clone.deref().clone();
            let value = event
                .target()
                .expect("An input value for an HtmlInputElement")
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.search_text = value;
            search_state_clone.set(modification);
        })
    };

    let search_state_clone = search_state.clone();
    let _model_clone = model.clone();
    let search_clear_onclick = {
        let search_state_clone = search_state_clone.clone();
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            log!("search_clear_onclick");
            //let mut modification = search_state_clone.deref().clone();
            // let search_state_clone = search_state_clone.clone();
            let modification = TerritorySearchPage {
                count: 0,
                load_error: false,
                load_error_message: "".to_string(),
                search_text: "".to_string(),
                success: true,
                territories: vec![],
            };
            //modification.search_text = "".into();
            search_state_clone.set(modification);

            // let model_clone = model_clone.clone();
            // setup_number_filter(model_clone.clone(), "");

            //let search_state_clone = search_state_clone.clone();
            let model_clone = model_clone.clone();
            spawn_local(async move {
                //let search_state_clone = search_state_clone.clone();
                let model_clone = model_clone.clone();
                //let search_text = search_state_clone.search_text.clone();

                setup_number_filter(model_clone.clone(), "");
            });
        })
    };
    
    let cb: Callback<String, String> = Callback::from(move |name: String| {
        // let bounds = LatLngBounds::new(&LatLng::new(47.66, -122.00), &LatLng::new(47.46, -122.20));
        // leaflet_map.fitBounds(&bounds);
        format!("Bye {}", name)
    });

    let cb_clone = cb.clone();
    //let leaflet_map_clone: &Map = &leaflet_map;
    let search_state_clone = search_state.clone();
    let model_clone = model.clone();
    let search_text_onsubmit = Callback::from(move |event: SubmitEvent| {
        event.prevent_default();
        let cb_clone = cb_clone.clone();
        //let leaflet_map_clone = leaflet_map_clone.clone();
        let search_state_clone = search_state_clone.clone();
        let model_clone = model_clone.clone();
        spawn_local(async move {
            //let leaflet_map_clone = leaflet_map_clone.clone();
            let search_state_clone = search_state_clone.clone();
            let model_clone = model_clone.clone();
            let search_text = search_state_clone.search_text.clone();
            //if !search_text.is_empty() {
            // TODO: Clear search results if nothing is returned
            // TODO: Leave search text in the search box?

            // let result = get_territories(search_text).await;
            // cloned_state.set(result);
            // }

            setup_number_filter(model_clone.clone(), &search_text);

            let _bounds =
                LatLngBounds::new(&LatLng::new(47.66, -122.00), &LatLng::new(47.46, -122.20));

            //&leaflet_map.fitBounds(&bounds);
            let _result = cb_clone.emit(String::from("Bob")); 
        });
    });

    let _search_state_clone = search_state.clone();

    let _lat_lng = &leaflet_map.layerPointToLatLng(
        &Point::new(
            mouse_click_model.mouse_click_x as u32, 
            mouse_click_model.mouse_click_y as u32));
    
    // This seems to only work if it's last, it doesn't like clones of leaflet_map
    Timeout::new(100, move || {
        let _ = &leaflet_map.invalidateSize(false); // Parameter name: animate
    })
    .forget();

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
                                            value={search_state.search_text.clone()}
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
                                    //<span>{"  Mouse: "}{mouse_click_model.mouse_click_x}{","}{mouse_click_model.mouse_click_y}</span>
                                    //<span>{"  LatLng: "}{format!("{:.4},{:.4}",lat_lng.lat(),lat_lng.lng())}</span>
                                </div>
                            </div>
                        </li>
                    </ul>
                </MenuBarV2>
            </div>
            <div style="height: calc(100% - 57px);background-color:blue;">
                {
                    {map_container}
                }
                // <HomeButton />
                // <AssignPageLink />
                <MapMenu
                    bottom_vh={1}
                    svg_path_d="M6 3.5A1.5 1.5 0 0 1 7.5 2h1A1.5 1.5 0 0 1 10 3.5v1A1.5 1.5 0 0 1 8.5 6v1H14a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-1 0V8h-5v.5a.5.5 0 0 1-1 0V8h-5v.5a.5.5 0 0 1-1 0v-1A.5.5 0 0 1 2 7h5.5V6A1.5 1.5 0 0 1 6 4.5v-1zm-6 8A1.5 1.5 0 0 1 1.5 10h1A1.5 1.5 0 0 1 4 11.5v1A1.5 1.5 0 0 1 2.5 14h-1A1.5 1.5 0 0 1 0 12.5v-1zm6 0A1.5 1.5 0 0 1 7.5 10h1a1.5 1.5 0 0 1 1.5 1.5v1A1.5 1.5 0 0 1 8.5 14h-1A1.5 1.5 0 0 1 6 12.5v-1zm6 0a1.5 1.5 0 0 1 1.5-1.5h1a1.5 1.5 0 0 1 1.5 1.5v1a1.5 1.5 0 0 1-1.5 1.5h-1a1.5 1.5 0 0 1-1.5-1.5v-1z"
                >
                    <div>
                        <button onclick={group_core_onclick} class="btn btn-primary" aria-label="Core">{"C"}</button>
                        <button onclick={group_1_onclick} class="btn btn-primary" aria-label="1">{"1"}</button>
                        <button onclick={group_2_onclick} class="btn btn-primary" aria-label="2">{"2"}</button>
                        <button onclick={group_3_onclick} class="btn btn-primary" aria-label="3">{"3"}</button>
                        <button onclick={group_4_onclick} class="btn btn-primary" aria-label="4">{"4"}</button>
                        <button onclick={group_5_onclick} class="btn btn-primary" aria-label="5">{"5"}</button>
                        <button onclick={group_6_onclick} class="btn btn-primary" aria-label="6">{"6"}</button>
                        <button onclick={group_7_onclick} class="btn btn-primary" aria-label="7">{"7"}</button>
                        <button onclick={group_inner_onclick} class="btn btn-primary" aria-label="Inner">{"*"}</button>
                        //<button onclick={group_all_onclick} class="btn btn-primary" aria-label="All">{"A"}</button>
                    </div>
                </MapMenu>
            </div>
        </div>
    }
}

fn setup_number_filter(model: UseStateHandle<TerritoryMapModel>, number: &str) {
    log!("setup_number_filter {}", number);
    if number.to_string().is_empty() {
        log!("setup_number_filter Empty");
    }
    let model_clone = model.clone();
    let territories = model_clone.territories.clone();
    let tcount: usize = territories.len();
    log!(format!("Territories already loaded: {}", tcount));
    let mut new_territories: Vec<Territory> = vec![];
    for t in territories.iter() {
        //t.is_visible = false;
        let nt = Territory {
            id: t.id.clone(),
            number: t.number.clone(),
            status: t.status.clone(),
            stage_id: t.stage_id.clone(),
            stage: t.stage.clone(),
            description: t.description.clone(),
            notes: t.notes.clone(),
            address_count: t.address_count.clone(),
            area_code: t.area_code.clone(),
            last_completed_by: t.last_completed_by.clone(),
            signed_out_to: t.signed_out_to.clone(),
            signed_out: t.signed_out.clone(),
            assignee_link_key: t.assignee_link_key.clone(),
            group_id: t.group_id.clone(),
            sub_group_id: t.sub_group_id.clone(),
            is_hidden: t.group_id.clone().unwrap_or("".to_string()) == "outer".to_string(),
            is_active: (number.to_string().is_empty()
                || t.number.clone() == number.to_string()
                || t.signed_out_to.clone() == Some(number.to_string())
                || t.description
                    .clone()
                    .unwrap_or("".to_string())
                    .contains(number)
                || format!("g{}", t.group_id.clone().unwrap_or("".to_string()))
                    == number.to_string()
                || t.status.clone() == number.to_string()),
            border: t.border.clone(),
            addresses: t.addresses.clone(),
            ..Territory::default()
        };
        new_territories.push(nt);
    }

    let m = TerritoryMapModel {
        territories: new_territories, //model_clone.territories.clone(),
        territories_is_loaded: false,
        local_load: true,
        lat: model_clone.lat,
        lon: model_clone.lon,
        zoom: model_clone.zoom, //leaflet_map_clone.getZoom(),
        group_visible: model_clone.group_visible.clone(),
    };

    let _mcm = MouseClickModel {
        mouse_click_x: 0,
        mouse_click_y: 0,
    };

    model.set(m);
}

fn _print_click_lat_lng(map: &Map) {
    let clicked_lat_lon = map.layerPointToLatLng(&Point::new(50,50));
    log!(format!("From Map Cover: clickedLatLng: {},{}", clicked_lat_lon.lat(), clicked_lat_lon.lng()));
}

fn setup_filter(model: UseStateHandle<TerritoryMapModel>, group: &str) {
    let model_clone = model.clone();
    let territories = model_clone.territories.clone();
    let tcount: usize = territories.len();
    log!(format!("Territories already loaded: {}", tcount));
    let mut new_territories: Vec<Territory> = vec![];
    for t in territories.iter() {
        //t.is_visible = false;
        let nt = Territory {
            id: t.id.clone(),
            number: t.number.clone(),
            status: t.status.clone(),
            stage_id: t.stage_id.clone(),
            stage: t.stage.clone(),
            description: t.description.clone(),
            notes: t.notes.clone(),
            address_count: t.address_count.clone(),
            area_code: t.area_code.clone(),
            last_completed_by: t.last_completed_by.clone(),
            signed_out_to: t.signed_out_to.clone(),
            signed_out: t.signed_out.clone(),
            assignee_link_key: t.assignee_link_key.clone(),
            group_id: t.group_id.clone(),
            sub_group_id: t.sub_group_id.clone(),            
            is_hidden: t.group_id.clone().unwrap_or("".to_string()) == "outer".to_string(),
            is_active: (t.group_id.clone().unwrap() == group.to_string()
                || (group == "*" && t.group_id.clone().unwrap() != "outer".to_string())
                || (group == "all")),
            border: t.border.clone(),
            addresses: t.addresses.clone(),
            ..Territory::default()
        };
        new_territories.push(nt);
    }

    let m = TerritoryMapModel {
        territories: new_territories, //model_clone.territories.clone(),
        territories_is_loaded: false,
        local_load: true,
        lat: model_clone.lat,
        lon: model_clone.lon,
        zoom: model_clone.zoom, //leaflet_map_clone.getZoom(),
        group_visible: model_clone.group_visible.clone(),
    };

    let _mcm = MouseClickModel {
        mouse_click_x: 0,
        mouse_click_y: 0,
    };

    model.set(m);
}

#[function_component(HomeButton)]
fn home_button() -> Html {
    html! {
        <a
        style="
            position: absolute;
            height: auto;
            max-height: 60px;
            /*top: calc(max(100px, 40%) + 2vh);*/
            top: calc(56px + 1vh);
            right: 1vh;
            margin-right: 1vh; 
            width: auto; 
            z-index: 1000; /*Just above 'Leaflet' in the bottom right corner*/
        "
        class="btn btn-primary"
        href="/">
        <svg
            xmlns="http://www.w3.org/2000/svg"
            width="16"
            height="16"
            fill="currentColor"
            class="bi bi-house-fill"
            viewBox="0 0 16 16">
            <path d="M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L8 2.207l6.646 6.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.707 1.5Z"/>
            <path d="m8 3.293 6 6V13.5a1.5 1.5 0 0 1-1.5 1.5h-9A1.5 1.5 0 0 1 2 13.5V9.293l6-6Z"/>
        </svg>
    </a>
    }
}

// #[function_component(AssignPageLink)]
// fn assign_page_link() -> Html {
//     let navigator = use_navigator().unwrap();

//     let onclick = Callback::from(move |_| navigator.push(&Route::Assign { id: "4".to_string(), description: "".to_string(), assignee_name: "".to_string() }));
//     html! {

//         <button {onclick}
//             style={"
//                 position: absolute;
//                 height: auto;
//                 max-height: 60px;
//                 top: 5vh;
//                 right: 1vh;
//                 margin-right: 1vh;
//                 width: auto;
//                 z-index: 1000;"}
//                 class={"btn btn-primary"}>
//             <svg xmlns={"http://www.w3.org/2000/svg"} width={"16"} height={"16"} fill={"currentColor"} class={"bi bi-card-checklist"} viewBox={"0 0 16 16"}>
//                 <path d={"M14.5 3a.5.5 0 0 1 .5.5v9a.5.5 0 0 1-.5.5h-13a.5.5 0 0 1-.5-.5v-9a.5.5 0 0 1 .5-.5h13zm-13-1A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h13a1.5 1.5 0 0 0 1.5-1.5v-9A1.5 1.5 0 0 0 14.5 2h-13z"}/>
//                 <path d={"M7 5.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 1 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0zM7 9.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 0 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0z"}/>
//             </svg>
//         </button>

//     }
// }

// #[function_component(Secure)]
// fn secure() -> Html {
//     let navigator = use_navigator().unwrap();

//     let onclick = Callback::from(move |_| navigator.push(&Route::Home));
//     html! {
//         <div>
//             <h1>{ "Secure"} }</h1>
//             <button {onclick}>{ "Go Home"} }</button>
//         </div>
//     }
// }

fn add_tile_layer2(map: &Map) {
    TileLayer::new(
        "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
        &JsValue::NULL,
    )
    .addTo(map);
}

#[derive(Serialize, Deserialize)]
struct LegendBoxOptions {
    position: String,
}

#[derive(Serialize, Deserialize)]
struct PolylineOptions {
    color: String,
    opacity: f32,
}

#[derive(Serialize, Deserialize)]
struct TooltipOptions {
    sticky: bool,
    permanent: bool,
    //direction: String,
    opacity: f32,
    //className: String
}

#[derive(Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
struct PopupOptions {
    auto_close: bool,
}

fn render_map(element: &HtmlElement) -> Html {
    // Element must be passed as an address I guess
    let node: &Node = &element.clone().into();
    Html::VRef(node.clone())
}

//use yew::{Component, Context, html, Html};

// pub fn clicked_map(event: &MouseEvent) -> JsValue {
//     log!("You clicked the map!")
// }


