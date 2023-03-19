use crate::models::territories::{Territory};

pub fn popup_content(territory: &Territory) -> String  {
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

        // format!(
        //     "<br/><button data-territory-number={territory_number} onclick=\"wasm.try_it();alert('tested 3');\">Test</button>",
        //     territory_number = territory.number)
        format!("<br/><a 
                    style='margin-top:5px;color:white;'
                    class='btn btn-primary btn-sm'
                    href='/app/assign/{territory_number}/{description}/Current+Assignee'>
                    Assign
                </a>",
                territory_number = territory.number,
                )
    } else { "".to_string() };

    let group_id: String = match &territory.group_id {
        Some(v) => if v == "" { "".to_string() } else { v.clone() },
        None => "".to_string()
    };
    
    let territory_id: i32 = match territory.id {
        Some(v) => v,
        None => 0
    };

    let edit_button_html = 
        format!("<br/><a 
            style='margin-top:5px;color:white;'
            class='btn btn-primary btn-sm'
            href='/app/territory-edit?id={id}'>
            Edit
        </a>",
        id = territory.id.unwrap_or_default()
        );

    format!(
        "<div style='font-size:15px;'>
            <span><strong>{territory_number}</strong></span>
            <br/><span>{description}</span>
            <br/><span>Group {group_id}</span>
            <!--br/><span>Addresses: {address_count}</span-->
            <br/><span>{status}</span>
            {assignee_line}
            {assign_button_html}
            {edit_button_html}
            <br/><span><small><small>TID: {territory_id}</small></small></span>
        </div>",
        territory_number = territory.number,
        description = territory.description.clone().unwrap(),
        address_count = territory.address_count,
    )
}
