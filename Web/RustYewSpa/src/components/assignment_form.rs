use yew::prelude::*;
use urlencoding::decode;

#[derive(Properties, PartialEq, Clone)]
pub struct AssignmentFormProps {
    pub territory_number: String,
    pub assignee_name: String,
    pub description: String,
}

pub struct AssignmentForm {
    temp_description: String
}
//  {
//     props: MapMenuProps,
// }

pub enum AssignmentFormMsg {
    SetDescription(String),
}

impl Component for AssignmentForm {
    type Message = AssignmentFormMsg;
    type Properties = AssignmentFormProps;

    fn create(_ctx: &Context<Self>) -> Self {
        Self {
            temp_description: "".to_string(),
        }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            AssignmentFormMsg::SetDescription(description) => {
                self.temp_description = description;
                return false;
            }
        }        
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let assignee_name: String = format!("{}", decode(&ctx.props().assignee_name).expect("UTF-8"));
        let description: String = format!("{}", decode(&ctx.props().description).expect("UTF-8"));

        html! {
            <div class={"container"}>
            <div id={"assignment-buttons"}>
                <form>
                    <div class={"form-group"}>
                        <label for={"territory-number"}>{"Territory Number:"}</label>
                        <input name={"territoryNumber"} type={"text"} class={"form-control"} id={"modal-territory-number"} 
                            value={ctx.props().territory_number.clone()} /> // readonly
                        <label for={"modal-description"}>{"Description:"}</label>
                        <div class={"input-group-append"}>
                            <input 
                                id={"modal-current-description"}
                                name={"description"} 
                                type={"text"} 
                                class={"form-control"} 
                                placeholder={""} 
                                value={description} 
                                //oninput={ctx.link().callback(|e: InputData|  AssignmentFormMsg::SetDescription(e.value))}
                                /> 
                        </div>
                        // <label for={"modal-number"}>{"Current Assignee:"}</label>
                        // <div class={"input-group-append"}>
                        //     <input value={assignee_name} name={"assignee"} type={"text"} class={"form-control"} placeholder={"Nobody assigned"} id={"modal-current-assignee"}/> // readonly
                        //     <button id={"unassign-button"} type={"button"} class={"btn btn-primary disabled"}>{"Unassign"}</button>
                        // </div>
                    </div>
                    <div id={"unassign-status"}></div>
                </form>
                <form>
                    <div class={"form-group"}>
                        <label for={"user-menu"}>{"New Assignee:"}</label>
                        <div class={"input-group-append"}>
                            //<select id={"user-menu"} name={"albaUserId"} class={"custom-select"}></select>
                            //<UserSelector onchange={userselector_onchange}/>
                            <button id={"assign-button"} type={"button"} class={"btn btn-primary disabled"}>{"Assign"}</button>
                        </div>
                    </div>
                    <div id={"assign-status"}></div>
                    <div><a id={"assigned-link"} href={"#"}></a></div>
                    <div id={"phone-number-section"} class={"form-group"} style={"display:none;"}>
                        <label for={"phone-number"}>{"SMS Phone Number:"}</label>
                        <div class={"input-group-append"}>
                            <input id={"phone-number"} name={"phoneNumber"} type={"text"} class={"form-control"}/>
                            <button id={"send-sms-button"} type={"button"} class={"btn btn-primary"}>{"Send"}</button>
                        </div>
                        <div id={"send-sms-status"}></div>
                    </div>
                </form>
            </div>
        </div>
        }
    }
}
