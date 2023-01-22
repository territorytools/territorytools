use wasm_bindgen::JsCast;
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone)]
pub struct MapMenuProps {
    //id: String,
    #[prop_or_default]
    pub children: Children,
}

pub struct MapMenu;
//  {
//     props: MapMenuProps,
//     //link: ComponentLink<Self>,
// }

pub enum MapMenuMsg {
    Click,
}

impl Component for MapMenu {
    type Message = MapMenuMsg;
    type Properties = MapMenuProps;

    fn create(_ctx: &Context<Self>) -> Self {
        Self {}
    }

    // fn create(props: Self::Properties) -> Self {
    //     Self { props }
    // }


    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            MapMenuMsg::Click => {
                let window = web_sys::window().expect("no global `window` exists");
                let document = window.document().expect("should have a document on window");

                let to_be_hidden = document
                    .get_element_by_id("to-be-hidden")
                    .expect("should have #to_be_hidden on the page")
                    .dyn_into::<web_sys::HtmlElement>()
                    .expect("#to_be_hidden should be an `HtmlElement`");

                // let hide_legend_button = document
                //     .get_element_by_id("hide-legend-button")
                //     .expect("should have #hide-legend-button on the page")
                //     .dyn_into::<web_sys::HtmlElement>()
                //     .expect("#hide-legend-button should be an `HtmlElement`");
                
                let show_legend_button_icon = document
                    .get_element_by_id("show-legend-button-icon")
                    .expect("should have #hide-legend-button-text on the page")
                    .dyn_into::<web_sys::HtmlElement>()
                    .expect("#hide-legend-button-text should be an `HtmlElement`");

                let hide_legend_button_icon = document
                    .get_element_by_id("hide-legend-button-icon")
                    .expect("should have #hide-legend-button-icon on the page")
                    .dyn_into::<web_sys::HtmlElement>()
                    .expect("#hide-legend-button-icon should be an `HtmlElement`");

                let was_visible = to_be_hidden.get_attribute("data-visible")
                    .expect("data-visible attribute should exist")
                    .to_string();

                if was_visible == "true" {
                    to_be_hidden.set_attribute("data-visible", "false")
                        .expect("'data-visible' should have been set to 'false'");

                    hide(to_be_hidden);
                    hide(hide_legend_button_icon);
                    show(show_legend_button_icon);
                } else {
                    to_be_hidden.set_attribute("data-visible", "true")
                        .expect("'data-visible' should have been set to 'true'");

                    show(to_be_hidden);
                    show(hide_legend_button_icon);
                    hide(show_legend_button_icon);
                }
            }
        };
        true
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let click_callback = ctx.link().callback(|_| MapMenuMsg::Click);
        html! {
            <div style={"
                background-color: white;
                font-family: arial;
                border-radius: 5px;
                display: block;
                border-style: solid;
                border-color: blue;
                border-width: 1px;
                position: absolute;
                height: auto;
                max-height: 60px;
                bottom: 1vh;
                right: 1vh;
                margin-right: 1vh; 
                width: auto; 
                z-index: 1000; /*Just above 'Leaflet' in the bottom right corner*/
                "}>
                <div style={"width:auto;z-index:1001;display:flex;flex-direction:row;"}>
                    <div id={"to-be-hidden"} style={"display: none;"} data-visible={"false"}>
                        { for &mut ctx.props().children.iter() }
                    </div>
                    <button id={"hide-legend-button"} onclick={click_callback} class={"btn btn-primary btn-sm"}>
                        <div id={"hide-legend-button-icon"} style={"display:none;"}>
                            // chevron-bar-right
                            <svg xmlns={"http://www.w3.org/2000/svg"}width={"16"} height={"16"} fill={"currentColor"} class={"bi bi-chevron-bar-right"} viewBox={"0 0 16 16"}>
                                <path fill-rule={"evenodd"} d={"M4.146 3.646a.5.5 0 0 0 0 .708L7.793 8l-3.647 3.646a.5.5 0 0 0 .708.708l4-4a.5.5 0 0 0 0-.708l-4-4a.5.5 0 0 0-.708 0zM11.5 1a.5.5 0 0 1 .5.5v13a.5.5 0 0 1-1 0v-13a.5.5 0 0 1 .5-.5z"}/>
                            </svg>
                        </div>
                        <div id={"show-legend-button-icon"}>
                            // bi-palette-fill
                            <svg xmlns={"http://www.w3.org/2000/svg"} width={"16"} height={"16"} fill={"currentColor"} class={"bi bi-palette-fill"} viewBox={"0 0 16 16"}>
                                <path d={"M12.433 10.07C14.133 10.585 16 11.15 16 8a8 8 0 1 0-8 8c1.996 0 1.826-1.504 1.649-3.08-.124-1.101-.252-2.237.351-2.92.465-.527 1.42-.237 2.433.07zM8 5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm4.5 3a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zM5 6.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm.5 6.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z"}/>
                            </svg>
                        </div>
                    </button>
                </div>
            </div>
        }
    }
}

fn show(element: web_sys::HtmlElement) {
    element
        .style()
        .set_property("display", "block")
        .expect("'display' should have been set to 'block'")
}

fn hide(element: web_sys::HtmlElement) {
    element
        .style()
        .set_property("display", "none")
        .expect("'display' should have been set to 'none'")
}