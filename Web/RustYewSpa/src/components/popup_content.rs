use crate::models::territories::{Territory};

pub fn popup_content(territory: &Territory) -> String  {
    //let onclick_string = format!("alert(\"You clicked {}\");", territory.number);
    let assignee_line = {
        match &territory.signed_out_to {
            Some(_t) => format!("<br/><span>{}</span>", territory.signed_out_to.clone().unwrap()),
            None => "".to_string()
        }
    };
    
    // let assignee_name = {
    //     match &territory.signed_out_to {
    //         Some(_) => territory.signed_out_to.clone().unwrap(),
    //         None => "".to_string()
    //     }
    // };

    let status = {
        if territory.last_completed_by.is_none() {
            &territory.status
        } else {
            "Completed"
        }
    };

    let assign_button_html = 
        if territory.status == "Available".to_string() && status != "Completed".to_string() {
        //href='/app/assign/{territory_number}/{description}/{assignee_name}'>
        format!("<br/><a 
                    style='margin-top:5px;color:white;'
                    class='btn btn-primary btn-sm'
                    href='/app/assign/{territory_number}/{description}/Current+Assignee'>
                    Assign
                </a>",
                territory_number = territory.number,
                description = territory.description.clone().unwrap(),
                // assignee_name = assignee_name,
                )
    } else { "".to_string() };

    format!(
        "<div style='font-size:15px;'>
            <span><strong>{territory_number}</strong></span>
            <br/><span>{description}</span>
            <br/><span>Addresses: {address_count}</span>
            <br/><span>{status}</span>
            {assignee_line}
            {assign_button_html}
            <br/>
        </div>",
        territory_number = territory.number,
        description = territory.description.clone().unwrap(),
        status = status,
        //onclick_string = onclick_string,
        address_count = territory.address_count,
        assignee_line = assignee_line
    )
}
