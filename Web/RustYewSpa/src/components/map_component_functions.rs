use crate::libs::leaflet::{LatLng, Polygon};
use crate::components::popup_content::popup_content;
use crate::components::popup_content::popup_content_w_button;
use crate::models::territories::Territory;

use wasm_bindgen::{prelude::*};
use serde::{Deserialize, Serialize};
use yew::{html::ImplicitClone};

#[derive(Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct PopupOptions {
    auto_close: bool,
}

#[derive(Serialize, Deserialize)]
pub struct TooltipOptions {
    sticky: bool,
    permanent: bool,
    opacity: f32,
}

#[derive(Serialize, Deserialize)]
struct PolylineOptions {
    color: String,
    opacity: f32,
}

#[derive(PartialEq, Clone, Default)]
pub struct TerritoryLatLng {
    pub lat: f32,
    pub lon: f32,
}

impl ImplicitClone for TerritoryLatLng {}

#[derive(PartialEq, Clone, Default)]
pub struct TerritoryPolygon {
    pub layer_id: i32,
    pub color: String,
    pub opacity: f32,
    pub border: Vec<TerritoryLatLng>,
    pub popup_html: String,
    pub tooltip_text: String,
}

impl ImplicitClone for TerritoryPolygon {}

pub fn polygon_from_territory_polygon(tpoly: &TerritoryPolygon) -> Polygon {
    let mut vertices: Vec<LatLng> = Vec::new();
    for v in &tpoly.border {
        vertices.push(LatLng::new(v.lat.into(), v.lon.into()));
    }

    let poly = Polygon::new_with_options(
        vertices.iter().map(JsValue::from).collect(),
        &serde_wasm_bindgen::to_value(&PolylineOptions {
            color: tpoly.color.to_string(),
            opacity: tpoly.opacity,
        })
        .expect("Unable to serialize polygon options"),
    );

    if tpoly.border.len() > 2 {
        poly.bindTooltip(
            &JsValue::from_str(tpoly.tooltip_text.as_str()),
            &serde_wasm_bindgen::to_value(&TooltipOptions {
                sticky: true,
                permanent: false,
                opacity: 0.9,
            })
            .expect("Unable to serialize tooltip options"),
        );
    }

    poly.bindPopup(
        &JsValue::from_str(tpoly.popup_html.as_str()),
        &serde_wasm_bindgen::to_value(&PopupOptions { auto_close: true })
            .expect("Unable to serialize popup options"),
    );

    poly
}

pub fn tpoly_from_territory(t: &Territory) -> TerritoryPolygon {
    tpoly_from_territory_w_button(t, true)
}

pub fn tpoly_from_territory_w_button(t: &Territory, edit_territory_button_enabled: bool) -> TerritoryPolygon {
    let mut polygon: Vec<TerritoryLatLng> = Vec::new();

    for v in &t.border {
        if v.len() > 1 {
            polygon.push(TerritoryLatLng { lat: v[0].into(), lon: v[1].into()});
        }
    }

    let completed_by: String = {
        match t.last_completed_by {
            Some(_) => "yes".to_string(),
            None => "no".to_string(),
        }
    };

    let group_id: String = {
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
            "magenta".to_string()
        } else if t.status == "Completed" || t.status == "Available" && completed_by == "yes" {
            "blue".to_string() // Completed
        } else if t.status == "Available" {
            "black".to_string()
        } else {
            "#090".to_string()
        }
    };

    let opacity: f32 = 1.0;
    // {
    //     if t.is_active {
    //         1.0
    //     } else {
    //         0.01
    //     }
    // };

    // if area_code == "TER" {
    //     let polyline = Polyline::new_with_options(
    //         polygon.iter().map(JsValue::from).collect(),
    //         &serde_wasm_bindgen::to_value(&PolylineOptions {
    //             color: territory_color.into(),
    //             opacity: 1.0,
    //         })
    //         .expect("Unable to serialize polygon options"),
    //     );
        
    //     log!("NOT Fitting bounds 2...");
    //     let bounds = polyline.getBounds();
    //     //self.map.fitBounds(&bounds);
    
    //     // // //polyline.addTo(&self.map);
    //     return polyline
    // } else {
        let poly = TerritoryPolygon {
            layer_id: 0,
            color: territory_color.into(),
            opacity: opacity.into(),
            border: polygon, //.iter().map().collect(),
            tooltip_text: format!("{group_id}: {area_code}: {}", t.number),
            popup_html: popup_content_w_button(&t, false),
        };

        //if !t.is_hidden && t.group_id.clone().unwrap_or("".to_string()) != "outer".to_string() {
            //poly.addTo(&self.map);
            return poly
        //}
    //}
}

pub fn get_southwest_corner(tpolygons: Vec<TerritoryPolygon>) -> TerritoryLatLng {
    let mut south: f32 = 0.0;
    let mut west: f32 = 0.0;

    for tp in tpolygons {
        for v in tp.border {
            if south > v.lat || south == 0.0 {
                south = v.lat;
            }
            if west > v.lon  || west  == 0.0 {
                west = v.lon ;
            }
        }
    }

    TerritoryLatLng { lat: south, lon: west }
}


pub fn get_northeast_corner(tpolygons: Vec<TerritoryPolygon>) -> TerritoryLatLng {
    let mut north: f32 = 0.0;
    let mut east: f32 = 0.0;

    for tp in tpolygons {
        for v in tp.border {
            if north < v.lat || north == 0.0 {
                north = v.lat;
            }          
            if east < v.lon || east == 0.0 {
                east = v.lon ;
            }
        }
    }

    TerritoryLatLng { lat: north, lon: east }
}