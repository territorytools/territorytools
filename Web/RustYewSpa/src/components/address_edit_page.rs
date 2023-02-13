use crate::components::menu_bar::MenuBar;
use crate::models::addresses::Address;
use std::ops::Deref;
//use crate::models::territories::Territory;
//use serde::{Deserialize, Serialize};
use gloo_console::log;
use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use wasm_bindgen_futures::spawn_local;
use yew::prelude::*;
use reqwasm::http::{Request, Method};

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/put_address.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/addresses/save";

#[cfg(debug_assertions)]
const ASSIGN_METHOD: &str = "GET";

#[cfg(not(debug_assertions))]
const ASSIGN_METHOD: &str = "PUT";

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressEditModel {
    pub address: Address,
    pub alba_address_id: i32,
    pub territory_number: Option<String>,
    pub name: Option<String>,
    pub street: Option<String>,
    pub city: Option<String>,
    pub postal_code: Option<String>,
}

#[derive(Properties, PartialEq, Clone, Default)]
pub struct AddressEditProps {
    pub alba_address_id: i32,
}

#[function_component(AddressEditPage)]
pub fn address_edit_page() -> Html {
    let state = use_state(|| AddressEditModel::default());
    let cloned_state = state.clone();

    let name_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.name = Some(value);

            log!(format!("Address Name set to {name:?}", name = modification.address.name.clone()));

            state.set(modification);
        })
    };

    let street_onchange = {
        let state = cloned_state.clone();
        Callback::from(move |event: Event| {
            let mut modification = state.deref().clone();
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();

            modification.address.street = Some(value);

            log!(format!("Address Address set to {street:?}", street = modification.address.street));

            state.set(modification);
        })
    };

    // let onsubmit = Callback::from(move |event: SubmitEvent| {
    //     event.prevent_default();
    // });
    let onsubmit = Callback::from(move |event: SubmitEvent| {   //model: AddressEditModel| { //
        let cloned_state = cloned_state.clone();
        //let navigator = navigator.clone();
        spawn_local(async move {
            log!("Spawing request for address change...");
            let model = cloned_state.clone();
            let uri_string: String = format!("{path}", 
                path = DATA_API_PATH);

            let uri: &str = uri_string.as_str();
            
            let method: Method = match ASSIGN_METHOD {
                "PUT" => Method::PUT,
                "GET" => Method::GET,
                &_ =>  Method::GET,
            };

            let name = match &model.name {
                Some(v) => v.to_string(),
                _ => "".to_string(),
            };

            log!(format!("Name would have been: {:?}", &model.name.clone()));

            let street = match &model.street {
                Some(v) => v.to_string(),
                _ => "".to_string(),
            };
            let body = format!("{{ \"name\": \"{n}\", \"street\": \"{s:?}\" }}", n = name, s = street);

            log!(format!("Body would have been: {body}"));

            let resp = Request::new(uri)
                .method(method)
                .header("Content-Type", "application/json")
                //.body(body)
                .send()
                .await
                .expect("A result from the endpoint");

            // let link_contract: TerritoryLinkContract = if resp.status() == 200 {
            //     resp.json().await.unwrap()
            // } else {
            //     TerritoryLinkContract::default()
            // };
            
            // let result = TerritoryEditResult {
            //     success: (resp.status() == 200),
            //     status: resp.status(),
            //     completed: true,
            // };

            // // // //cloned_state.set(result);
            
            // TODO: Check for errors
            // if resp.status() == 200 {
            //     navigator.push(&Route::Map);
            // }
        });
    });

    html! {
        <>
        <MenuBar/>
        <div class="container">
            <span><strong>{"Edit 地址 Address"}</strong></span>
            <hr/>
            <form {onsubmit} class="row g-3">
                <div class="col-12 col-sm-6 col-md-4">
                    <label for="input-language" class="form-label">{"Language"}</label>
                    <select id="input-language" class="form-select">
                        <option selected={true} value="0">{"Select language"}</option>
                        <option value="83">{"中文 Chinese"}</option>
                        <option value="5">{"广东话 Cantonese"}</option>
                        <option value="188">{"福建话 Fukien"}</option>
                        <option value="258">{"福州话 Fuzhounese"}</option>
                        <option value="190">{"客家话 Hakka"}</option>
                        <option value="4">{"普通话 Mandarin"}</option>
                        <option value="189">{"潮州话 Teochew"}</option>
                        <option value="73">{"台山话 Toisan"}</option>
                        <option value="259">{"温州话 Wenzhounese"}</option>

                    </select>
                    </div>
                        <div class="col-12 col-sm-6 col-md-4">
                            <label for="input-status" class="form-label">{"Status"}</label>
                            <select id="input-status" class="form-select">
                                <option selected={true} value="New">{"不确定 New"}</option>
                                <option value="Valid">{"确定 Valid"}</option>
                                <option value="Do not call">{"不要拜访 Do not call"}</option>
                                <option value="Moved">{"搬家 Moved"}</option>
                                <option value="Duplicate">{"地址重复 Duplicate"}</option>
                                <option value="Not valid">{"不说中文 Not valid"}</option>
                            </select>
                        </div>
                        <div class="col-12">
                            <label for="inputName" class="form-label">{"姓名 Name"}</label>
                            <input onchange={name_onchange} type="text" class="form-control" id="inputName" placeholder="Name"/>
                        </div>
                        <div class="col-12 col-md-9">
                            <label for="inputAddress" class="form-label">{"地址 Address"}</label>
                            <input onchange={street_onchange} type="text" class="form-control" id="inputAddress" placeholder="1234 Main St"/>
                        </div>
                        <div class="col-12 col-md-3">
                            <label for="inputUnit" class="form-label">{"单元号 Unit"}</label>
                            <input type="text" class="form-control" id="inputUnit" placeholder="Apartment, studio, or floor"/>
                        </div>
                        <div class="col-md-6">
                            <label for="inputCity" class="form-label">{"城市 City"}</label>
                            <input type="text" class="form-control" id="inputCity"/>
                        </div>
                        <div class="col-md-4">
                            <SelectAddressState />
                        </div>
                        <div class="col-md-2">
                            <label for="inputZip" class="form-label">{"邮政编码 Zip"}</label>
                            <input type="text" class="form-control" id="inputZip"/>
                        </div>
                        <div class="col-12 col-sm-4 col-md-4">
                            <label for="input-latitude" class="form-label">{"纬度 Latitude"}</label>
                            <input type="text" class="form-control" id="input-latitude" placeholder="纬度 Latitude"/>
                        </div>
                        <div class="col-12 col-sm-4 col-md-4">
                            <label for="input-longitude" class="form-label">{"经度 Longitude"}</label>
                            <input type="text" class="form-control" id="input-longitude" placeholder="经度 Longitude"/>
                        </div>
                        <div class="col-12">
                            <label for="input-phone" class="form-label">{"电话 Phone"}</label>
                            <input type="text" class="form-control" id="input-phone" placeholder="000-000-0000"/>
                        </div>
                        <div class="col-12">
                            <label for="input-notes" class="form-label">{"笔记 Notes"}</label>
                            <textarea type="text" rows="2" cols="30" class="form-control" id="input-notes" placeholder="Notes"/>
                        </div>
                        // <div class="col-12">
                        //     <div class="form-check">
                        //     <input class="form-check-input" type="checkbox" id="gridCheck"/>
                        //     <label class="form-check-label" for="gridCheck">
                        //         {"Check me out"}
                        //     </label>
                        //     </div>
                        // </div>
                        <div class="col-12">
                            <button type="submit" class="me-1 btn btn-primary">{"Save"}</button>
                            <button class="me-1 btn btn-secondary">{"Close"}</button>
                        </div>
                        <div class="col-12">
                            <span><small>{"AAID: 123456789"}</small></span>
                        </div>
            </form>
        </div>
        </>
    }
}

#[function_component]
fn SelectAddressState() -> Html {
    html! {
        <>
        <label for="inputState" class="form-label">{"省份 State"}</label>
        <select id="inputState" class="form-select">
            <option value="--">{"Unknown --"}</option>
            <option value="AL">{"阿拉巴马州 Alabama (AL)"}</option>
            <option value="AK">{"阿拉斯加州 Alaska (AK)"}</option>
            <option value="AZ">{"亚利桑那 Arizona (AZ)"}</option>
            <option value="AR">{"阿肯色州 Arkansas (AR)"}</option>
            <option value="CA">{"加州 California (CA)"}</option>
            <option value="CO">{"科罗拉多州 Colorado (CO)"}</option>
            <option value="CT">{"康涅狄格州 Connecticut (CT)"}</option>
            <option value="DE">{"特拉华州 Delaware (DE)"}</option>
            <option value="FL">{"佛罗里达 Florida (FL)"}</option>
            <option value="GA">{"乔治亚州 Georgia (GA)"}</option>
            <option value="HI">{"夏威夷 Hawaii (HI)"}</option>
            <option value="ID">{"爱达荷州 Idaho (ID)"}</option>
            <option value="IL">{"伊利诺伊州 Illinois (IL)"}</option>
            <option value="IN">{"印第安纳州 Indiana (IN)"}</option>
            <option value="IA">{"爱荷华州 Iowa (IA)"}</option>
            <option value="KS">{"堪萨斯州 Kansas (KS)"}</option>
            <option value="KY">{"肯塔基州 Kentucky (KY)"}</option>
            <option value="LA">{"路易斯安那州 Louisiana (LA)"}</option>
            <option value="ME">{"缅因州 Maine (ME)"}</option>
            <option value="MD">{"马里兰州 Maryland (MD)"}</option>
            <option value="MA">{"马萨诸塞州 Massachusetts (MA)"}</option>
            <option value="MI">{"密歇根州 Michigan (MI)"}</option>
            <option value="MN">{"明尼苏达州 Minnesota (MN)"}</option>
            <option value="MS">{"密西西比州 Mississippi (MS)"}</option>
            <option value="MO">{"密苏里州 Missouri (MO)"}</option>
            <option value="MT">{"蒙大拿 Montana (MT)"}</option>
            <option value="NE">{"内布拉斯加州 Nebraska (NE)"}</option>
            <option value="NV">{"内华达州 Nevada (NV)"}</option>
            <option value="NH">{"新罕布什尔 New Hampshire (NH)"}</option>
            <option value="NJ">{"新泽西州 New Jersey (NJ)"}</option>
            <option value="NM">{"新墨西哥 New Mexico (NM)"}</option>
            <option value="NY">{"纽约 New York (NY)"}</option>
            <option value="NC">{"北卡罗来纳 North Carolina (NC)"}</option>
            <option value="ND">{"北达科他州 North Dakota (ND)"}</option>
            <option value="OH">{"俄亥俄州 Ohio (OH)"}</option>
            <option value="OK">{"俄克拉何马州 Oklahoma (OK)"}</option>
            <option value="OR">{"俄勒冈州 Oregon (OR)"}</option>
            <option value="PA">{"宾夕法尼亚州 Pennsylvania (PA)"}</option>
            <option value="RI">{"罗德岛 Rhode Island (RI)"}</option>
            <option value="SC">{"南卡罗来纳 South Carolina (SC)"}</option>
            <option value="SD">{"南达科他州 South Dakota (SD)"}</option>
            <option value="TN">{"田纳西州 Tennessee (TN)"}</option>
            <option value="TX">{"得克萨斯州 Texas (TX)"}</option>
            <option value="UT">{"犹他州 Utah (UT)"}</option>
            <option value="VT">{"佛蒙特 Vermont (VT)"}</option>
            <option value="VA">{"弗吉尼亚州 Virginia (VA)"}</option>
            <option selected={true} value="WA">{"华盛顿 Washington (WA)"}</option>
            <option value="WV">{"西弗吉尼亚 West Virginia (WV)"}</option>
            <option value="WI">{"威斯康星州 Wisconsin (WI)"}</option>
            <option value="WY">{"怀俄明州 Wyoming (WY)"}</option>
        </select>
        </>
    }
}