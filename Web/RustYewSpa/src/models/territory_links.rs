
use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug, Default)]
#[serde(rename_all = "camelCase")]
pub struct TerritoryLinkContract
{
    pub id: String,
    pub territory_uri: Option<String>,
    pub alba_mobile_territory_key: Option<String>,
    pub territory_number: String,
    pub territory_description: Option<String>,
    pub created: Option<String>, // Date
    // pub CreatedById: String,
    pub expires: Option<String>, // Date
    #[serde(default)]
    pub expired: bool,
    // pub AssigneeId: String,
    #[serde(default)]
    pub assigned_date_utc: Option<String>,
    pub assignee_name: String,
    pub assignee_email: Option<String>,
    pub assignee_phone: Option<String>,
    #[serde(default)]
    pub territory_last_completed_by: Option<String>,
    #[serde(default)]
    pub territory_last_completed_date: Option<String>,
    #[serde(default)]
    pub stage_id: i32,
    // pub GroupId: String,
    pub successful: bool,
}