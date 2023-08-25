use crate::components::{
    control::{Cities, Control},
    map_component::{City, MapComponent, Point},
};
use crate::components::territory_map::TerritoryMapModel;
use crate::models::territories::Territory;
use yew_hooks::use_async;
use gloo_console::log;
use yew::prelude::*;
use yew::html::Scope;
 use reqwasm::http::Request;
 use reqwasm::http::Response;
//use gloo_net::http::Request;
//use web_sys::{Request, RequestInit};
use wasm_bindgen_futures::JsFuture;

#[cfg(debug_assertions)]
const DATA_API_PATH: &str = "/data/territory-borders-all.json";

#[cfg(not(debug_assertions))]
const DATA_API_PATH: &str = "/api/territories/borders";

pub enum Msg {
    SelectCity(City),
    LoadBorders(TerritoryMapModel),
}

pub struct Model {
    city: City,
    cities: Cities,
    territory_map: TerritoryMapModel
}

impl Component for Model {
    type Message = Msg;
    type Properties = ();

    fn create(_ctx: &Context<Self>) -> Self {
        let aachen = City {
            name: "Aachen".to_string(),
            lat: Point(50.7597f64, 6.0967f64),
        };
        let stuttgart = City {
            name: "Stuttgart".to_string(),
            lat: Point(48.7784f64, 9.1742f64),
        };
        let cities: Cities = Cities {
            list: vec![aachen, stuttgart],
        };
        let city = cities.list[0].clone();
        let territory_map: TerritoryMapModel = TerritoryMapModel::default();

        wasm_bindgen_futures::spawn_local(async move {
            let group_id: String = "2".to_string();//group_id;
            let uri: String =
                format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

            // let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
            //     .send()
            //     .await
            //     .unwrap()
            //     .json()
            //     .await
            //     .unwrap();

            // let m = TerritoryMapModel {
            //     territories: fetched_territories,
            //     territories_is_loaded: true,
            //     local_load: false,
            //     lat: 47.66,
            //     lon: -122.20,
            //     zoom: 10.0,
            //     group_visible: "*".to_string(),
            // };

            log!("Model got territory borders! triggered on create.");

            //self.territory_map = m;
            //ctx.props().select_city.emit(city);
            //ctx.link().callback(Msg::LoadBorders)
        });

        Self { city, cities, territory_map }
    }

    fn update(&mut self, _ctx: &Context<Self>, msg: Self::Message) -> bool {
        // Calling this update(Message) calls map_components.change(Properties)
        match msg {
            Msg::SelectCity(city) => {
                self.city = self
                    .cities
                    .list
                    .iter()
                    .find(|c| c.name == city.name)
                    .unwrap()
                    .clone();
            },
            Msg::LoadBorders(territoryMap) => {
                log!("model.LoadBorders(TerritoryMapModel) message is running! (from the load_data_3 function maybe?)");

                log!(format!("model.LoadBorders: territoryMap.lat: {}", territoryMap.lat));
                //load_data();


                // self.territory_map = TerritoryMapModel
                // {
                //     //territories: fetched_territories,
                //     territories: vec![],
                //     territories_is_loaded: true,
                //     local_load: false,
                //     lat: 47.66,
                //     lon: -122.20,
                //     zoom: 10.0,
                //     group_visible: "*".to_string(),
                // };
            },
           
        }
        true
    }

    fn changed(&mut self, _ctx: &Context<Self>, old_props: &Self::Properties) -> bool {
        false
    }

    fn rendered(&mut self, ctx: &Context<Self>, first_render: bool) {
        if first_render {
            let ctx = ctx.clone();
            let link: &Scope<Model> = ctx.link();
            //load_data(ctx);
            //load_data_2().run();
            let group_id: String = "23".to_string();//group_id;
            let uri: String =
                format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);
            
            log!("model.rendered: Calling load_data_4()...");
            
            let group_id: String = group_id;
            let uri: String =
                format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);
            
            ctx.link().send_future(async move {
                // let mut opts = RequestInit::new();
                // opts.method("GET");
                // let request = Request::new_with_str_and_init(uri.as_str(), &opts); //?;
                // let window = web_sys::window().unwrap();
                // JsFuture::from(window.fetch_with_request(&request)).await?;

                let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
                    .send()
                    .await
                    .unwrap()
                    .json()
                    .await
                    .unwrap();

                //fetched_territories
                let m = TerritoryMapModel {
                    territories: vec![],
                    territories_is_loaded: true,
                    local_load: false,
                    lat: 47.66,
                    lon: -122.20,
                    zoom: 10.0,
                    group_visible: String::from("*"),
                };
                log!("model:rendered:send_future: Sending load borders message");
                Msg::LoadBorders(m)
            });

            // https://docs.rs/reqwest/0.10.1/reqwest/blocking/index.html
            // let body = reqwest::blocking::get("https://www.rust-lang.org")?
            //     .text()?;

            // let client = Client::new();

            // let resp = client.post("http://httpbin.org/post")
            //     .body("possibly too large")
            //     .send()?;

            // match resp.status() {
            //     StatusCode::OK => println!("success!"),
            //     StatusCode::PAYLOAD_TOO_LARGE => {
            //         println!("Request payload is too large!");
            //     }
            //     s => println!("Received response status: {:?}", s),
            // };
            
            // let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
            //     .send()
            //     .await
            //     .unwrap()
            //     .json()
            //     .await
            //     .unwrap();    
            // let m = TerritoryMapModel {
            //     territories: fetched_territories,
            //     territories_is_loaded: true,
            //     local_load: false,
            //     lat: 47.66,
            //     lon: -122.20,
            //     zoom: 10.0,
            //     group_visible: String::from("*"),
            // };
           
        }
            
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let cb = ctx.link().callback(Msg::SelectCity); // Call self back with this message
        let tcb = ctx.link().callback(Msg::LoadBorders); // Call self back with this message
        html! {
            <>
                <MapComponent city={&self.city} territory_map={&self.territory_map} />
                <Control select_city={cb} border_loader={tcb} cities={&self.cities}/>
            </>
        }
    }
}


pub fn load_data_4(link: &Scope<Model>) {
    log!("load_data_4(): Starting...");
    let group_id: String = "2".to_string();
    let uri: String =
        format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);


                let group_id: String = group_id;
                // TODO: Try activeGroupId instead of groupId, needs to be set up in the API too
                let uri: String =
                    format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

                //if !model_clone.territories_is_loaded {
                    /*
                    let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
                        .send()
                        .await
                        .unwrap()
                        .json()
                        .await
                        .unwrap();

                    let m = TerritoryMapModel {
                        territories: fetched_territories,
                        territories_is_loaded: true,
                        local_load: false,
                        lat: 47.66,
                        lon: -122.20,
                        zoom: 10.0,
                        group_visible: String::from("*"),
                    };
                    */
                    
                    log!("load_data_3: Sending message through link...");
                    /*&link.send_message(Msg::LoadBorders(m));*/
                    ////model.set(m);
                //}
       

    ////&model
}

pub fn load_data_3(link: &Scope<Model>) {
    log!("load_data_3(): Starting...");
    let group_id: String = "2".to_string();//group_id;
    let uri: String =
        format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);
    
    //let model //: UseStateHandle<TerritoryMapModel> 
    //    = use_state(|| TerritoryMapModel::default());
    
    // let territories = Box::new(use_state(|| None));
    // let error = Box::new(use_state(|| None));
    
    //let model_clone = model.clone();
    let link = link.clone();
    use_effect_with_deps(
        move |_| {
            let link = link.clone();
            //let model_clone = model_clone.clone();
            wasm_bindgen_futures::spawn_local(async move {
                //let uri: &str = "/data/territory-borders-all.json";
                //let uri: &str = "/api/territories/borders";

                let group_id: String = group_id;
                // TODO: Try activeGroupId instead of groupId, needs to be set up in the API too
                let uri: String =
                    format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

                //if !model_clone.territories_is_loaded {
                    /*
                    let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
                        .send()
                        .await
                        .unwrap()
                        .json()
                        .await
                        .unwrap();

                    let m = TerritoryMapModel {
                        territories: fetched_territories,
                        territories_is_loaded: true,
                        local_load: false,
                        lat: 47.66,
                        lon: -122.20,
                        zoom: 10.0,
                        group_visible: String::from("*"),
                    };
                    */
                    
                    log!("load_data_3: Sending message through link...");
                    /*&link.send_message(Msg::LoadBorders(m));*/
                    ////model.set(m);
                //}
            });
            || ()
        },
        (),
    );

    ////&model
}

/*
pub fn load_data_2() ->  Vec<Territory>  {
    let group_id: String = "2".to_string();//group_id;
    let uri: String =
        format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

    // let request = Request::get(uri.as_str())
    //     .body(Nothing)
    //     .expect("Could not build request.");

    // let callback = link.callback(
    //         move |response: Response<Json<Result<Vec<Territory>, anyhow::Error>>>| {
    //             let Json(data) = response.into_body().json().unwrap();
    //             match data {
    //                 Ok(data) => Msg::FetchDataSuccess(data),
    //                 Err(_) => Msg::FetchDataError,
    //             }
    //         },
    //     );
    
    let fetched_territories = use_async(async move{
        //let fetched_territories: Vec<Territory> = 
        Request::get(uri.as_str())
            .send()
            .await
            .unwrap()
            .json()
            .await
            //.unwrap();
    });

    let model = TerritoryMapModel {
        territories: fetched_territories,
        territories_is_loaded: true,
        local_load: false,
        lat: 47.66,
        lon: -122.20,
        zoom: 10.0,
        group_visible: "*".to_string(),
    };

    log!("Model got territory borders! the one you're looking for.");

    //self.territory_map = m;
    //ctx.link().callback(Msg::LoadBorders).emit(m);
    
    //link.send_message(Msg::LoadBorders(m));
    //model
    fetched_territories.run()
} */
/*
//pub fn load_data(ctx: &Context<Model>) {
pub fn load_data() {
    // let ctx = ctx.clone();
    // let link = ctx.link().clone();

    wasm_bindgen_futures::spawn_local(async move {
        let group_id: String = "2".to_string();//group_id;
        let uri: String =
            format!("{base_path}?groupId={group_id}", base_path = DATA_API_PATH);

        // let request = Request::get(uri.as_str())
        //     .body(Nothing)
        //     .expect("Could not build request.");

        // let callback = link.callback(
        //         move |response: Response<Json<Result<Vec<Territory>, anyhow::Error>>>| {
        //             let Json(data) = response.into_body().json().unwrap();
        //             match data {
        //                 Ok(data) => Msg::FetchDataSuccess(data),
        //                 Err(_) => Msg::FetchDataError,
        //             }
        //         },
        //     );

        let fetched_territories: Vec<Territory> = Request::get(uri.as_str())
            .send()
            .await
            .unwrap()
            .json()
            .await
            .unwrap();

        // let m = TerritoryMapModel {
        //     territories: fetched_territories,
        //     territories_is_loaded: true,
        //     local_load: false,
        //     lat: 47.66,
        //     lon: -122.20,
        //     zoom: 10.0,
        //     group_visible: "*".to_string(),
        // };

        log!("load_data: Model got territory borders! the one you're looking for.");

        //self.territory_map = m;
        //ctx.link().callback(Msg::LoadBorders).emit(m);
        
        //link.send_message(Msg::LoadBorders(m));

        
       
    });

    // TerritoryMapModel {
    //     territories: fetched_territories,
    //     territories_is_loaded: true,
    //     local_load: false,
    //     lat: 47.66,
    //     lon: -122.20,
    //     zoom: 10.0,
    //     group_visible: "*".to_string(),
    // }
}
*/