use crate::components::route_stuff::Route;
use yew_router::prelude::use_navigator;
use yew::prelude::*;

#[function_component(MenuBar)]
pub fn menu_bar() -> Html {
    // let navigator = use_navigator().unwrap();

    // let onclick = Callback::from(move |_| navigator.push(&Route::Assign));
    html! {
        <header>
            <nav class={"navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"}>
                <div class={"container"}>
                    <a class={"navbar-brand"} href={"/"}>
                        <img src={"/favicon-32x32.png"} alt={"Logo"} class="mx-1" style={"width:20px;"} />
                        {"TT"}
                    </a>
                    <ul class={"navbar-nav flex-grow-1"}>
                        <li class={"nav-item"}>
                            <MapPageLink />
                        </li>                            
                    </ul>
                    <button class={"navbar-toggler"} type={"button"} data-toggle={"collapse"} data-target={".navbar-collapse"}
                        aria-controls={"navbarSupportedContent"} aria-expanded={"false"} aria-label={"Toggle navigation"}>
                        <span class={"navbar-toggler-icon"}></span>
                    </button>
                    <div class={"navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse"}>
                        // <ul class={"navbar-nav flex-grow"}>
                        //     // Dropdown 
                        //     <li class={"nav-item dropdown"}>
                        //         <a class={"nav-link dropdown-toggle"} href={"#"} id={"navbardrop"} data-toggle={"dropdown"}>
                        //             <i class={"fas fa-language fa-lg text-body"}></i>
                        //         </a>
                        //         <div class={"dropdown-menu dropdown-menu-right"}>
                        //         </div>
                        //     </li>
                        //     <li>
                        //         <ul class={"navbar-nav"}>
                        //             <li class={"nav-item dropdown"}>
                        //                 <a class={"nav-link dropdown-toggle"} href={"#"} id={"navbardrop"} data-toggle={"dropdown"}>
                        //                     <i class={"fas fa-user-circle fa-lg text-body"}></i>
                        //                     <span style={"color:black;"}>{"User"}</span>
                        //                 </a>
                        //             </li>
                        //         </ul>
                        //     </li>
                        // </ul>
                        // <ul class={"navbar-nav flex-grow-1"}>
                        //     <li class={"nav-item"}>
                        //         <a class={"nav-link text-dark"} href={"/"}>{"Home"}</a>
                        //         // <svg xmlns={"http://www.w3.org/2000/svg"} width={"16"} height={"16"} fill={"currentColor"} class={"bi bi-diagram-3-fill"} viewBox={"0 0 16 16"}>
                        //         //     <path fill-rule={"evenodd"} d={"M6 3.5A1.5 1.5 0 0 1 7.5 2h1A1.5 1.5 0 0 1 10 3.5v1A1.5 1.5 0 0 1 8.5 6v1H14a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-1 0V8h-5v.5a.5.5 0 0 1-1 0V8h-5v.5a.5.5 0 0 1-1 0v-1A.5.5 0 0 1 2 7h5.5V6A1.5 1.5 0 0 1 6 4.5v-1zm-6 8A1.5 1.5 0 0 1 1.5 10h1A1.5 1.5 0 0 1 4 11.5v1A1.5 1.5 0 0 1 2.5 14h-1A1.5 1.5 0 0 1 0 12.5v-1zm6 0A1.5 1.5 0 0 1 7.5 10h1a1.5 1.5 0 0 1 1.5 1.5v1A1.5 1.5 0 0 1 8.5 14h-1A1.5 1.5 0 0 1 6 12.5v-1zm6 0a1.5 1.5 0 0 1 1.5-1.5h1a1.5 1.5 0 0 1 1.5 1.5v1a1.5 1.5 0 0 1-1.5 1.5h-1a1.5 1.5 0 0 1-1.5-1.5v-1z"}/>
                        //         // </svg>
                        //     </li>
                        //     <li class={"nav-item"}>
                        //         <MapPageLink />
                        //     </li>                            
                        // </ul>
                    </div>
                </div>
            </nav>
        </header>   
    }
}

#[function_component(MapPageLink)]
pub fn map_page_link() -> Html {
    let navigator = use_navigator().unwrap();

    let onclick = Callback::from(move |_| navigator.push(&Route::Map));
    html! {
        <>
            
            <a {onclick} class={"nav-link text-dark"} style={"cursor:pointer;"}>
                <div>
                    <svg style={" width: 20px; height: auto; "} xmlns={"http://www.w3.org/2000/svg"} fill={"currentColor"} viewBox={"0 0 576 512"}>
                        //0 0 576 512
                        // Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc.
                        <path d={"M408 120c0 54.6-73.1 151.9-105.2 192c-7.7 9.6-22 9.6-29.6 0C241.1 271.9 168 174.6 168 120C168 53.7 221.7 0 288 0s120 53.7 120 120zm8 80.4c3.5-6.9 6.7-13.8 9.6-20.6c.5-1.2 1-2.5 1.5-3.7l116-46.4C558.9 123.4 576 135 576 152V422.8c0 9.8-6 18.6-15.1 22.3L416 503V200.4zM137.6 138.3c2.4 14.1 7.2 28.3 12.8 41.5c2.9 6.8 6.1 13.7 9.6 20.6V451.8L32.9 502.7C17.1 509 0 497.4 0 480.4V209.6c0-9.8 6-18.6 15.1-22.3l122.6-49zM327.8 332c13.9-17.4 35.7-45.7 56.2-77V504.3L192 449.4V255c20.5 31.3 42.3 59.6 56.2 77c20.5 25.6 59.1 25.6 79.6 0zM288 152c22.1 0 40-17.9 40-40s-17.9-40-40-40s-40 17.9-40 40s17.9 40 40 40z"}/>
                    </svg>
                    <span style={"margin-left:5px;"}>{"地图 Map"}</span>
                </div>
            </a>
        </>

    }
}


#[function_component(HomePageLink)]
fn home_page_link() -> Html {
    html! {
        <>
            <a class={"nav-link text-dark"} style={"cursor:pointer;"} href={"/"}>
                <div style="width:auto;">
                    <a href="/">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-house-fill mx-1" viewBox="0 0 16 16">
                            <path d="M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L8 2.207l6.646 6.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.707 1.5Z"/>
                            <path d="m8 3.293 6 6V13.5a1.5 1.5 0 0 1-1.5 1.5h-9A1.5 1.5 0 0 1 2 13.5V9.293l6-6Z"/>
                        </svg>
                        <span>{"Home"}</span>
                    </a>
                </div>
            </a>
        </>

    }
}