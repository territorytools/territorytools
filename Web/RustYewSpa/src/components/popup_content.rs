use crate::models::territories::{Territory};

pub fn popup_content(territory: &Territory) -> String  {
    //let onclick_string = format!("alert(\"You clicked {}\");", territory.number);
    let assignee_line = {
        match &territory.signed_out_to {
            Some(t) => format!("<br/><span>{}</span>", territory.signed_out_to.clone().unwrap()),
            Nome => "".to_string()
        }
    };

    let status = {
        if territory.last_completed_by.is_none() {
            &territory.status
        } else {
            "Completed"
        }
    };

    format!(
        "<div style='font-size:15px;'>
            <span><strong>{territory_number}</strong></span>
            <br/><span>{description}</span>
            <br/><span>Addresses: {address_count}</span>
            <br/><span>{status}</span>
            {assignee_line}
            <br/><button 
                    style='margin-top:5px;color:white;'
                    class='btn btn-primary btn-sm'
                    onclick=\"if(typeof window !== 'undefined'){{window.location.href='/app/assign/{territory_number}';}}\">
                    Assign
                </button>
            <br/>
        </div>",
        territory_number = territory.number,
        description = territory.description.clone().unwrap(),
        status = status,
        //onclick_string = onclick_string,
        address_count = territory.address_count,
        assignee_line = assignee_line,
    )
}

// <form>
//     <input type='text' value='{description}'/>
//     <br/>Addresses: <strong>{address_count}</strong>
//     <br/>{status}
//     <br/><button 
//         onclick='{onclick_string}'>
//         Save
//     </button>            
// </form>

