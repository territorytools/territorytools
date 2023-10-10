use crate::models::users::User;
use reqwasm::http::Request;
use wasm_bindgen::JsCast;
use web_sys::HtmlSelectElement;
use yew::prelude::*;

#[derive(Properties, Default, Clone, PartialEq)]
pub struct Props {
    // pub data_test: String,
    pub id: String,
    // pub label: String,
    // pub options: Vec<SelectOption>,
    #[prop_or_default]
    pub email_as_value: bool,
    pub onchange: Callback<String>,
}


#[function_component(UserSelector)]
pub fn user_selector(props: &Props) -> Html {
    
    let users = use_state(|| vec![]);
    {
        let users = users.clone();
        use_effect_with((), move |_| {
            let users = users.clone();
            wasm_bindgen_futures::spawn_local(async move {
                let uri: &str = "/api/users?active=true";

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
        });
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
        <select id={props.id.clone()} name={"albaUserId"} class={"form-select shadow-sm"} {onchange}>
            <option value={"0"}>{"Select User"}</option>
            {                
                users.iter().map(|user| {   
                    let user_full_name: String = {
                        match user.alba_full_name {
                            Some(_) => user.alba_full_name.clone().unwrap(),
                            None => "".to_string()
                        }
                    };
                    
                    let value = if props.email_as_value {
                        user.normalized_email.clone().unwrap_or_default()
                    }  else {
                        user.alba_user_id.clone().unwrap_or_default()
                    };

                    html!{
                        <option value={value}>{user_full_name}</option>
                    }
                }).collect::<Html>()
            }
        </select>
    }
}