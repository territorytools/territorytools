//use crate::components::route_stuff::Route;
//use yew_router::prelude::use_navigator;
use yew::prelude::*;
use std::ops::Deref;

#[derive(Properties, PartialEq, Clone)]
pub struct MenuBarV2Props {
    #[prop_or_default]
    pub children: Children,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct MenuBarV2Model {
    pub show_extra_menu: bool,
}

#[function_component]
pub fn MenuBarV2(props: &MenuBarV2Props) -> Html {
    //let model: yew::UseStateHandle<MenuBarV2Model> = use_state(|| MenuBarV2Model::default());

    // let model_clone = model.clone();
    // let toggle_onclick = {
    //     Callback::from(move |event: MouseEvent| {
    //       let mut modification = model_clone.deref().clone();
    //       modification.show_extra_menu = !model_clone.show_extra_menu;
    //       model.set(modification);        })
    // };

    html! {
      //<header>
        <nav class="navbar navbar-expand-sm navbar-light bg-white border-bottom shadow-sm py-0 mb-3">
          //div class="container">
            <div class="container"> //-fluid">
              
              // TODO: Use this later, for now we don't need it to collapse
              // <button class="navbar-toggler" type="button" data-bs-toggle="collapse" 
              //   data-bs-target="#navbarTogglerDemo01" aria-controls="navbarTogglerDemo01" aria-expanded="false" aria-label="Toggle navigation">
              //   <span class="navbar-toggler-icon"></span>
              // </button>

              //<div class="collapse navbar-collapse" id="navbarTogglerDemo01">
              //<div class="navbar" id="navbarTogglerDemo01">
                // <div class="navbar-nav">
                //   <button 
                //       //onclick={toggle_onclick}
                //       href="/"
                //       class="navbar-toggler" 
                //       type="button" 
                //       data-bs-toggle="collapse" 
                //       data-bs-target="#navbarNav" 
                //       aria-controls="navbarNav" 
                //       aria-expanded="false" 
                //       aria-label="Toggle navigation">
                //     <span class="navbar-toggler-icon"></span>
                //   </button>
                // </div>
                // <a class="navbar-brand mb-0 mb-lg-0" href="/">
                //   <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-list" viewBox="0 0 16 16">
                //     <path fill-rule="evenodd" d="M2.5 12a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5z"/>
                //   </svg>
                //     //<img src="/favicon-32x32.png" alt="Logo" class="mx-1" style="width:20px;" />
                //     //"Territory Tools"
                // </a>

                // <ul class="navbar-nav mb-0 mb-lg-0">
                //   <li class="nav-item">
                //     <a class="nav-link active" aria-current="page" href="/">
                //       <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-house-fill" viewBox="0 0 16 16">
                //         <path d="M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L8 2.207l6.646 6.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.707 1.5Z"/>
                //         <path d="m8 3.293 6 6V13.5a1.5 1.5 0 0 1-1.5 1.5h-9A1.5 1.5 0 0 1 2 13.5V9.293l6-6Z"/>
                //       </svg>
                //       <span class="ms-1">{"Home"}</span>
                //     </a>
                //   </li>
                // </ul>
               
                 // <div class="collapse navbar-collapse" id="collapsibleNavbar">
                  <div class="nav ms-0 me-auto mb-05 mt-0 mb-lg-0">
                    <li class="nav-item ">
                      <a class="nav-link text-body active" aria-current="page" href="/">
                        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-list" viewBox="0 0 16 16">
                          <path fill-rule="evenodd" d="M2.5 12a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5z"/>
                        </svg>
                      </a>
                    </li>
                    { for &mut props.children.iter() }
                  </div>
                // </div>
               

                // <form class="d-flex">
                //   <input class="form-control me-2" type="search" placeholder="Search" aria-label="Search" />
                //   <button class="btn btn-outline-primary" type="submit">{"Search"}</button>
                // </form>
                
              </div>
            //</div>
          //</div>
        </nav>
      //</header>   
    }
}

