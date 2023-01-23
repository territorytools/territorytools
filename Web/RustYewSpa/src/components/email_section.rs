use yew::prelude::*;

#[derive(Properties, PartialEq, Clone)]
pub struct EmailSectionProps {
    pub territory_number: String,
    pub assignee_email: String,
    pub territory_uri: String,
}

#[function_component(EmailSection)]
pub fn email_section(props: &EmailSectionProps) -> Html {
    let email_link_href = format!(
        "mailto://{assignee_email}?body=Territory%20{territory_number}%20{territory_uri}",
        assignee_email = props.assignee_email,
        territory_number = props.territory_number,
        territory_uri = props.territory_uri,
    );

    html! {
        <div id={"email-section"} class={"form-group"}>
            <label for={"email-address"}>{"Send as email message:"}</label>
            <div class={"input-group-append"}>
                <input 
                    id={"email-address"} 
                    value={props.assignee_email.clone()} 
                    name={"emailAddress"} 
                    type={"text"} 
                    class={"form-control"} 
                    readonly={true} 
                />
                <a 
                    class={"btn btn-primary"} 
                    id={"email-link"} 
                    href={email_link_href} 
                    target="_blamk"
                >
                {"Send"}
                </a>
            </div>
        </div>
    }
}