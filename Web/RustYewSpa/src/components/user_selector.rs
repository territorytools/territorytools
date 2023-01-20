use crate::components::territory_summary::TerritorySummary;
use crate::components::popup_content::popup_content;
use crate::models::territories::{Territory};
use crate::models::users::{User};
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use leaflet::{LatLng, Map, TileLayer, Polygon, Polyline, Control};
use reqwasm::http::{Request};
use yew::prelude::*;
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
const DATA_USERS_API_PATH: &str = "/data/users.json";

#[cfg(not(debug_assertions))]
const DATA_USERS_API_PATH: &str = "/api/users?active=true";


#[function_component(UserSelector)]
pub fn user_selector() -> Html {
    
    let users = use_state(|| vec![]);
    {
        let users = users.clone();
        use_effect_with_deps(move |_| {
            let users = users.clone();
            wasm_bindgen_futures::spawn_local(async move {
                let uri: &str = DATA_USERS_API_PATH;

                let fetched_users: Vec<User> = Request::get(uri)
                    .send()
                    .await
                    .unwrap()
                    .json()
                    .await
                    .unwrap();
                    users.set(fetched_users);
            });
            || ()
        }, ());
    }
    users.iter().map(|user| { 
        let user_full_name: String = {
            match user.alba_full_name {
                Some(_) => user.alba_full_name.clone().unwrap(),
                None => "".to_string()
            }
        };
        log!("User: {}", user_full_name);
    });

    html! {
        <select id={"user-menu"} name={"albaUserId"} class={"custom-select"}>
            <option value={"0"}>{"Select User"}</option>
            {
                
                users.iter().map(|user| {   
                    let user_full_name: String = {
                        match user.alba_full_name {
                            Some(_) => user.alba_full_name.clone().unwrap(),
                            None => "".to_string()
                        }
                    };
                    let alba_user_id: String = {
                        match user.alba_user_id {
                            Some(_) => user.alba_user_id.clone().unwrap(),
                            None => "".to_string()
                        }
                    };  
                    html!{<option value={alba_user_id.to_string()}>{user_full_name}</option>}
                }).collect::<Html>()
            }
        </select>
    }
}