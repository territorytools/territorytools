use gloo_console::log;
use web_sys::HtmlElement;
use wasm_bindgen::JsCast;
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct SvgMapModel {
    pub mouse_x: f64,
    pub mouse_y: f64,
    pub touch_0_x: f64,
    pub touch_0_y: f64,
    pub zoom_cursor_x: f64,
    pub zoom_cursor_y: f64,
    pub relative_zoom_cursor_x: f64,
    pub relative_zoom_cursor_y: f64,    
    pub mouse_down: bool,
    pub pan_translate_x: f64,
    pub pan_translate_y: f64,
    pub zoom_origin_x: f64,
    pub zoom_origin_y: f64,
    pub wheel_delta: f64,
    pub pinch_width_start: f64,
    pub zoom: f64,
}

#[function_component(SvgMap)]
pub fn svg_map() -> Html {
    //log!("Starting SVG");
             
    let state: yew::UseStateHandle<SvgMapModel> = use_state(|| {
        log!("Creating new SVG State");
        SvgMapModel {
            mouse_x: 0.0,
            mouse_y: 0.0,
            touch_0_x: 0.0,
            touch_0_y: 0.0,
            zoom_cursor_x: 0.0,
            zoom_cursor_y: 0.0,
            relative_zoom_cursor_x: 0.0,
            relative_zoom_cursor_y: 0.0,
            mouse_down: false,
            pan_translate_x: 30.0,
            pan_translate_y: 40.0,
            zoom_origin_x: 0.0,
            zoom_origin_y: 0.0,
            wheel_delta: 0.0,
            pinch_width_start: 0.0,
            zoom: 1.0,
    }});
    let cloned_state = state.clone();
    let onmousemove = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let mrect = target.get_bounding_client_rect();
            let mx = (e.client_x() as f64) - mrect.left();
            let my = (e.client_y() as f64) - mrect.top();

            let window = web_sys::window().expect("no global `window` exists");
            let document = window.document().expect("should have a document on window");
            let prect = document
                .get_element_by_id("pan-pane")
                .expect("should have #pan-pane on the page")
                .dyn_into::<web_sys::HtmlElement>()
                .expect("#pan-pane should be an `HtmlElement`")
                .get_bounding_client_rect();

            let zmx = (e.client_x() as f64) - prect.left();
            let zmy = (e.client_y() as f64) - prect.top();
        
            let _pan_translate_x = cloned_state.pan_translate_x; // (cloned_state.pan_translate_x - zmx/cloned_state.zoom*cloned_state.zoom)/cloned_state.zoom;
            let _pan_translate_y = cloned_state.pan_translate_y; // (cloned_state.pan_translate_y - zmy/cloned_state.zoom*cloned_state.zoom)/cloned_state.zoom;

            if cloned_state.mouse_down {
                log!(format!("MouseMove (Down) Left: {:.1} ; Top: {:.1}", mx, my));
                // Delta to previous mouse position
                let x_delta = cloned_state.mouse_x - mx;
                let y_delta = cloned_state.mouse_y - my;
                cloned_state.set(SvgMapModel {
                    mouse_x: mx,
                    mouse_y: my,
                    touch_0_x: cloned_state.touch_0_x,
                    touch_0_y: cloned_state.touch_0_y,
                    zoom_cursor_x: zmx,
                    zoom_cursor_y: zmy,
                    relative_zoom_cursor_x: zmx/cloned_state.zoom,
                    relative_zoom_cursor_y: zmy/cloned_state.zoom, 
                    mouse_down: cloned_state.mouse_down,
                    pan_translate_x: cloned_state.pan_translate_x - x_delta/cloned_state.zoom,
                    pan_translate_y: cloned_state.pan_translate_y - y_delta/cloned_state.zoom,
                    zoom_origin_x: zmx/cloned_state.zoom,
                    zoom_origin_y: zmy/cloned_state.zoom,
                    wheel_delta: cloned_state.wheel_delta,
                    pinch_width_start: cloned_state.pinch_width_start,
                    zoom: cloned_state.zoom,
                });
            } else {
                if mx != 0.0 && my != 0.0 {
                    log!(format!("MouseMove (Up) Left: {:.1} ; Top: {:.1} rect: top: {:.1} left: {:.1} ZLeft: {:.1} ZTop: {:.1}", mx, my, mrect.top(), mrect.left(), zmx, zmy));
                    cloned_state.set(SvgMapModel {
                        mouse_x: mx,
                        mouse_y: my,
                        touch_0_x: cloned_state.touch_0_x,
                        touch_0_y: cloned_state.touch_0_y,
                        zoom_cursor_x: zmx,
                        zoom_cursor_y: zmy,
                        relative_zoom_cursor_x: zmx/cloned_state.zoom,
                        relative_zoom_cursor_y: zmy/cloned_state.zoom,   
                        mouse_down: cloned_state.mouse_down,
                        pan_translate_x: cloned_state.pan_translate_x,
                        pan_translate_y: cloned_state.pan_translate_y,
                        zoom_origin_x: zmx/cloned_state.zoom, //cloned_state.zoom_origin_x,
                        zoom_origin_y: zmy/cloned_state.zoom, //cloned_state.zoom_origin_y,
                        wheel_delta: cloned_state.wheel_delta,
                        pinch_width_start: cloned_state.pinch_width_start,
                        zoom: cloned_state.zoom,
                    });
                }
            }
        }
    });

    let cloned_state = state.clone();
    let ontouchstart = Callback::from(move |e: TouchEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let touch_0 = e.touches().item(0).expect("One touch object");
            let x = (touch_0.client_x() as f64) - rect.left();
            let y = (touch_0.client_y() as f64) - rect.top();
            log!(format!("TouchStart Left: {} ; Top: {}", x, y));
            let _wide = if e.touches().length() == 2 {
                let touch_1 = e.touches().item(1).expect("Second touch object");
                let w = (touch_0.client_x() - touch_1.client_x()).abs() as f64;
                let h = (touch_0.client_y() - touch_1.client_y()).abs() as f64;
                if w >= h {
                    w
                } else {
                    h
                }
            } else {
                0.0
            };

            cloned_state.set(SvgMapModel {
                mouse_x: cloned_state.mouse_x,
                mouse_y: cloned_state.mouse_y,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                zoom_cursor_x: cloned_state.zoom_cursor_x,
                zoom_cursor_y: cloned_state.zoom_cursor_y,
                relative_zoom_cursor_x: cloned_state.relative_zoom_cursor_x,
                relative_zoom_cursor_y: cloned_state.relative_zoom_cursor_y,
                mouse_down: cloned_state.mouse_down,
                pan_translate_x: cloned_state.pan_translate_x,
                pan_translate_y: cloned_state.pan_translate_y,
                zoom_origin_x: cloned_state.zoom_origin_x,
                zoom_origin_y: cloned_state.zoom_origin_y,
                wheel_delta: cloned_state.wheel_delta,
                pinch_width_start: cloned_state.pinch_width_start,
                zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let ontouchmove = Callback::from(move |e: TouchEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let touch_0 = e.touches().item(0).expect("One touch object");
            let x = (touch_0.client_x() as f64) - rect.left();
            let y = (touch_0.client_y() as f64) - rect.top();

            let window = web_sys::window().expect("no global `window` exists");
            let document = window.document().expect("should have a document on window");
            let prect = document
                .get_element_by_id("pan-pane")
                .expect("should have #pan-pane on the page")
                .dyn_into::<web_sys::HtmlElement>()
                .expect("#pan-pane should be an `HtmlElement`")
                .get_bounding_client_rect();

            let px = (touch_0.client_x() as f64) - prect.left();
            let py = (touch_0.client_y() as f64) - prect.top();

            log!(format!("TouchMove Left: {} ; Top: {} PLeft: {} PTop: {} Count: {}", x, y, px, py, e.touches().length()));
            let wide = if e.touches().length() == 2 {
                
                log!(format!("TouchMove PINCH: {} ; Top: {} PLeft: {} PTop: {} Count: {}", x, y, px, py, e.touches().length()));
      
                let touch_1 = e.touches().item(1).expect("Second touch object");
                let w = (touch_0.client_x() - touch_1.client_x()).abs() as f64;
                let h = (touch_0.client_y() - touch_1.client_y()).abs() as f64;
                if w >= h {
                    w
                } else {
                    h
                }
            } else {
                0.0
            };

            let pinch_ratio: f64 = if cloned_state.pinch_width_start > 1.0 && wide > 1.0 {
                let ratio = ((wide as f64) / cloned_state.pinch_width_start).abs();
                if ratio >= 2.0 {
                    2.0
                } else {
                    if ratio < 0.5 {
                        0.5
                    } else {
                        ratio
                    }
                }
            } else {
                1.0
            };

            let mut _zoom: f64 = cloned_state.zoom * pinch_ratio;
            if _zoom > 12.0 {
                _zoom = 12.0
            };
            if _zoom < 0.1 {
                _zoom = 0.1
            };

            let _x_delta = cloned_state.mouse_x - x;
            let _y_delta = cloned_state.mouse_y - y;
            cloned_state.set(SvgMapModel {
                mouse_x: cloned_state.mouse_x,
                mouse_y: cloned_state.mouse_y,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                zoom_cursor_x: cloned_state.zoom_cursor_x,
                zoom_cursor_y: cloned_state.zoom_cursor_y,
                relative_zoom_cursor_x: cloned_state.relative_zoom_cursor_x,
                relative_zoom_cursor_y: cloned_state.relative_zoom_cursor_y,
                mouse_down: cloned_state.mouse_down,
                pan_translate_x: cloned_state.pan_translate_x,
                pan_translate_y: cloned_state.pan_translate_y,
                zoom_origin_x: cloned_state.zoom_origin_x,
                zoom_origin_y: cloned_state.zoom_origin_y,
                wheel_delta: cloned_state.wheel_delta,
                pinch_width_start: cloned_state.pinch_width_start,
                zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let onmousedown = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.client_x() as f64) - rect.left();
            let y = (e.client_y() as f64) - rect.top();
            log!(format!("MouseDown Down: {} ; Top: {}", x, y));
            cloned_state.set(SvgMapModel {
                mouse_x: cloned_state.mouse_x,
                mouse_y: cloned_state.mouse_y,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                zoom_cursor_x: cloned_state.zoom_cursor_x,
                zoom_cursor_y: cloned_state.zoom_cursor_y,
                relative_zoom_cursor_x: cloned_state.relative_zoom_cursor_x,
                relative_zoom_cursor_y: cloned_state.relative_zoom_cursor_y,
                mouse_down: true,
                pan_translate_x: cloned_state.pan_translate_x,
                pan_translate_y: cloned_state.pan_translate_y,
                zoom_origin_x: cloned_state.zoom_origin_x,
                zoom_origin_y: cloned_state.zoom_origin_y,
                wheel_delta: cloned_state.wheel_delta,
                pinch_width_start: cloned_state.pinch_width_start,
                zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let onmouseup = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.client_x() as f64) - rect.left();
            let y = (e.client_y() as f64) - rect.top();
            
            let window = web_sys::window().expect("no global `window` exists");
            let document = window.document().expect("should have a document on window");
            let prect = document
                .get_element_by_id("pan-pane")
                .expect("should have #pan-pane on the page")
                .dyn_into::<web_sys::HtmlElement>()
                .expect("#pan-pane should be an `HtmlElement`")
                .get_bounding_client_rect();
            let zmx = (e.client_x() as f64) - prect.left();
            let zmy = (e.client_y() as f64) - prect.top();
        
            log!(format!("MouseUp Left: {} ; Top: {}", x, y));
            cloned_state.set(SvgMapModel {
                mouse_x: cloned_state.mouse_x,
                mouse_y: cloned_state.mouse_y,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                zoom_cursor_x: zmx,
                zoom_cursor_y: zmy,
                relative_zoom_cursor_x: cloned_state.relative_zoom_cursor_x,
                relative_zoom_cursor_y: cloned_state.relative_zoom_cursor_y,
                mouse_down: false,
                pan_translate_x: cloned_state.pan_translate_x,
                pan_translate_y: cloned_state.pan_translate_y,
                zoom_origin_x: cloned_state.zoom_origin_x,
                zoom_origin_y: cloned_state.zoom_origin_y,
                wheel_delta: cloned_state.wheel_delta,
                pinch_width_start: cloned_state.pinch_width_start,
                zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let onmouseleave = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.client_x() as f64) - rect.left();
            let y = (e.client_y() as f64) - rect.top();
            log!(format!("MouseLeave Left? : {} ; Top? : {}", x, y));
            cloned_state.set(SvgMapModel {
                mouse_x: cloned_state.mouse_x,
                    mouse_y: cloned_state.mouse_y,
                    touch_0_x: cloned_state.touch_0_x,
                    touch_0_y: cloned_state.touch_0_y,
                    zoom_cursor_x: cloned_state.zoom_cursor_x,
                    zoom_cursor_y: cloned_state.zoom_cursor_y,
                    relative_zoom_cursor_x: cloned_state.relative_zoom_cursor_x,
                    relative_zoom_cursor_y: cloned_state.relative_zoom_cursor_y,
                    mouse_down: cloned_state.mouse_down,
                    pan_translate_x: cloned_state.pan_translate_x,
                    pan_translate_y: cloned_state.pan_translate_y,
                    zoom_origin_x: cloned_state.zoom_origin_x,
                    zoom_origin_y: cloned_state.zoom_origin_y,
                    wheel_delta: cloned_state.wheel_delta,
                    pinch_width_start: cloned_state.pinch_width_start,
                    zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let onwheel = Callback::from(move |e: WheelEvent| {
        log!("Wheel Event Fired");
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.client_x() as f64) - rect.left();
            let y = (e.client_y() as f64) - rect.top();
            log!("Wheel Event: x y calculated");
            let window = web_sys::window().expect("no global `window` exists");
            log!("Wheel Event: got window");
            let document = window.document().expect("should have a document on window");
            log!("Wheel Event: got document");
            let prect = document
                .get_element_by_id("pan-pane")
                .expect("should have #pan-pane on the page")
                .dyn_into::<web_sys::HtmlElement>()
                .expect("#pan-pane should be an `HtmlElement`")
                .get_bounding_client_rect();
            log!("Wheel Event: prect calculated");
            let zmx = (e.client_x() as f64) - prect.left();
            let zmy = (e.client_y() as f64) - prect.top();
            log!("Wheel Event: zmx zmy calculated");
            log!(format!("OnWheel? : {} ; Top? : {}", x, y));

            let wheel_y = e.delta_y() as f64;
            let mut zoom = cloned_state.zoom - (wheel_y / 300.0);

            if zoom > 12.0 {
                zoom = 12.0
            };
            if zoom < 0.1 {
                zoom = 0.1
            };

            // let pan_translate_x =  (cloned_state.pan_translate_x - (zmx) * zoom)/zoom;
            // let pan_translate_y =  (cloned_state.pan_translate_y - (zmy) * zoom)/zoom;

            cloned_state.set(SvgMapModel {
                mouse_x: cloned_state.mouse_x,
                mouse_y: cloned_state.mouse_y,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                zoom_cursor_x: zmx,
                zoom_cursor_y: zmy,
                // keeps cursor in the right place
                relative_zoom_cursor_x: zmx/zoom,
                relative_zoom_cursor_y: zmy/zoom,
                mouse_down: cloned_state.mouse_down,
                pan_translate_x: cloned_state.pan_translate_x,
                pan_translate_y: cloned_state.pan_translate_y,
                zoom_origin_x: zmx/zoom,
                zoom_origin_y: zmy/zoom,
                // zoom_origin_x: cloned_state.zoom_origin_x,
                // zoom_origin_y: cloned_state.zoom_origin_y,
                wheel_delta: wheel_y,
                pinch_width_start: cloned_state.pinch_width_start,
                zoom: zoom,
            });
        }
    });

    let s = state.clone();
    
    // let relative_zoom_cursor_x = s.relative_zoom_cursor_x;
    // let relative_zoom_cursor_y = s.relative_zoom_cursor_y;
    // let zoom_origin_x = s.zoom_origin_x;
    // let zoom_origin_y = s.zoom_origin_y;
    // let zoom_translate_x = s.pan_translate_x; //30.0;// (s.pan_translate_x - s.zoom_origin_x * s.zoom)/s.zoom;
    // let zoom_translate_y = s.pan_translate_y; //40.0; //(s.pan_translate_y - s.zoom_origin_y * s.zoom)/s.zoom;
    
    // let new_state = state.clone();
    // new_state.set(SvgMapModel {
    //     mouse_x: s.mouse_x,
    //     mouse_y: s.mouse_y,
    //     touch_0_x: s.touch_0_x,
    //     touch_0_y: s.touch_0_y,
    //     zoom_cursor_x: s.zoom_cursor_x,
    //     zoom_cursor_y: s.zoom_cursor_y,
    //     mouse_down: s.mouse_down,
    //     pan_translate_x: zoom_translate_x,
    //     pan_translate_y: zoom_translate_y,
    //     zoom_origin_x: zoom_origin_x,
    //     zoom_origin_y: zoom_origin_y,
    //     wheel_delta: s.wheel_delta,
    //     pinch_width_start: s.pinch_width_start,
    //     zoom: s.zoom,
    // });

    html! {
        <div style="height:100%;">
            <div style="height:150px;">
                <span>
                    {format!(" Mouse: {:.1},{:.1}", s.mouse_x, s.mouse_y)}
                    {format!(" down: {}", s.mouse_down)}
                    {format!(" wheel delta: {:.1}", s.wheel_delta)}</span><br/>

                <span>
                    {format!(" Zoom: factor: {:.4} cursor (ZC): {:.1},{:.1} relative cursor (RC): {:.1},{:.1} origin (ZO): {:.1},{:.1} ", 
                    s.zoom,
                    s.zoom_cursor_x, s.zoom_cursor_y,
                    s.relative_zoom_cursor_x, s.relative_zoom_cursor_y,
                    s.zoom_origin_x, s.zoom_origin_y)}
                    </span><br/>

                <span>
                    {format!(" Translate (Pan): {:.1},{:.1} ", 
                    s.pan_translate_x, s.pan_translate_y)}
                    </span><br/>
            </div>

            <div id="mouse-pane" {onmousemove} {onmousedown} {onmouseup} {onmouseleave} {onwheel} {ontouchmove} {ontouchstart}
                style="touch-action: none;width:100%;height:calc(100% - 150px);background-color:gray;overflow:hidden;">

                // Pan pane
                <div id="pan-pane" style={format!(
                    "transform-origin: {}px {}px;transform: scale({}) translate({}px, {}px);touch-action: none;pointer-events: none;width:500px;height:500px;background-color:red;",
                    //0.0, 0.0, 
                    s.zoom_origin_x, s.zoom_origin_y, 
                    s.zoom,
                    s.pan_translate_x, s.pan_translate_y)} >
                    
                    <div style={format!("touch-action: none;pointer-events: none;position:absolute;background-color:green;left:{}px;top:{}px;z-index:1000;", 
                        s.zoom_cursor_x, s.zoom_cursor_y)}>{"ZC"}</div>

                    // // Zoom pane
                    // <div id="zoom-pane"
                    //     style={format!(
                    //     "transform-origin: {}px {}px;transform: scale({});position:absolute;touch-action: none;pointer-events: none;width:500px;height:500px;background-color:blue;",
                    //     s.zoom_origin_x, s.zoom_origin_y,
                    //     s.zoom)} >

                     
                    <div style={format!("touch-action: none;pointer-events: none;position:absolute;background-color:yellow;left:{}px;top:{}px;z-index:100;width:50px;", 
                        s.relative_zoom_cursor_x, s.relative_zoom_cursor_y)}>{"RC"}</div>
    
                        
                    <div style={format!("touch-action: none;pointer-events: none;position:absolute;background-color:cyan;left:{}px;top:{}px;width:70px", 
                        s.zoom_origin_x, s.zoom_origin_y)}>{"ZO"}</div>

                    
                    <svg  style="touch-action: none;" width="700" height="1024" viewBox="0 0 700 512" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <image x="-256" y="-256" width="256" height="256"
                            href="https://c.tile.openstreetmap.org/11/328/710.png" />
                        <image x="0" y="0" width="256" height="256"
                            href="https://c.tile.openstreetmap.org/11/329/711.png" />
                        <image x="0" y="256" width="256" height="256"
                            href="https://c.tile.openstreetmap.org/11/329/712.png" />
                        <image x="0" y="512" width="256" height="256"
                            href="https://c.tile.openstreetmap.org/11/329/713.png" />
                        <image x="256" y="0" width="256" height="256"
                            href="https://c.tile.openstreetmap.org/11/330/711.png" />
                        <image x="256" y="256" width="256" height="256"
                            href="https://c.tile.openstreetmap.org/11/330/712.png" />
                        <image x="256" y="512" width="256" height="256"
                            href="https://c.tile.openstreetmap.org/11/330/713.png" />
                        <path d="M60.5776 13.8268L51.8673 42.6431L77.7475 37.331L60.5776 13.8268Z" fill="#DEB819"/>
                        <path d="M108.361 94.9937L138.708 90.686L115.342 69.8642" stroke="black" stroke-width="4" stroke-linecap="round" stroke-linejoin="round"/>
                        <g>
                            <circle cx="75.3326" cy="73.4918" r="55" fill="#FDD630"/>
                            <circle cx="75.3326" cy="73.4918" r="52.5" stroke="black" stroke-width="5"/>
                        </g>
                        <circle cx="71" cy="99" r="5" fill="white" fill-opacity="0.75" stroke="black" stroke-width="3"/>
                        <circle cx="0" cy="0" r="8" fill="yellow" fill-opacity="0.75" stroke="black" stroke-width="3"/>
                    </svg>
                    // </div>
                </div>
            </div>
        </div>
    }
}


