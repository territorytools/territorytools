use gloo_console::log;
use web_sys::HtmlInputElement;
use wasm_bindgen::JsCast;
use yew::function_component;
use yew::prelude::*;


// #[macro_export]
// macro_rules! callback_value {
//     ($cloner:ident, $field:expr) => (
//         {
//             //let state = cloned_state.clone();
//             let state = $cloner.clone();
//             Callback::from(move |event: Event| {
//                 let mut modification = state.deref().clone();
//                 let value = event
//                 .target()
//                 .unwrap()
//                 .unchecked_into::<HtmlInputElement>()
//                 .value();

//                 log!(format!("callback_value!: {}", value.clone()));
//                 //modification.user.given_name = Some(value.to_string());
//                 //modification.$field = Some(value.to_string());
//                 $field = Some(value.to_string());
//                 state.set(modification);
//             })
//         }
//     )
// }

#[derive(Properties, PartialEq, Clone)]
pub struct Props {
    pub model: String,
    pub field: String,
    pub value: Option<String>,
}

#[function_component(TextBox)]
pub fn text_box(props: &Props) -> Html {
    // let model = props.model;
    // let field = props.field;
    //let mycallback = callback_value!(props.model, props.field);
    //let mycallback = callback_value!(statemodification.user.given_name);
    log!(format!("text_box::props::value::{}", props.value.clone().unwrap_or_default()));
    //let v = "Hello world";
    html!{
        <input 
            id="text-box-input" 
            value={props.value.clone()} 
            type="text"
            //onchange={mycallback.clone()}
            class="form-control shadow-sm" />     
    }
}

