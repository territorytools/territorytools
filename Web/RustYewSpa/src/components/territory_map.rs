use crate::components::popup_content::popup_content;
use crate::components::map_menu::MapMenu;
use crate::models::territories::{Territory};
use crate::functions::document_functions::set_document_title;

use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use leaflet::{LatLng, Map, TileLayer, Polygon, Polyline};
use reqwasm::http::{Request};
use yew::prelude::*;
use gloo_utils::document;
use gloo_console::log;
use gloo_timers::callback::Timeout;
use serde::{Serialize, Deserialize};
//use js_sys::{Array, Date};
use web_sys::{
    Element,
    HtmlElement,
    Node
};
use yew_router::hooks::use_location;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
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
}

#[function_component(TerritoryMap)]
pub fn territory_map() -> Html {
    set_document_title("Territory Map");
    
    let model = use_state(|| TerritoryMapModel::default());
    let location = use_location().expect("Should be a location to get query string");
    //log!("territory_map Query: {}", location.query_str());
    let parameters: TerritoryMapParameters = location.query().expect("An object");
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
    use_effect_with_deps(move |_| {
        let model_clone = model_clone.clone();
        wasm_bindgen_futures::spawn_local(async move {
            
            //let uri: &str = "/data/territory-borders-all.json";
            //let uri: &str = "/api/territories/borders";

            let group_id: String = group_id;
            // TODO: Try activeGroupId instead of groupId, needs to be set up in the API too
            let uri: String = format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

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
                        zoom: 11.0
                    };

                    model_clone.set(m);
            }
        });
        || ()
    }, ());


    let model_clone = model.clone();
    let tcount: usize = model_clone.territories.len();
    log!("Map Comp Loaded Territories 2: ", tcount);
    // Figure out how to show when there is no data
    if tcount == 0 {
        return html!{
            <div style={"width:100%;"}>     
            {
                {map_container}
            }   
            </div>
        }
    }
    
    // TerritorySummary // let mut available_count = 0;
    // TerritorySummary // let mut signed_out_count = 0;
    // TerritorySummary // let mut completed_count = 0;
    // TerritorySummary // let mut total_count = 0;
    // TerritorySummary // let mut hidden_count = 0;

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
                None => "no".to_string()
            }
        };

        let group_id: String = {
            match &t.group_id {
                Some(v) => v.to_string(),
                None => "".to_string()
            }
        };
        
        let area_code: String = {
            match t.area_code {
                Some(_) => t.area_code.clone().unwrap(),
                None => "".to_string()
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

        let opacity: f32 = {
            if t.is_active { 1.0 } else { 0.1 }
        };

        if area_code == "TER" {
            let polyline = Polyline::new_with_options(polygon.iter().map(JsValue::from).collect(),
            &serde_wasm_bindgen::to_value(&PolylineOptions { 
                color: territory_color.into(),
                opacity: 1.0,
            }).expect("Unable to serialize polygon options")
            );
            
            let bounds = polyline.getBounds();
            // this might be the bad one// leaflet_map.fitBounds(&bounds);

            polyline.addTo(&leaflet_map);
            // TerritorySummary // hidden_count += 1;
        } else {
            
            let poly = Polygon::new_with_options(polygon.iter().map(JsValue::from).collect(),
            &serde_wasm_bindgen::to_value(&PolylineOptions { 
                color: territory_color.into(),
                opacity: opacity.into(),
            }).expect("Unable to serialize polygon options")
            );
            
            let tooltip_text: String = format!(
                "{group_id}: {area_code}: {}", 
                t.number);

            let popup_text = popup_content(&t);
            
            if t.border.len() > 2 {
                poly.bindTooltip( 
                    &JsValue::from_str(&tooltip_text), 
                    &serde_wasm_bindgen::to_value(&TooltipOptions {
                        sticky: true,
                        permanent: false,
                        opacity: 0.9
                    }).expect("Unable to serialize tooltip options")
                );
            }

            poly.bindPopup( 
                &JsValue::from_str(&popup_text), 
                &serde_wasm_bindgen::to_value(&PopupOptions {
                    auto_close: true
                }).expect("Unable to serialize popup options")
            );
        
            if !t.is_hidden {
                poly.addTo(&leaflet_map);
            }
        }
    }

    // from rendered
    //if !model_clone.local_load {
        //leaflet_map.setView(&LatLng::new(47.66, -122.20), 11.0);
        leaflet_map.setView(&LatLng::new(model_clone.lat, model_clone.lon), model_clone.zoom);
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
    
    let _model_clone = model.clone();
    let leaflet_map_clone = leaflet_map.clone();
    let trick_onclick = {
        //let props_onclick = props.onclick.clone();
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            // if let Some(props_onclick) = props_onclick.clone() {
            //     props_onclick.emit(event);
            // }
            log!("Click works");
            
            let territories = model_clone.territories.clone();
            let tcount: usize = territories.len();
            log!(format!("Territories already loaded: {}", tcount));
            let mut new_territories: Vec<Territory> = vec![];
            for t in territories.iter() {     
                //t.is_visible = false;
                let nt = Territory {
                    number: t.number.clone(),
                    status: t.status.clone(),
                    description: t.description.clone(),
                    address_count: t.address_count.clone(),
                    area_code: t.area_code.clone(),
                    last_completed_by: t.last_completed_by.clone(),
                    signed_out_to: t.signed_out_to.clone(),
                    group_id: t.group_id.clone(),
                    sub_group_id: t.sub_group_id.clone(),
                    is_active: t.is_active.clone(),
                    is_hidden: if t.status.clone() == "Available".to_string() {
                        true
                    } else { false },
                    border: t.border.clone(),
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
            };

            model_clone.set(m);
        })
    };

    let leaflet_map_clone = leaflet_map.clone();
    let group_2_onclick = {
        //let props_onclick = props.onclick.clone();
        let model_clone = model.clone();
        Callback::from(move |_event: MouseEvent| {
            // if let Some(props_onclick) = props_onclick.clone() {
            //     props_onclick.emit(event);
            // }
            log!("Click works");
            
            let territories = model_clone.territories.clone();
            let tcount: usize = territories.len();
            log!(format!("Territories already loaded: {}", tcount));
            let mut new_territories: Vec<Territory> = vec![];
            for t in territories.iter() {     
                //t.is_visible = false;
                let nt = Territory {
                    number: t.number.clone(),
                    status: t.status.clone(),
                    description: t.description.clone(),
                    address_count: t.address_count.clone(),
                    area_code: t.area_code.clone(),
                    last_completed_by: t.last_completed_by.clone(),
                    signed_out_to: t.signed_out_to.clone(),
                    group_id: t.group_id.clone(),
                    sub_group_id: t.sub_group_id.clone(),
                    is_active: t.is_active.clone(),
                    is_hidden: if t.group_id.clone() != Some("2".to_string()) {
                        true
                    } else { false },
                    border: t.border.clone(),
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
            };

            model_clone.set(m);
        })
    };

    // This seems to only work if it's last, it doesn't like clones of leaflet_map
    Timeout::new(
        100,
        move || {
            let _ = &leaflet_map.invalidateSize(false); // Parameter name: animate
        }).forget();

    html!{
        <div style={"width:100%;"}>        
            {
                {map_container}
            }
            <HomeButton />
            // <AssignPageLink />
            <MapMenu

                bottom_vh={1}
                svg_path_d={"M6 3.5A1.5 1.5 0 0 1 7.5 2h1A1.5 1.5 0 0 1 10 3.5v1A1.5 1.5 0 0 1 8.5 6v1H14a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-1 0V8h-5v.5a.5.5 0 0 1-1 0V8h-5v.5a.5.5 0 0 1-1 0v-1A.5.5 0 0 1 2 7h5.5V6A1.5 1.5 0 0 1 6 4.5v-1zm-6 8A1.5 1.5 0 0 1 1.5 10h1A1.5 1.5 0 0 1 4 11.5v1A1.5 1.5 0 0 1 2.5 14h-1A1.5 1.5 0 0 1 0 12.5v-1zm6 0A1.5 1.5 0 0 1 7.5 10h1a1.5 1.5 0 0 1 1.5 1.5v1A1.5 1.5 0 0 1 8.5 14h-1A1.5 1.5 0 0 1 6 12.5v-1zm6 0a1.5 1.5 0 0 1 1.5-1.5h1a1.5 1.5 0 0 1 1.5 1.5v1a1.5 1.5 0 0 1-1.5 1.5h-1a1.5 1.5 0 0 1-1.5-1.5v-1z"}>
                <div>
                // TODO: Try saving the JSON locally, even just in memory, and filtering it here in the browser
                    <button onclick={trick_onclick} class="btn btn-primary">{"#"}</button>
                    <a href={"/app/map?group_id=core"} class={"btn btn-primary"}>{"0"}</a>
                    <a href={"/app/map?group_id=1"} class={"btn btn-primary"}>{"1"}</a>
                    //<a href={"/app/map?group_id=2"} class={"btn btn-primary"}>{"2"}</a>
                    <button onclick={group_2_onclick} class="btn btn-primary">{"2"}</button>
                    <a href={"/app/map?group_id=3"} class={"btn btn-primary"}>{"3"}</a>
                    <a href={"/app/map?group_id=4"} class={"btn btn-primary"}>{"4"}</a>
                    <a href={"/app/map?group_id=5"} class={"btn btn-primary"}>{"5"}</a>
                    <a href={"/app/map?group_id=6"} class={"btn btn-primary"}>{"6"}</a>
                    <a href={"/app/map?group_id=7"} class={"btn btn-primary"}>{"7"}</a>
                    <a href={"/app/map"} class={"btn btn-primary"}>{"*"}</a>
                </div>
            </MapMenu>       
            // <MapMenu 
            //     bottom_vh={1}
            //     svg_path_d={"M12.433 10.07C14.133 10.585 16 11.15 16 8a8 8 0 1 0-8 8c1.996 0 1.826-1.504 1.649-3.08-.124-1.101-.252-2.237.351-2.92.465-.527 1.42-.237 2.433.07zM8 5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm4.5 3a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zM5 6.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm.5 6.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z"}>        
            //     <TerritorySummary 
            //         available={available_count}
            //         signed_out={signed_out_count}
            //         completed={completed_count}
            //         total={total_count}
            //         hidden={hidden_count} />      
            // </MapMenu>            
        </div>
    }
}


#[function_component(HomeButton)]
fn home_button() -> Html {
    html! {
        <a 
        style={"
            position: absolute;
            height: auto;
            max-height: 60px;
            top: 1vh;
            right: 1vh;
            margin-right: 1vh; 
            width: auto; 
            z-index: 1000; /*Just above 'Leaflet' in the bottom right corner*/
        "}
        class={"btn btn-primary"}
        href={"/"}>
        <svg 
            xmlns={"http://www.w3.org/2000/svg"}
            width={"16"}
            height={"16"}
            fill={"currentColor"}
            class={"bi bi-house-fill"}
            viewBox={"0 0 16 16"}>
            <path d={"M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L8 2.207l6.646 6.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.707 1.5Z"}/>
            <path d={"m8 3.293 6 6V13.5a1.5 1.5 0 0 1-1.5 1.5h-9A1.5 1.5 0 0 1 2 13.5V9.293l6-6Z"}/>
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