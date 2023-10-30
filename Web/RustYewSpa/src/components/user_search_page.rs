use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::functions::document_functions::set_document_title;
use crate::models::users::UserSummary;
use crate::modals::unauthorized::UnauthorizedModal;
use crate::Route;

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
    pub result: UserSearchPageResult,
    pub show_unauthorized_modal: bool,
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

        Self {
            _listener: listener,
            users: vec![],
            result: UserSearchPageResult::default(),
            show_unauthorized_modal: false,
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.users = result.users.clone();
                self.result = result.clone();
                
                if self.result.load_error_message == "Unauthorized" {
                    self.show_unauthorized_modal = true;
                }

                true
            },
            Msg::RefreshFromSearchText() => {
                let search_text = ctx.search_query().search_text.clone().unwrap_or_default();  
                ctx.link().send_future(async move {
                    Msg::Load(get_users(search_text.clone()).await)
                });
                false
            },
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        set_document_title("Users");
        
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

        let navigator = ctx.link().navigator().unwrap();
        let search_clear_onclick = {
            Callback::from(move |_: MouseEvent| {
                let query = UserSearchQuery {
                    search_text: Some("".to_string()),
                };

                let _ = navigator.push_with_query(&Route::UserSearch, &query);
            })
        };

        let count = self.users.len();
        let search_text = ctx.search_query().search_text.clone().unwrap_or_default();
        let return_url = format!("%2Fapp%2Fuser-search%3Fsearch_text%3D{}", search_text.clone());

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
                        <div class="d-flex flex-colum mb-2">
                            <div class="input-group">
                                <input 
                                    {onchange} 
                                    type="text" 
                                    value={search_text}
                                    style="max-width:400px;" 
                                    placeholder="Enter search text" 
                                    class="form-control" />
                                <button 
                                    id="clear-search-button"
                                    onclick={search_clear_onclick} 
                                    class="btn btn-outline-primary">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" 
                                        class="bi bi-x-lg" viewBox="0 0 16 16">
                                        <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                                    </svg>
                                </button> 
                                <button type="submit" class="btn btn-primary">{"Search"}</button>
                            </div>
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
                                            {user.territory_summary.clone().unwrap_or_default()}
                                        </div>
                                    </div>
                                </a>
                            }
                        }).collect::<Html>()
                    }
        
                </div>

                if self.show_unauthorized_modal {
                    <UnauthorizedModal return_url={return_url} />              
                }
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
