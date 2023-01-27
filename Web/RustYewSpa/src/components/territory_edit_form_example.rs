use crate::components::{
    bb_button::BBButton,
    bb_text_input::{BBTextInput, InputType},
    user_selector::UserSelector,
};

use std::ops::Deref;
use yew::prelude::*;
use urlencoding::decode;

#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    pub onsubmit: Callback<TerritoryModification>,
    pub territory_number: String,
    pub description: String,
    pub group_id: String,
}

#[derive(Default, Clone)]
pub struct TerritoryModification {
    pub territory_number: String,
    pub description: String,
    pub group_id: String,
}

#[function_component(TerritoryEditFormExample)]
pub fn territory_edit_form(props: &Props) -> Html {
    let state = use_state(||TerritoryModification {
        territory_number: props.territory_number.clone(),
        description: props.description.clone(),
        group_id: props.group_id.clone(),
    });

    let _territory_number: String = format!("{}", decode(&props.territory_number).expect("UTF-8"));
    let description: String = format!("{}", decode(&props.description).expect("UTF-8"));
    let group_id: String = format!("{}", decode(&props.group_id).expect("UTF-8"));

    let territory_number_onchange = {
        let state = state.clone();
        Callback::from(move |territory_number: String| {
            let mut modification = state.deref().clone();
            modification.territory_number = territory_number;
            state.set(modification);
        })
    };

    let description_onchange = {
        let state = state.clone();
        Callback::from(move |description: String| {
            let mut modification = state.deref().clone();
            modification.description = description;
            state.set(modification);
        })
    };

    let group_id_onchange = {
        let state = state.clone();
        Callback::from(move |group_id: String| {
            let mut modification = state.deref().clone();
            modification.group_id = group_id;
            state.set(modification);
        })
    };

    let other_onchange = {
        let state = state.clone();
        Callback::from(move |group_id: String| {
            // let mut modification = state.deref().clone();
            // modification.group_id = group_id;
            // state.set(modification);
        })
    };

    let assignee_onchange = {
        let state = state.clone();
        Callback::from(move |group_id: String| {
            // let mut modification = state.deref().clone();
            // modification.group_id = group_id;
            // state.set(modification);
        })
    };

    let onsubmit = {
        let onsubmit_prop = props.onsubmit.clone();
        let state = state;
        Callback::from(move |event: SubmitEvent| {
            event.prevent_default();
            let modification = state.deref().clone();
            onsubmit_prop.emit(modification);
        })
    };

    html! {
        <>
        <div class={"container"}>
            <form {onsubmit}>
                <div class={"form-group"}>
                    <BBTextInput value={props.territory_number.clone()} data_test="territory_number" label="区域号码 Territory Number" placeholder="Number" class="form-control" input_type={InputType::Text} onchange={territory_number_onchange} readonly={true} />
                    <BBTextInput value={description} data_test="description" label="区域名称 Description" placeholder="What description do you want?" class="form-control" input_type={InputType::Text} onchange={description_onchange} />
                    //<BBTextInput value={group_id} data_test="group_id" label="Group ID" placeholder="What group_id do you want?" class="form-control" input_type={InputType::Text} onchange={group_id_onchange} />
                    // <label>{"委派给 Assign to"}</label>
                    // <div class={"input-group-append"}>
                    // <UserSelector onchange={assignee_onchange} />
                    // <BBButton label={"Assign"} data_test="submit" />
                    // </div>
                
                    //<BBButton label={"Close"} class={"btn btn-secondary"} data_test="close" />
                </div>
            </form>
        </div>
        <div class={"container"}>
            
            <div class={"row border-top border-bottom py-2"}>
                <div class={"col-12"}>
                    <svg xmlns={"http://www.w3.org/2000/svg"} width={"64"} height={"64"} fill={"currentColor"} class={"bi bi-envelope-check"} viewBox={"0 0 32 32"}>
                        //<path d={"M2 2a2 2 0 0 0-2 2v8.01A2 2 0 0 0 2 14h5.5a.5.5 0 0 0 0-1H2a1 1 0 0 1-.966-.741l5.64-3.471L8 9.583l7-4.2V8.5a.5.5 0 0 0 1 0V4a2 2 0 0 0-2-2H2Zm3.708 6.208L1 11.105V5.383l4.708 2.825ZM1 4.217V4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v.217l-7 4.2-7-4.2Z"}/>
                        <path d={"M8.47 1.318a1 1 0 0 0-.94 0l-6 3.2A1 1 0 0 0 1 5.4v.817l5.75 3.45L8 8.917l1.25.75L15 6.217V5.4a1 1 0 0 0-.53-.882l-6-3.2ZM15 7.383l-4.778 2.867L15 13.117V7.383Zm-.035 6.88L8 10.082l-6.965 4.18A1 1 0 0 0 2 15h12a1 1 0 0 0 .965-.738ZM1 13.116l4.778-2.867L1 7.383v5.734ZM7.059.435a2 2 0 0 1 1.882 0l6 3.2A2 2 0 0 1 16 5.4V14a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V5.4a2 2 0 0 1 1.059-1.765l6-3.2Z"}/>
                        //<path fill={"green"} d={"M16 12.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Zm-1.993-1.679a.5.5 0 0 0-.686.172l-1.17 1.95-.547-.547a.5.5 0 0 0-.708.708l.774.773a.75.75 0 0 0 1.174-.144l1.335-2.226a.5.5 0 0 0-.172-.686Z"}/>
                        
                        <path fill={"blue"} style="transform: translate(10px,12px)" d="M6.75 1a.75.75 0 0 1 .75.75V8a.5.5 0 0 0 1 0V5.467l.086-.004c.317-.012.637-.008.816.027.134.027.294.096.448.182.077.042.15.147.15.314V8a.5.5 0 1 0 1 0V6.435a4.9 4.9 0 0 1 .106-.01c.316-.024.584-.01.708.04.118.046.3.207.486.43.081.096.15.19.2.259V8.5a.5.5 0 0 0 1 0v-1h.342a1 1 0 0 1 .995 1.1l-.271 2.715a2.5 2.5 0 0 1-.317.991l-1.395 2.442a.5.5 0 0 1-.434.252H6.035a.5.5 0 0 1-.416-.223l-1.433-2.15a1.5 1.5 0 0 1-.243-.666l-.345-3.105a.5.5 0 0 1 .399-.546L5 8.11V9a.5.5 0 0 0 1 0V1.75A.75.75 0 0 1 6.75 1zM8.5 4.466V1.75a1.75 1.75 0 1 0-3.5 0v5.34l-1.2.24a1.5 1.5 0 0 0-1.196 1.636l.345 3.106a2.5 2.5 0 0 0 .405 1.11l1.433 2.15A1.5 1.5 0 0 0 6.035 16h6.385a1.5 1.5 0 0 0 1.302-.756l1.395-2.441a3.5 3.5 0 0 0 .444-1.389l.271-2.715a2 2 0 0 0-1.99-2.199h-.581a5.114 5.114 0 0 0-.195-.248c-.191-.229-.51-.568-.88-.716-.364-.146-.846-.132-1.158-.108l-.132.012a1.26 1.26 0 0 0-.56-.642 2.632 2.632 0 0 0-.738-.288c-.31-.062-.739-.058-1.05-.046l-.048.002zm2.094 2.025z"/>
                      
                        //<path fill={"blue"} style={"transform: translate(4px,11px)"} d={"M8.5 4.466V1.75a1.75 1.75 0 1 0-3.5 0v5.34l-1.2.24a1.5 1.5 0 0 0-1.196 1.636l.345 3.106a2.5 2.5 0 0 0 .405 1.11l1.433 2.15A1.5 1.5 0 0 0 6.035 16h6.385a1.5 1.5 0 0 0 1.302-.756l1.395-2.441a3.5 3.5 0 0 0 .444-1.389l.271-2.715a2 2 0 0 0-1.99-2.199h-.581a5.114 5.114 0 0 0-.195-.248c-.191-.229-.51-.568-.88-.716-.364-.146-.846-.132-1.158-.108l-.132.012a1.26 1.26 0 0 0-.56-.642 2.632 2.632 0 0 0-.738-.288c-.31-.062-.739-.058-1.05-.046l-.048.002z"}/>
                      
                    </svg>
                    <span class={"alert alert-primary"}>{"发送信件后触摸图标"}</span>
                </div>
            </div>
            <h5>{"Addresses"}</h5>
            <div class={"row border-top py-2"}>
                <div class={"col-xs-3 col-1"}>
                    <svg class={"d-none d-md-block"} xmlns={"http://www.w3.org/2000/svg"} width={"48"} height={"48"} fill={"currentColor"} class={"bi bi-envelope-check"} viewBox={"0 0 16 16"}>
                        <path d={"M8.47 1.318a1 1 0 0 0-.94 0l-6 3.2A1 1 0 0 0 1 5.4v.817l5.75 3.45L8 8.917l1.25.75L15 6.217V5.4a1 1 0 0 0-.53-.882l-6-3.2ZM15 7.383l-4.778 2.867L15 13.117V7.383Zm-.035 6.88L8 10.082l-6.965 4.18A1 1 0 0 0 2 15h12a1 1 0 0 0 .965-.738ZM1 13.116l4.778-2.867L1 7.383v5.734ZM7.059.435a2 2 0 0 1 1.882 0l6 3.2A2 2 0 0 1 16 5.4V14a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V5.4a2 2 0 0 1 1.059-1.765l6-3.2Z"}/>
                    </svg>
                    <svg  class={"d-block d-md-none"} xmlns={"http://www.w3.org/2000/svg"} width={"32"} height={"32"} fill={"currentColor"} class={"bi bi-envelope-check"} viewBox={"0 0 16 16"}>
                        <path d={"M8.47 1.318a1 1 0 0 0-.94 0l-6 3.2A1 1 0 0 0 1 5.4v.817l5.75 3.45L8 8.917l1.25.75L15 6.217V5.4a1 1 0 0 0-.53-.882l-6-3.2ZM15 7.383l-4.778 2.867L15 13.117V7.383Zm-.035 6.88L8 10.082l-6.965 4.18A1 1 0 0 0 2 15h12a1 1 0 0 0 .965-.738ZM1 13.116l4.778-2.867L1 7.383v5.734ZM7.059.435a2 2 0 0 1 1.882 0l6 3.2A2 2 0 0 1 16 5.4V14a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V5.4a2 2 0 0 1 1.059-1.765l6-3.2Z"}/>
                    </svg>
                </div>           
                <div class={"col-xs-9 col-11 pl-4"}>
                    <strong>{"Bo, Aike"}</strong>
                    <br/>
                    <span style={"width:10px;margin-top:0;"}>{"31434 Ash Way, #701-B Lynnwood, WA 98087"}</span>
                </div>
            </div>
            <div class={"row border-top py-2"}>
                <div class={"col-xs-3 col-1"}>
                    <svg class={"d-none d-md-block"} xmlns={"http://www.w3.org/2000/svg"} width={"48"} height={"48"} fill={"currentColor"} class={"bi bi-envelope-check"} viewBox={"0 0 16 16"}>
                        <path d={"M2 2a2 2 0 0 0-2 2v8.01A2 2 0 0 0 2 14h5.5a.5.5 0 0 0 0-1H2a1 1 0 0 1-.966-.741l5.64-3.471L8 9.583l7-4.2V8.5a.5.5 0 0 0 1 0V4a2 2 0 0 0-2-2H2Zm3.708 6.208L1 11.105V5.383l4.708 2.825ZM1 4.217V4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v.217l-7 4.2-7-4.2Z"}/>
                        <path fill={"rgb(40,167,69)"} d={"M16 12.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Zm-1.993-1.679a.5.5 0 0 0-.686.172l-1.17 1.95-.547-.547a.5.5 0 0 0-.708.708l.774.773a.75.75 0 0 0 1.174-.144l1.335-2.226a.5.5 0 0 0-.172-.686Z"}/>
                    </svg>
                    <svg class={"d-block d-md-none"} xmlns={"http://www.w3.org/2000/svg"} width={"32"} height={"32"} fill={"currentColor"} class={"bi bi-envelope-check"} viewBox={"0 0 16 16"}>
                        <path d={"M2 2a2 2 0 0 0-2 2v8.01A2 2 0 0 0 2 14h5.5a.5.5 0 0 0 0-1H2a1 1 0 0 1-.966-.741l5.64-3.471L8 9.583l7-4.2V8.5a.5.5 0 0 0 1 0V4a2 2 0 0 0-2-2H2Zm3.708 6.208L1 11.105V5.383l4.708 2.825ZM1 4.217V4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v.217l-7 4.2-7-4.2Z"}/>
                        <path fill={"rgb(40,167,69)"} d={"M16 12.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Zm-1.993-1.679a.5.5 0 0 0-.686.172l-1.17 1.95-.547-.547a.5.5 0 0 0-.708.708l.774.773a.75.75 0 0 0 1.174-.144l1.335-2.226a.5.5 0 0 0-.172-.686Z"}/>
                    </svg>
                </div>           
                <div class={"col-xs-9 col-11 pl-4"}>
                    <strong>{"Ou, Dixiong"}</strong><span class={"badge bg-success ml-2 text-white"}>{"寄出: 2023年1月12日"}</span>
                    <br/>
                    <span>{"112233 NE Bellevue-Kirkland Way, #701-B Lynnwood, WA 98087"}</span>
                    <br/>
                    <span><em class={"text-success"}>{"I sent the letter that I really like, the one with the thing, and a brochure."}</em></span>
                </div>
            </div>
            <div class={"row border-top py-2"}>
            <div class={"col-xs-3 col-1"}>
                <svg class={"d-none d-md-block"} xmlns="http://www.w3.org/2000/svg" width="48" height="48" fill="currentColor" class="bi bi-envelope-x" viewBox="0 0 16 16">
                    <path d="M2 2a2 2 0 0 0-2 2v8.01A2 2 0 0 0 2 14h5.5a.5.5 0 0 0 0-1H2a1 1 0 0 1-.966-.741l5.64-3.471L8 9.583l7-4.2V8.5a.5.5 0 0 0 1 0V4a2 2 0 0 0-2-2H2Zm3.708 6.208L1 11.105V5.383l4.708 2.825ZM1 4.217V4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v.217l-7 4.2-7-4.2Z"/>
                    <path fill={"rgb(220, 53, 69)"} d="M16 12.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Zm-4.854-1.354a.5.5 0 0 0 0 .708l.647.646-.647.646a.5.5 0 0 0 .708.708l.646-.647.646.647a.5.5 0 0 0 .708-.708l-.647-.646.647-.646a.5.5 0 0 0-.708-.708l-.646.647-.646-.647a.5.5 0 0 0-.708 0Z"/>
                </svg>
                <svg class={"d-block d-md-none"} xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-envelope-x" viewBox="0 0 16 16">
                    <path d="M2 2a2 2 0 0 0-2 2v8.01A2 2 0 0 0 2 14h5.5a.5.5 0 0 0 0-1H2a1 1 0 0 1-.966-.741l5.64-3.471L8 9.583l7-4.2V8.5a.5.5 0 0 0 1 0V4a2 2 0 0 0-2-2H2Zm3.708 6.208L1 11.105V5.383l4.708 2.825ZM1 4.217V4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v.217l-7 4.2-7-4.2Z"/>
                    <path fill={"rgb(220, 53, 69)"} d="M16 12.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Zm-4.854-1.354a.5.5 0 0 0 0 .708l.647.646-.647.646a.5.5 0 0 0 .708.708l.646-.647.646.647a.5.5 0 0 0 .708-.708l-.647-.646.647-.646a.5.5 0 0 0-.708-.708l-.646.647-.646-.647a.5.5 0 0 0-.708 0Z"/>
                </svg>
            </div>           
            <div class={"col-xs-9 col-11 pl-4"}>
                <strong>{"Du, Huiming & Du, Jiami"}</strong><span class={"badge bg-danger ml-2 text-white"}>{"被退回: 2023年3月3日"}</span>
                <br/>
                <span style={"width:10px;margin-top:0;"}>{"8878 NE Lynnwood-Kirkland St, #1919-C Lynnwood, WA 98087"}</span>
            </div>
        </div>
        <br/>
        <BBButton label={"完成了 Complete"} class={"btn btn-primary"} data_test="submit" />
        </div>
        </>
    }
}
