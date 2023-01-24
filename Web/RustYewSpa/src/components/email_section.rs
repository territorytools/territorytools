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
        "mailto:{assignee_email}?subject=Territory%20{territory_number}&body=Territory%20{territory_number}%20{territory_uri}",
        assignee_email = props.assignee_email,
        territory_number = props.territory_number,
        territory_uri = props.territory_uri,
    );

    html! {
        <div class={"form-group"}>
            <label for={"email-address"}>{"Send as text message:"}</label>
            <div class={"input-group-append"}>
                <input 
                    value={props.assignee_email.clone()} 
                    type={"text"} 
                    class={"form-control"} 
                    readonly={true} 
                />
                <a 
                    class={"btn btn-primary"} 
                    href={email_link_href} 
                    target="_blamk"
                >
                {"Send"}
                </a>
            </div>
        </div>
    }
}