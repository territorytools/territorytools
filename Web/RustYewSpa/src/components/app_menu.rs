use crate::components::menu_bar_v2::MenuBarV2;
use crate::components::menu_bar::MapPageLink;
use crate::Route;

use serde::Deserialize;
use serde::Serialize;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::prelude::LocationHandle;
use yew_router::scope_ext::RouterScopeExt;

pub enum Msg {
}

//#[derive(Properties, PartialEq, Clone, Default)]
pub struct AppMenuPage {
    _listener: LocationHandle,
}

impl Component for AppMenuPage {
    type Message = Msg;
    type Properties = ();
    
    fn create(ctx: &Context<Self>) -> Self {
        let _link = ctx.link().clone();      
        let listener = ctx.link()
            .add_location_listener(
                Callback::from(move |_| {
                    //link.send_message(Msg::RefreshFromSearchText());
                })
            )
            .unwrap();

        Self {
            _listener: listener,
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        //set_document_title("Territory Search");
        
        let _onsubmit = Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            // If we don't prevent_default() it will clear the box and search again
        });

        let navigator = ctx.link().navigator().unwrap();
        let _onchange = {
            Callback::from(move |event: Event| {
                let value = event
                    .target()
                    .expect("An input value for an HtmlInputElement")
                    .unchecked_into::<HtmlInputElement>()
                    .value();

                let query = AppMenuQuery {
                    search_text: Some(value.clone()),
                };
                let _ = navigator.push_with_query(&Route::UserSearch, &query);
            })
        };
    

        html! {
            <>
                <MenuBarV2>
                    <ul class="navbar-nav ms-2 me-auto mb-0 mb-lg-0">
                        <li class={"nav-item"}>
                            <MapPageLink />
                        </li>  
                    </ul>
                </MenuBarV2>
                <div class="container mt-3">
                    <span><strong>{"Menu"}</strong></span>
                    <hr/>
                    <div class="d-grid gap-2 col-12 col-sm-6 col-md-6 col-lg-4 mx-auto" style="lightgray;">
                        <a href="/" class="btn btn-outline-primary">{"Login"}</a>
                        <a href="/app/my-territories" class="btn btn-outline-primary">{"My Territories"}</a>
                        <a href="/app/user-search" class="btn btn-outline-primary">{"Users"}</a>
                        <a href="/app/map" class="btn btn-outline-primary">{"Big Map"}</a>
                        <a href="/app/territory-search" class="btn btn-outline-primary">{"Search Territories"}</a>
                        <a href="/app/links" class="btn btn-outline-primary">{"Links"}</a>
                        <a href="/app/address-search" class="btn btn-outline-primary">{"Search Addresses"}</a>
                        <a href="/" class="btn btn-outline-primary">{"Phone Territories Etc"}</a>
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