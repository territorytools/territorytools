use yew::prelude::*;

#[derive(Properties, PartialEq, Clone)]
pub struct SmsSectionProps {
    pub territory_number: String,
    pub assignee_phone: String,
    pub territory_uri: String,
}

#[function_component(SmsSection)]
pub fn sms_section(props: &SmsSectionProps) -> Html {
    let sms_link_href = format!(
        "sms://{assignee_phone}?body=Territory%20{territory_number}%20{territory_uri}",
        assignee_phone = props.assignee_phone,
        territory_number = props.territory_number,
        territory_uri = props.territory_uri,
    );

    html! {
        <>
        //<div class={"form-group"}>
            <label for={"sms-number"}>{"Send as test message:"}</label>
            <div class={"input-group"}>
                <input 
                    id="assignee-phone-input"
                    value={props.assignee_phone.clone()} 
                    type={"text"} 
                    class={"form-control"} 
                    readonly={true} 
                />
                <a 
                    class={"btn btn-primary"} 
                    href={sms_link_href} 
                    target="_blank"
                >
                {"Send"}
                </a>
            </div>
        //</div>
        </>
    }
}