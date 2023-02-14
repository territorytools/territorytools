use wasm_bindgen::JsCast;
use web_sys::HtmlInputElement;
use yew::prelude::*;

#[derive(Properties, Clone, PartialEq)]
pub struct SelectAddressStateProps {
    pub onchange: Callback<String>,
}

#[function_component]
pub fn SelectAddressState(props: &SelectAddressStateProps) -> Html {
    let state = use_state(String::new);
    let onchange = {
        let emit_onchange = props.onchange.clone();
        let state = state.clone();
        Callback::from(move |event: Event| {
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlInputElement>()
                .value();
            emit_onchange.emit(value.clone());
            state.set(value);
        })
    };

    html! {
        <>
        <label for="inputState" class="form-label">{"省份 State"}</label>
        <select {onchange} id="inputState" name="state" class="form-select" autocomplete="address-level1">
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