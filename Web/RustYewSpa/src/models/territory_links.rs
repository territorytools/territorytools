
use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, PartialEq, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct TerritoryLinkContract
{
    pub id: String,
    pub territory_uri: String,
    pub alba_mobile_territory_key: String,
    pub territory_number: String,
    // pub TerritoryDescription: String,
    // pub Created: Option<String>, // Date
    // pub CreatedById: String,
    // pub Expires: Option<String>, // Date
    // pub AssigneeId: String,
    // pub AssigneeName: String,
    pub assignee_email: String,
    pub assignee_phone: String,
    // pub GroupId: String,
    pub successful: bool,
}