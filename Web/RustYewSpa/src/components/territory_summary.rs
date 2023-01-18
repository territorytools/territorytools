use yew::Properties;
use yew::function_component;
use yew::prelude::*;

#[derive(PartialEq, Properties, Clone)]
pub struct TerritorySummaryProperties {
    pub available: i32,
    pub signed_out: i32,
    pub completed: i32,
    pub total: i32,
    pub hidden: i32
}

#[function_component(TerritorySummary)]
pub fn territory_summary(props: &TerritorySummaryProperties) -> Html {
    html!{
        <div style={"margin:10px;font-size:12px;"}>
            <span class={"color-legend color-legend-available"}>{format!("Available: {}", props.available)}</span>
            <span class={"color-legend color-legend-signed-out"}>{format!("Signed-out: {}", props.signed_out)}</span>
            <span class={"color-legend color-legend-completed"}>{format!("Completed: {}", props.completed)}</span>
        </div>        
    }
}
