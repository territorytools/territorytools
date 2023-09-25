use crate::models::users::User;
use wasm_bindgen::JsCast;
use reqwasm::http::Request;
use yew::prelude::*;
use web_sys::HtmlSelectElement;

// Uncomment for debugging without an API server
//const DATA_USERS_API_PATH: &str = "/data/users.json";

const DATA_USERS_API_PATH: &str = "/api/users?active=true";


#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    // pub data_test: String,
    // pub id: String,
    // pub label: String,
    // pub options: Vec<SelectOption>,
    pub onchange: Callback<String>,
}


#[function_component(UserSelector)]
pub fn user_selector(props: &Props) -> Html {
    
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
    // users.iter().map(|user| { 
    //     let user_full_name: String = {
    //         match user.alba_full_name {
    //             Some(_) => user.alba_full_name.clone().unwrap(),
    //             None => "".to_string()
    //         }
    //     };
    //     log!("User: {}", user_full_name);
    // });

    let onchange = {
        let props_onchange = props.onchange.clone();
        Callback::from(move |event: Event| {
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlSelectElement>()
                .value();
            props_onchange.emit(value);
        })
    };

    html! {
        <select id={"user-menu"} name={"albaUserId"} class={"form-select shadow-sm"} {onchange}>
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