use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::models::users::UserSummary;
use crate::Route;

use gloo_console::log;
use reqwasm::http::Request;
use serde::Deserialize;
use serde::Serialize;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::prelude::LocationHandle;
use yew_router::scope_ext::RouterScopeExt;

pub enum Msg {
    Load(UserSearchPageResult),
    RefreshFromSearchText(),
}

//#[derive(Properties, PartialEq, Clone, Default)]
pub struct UserSearchPage {
    _listener: LocationHandle,
    pub users: Vec<UserSummary>,
    pub result: UserSearchPageResult    
}

impl Component for UserSearchPage {
    type Message = Msg;
    type Properties = ();
    
    fn create(ctx: &Context<Self>) -> Self {
        let link = ctx.link().clone();      
        let listener = ctx.link()
            .add_location_listener(
                Callback::from(move |_| {
                    link.send_message(Msg::RefreshFromSearchText());
                })
            )
            .unwrap();

        return Self {
            _listener: listener,
            users: vec![],
            result: UserSearchPageResult::default(),
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.users = result.users.clone();
                true
            },
            Msg::RefreshFromSearchText() => {
                let search_text = ctx.search_query().search_text.clone().unwrap_or_default();  
                ctx.link().send_future(async move {
                    Msg::Load(get_users(search_text.clone()).await)
                });
                false
            }
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        //set_document_title("Territory Search");
        
        let onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            // If we don't prevent_default() it will clear the box and search again
        });

        let navigator = ctx.link().navigator().unwrap();
        let onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = UserSearchQuery {
                    search_text: Some(value.clone()),
                };
                let _ = navigator.push_with_query(&Route::UserSearch, &query);
            })
        };
    
        let count = self.users.len();
        let search_text = ctx.search_query().search_text.clone().unwrap_or_default();  

        html! {
            <>
                <MenuBarV2>
                    <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                        <li class={"nav-item"}>
                            <MapPageLink />
                        </li>  
                    </ul>
                </MenuBarV2>
                <div class="container">
                    <span><strong>{"User Search"}</strong></span>
        
                    <hr/>
                    <form {onsubmit} >
                    <div class="d-flex flex-row">
                        <div class="d-flex flex-colum mb-2 shadow-sm">
                            <input {onchange} type="text" value={search_text} style="max-width:400px;" placeholder="Enter search text" class="form-control" />
                            <button type="submit" class="btn btn-primary">{"Search"}</button>
                            if self.result.load_error { 
                                <span class="mx-1 badge bg-danger">{"Error"}</span> 
                                <span class="mx-1" style="color:red;">{self.result.load_error_message.clone()}</span>
                            }    
                        </div>
                    </div>
                    </form>
                    <div class="row">
                        <div class="col">
                            <span>{"Count: "}{count}</span>
                        </div>
                    </div>
                    <div class="row" style="border-top: 1px solid lightgray;">
                         <div class="col-8 col-md-6 col-lg-3">
                            <strong>{"Name"}</strong>
                        </div>
                        <div class="col-4 col-md-2 col-lg-1">
                            <strong>{"Group"}</strong>
                        </div>
                        <div class="col-4 col-md-3 col-lg-3">
                            <strong>{"Email"}</strong>
                        </div>
                        <div class="col-12 col-md-12 col-lg-5">
                            <strong>{"Summary"}</strong>
                        </div>
                    </div>
                    
                    {
                        self.users.iter().map(|user| {
                            let _user_id = user.id;
                            let full_name = user.alba_full_name.clone();
                            let _my_territories_link = format!("/app/my-territories?impersonate={}", full_name.clone().unwrap_or_default());
                            let edit_user_link = format!("/app/user-edit?user_id={}", user.id);
                            let email_link = format!("mailto:{}", user.normalized_email.clone().unwrap_or_default().to_lowercase());
                            html! {
                                <a href={edit_user_link.clone()} class="text-decoration-none text-black" style="color:black;">
                                    <div  class="row" style="border-top: 1px solid lightgray;">
                                        // <div class="col-2 col-md-2 col-lg-1">
                                        //     <a href={edit_user_link.clone()} class="btn btn-primary mx-1">
                                        //         <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="" viewBox="0 0 16 16">
                                        //             <path d="M15.502 1.94a.5.5 0 0 1 0 .706L14.459 3.69l-2-2L13.502.646a.5.5 0 0 1 .707 0l1.293 1.293zm-1.75 2.456-2-2L4.939 9.21a.5.5 0 0 0-.121.196l-.805 2.414a.25.25 0 0 0 .316.316l2.414-.805a.5.5 0 0 0 .196-.12l6.813-6.814z"/>
                                        //             <path fill-rule="evenodd" d="M1 13.5A1.5 1.5 0 0 0 2.5 15h11a1.5 1.5 0 0 0 1.5-1.5v-6a.5.5 0 0 0-1 0v6a.5.5 0 0 1-.5.5h-11a.5.5 0 0 1-.5-.5v-11a.5.5 0 0 1 .5-.5H9a.5.5 0 0 0 0-1H2.5A1.5 1.5 0 0 0 1 2.5v11z"/>
                                        //         </svg>
                                        //     </a>
                                        // </div>
                                        // <div class="col-10 col-md-10 col-lg-10">
                                        //     <div class="row">
                                                <div class="col-8 col-md-6 col-lg-3">
                                                    <strong>{user.alba_full_name.clone()}</strong>
                                                </div>
                                                <div class="col-4 col-md-2 col-lg-1">
                                                    {user.group_id.clone().unwrap_or_default()}
                                                </div>
                                                <div class="col-4 col-md-3 col-lg-3">
                                                    <a href={email_link}>
                                                        {user.normalized_email.clone().unwrap_or_default().to_lowercase()}
                                                    </a>
                                                </div>
                                                <div class="col-12 col-md-12 col-lg-5">
                                                    // <a href={my_territories_link} class="btn btn-primary btn-sm me-1">
                                                    //     <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="" viewBox="0 0 16 16">
                                                    //         <path fill-rule="evenodd" d="M5 11.5a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm-3 1a1 1 0 1 0 0-2 1 1 0 0 0 0 2zm0 4a1 1 0 1 0 0-2 1 1 0 0 0 0 2zm0 4a1 1 0 1 0 0-2 1 1 0 0 0 0 2z"/>
                                                    //     </svg>                                            
                                                    // </a>
                                                    {user.territory_summary.clone().unwrap_or_default()}
                                                </div>
                                        //     </div>
                                        // </div>
                                    </div>
                                </a>
                            }
                        }).collect::<Html>()
                    }
        
                </div>
            </>
        }
    }
}

#[derive(Properties, PartialEq, Clone, Default, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct UserSearchResults {
    pub count: i32,
    pub users: Vec<UserSummary>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct UserSearchPageResult {
    pub success: bool,
    pub count: i32,
    pub search_text: String,
    pub users: Vec<UserSummary>,
    pub load_error: bool,
    pub load_error_message: String,
}

async fn get_users(search_text: String) -> UserSearchPageResult {
    let search_text = search_text.clone();
    let uri_string: String = format!("/api/users/list?filter={search_text}");
    let uri: &str = uri_string.as_str();
    let resp = Request::get(uri)
        .header("Content-Type", "application/json")
        .send()
        .await
        .expect("A result from the /api/users endpoint");
    
    log!(format!("load users from search result code: {}", resp.status().to_string()));

    let address_result: UserSearchResults = if resp.status() == 200 {
        resp
        .json()
        .await
        .expect("Valid address search result in JSON format")
    } else {
        UserSearchResults {
            count: 0,
            users: vec![],
        }
    };
    
    UserSearchPageResult {
        success: (resp.status() == 200),
        count: address_result.count,
        users: address_result.users,
        search_text: search_text.to_string(),
        load_error: resp.status() != 200,
        load_error_message: if resp.status() == 401 {
                "Unauthorized".to_string()
            } else if resp.status() == 403 {
                "Forbidden".to_string()
            } else {
                format!("Error {:?}", resp.status())
            }
    }
}

#[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
pub struct UserSearchQuery {
    pub search_text: Option<String>,
}

pub trait SearchQuery {
    fn search_query(&self) -> UserSearchQuery;
}

impl SearchQuery for &Context<UserSearchPage> {
    fn search_query(&self) -> UserSearchQuery {
        let location = self.link().location().expect("Location or URI");
        location.query::<UserSearchQuery>().unwrap_or(UserSearchQuery::default())    
    }
}
