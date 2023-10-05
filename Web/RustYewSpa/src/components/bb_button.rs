use stylist::{yew::styled_component, Style};
use yew::prelude::*;

#[derive(Clone, PartialEq)]
pub enum ButtonColor {
    Normal,
}

impl Default for ButtonColor {
    fn default() -> Self {
        Self::Normal
    }
}

impl ToString for ButtonColor {
    fn to_string(&self) -> String {
        match self {
            ButtonColor::Normal => "normal",
        }
        .to_owned()
    }
}

#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    pub data_test: String,
    pub label: String,
    pub onclick: Option<Callback<MouseEvent>>,
   //pub color: Option<ButtonColor>,
    pub class: String,
}

#[styled_component(BBButton)]
pub fn bb_button(props: &Props) -> Html {
    let stylesheet = Style::new(css!(
        r#"
          margin-right:3px;
        "#
    ))
    .unwrap();

    //let color = props.color.clone().unwrap_or_default();
    let class = props.class.clone();

    let onclick = {
        let props_onclick = props.onclick.clone();
        Callback::from(move |event: MouseEvent| {
            if let Some(props_onclick) = props_onclick.clone() {
                props_onclick.emit(event);
            }
        })
    };

    html! {
    //   <span class={stylesheet}>
    //     <button data-test={props.data_test.clone()} {onclick} {class}>{&props.label}</button>
    //   </span>
    }
}
