use crate::models::users::User;
use crate::components::address_edit_page::EnglishChineseIdOption;

use reqwasm::http::Request;
use wasm_bindgen::JsCast;
use web_sys::HtmlSelectElement;
use yew::prelude::*;

#[derive(Properties, Default, Clone, PartialEq)]
pub struct Props {
    pub id: String,
    pub onchange: Callback<String>,
    pub value: i32,
}


#[function_component(LanguageSelector)]
pub fn language_selector(props: &Props) -> Html {
    
    /*let users = use_state(|| vec![]);
    {
        let users = users.clone();
        use_effect_with((), move |_| {
            let users = users.clone();
            wasm_bindgen_futures::spawn_local(async move {
                let uri: &str = "/api/languages?active=true";

                let fetched_users: Vec<User> = Request::get(uri)
                    .send()
                    .await
                    .unwrap()
                    .json()
                    .await
                    .unwrap();
                    users.set(fetched_users);
            });
            || ()
        });
    }*/

    let onchange = {
        let props_onchange = props.onchange.clone();
        Callback::from(move |event: Event| {
            let value = event
                .target()
                .unwrap()
                .unchecked_into::<HtmlSelectElement>()
                .value();
            props_onchange.emit(value);
        })
    };

    let selected_language_id = props.value;

    html! {
        <select id={props.id.clone()} {onchange} class="form-select shadow-sm">
            <option value="83">{"Select language"}</option>
            <EnglishChineseIdOption id={10} english="English" chinese="" selected={selected_language_id} />
            <optgroup label="Chinese Languages">
                <EnglishChineseIdOption id={83} english="Chinese" chinese="中文" selected={selected_language_id} />
                <EnglishChineseIdOption id={5} english="Cantonese" chinese="广东话" selected={selected_language_id} />
                <EnglishChineseIdOption id={188} english="Fukien" chinese="福建话" selected={selected_language_id} />
                <EnglishChineseIdOption id={258} english="Fuzhounese" chinese="福州话" selected={selected_language_id} />
                <EnglishChineseIdOption id={190} english="Hakka" chinese="客家话" selected={selected_language_id} />
                <EnglishChineseIdOption id={4} english="Mandarin" chinese="普通话" selected={selected_language_id} />
                <EnglishChineseIdOption id={189} english="Teochew" chinese="潮州话" selected={selected_language_id} />
                <EnglishChineseIdOption id={73} english="Toisan" chinese="台山话" selected={selected_language_id} />
                <EnglishChineseIdOption id={259} english="Wenzhounese" chinese="温州话" selected={selected_language_id} />
            </optgroup>

            <optgroup label="Non-Chinese Languages">    
                <EnglishChineseIdOption id={161} english="Afrikaans" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={246} english="Akateko" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={123} english="Albanian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={82} english="American Sign Language (ASL)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={12} english="Amharic" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={13} english="Arabic" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={257} english="Arabic (Maghrebi)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={87} english="Armenian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={124} english="Armenian (Western)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={118} english="Assyrian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={51} english="Awadhi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={46} english="Azerbaijani" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={71} english="Belarusian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={220} english="Belize Kriol" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={19} english="Bengali" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={45} english="Bhojpuri" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={142} english="Bicol" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={187} english="Bissau Guinean Creole" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={94} english="Blackfoot" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={158} english="Bosnian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={166} english="Botswana Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={268} english="Braille" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={205} english="British Sign Language (BSL)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={56} english="Bulgarian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={105} english="Bulgarian Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={84} english="Cambodian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={245} english="Cambodian Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={226} english="Catalan" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={58} english="Cebuano" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={151} english="Chavacano" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={233} english="Cherokee" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={160} english="Chichewa" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={83} english="Chinese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={5} english="Chinese Cantonese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={188} english="Chinese Fukien" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={258} english="Chinese (Fuzhounese)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={190} english="Chinese Hakka" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={4} english="Chinese Mandarin" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={189} english="Chinese Teochew" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={73} english="Chinese Toisan" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={259} english="Chinese (Wenzhounese)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={231} english="Choctaw" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={173} english="Chuj" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={157} english="Chuukese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={249} english="Country Sign (KS)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={86} english="Cree" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={119} english="Creole" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={88} english="Croatian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={66} english="Czech" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={180} english="Damara" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={68} english="Danish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={75} english="Dinka" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={53} english="Dutch" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={241} english="Dutch Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={254} english="Ecuadorian Sign Language" chinese="" selected={selected_language_id} />                            
                <EnglishChineseIdOption id={70} english="Estonian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={176} english="Ewe" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={115} english="Faroese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={76} english="Farsi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={14} english="Finnish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={126} english="Flemish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={3} english="French" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={230} english="Fula" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={107} english="Georgian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={24} english="German" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={222} english="German Sign Language (GSL)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={64} english="Greek" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={199} english="Greek Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={156} english="Guarani" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={35} english="Gujarati" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={72} english="Haitian Creole" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={139} english="Hakha Chin" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={48} english="Hausa" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={238} english="Hawaiian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={269} english="Hawai’i Pidgin" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={101} english="Hebrew" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={145} english="Hiligaynon" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={8} english="Hindi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={203} english="Hmong (Green)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={99} english="Hmong (White)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={244} english="Honduran Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={63} english="Hungarian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={150} english="Ibanag" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={114} english="Icelandic" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={152} english="Ifugao" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={225} english="Igbo" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={95} english="Iloko" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={194} english="Indian Sign Language (ISL)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={77} english="Indonesian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={242} english="Indonesian Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={228} english="Inuktitut" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={200} english="Irish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={207} english="Irish Sign Language (ISL)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={33} english="Italian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={153} english="Itawit" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={149} english="Ivatan" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={234} english="Jamaican Creole" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={211} english="Jamaican Sign Language (JSL)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={23} english="Japanese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={25} english="Javanese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={250} english="Kabuverdianu" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={40} english="Kannada" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={97} english="Karen" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={104} english="Karen Sgaw" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={237} english="Kayah Li" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={65} english="Kazakh" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={251} english="Kekchi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={148} english="Kinaray-a" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={132} english="Kinyarwanda" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={131} english="Kirundi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={229} english="Konkani (Roman)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={26} english="Korean" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={255} english="Kosraean" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={102} english="Krio" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={272} english="Kunama" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={59} english="Kurdish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={215} english="Kurdish Behdînî" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={213} english="Kurdish Kurmanji" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={214} english="Kurdish Sorani" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={179} english="Kwangali" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={178} english="Kwanyama" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={108} english="Kyrgyz" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={140} english="Lakota" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={78} english="Laotian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={109} english="Latvian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={127} english="Lingala" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={110} english="Lithuanian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={106} english="Low German" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={135} english="Lu Mien" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={116} english="Macedonian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={239} english="Macuxi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={47} english="Maithili" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={155} english="Malagasy" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={79} english="Malay" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={39} english="Malayalam" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={217} english="Maltese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={218} english="Maltese Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={129} english="Mam" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={31} english="Marathi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={182} english="Marshallese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={195} english="Mauritian Creole" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={201} english="Mauritian Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={136} english="Mayan" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={197} english="Mexican Sign Language (LSM)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={100} english="Mien" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={128} english="Mixtec" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={192} english="Mohawk" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={111} english="Moldovan" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={7} english="Mongolian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={266} english="Mortlockese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={49} english="Myanmar" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={154} english="Nahuatl" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={181} english="Navajo" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={61} english="Nepali" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={253} english="Nicaraguan Sign Language (ISN)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={113} english="Norwegian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={11} english="Nuer" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={15} english="Ojibwe" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={41} english="Oriya" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={177} english="Otjiherero" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={143} english="Pangasinan" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={141} english="Papiamento" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={196} english="Pashto" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={186} english="Pennsylvania German" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={38} english="Persian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={235} english="Pidgin English (Hawaiian)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={216} english="Pidgin English (West African)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={243} english="Pohnpeian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={36} english="Polish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={262} english="Poptí" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={22} english="Portuguese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={175} english="Portuguese Sign Language (LGP)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={42} english="Punjabi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={208} english="Punjabi (Shahmukhi)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={261} english="Purépecha" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={130} english="Q'anjob'al" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={236} english="Qingtianhua" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={121} english="Quebec Sign Language (LSQ)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={103} english="Quechua" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={171} english="Quiché" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={232} english="Quichua" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={264} english="Quichua (Cañar)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={263} english="Quichua (Chimborazo)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={265} english="Quichua (Quisapincha)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={223} english="Romani" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={44} english="Romanian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={193} english="Romanian Sign Language (LSR)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={2} english="Russian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={221} english="Russian Sign Language (RSL)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={267} english="Saint Lucian Creole" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={98} english="Samoan" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={60} english="Saraiki" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={92} english="Serbian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={224} english="Shilha" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={159} english="Shona" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={247} english="Sicilian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={55} english="Sindhi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={62} english="Sinhalese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={122} english="Slovak" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={93} english="Slovakian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={89} english="Slovenian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={80} english="Somali" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={134} english="Soussou" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={165} english="South African Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={16} english="Spanish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={138} english="Spanish Sign Language (LSE)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={204} english="Sranan Tongo" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={43} english="Sunda" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={147} english="Surigaonon" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={81} english="Swahili" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={185} english="Swati" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={67} english="Swedish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={74} english="Tagalog" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={202} english="Tajik" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={212} english="Tajik (Northern)" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={32} english="Tamil" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={146} english="Tausug" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={210} english="Tedim" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={30} english="Telugu" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={227} english="Tetum Dili" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={52} english="Thai" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={9} english="Tigrinya" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={174} english="Tlicho" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={120} english="Tongan" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={252} english="Trinidadian Creole" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={260} english="Triqui" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={183} english="Tsonga" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={198} english="Tsostil" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={164} english="Tswana" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={28} english="Turkish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={219} english="Turkmen" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={96} english="Twi" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={172} english="Tzotzil" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={270} english="Uighur" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={37} english="Ukrainian" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={1} english="Unknown" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={34} english="Urdu" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={112} english="Uzbek" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={184} english="Venda" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={6} english="Vietnamese" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={209} english="Vietnamese Sign Language" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={240} english="Wapishana" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={144} english="Waray-Waray" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={133} english="Wolof" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={163} english="Xhosa" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={248} english="Yiddish" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={54} english="Yoruba" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={191} english="Zapotec" chinese="" selected={selected_language_id} />
                <EnglishChineseIdOption id={162} english="Zulu" chinese="" selected={selected_language_id} />
            </optgroup>
        </select>        
    }
}