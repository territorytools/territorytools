use crate::models::territories::{Territory};

// pub fn popup_content(territory: &Territory) -> String  {
//     popup_content_w_button(territory, true, false)
// }
#[derive(Clone, PartialEq)]
pub struct PopupContentOptions {
    pub edit_territory_button_enabled: bool,
    pub territory_open_enabled: bool,
    pub show_stage: bool,
    pub as_of_date: Option<String>,
}

impl Default for PopupContentOptions {
    fn default() -> Self {
        PopupContentOptions {
            edit_territory_button_enabled: false,
            territory_open_enabled: false,
            show_stage: false,
            as_of_date: None,    
        }
    }
}

pub fn popup_content_w_button(territory: &Territory, options: PopupContentOptions) -> String { //edit_territory_button_enabled: bool, territory_open_enabled: bool) -> String  {
    let assignee_line = {
        match &territory.signed_out_to {
            Some(_t) => format!("<br/><span>{}</span>", territory.signed_out_to.clone().unwrap()),
            None => "".to_string()
        }
    };
    
    let status = {
        if territory.last_completed_by.is_none() {
            &territory.status
        } else {
            "Completed"
        }
    };
    
    let assign_button_html = 
        if territory.status == "Available".to_string() && status != "Completed".to_string() {
        let description: String = match &territory.description {
            Some(v) => if v == "" { "(empty)".to_string() } else { v.clone() },
            None => "(empty)".to_string()
        };
        
        let _id = territory.id.unwrap_or_default();
        if options.edit_territory_button_enabled { //1 == 1 { //id == 0 {
            format!("<br/><a 
                style='margin-top:5px;color:white;'
                class='btn btn-primary btn-sm'
                href='/app/assign/{territory_number}/{description}/Current+Assignee'>
                Assign
                </a>",
                territory_number = territory.number,
            )
        } else { 
            "".to_string()
        }

 
    } else { "".to_string() };

    let group_id: String = match &territory.group_id {
        Some(v) => if v.is_empty() { "".to_string() } else { v.clone() },
        None => "".to_string()
    };
    
    let territory_id: i32 = territory.id.unwrap_or(0);
 
    let id = territory.id.unwrap_or_default();
    let _description: String = match &territory.description {
        Some(v) => if v.is_empty() { "(empty)".to_string() } else { v.clone() },
        None => "(empty)".to_string()
    };

    let edit_button_html = if options.edit_territory_button_enabled {
        format!("<a 
            style='margin-top:5px;color:white;'
            class='btn btn-primary btn-sm'
            href='/app/territory-edit?id={id}'>
            Edit
        </a>")
    } else { 
        "".to_string() 
    };

    let assignee_link_key = territory.assignee_link_key.clone().unwrap_or("".to_string());
    let open_button_html = if options.territory_open_enabled {
        if assignee_link_key.is_empty() {
           "<a 
                style='margin-top:5px;color:white;'
                class='btn btn-secondary btn-sm'
                disabled
                href='#'>
                Open
            </a>".to_string()
        } else {
            "<a 
                style='margin-top:5px;color:white;'
                class='btn btn-primary btn-sm'
                href='/mtk/{assignee_link_key}'>
                Open
            </a>".to_string()
        }
    } else { 
        "".to_string()
    };
    let stage_html = if options.show_stage { 
        format!("<br/><span>Stage: {}</span>", territory.stage.clone().unwrap_or("".to_string()))
    } else { 
        "".to_string() 
    };

    let mut stage_changes = territory
        .stage_changes
        .iter()
        .map(|c| format!("{}: {}", 
            c.change_date_utc.clone(), 
            c.stage.clone().unwrap_or_default()))
        .collect::<Vec<_>>();
   
    stage_changes.sort();
    let stage_changes_string = stage_changes.join(",");

    let active = territory.addresses_active;
    let total = territory.addresses_total;
    let unvisited = territory.addresses_unvisited;
    let visited = active-territory.addresses_unvisited;
    let status_letter = &status[..1];

    format!(
        "<div style='font-size:15px;'>
            <span><strong>{territory_number}</strong></span>
            <br/><span>{description}</span>
            <br/><span>Group {group_id}</span>
            <!--br/><span>Addresses: {address_count}</span-->
            <!--br/><span>{status}</span-->
            <br/>Visited: {visited}/{active}            
            {stage_html}
            <br/><span>Changes: {stage_changes_string}</span>
            {assignee_line}<br/>
            {open_button_html}
            {assign_button_html}
            {edit_button_html}
            <br/><span><small><small>
                TID:{territory_id} 
                St:{status_letter} 
                A:{unvisited}/{active}/{total}
            </small></small></span>
        </div>",
        territory_number = territory.number,
        description = territory.description.clone().unwrap_or("".to_string()),
        address_count = territory.address_count,
    )
}
