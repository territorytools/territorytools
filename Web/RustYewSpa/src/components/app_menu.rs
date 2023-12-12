use crate::functions::document_functions::set_document_title;
use crate::models::users::SessionUser;

use reqwasm::http::Request;
use serde::Deserialize;
use serde::Serialize;
use yew::prelude::*;
use yew_router::prelude::LocationHandle;
use yew_router::scope_ext::RouterScopeExt;

pub enum Msg {
    Load(MenuResult),
    Refresh(),
}

pub struct AppMenuPage {
    _listener: LocationHandle,
    session: Session,
}

impl Component for AppMenuPage {
    type Message = Msg;
    type Properties = ();
    
    fn create(ctx: &Context<Self>) -> Self {
        let link = ctx.link().clone();      
        let listener = ctx.link()
            .add_location_listener(
                Callback::from(move |_| {
                    link.send_message(Msg::Refresh());
                })
            )
            .unwrap();

        Self {
            _listener: listener,
            session: Session::default(),
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Load(result) => {
                self.session = result.session.clone();
            },
            Msg::Refresh() => {
                ctx.link().send_future(async move {
                    Msg::Load(get_menu("main").await)
                });
            }
        }

        true
    }

    fn view(&self, _ctx: &Context<Self>) -> Html {
        set_document_title("Menu");
        
        let _onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            // if we don't prevent_default() it will clear the box and search again
        });

        let user = self.session.current_user.clone().unwrap_or_default();

        html! {
            <>
                <menubarv2>
                    <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                        <li class={"nav-item"}>
                            <mappagelink />
                        </li>  
                    </ul>
                </menubarv2>
                <div class="container mt-3">
                    <span><strong>{"menu"}</strong></span>
                    <hr/>
                    <div class="d-grid gap-2 col-12 col-sm-6 col-md-6 col-lg-4 mx-auto" style="lightgray;">
                        <a href="/" class="btn btn-outline-primary">{"Login"}</a>
                        if user.is_active {
                            <a href="/app/my-territories" class="btn btn-outline-primary">{"My Territories"}</a>
                            if user.can_assign_territories {
                                <a href="/app/user-search" class="btn btn-outline-primary">{"Users"}</a>
                            }
                            <a href="/app/map" class="btn btn-outline-primary">{"Big Map"}</a>
                            if user.can_assign_territories {
                                <a href="/app/territory-search" class="btn btn-outline-primary">{"Search Territories"}</a>
                            }
                            if user.can_edit_territories {
                                <a href="/app/links" class="btn btn-outline-primary">{"Links"}</a>
                            } 
                            <a href="/app/address-search" class="btn btn-outline-primary">{"Search Addresses"}</a>
                            <a href="/" class="btn btn-outline-primary">{"Phone Territories Etc"}</a>
                        }
                    </div>
                </div>
            </>
        }
    }
}


#[derive(Clone, Default, Deserialize, PartialEq, Serialize)]
pub struct AppMenuQuery {
    pub search_text: Option<String>,
}

pub trait SearchQuery {
    fn search_query(&self) -> AppMenuQuery;
}

impl SearchQuery for &Context<AppMenuPage> {
    fn search_query(&self) -> AppMenuQuery {
        let location = self.link().location().expect("Location or URI");
        location.query::<AppMenuQuery>().unwrap_or(AppMenuQuery::default())    
    }
}


async fn get_menu(_name: &str) -> MenuResult {
    let resp = Request::get("/api/menu/main")
        .header("Content-Type", "application/json")
        .send()
        .await
        .expect("A result from the /api/users endpoint");
    
    let result: MenuResult = if resp.status() == 200 {
        resp
        .json()
        .await
        .expect("Valid JSON")
    } else {
        MenuResult {
            session: Session::default(),
            menu_name: String::default(),
        }
    };
    
    result
}

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct MenuResult {
    pub session: Session,
    pub menu_name: String,
}

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Session {
    #[prop_or_default]
    pub current_user: Option<SessionUser>,
    pub current_account_id: i32,
}