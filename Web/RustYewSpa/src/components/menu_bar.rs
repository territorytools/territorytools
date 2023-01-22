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
                        <img src={"/favicon-32x32.png"} alt={"Logo"} style={"width:20px;"} />
                        {"Territory Tools"}
                    </a>
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
                        <ul class={"navbar-nav flex-grow-1"}>
                            <li class={"nav-item"}>
                                <a class={"nav-link text-dark"} href={"/"}>{"Home"}</a>
                            </li>
                            <li class={"nav-item"}>
                                <MapPageLink />
                            </li>                            
                        </ul>
                    </div>
                </div>
            </nav>
        </header>   
    }
}

#[function_component(MapPageLink)]
fn map_page_link() -> Html {
    let navigator = use_navigator().unwrap();

    let onclick = Callback::from(move |_| navigator.push(&Route::Map));
    html! {

        <a {onclick} class={"nav-link text-dark"} style={"cursor: pointer;"}>
            {"Map"}
        </a>

    }
}