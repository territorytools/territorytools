use reqwasm::http::{Request, Method};
use std::ops::Deref;
use serde::Serialize;
use wasm_bindgen::JsCast;
use wasm_bindgen_futures::spawn_local;
use web_sys::HtmlInputElement;
use yew::prelude::*;

use crate::components::button_with_confirm::ButtonWithConfirm;
use crate::components::selector_option_bilingual::EnglishChineseIdOption;
use crate::models::territory_links::TerritoryLinkContract;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct StageAssignmentResult {
    pub link_contract: TerritoryLinkContract,
    pub success: bool,
    pub load_failed: bool,
    pub load_failed_message: String,
    pub status: u16,
    pub completed: bool,
}

#[derive(Properties, Default, Clone, PartialEq)]
pub struct Props {
    #[prop_or_default]
    pub hidden: bool,
    pub onchange: Callback<i32>,
    pub territory_id: i32,
    pub stage_id: i32,
    pub signed_out_to: Option<String>,
}

#[derive(Properties, PartialEq, Clone, Serialize, Default)]
pub struct StageSelectorModel {
    pub territory_id: i32,
    pub stage_id: i32,
    pub signed_out_to: Option<String>,
}

#[function_component(StageSelector)]
pub fn stage_selector(props: &Props) -> Html {
    let state: yew::UseStateHandle<StageSelectorModel> = use_state(StageSelectorModel::default);
    let stage_change_result_state: yew::UseStateHandle<StageAssignmentResult> = use_state(StageAssignmentResult::default);

    let cloned_state = state.clone();
    let stage_id_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.stage_id = value.parse().unwrap_or_default();
            state.set(modification);
        })
    };

    let stage_change_result_state_clone = stage_change_result_state.clone();
    let props_clone = props.clone();
    let save_stage_onclick = Callback::from(move |_: i32| { 
        let stage_change_result_state_clone = stage_change_result_state_clone.clone();
        let territory_id = props_clone.territory_id;
        let stage_id = props_clone.stage_id;
        let signed_out_to = props_clone.signed_out_to.clone().unwrap_or_default();
        spawn_local(async move {
            let uri_string: String = format!("{path}?territoryId={territory_id}&stageId={stage_id}&assignee={signed_out_to}", 
                path = "/api/territory-marking/stages"
            );

            let uri: &str = uri_string.as_str();
            let resp = Request::new(uri)
                .method(Method::POST)                
                .send()
                .await
                .expect("A result from the endpoint");
            
            let result = StageAssignmentResult {
                success: (resp.status() == 200),
                load_failed: (resp.status() != 200),
                load_failed_message: match resp.status() {
                    405 => "Bad Method".to_string(),
                    _ => "Error".to_string(),
                },
                link_contract: TerritoryLinkContract::default(),
                status: resp.status(),
                completed: true,
            };

            stage_change_result_state_clone.set(result);

            if resp.status() == 200 {

            }
        });
    });

    let selected_stage_id = props.stage_id;

    html!{
        <>
            if !props.hidden {
                <>
                    <label for="input-stage" class="form-label">{"Stage"}</label>                
                    <div class="input-group">
                        <select 
                            id="input-stage" 
                            onchange={stage_id_onchange} 
                            class="form-select shadow-sm">
                            <EnglishChineseIdOption id={1} english="None" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={1000} english="Available for Check Out" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={2000} english="Letter: Writing" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={2100} english="Letter: Sent" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={2200} english="Letter: Returned (Done)" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={3000} english="Phone: Calling" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={3100} english="Phone: Done" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={4000} english="Visiting" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={4005} english="Visiting Started" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={4010} english="Visiting Not-at-Homes" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={4020} english="Visiting Done" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={4100} english="Territory Done" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={5000} english="Territory Cooling Off" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={6000} english="Reserved" chinese="" selected={selected_stage_id} />
                            <EnglishChineseIdOption id={6500} english="Ready to Visit" chinese="" selected={selected_stage_id} />
                        </select>
                        <ButtonWithConfirm 
                            id="save-stage-button" 
                            button_text="Save" 
                            on_confirm={save_stage_onclick.clone()} 
                        />
                    </div>
                    if stage_change_result_state.success {
                        <div class="row">
                            <div class="col">
                                <span class="mx-1 badge bg-success">{"Stage Change Saved"}</span> 
                            </div>
                        </div>
                    }
                    if stage_change_result_state.load_failed {
                        <div class="row">
                            <div class="col">
                                <span class="mx-1 badge bg-danger">{"Stage Change Error"}</span> 
                                <span class="mx-1" style="color:red;">{stage_change_result_state.load_failed_message.clone()}</span>
                                <span class="mx-1 badge bg-danger">{stage_change_result_state.status}</span>
                            </div>
                        </div>
                    }
                </>
            }
        </>
    }
}