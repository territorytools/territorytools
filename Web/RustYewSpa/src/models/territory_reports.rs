use serde::{Serialize, Deserialize};
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize, Debug)]
#[serde(rename_all = "camelCase")]
pub struct MonthCompletionSummary {
    pub month: String,
    pub visiting_done: i32,
    pub done: i32,
    pub cooling_off: i32,
    pub total: i32,
}
