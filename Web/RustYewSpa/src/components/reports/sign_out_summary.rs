use serde::{Serialize, Deserialize};
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default, Serialize, Deserialize, Debug)]
#[serde(rename_all = "camelCase")]
pub struct SignOutSummary {
    pub territory_id: i32,
    pub number: Option<String>,
    pub publisher: Option<String>,
    pub signed_out: Option<String>,
    pub completed: Option<String>,
}
