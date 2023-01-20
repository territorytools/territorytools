use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
use crate::components::map_menu::MapMenu;
use crate::components::route_stuff::Route;
use crate::models::territories::{Territory};
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use leaflet::{LatLng, Map, TileLayer, Polygon, Polyline, Control};
use reqwasm::http::{Request};
use yew::prelude::*;
use yew_router::prelude::*;
use gloo_utils::document;
use gloo_console::log;
use gloo_timers::callback::Timeout;
use serde::{Serialize, Deserialize};
//use js_sys::{Array, Date};
use web_sys::{
    Document,
    Element,
    HtmlElement,
    Window,
    Node
};

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

#[function_component(TerritoryMap)]
pub fn territory_map() -> Html {
    // from create
    let container: Element = document().create_element("div").unwrap();
    let container: HtmlElement = container.dyn_into().unwrap();
    let map_container = render_map(&container);
    // From render_map
    //let node: &Node = container.clone().into();
    // from create
    container.set_class_name("map");
    let leaflet_map = Map::new_with_element(&container, &JsValue::NULL);
    
    add_tile_layer2(&leaflet_map);
    
    let territories = use_state(|| vec![]);
    {
        let territories = territories.clone();
        use_effect_with_deps(move |_| {
            let territories = territories.clone();
            wasm_bindgen_futures::spawn_local(async move {
                
                //let uri: &str = "/data/territory-borders-all.json";
                //let uri: &str = "/api/territories/borders";
                let uri: &str = DATA_API_PATH;

                let fetched_territories: Vec<Territory> = Request::get(uri)
                    .send()
                    .await
                    .unwrap()
                    .json()
                    .await
                    .unwrap();
                territories.set(fetched_territories);
            });
            || ()
        }, ());
    }

    let tcount: usize = territories.len();
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
    
    let mut available_count = 0;
    let mut signed_out_count = 0;
    let mut completed_count = 0;
    let mut total_count = 0;
    let mut hidden_count = 0;
    for t in territories.iter() {      
        total_count += 1;

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

        // let description: String = {
        //     match t.description {
        //         Some(_) => t.description.clone().unwrap(),
        //         None => "".to_string()
        //     }
        // };

        // let status: String = {
        //     if t.status == "Available" && completed_by == "yes" {
        //         "Completed".to_string()
        //     } else {
        //         t.status.clone()
        //     }
        // };
        
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
                //"#b00".to_string()
                signed_out_count += 1;
                "magenta".to_string()
            } else if t.status == "Completed" || t.status == "Available" && completed_by == "yes" {
                //"#b0b".to_string() // Completed
                completed_count += 1;   
                "blue".to_string() // Completed
            } else if t.status == "Available" {
                //"#009".to_string()
                available_count += 1;
                "black".to_string()
            } else {
                "#090".to_string()
            }
        };

        if area_code != "TER" {
            hidden_count += 1;
        }

        if area_code == "TER" {
            let plolyline = Polyline::new_with_options(polygon.iter().map(JsValue::from).collect(),
            &serde_wasm_bindgen::to_value(&PolylineOptions { 
                color: territory_color.into() 
            }).expect("Unable to serialize polygon options")
            );

            plolyline.addTo(&leaflet_map);
        } else {
            
            let poly = Polygon::new_with_options(polygon.iter().map(JsValue::from).collect(),
            &serde_wasm_bindgen::to_value(&PolylineOptions { 
                color: territory_color.into() 
            }).expect("Unable to serialize polygon options")
            );
            
            let tooltip_text: String = format!(
                "{} : {}", 
                area_code,
                t.number);

            let popup_text = popup_content(&t);
            
            if t.border.len() > 2 {
                poly.bindTooltip( 
                    &JsValue::from_str(&tooltip_text), 
                    &serde_wasm_bindgen::to_value(&TooltipOptions {
                        sticky: true,
                        permanent: false,
                        //direction: "center".to_string(),
                        //className: "tool-tip".to_string(),
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
        
            poly.addTo(&leaflet_map);
        }
    }

    // from rendered
    leaflet_map.setView(&LatLng::new(47.66, -122.33), 10.0);

    // let legendBoxProps = leaflet::Control::extend(
    //     &serde_wasm_bindgen::to_value(&LegendBoxOptions { 
    //         position: "bottomright".to_string()
    //     }).expect("Unable to serialize legend box options")
    //     );
    
    // let legendBox = leaflet::Control {};
    // legendBox.addTo(&leaflet_map);

    // We must wait for 1/10th of a second for the browser to be ready
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
            <MapMenu>        
                <TerritorySummary 
                    available={available_count}
                    signed_out={signed_out_count}
                    completed={completed_count}
                    total={total_count}
                    hidden={hidden_count} />      
            </MapMenu>            
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
