use crate::libs::leaflet::{LatLng, Polygon};
use crate::components::popup_content::popup_content;
use crate::models::territories::Territory;

use wasm_bindgen::{prelude::*};
use serde::{Deserialize, Serialize};

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

pub fn polygon_from_territory(t: &Territory) -> Polygon {
    let mut polygon: Vec<LatLng> = Vec::new();

    for v in &t.border {
        if v.len() > 1 {
            polygon.push(LatLng::new(v[0].into(), v[1].into()));
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

    let opacity: f32 = {
        if t.is_active {
            1.0
        } else {
            0.01
        }
    };

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
        let poly = Polygon::new_with_options(
            polygon.iter().map(JsValue::from).collect(),
            &serde_wasm_bindgen::to_value(&PolylineOptions {
                color: territory_color.into(),
                opacity: opacity.into(),
            })
            .expect("Unable to serialize polygon options"),
        );
        
        let tooltip_text: String = format!("{group_id}: {area_code}: {}", t.number);
        let popup_text = popup_content(&t);

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

        //if !t.is_hidden && t.group_id.clone().unwrap_or("".to_string()) != "outer".to_string() {
            //poly.addTo(&self.map);
            return poly
        //}
    //}
}