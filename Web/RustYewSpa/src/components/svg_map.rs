use gloo_console::log;
use web_sys::HtmlElement;
use wasm_bindgen::JsCast;
use yew::prelude::*;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct SvgMapModel {
    pub rect_top: f64,
    pub rect_left: f64,
    pub mx: f64,
    pub my: f64,
    pub zoom_mouse_x: f64,
    pub zoom_mouse_y: f64,
    pub mouse_down: bool,
    pub pane_x: f64,
    pub pane_y: f64,
    pub start_pan_x: f64,
    pub start_pan_y: f64,
    pub pane_start_x: f64,
    pub pane_start_y: f64,
    pub pane_origin_x: f64,
    pub pane_origin_y: f64,
    pub x_delta: f64,
    pub y_delta: f64,
    pub wheel_delta: f64,
    pub touch_0_x: f64,
    pub touch_0_y: f64,
    pub pinch_width_start: f64,
    pub pinch_width: f64,
    pub pinch_ratio: f64,
    pub zoom: f64,
}

#[function_component(SvgMap)]
pub fn svg_map() -> Html {
    //log!("Starting SVG");
             
    let state: yew::UseStateHandle<SvgMapModel> = use_state(|| {
        log!("Creating new SVG State");
        SvgMapModel {
        rect_top: 0.0,
        rect_left: 0.0,
        mx: 0.0,
        my: 0.0,
        zoom_mouse_x: 0.0,
        zoom_mouse_y: 0.0,
        mouse_down: false,
        pane_x: 0.0,
        pane_y: 0.0,
        pane_start_x: 0.0,
        pane_start_y: 0.0,
        pane_origin_x: 0.0,
        pane_origin_y: 0.0,
        start_pan_x: 0.0,
        start_pan_y: 0.0,
        x_delta: 0.0,
        y_delta: 0.0,
        wheel_delta: 0.0,
        touch_0_x: 0.0,
        touch_0_y: 0.0,
        pinch_width_start: 0.0,
        pinch_width: 0.0,
        pinch_ratio: 1.0,
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
                .get_element_by_id("zoom-pane")
                .expect("should have #pan-pane on the page")
                .dyn_into::<web_sys::HtmlElement>()
                .expect("#pan-pane should be an `HtmlElement`")
                .get_bounding_client_rect();

            let px = (e.client_x() as f64) - prect.left();
            let py = (e.client_y() as f64) - prect.top();
        
            if cloned_state.mouse_down {
                log!(format!("MouseMove (Down) Left: {:.1} ; Top: {:.1}", mx, my));
                // Delta to previous mouse position
                let x_delta = cloned_state.mx - mx;
                let y_delta = cloned_state.my - my;
                cloned_state.set(SvgMapModel {
                    rect_top: mrect.top(),
                    rect_left: mrect.left(),
                    mx: mx,
                    my: my,
                    zoom_mouse_x: px,
                    zoom_mouse_y: py,
                    mouse_down: cloned_state.mouse_down,
                    pane_x: cloned_state.pane_x - x_delta,
                    pane_y: cloned_state.pane_y - y_delta,
                    pane_start_x: cloned_state.pane_start_x,
                    pane_start_y: cloned_state.pane_start_y,
                    pane_origin_x: cloned_state.pane_origin_x,
                    pane_origin_y: cloned_state.pane_origin_y,                    
                    start_pan_x: cloned_state.start_pan_x,
                    start_pan_y: cloned_state.start_pan_y,
                    x_delta: x_delta,
                    y_delta: y_delta,
                    wheel_delta: cloned_state.wheel_delta,
                    touch_0_x: cloned_state.touch_0_x,
                    touch_0_y: cloned_state.touch_0_y,
                    pinch_width_start: 0.0,
                    pinch_width: 0.0,
                    pinch_ratio: 1.0,
                    zoom: cloned_state.zoom,
                });
            } else {
                if mx != 0.0 && my != 0.0 {
                    log!(format!("MouseMove (Up) Left: {:.1} ; Top: {:.1} rect: top: {:.1} left: {:.1}", mx, my, mrect.top(), mrect.left()));
                    cloned_state.set(SvgMapModel {
                        rect_top: mrect.top(),
                        rect_left: mrect.left(),
                        mx: mx,
                        my: my,
                        zoom_mouse_x: px,
                        zoom_mouse_y: py,
                        mouse_down: cloned_state.mouse_down,
                        pane_x: cloned_state.pane_x,
                        pane_y: cloned_state.pane_y,
                        pane_start_x: cloned_state.pane_start_x,
                        pane_start_y: cloned_state.pane_start_y,
                        pane_origin_x: cloned_state.pane_origin_x,
                        pane_origin_y: cloned_state.pane_origin_y,                    
                        start_pan_x: 0.0,
                        start_pan_y: 0.0,
                        x_delta: 0.0,
                        y_delta: 0.0,
                        wheel_delta: cloned_state.wheel_delta,
                        touch_0_x: cloned_state.touch_0_x,
                        touch_0_y: cloned_state.touch_0_y,
                        pinch_width_start: 0.0,
                        pinch_width: 0.0,
                        pinch_ratio: 1.0,
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
            let wide = if e.touches().length() == 2 {
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
                rect_top: rect.top(),
                rect_left: rect.left(),
                mx: x,
                my: y,
                zoom_mouse_x: cloned_state.zoom_mouse_x,
                zoom_mouse_y: cloned_state.zoom_mouse_y,
                mouse_down: cloned_state.mouse_down,
                pane_x: cloned_state.pane_x,
                pane_y: cloned_state.pane_y,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                pane_origin_x: 0.0, //cloned_state.pane_origin_x,
                pane_origin_y: 0.0, //cloned_state.pane_origin_y,                    
                start_pan_x: cloned_state.start_pan_x,
                start_pan_y: cloned_state.start_pan_y,
                x_delta: cloned_state.x_delta,
                y_delta: cloned_state.y_delta,
                wheel_delta: cloned_state.wheel_delta,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                pinch_width_start: wide,
                pinch_width: wide,
                pinch_ratio: 1.0,
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

            log!(format!("TouchMove Left: {} ; Top: {} PLeft: {} PTop: {}", x, y, px, py));
            let wide = if e.touches().length() == 2 {
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

            let mut zoom: f64 = cloned_state.zoom * pinch_ratio;
            if zoom > 12.0 {
                zoom = 12.0
            };
            if zoom < 0.1 {
                zoom = 0.1
            };

            let x_delta = cloned_state.mx - x;
            let y_delta = cloned_state.my - y;
            cloned_state.set(SvgMapModel {
                rect_top: rect.top(),
                rect_left: rect.left(),
                mx: x,
                my: y,
                zoom_mouse_x: px,
                zoom_mouse_y: py,
                mouse_down: cloned_state.mouse_down,
                pane_x: cloned_state.pane_x - x_delta,
                pane_y: cloned_state.pane_y - y_delta,
                pane_origin_x: x - cloned_state.pane_x,
                pane_origin_y: y - cloned_state.pane_y,                 
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: cloned_state.start_pan_x,
                start_pan_y: cloned_state.start_pan_y,
                x_delta: cloned_state.x_delta,
                y_delta: cloned_state.y_delta,
                wheel_delta: cloned_state.wheel_delta,
                touch_0_x: x,
                touch_0_y: y,
                pinch_width_start: wide,
                pinch_width: wide,
                pinch_ratio: pinch_ratio,
                zoom: zoom,
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
                rect_top: rect.top(),
                rect_left: rect.left(),
                mx: cloned_state.mx,
                my: cloned_state.my,
                zoom_mouse_x: cloned_state.zoom_mouse_x,
                zoom_mouse_y: cloned_state.zoom_mouse_y,
                mouse_down: true,
                pane_x: cloned_state.pane_x,
                pane_y: cloned_state.pane_y,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                pane_origin_x: cloned_state.pane_origin_x,
                pane_origin_y: cloned_state.pane_origin_y,                    
                start_pan_x: cloned_state.mx,
                start_pan_y: cloned_state.my,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                pinch_width_start: cloned_state.pinch_width_start,
                pinch_width: 0.0,
                pinch_ratio: 1.0,
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
            log!(format!("MouseUp Left: {} ; Top: {}", x, y));
            cloned_state.set(SvgMapModel {
                rect_top: rect.top(),
                rect_left: rect.left(),
                mx: x,
                my: y,
                zoom_mouse_x: cloned_state.zoom_mouse_x,
                zoom_mouse_y: cloned_state.zoom_mouse_y,
                mouse_down: false,
                pane_x: cloned_state.pane_x,
                pane_y: cloned_state.pane_y,
                pane_origin_x: cloned_state.pane_origin_x,
                pane_origin_y: cloned_state.pane_origin_y,                    
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: 0.0,
                start_pan_y: 0.0,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                pinch_width_start: 0.0,
                pinch_width: 0.0,
                pinch_ratio: 1.0,
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
                rect_top: rect.top(),
                rect_left: rect.left(),
                mx: x,
                my: y,
                zoom_mouse_x: cloned_state.zoom_mouse_x,
                zoom_mouse_y: cloned_state.zoom_mouse_y,
                mouse_down: false,
                pane_x: cloned_state.pane_x,
                pane_y: cloned_state.pane_y,
                pane_origin_x: cloned_state.pane_origin_x,
                pane_origin_y: cloned_state.pane_origin_y,                    
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: 0.0,
                start_pan_y: 0.0,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                pinch_width_start: 0.0,
                pinch_width: 0.0,
                pinch_ratio: 1.0,
                zoom: cloned_state.zoom,
            });
        }
    });

    let cloned_state = state.clone();
    let onwheel = Callback::from(move |e: WheelEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.client_x() as f64) - rect.left();
            let y = (e.client_y() as f64) - rect.top();
            log!(format!("OnWheel? : {} ; Top? : {}", x, y));

            let wheel_y = e.delta_y() as f64;
            let mut zoom = cloned_state.zoom - (wheel_y / 300.0);

            if zoom > 12.0 {
                zoom = 12.0
            };
            if zoom < 0.1 {
                zoom = 0.1
            };

            cloned_state.set(SvgMapModel {
                rect_top: rect.top(),
                rect_left: rect.left(),
                mx: x,
                my: y,
                zoom_mouse_x: cloned_state.zoom_mouse_x,
                zoom_mouse_y: cloned_state.zoom_mouse_y,
                mouse_down: false,
                pane_x: cloned_state.pane_x,
                pane_y: cloned_state.pane_y,
                // TODO: The origin could be, I think, wherever the pointer is, always since it only affects zooming, not panning
                // pane_origin_x: cloned_state.pane_x - x,
                // pane_origin_y: cloned_state.pane_y - y,
                pane_origin_x: 20.0,
                pane_origin_y: 30.0,
                //pane_origin_x: x/cloned_state.zoom - cloned_state.pane_x/cloned_state.zoom,
                //pane_origin_y: y/cloned_state.zoom - cloned_state.pane_y/cloned_state.zoom,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: cloned_state.start_pan_x,
                start_pan_y: cloned_state.start_pan_y,
                x_delta: cloned_state.x_delta,
                y_delta: cloned_state.y_delta,
                wheel_delta: cloned_state.wheel_delta,
                touch_0_x: cloned_state.touch_0_x,
                touch_0_y: cloned_state.touch_0_y,
                pinch_width_start: cloned_state.pinch_width_start,
                pinch_width: 0.0,
                pinch_ratio: 1.0,
                zoom: zoom,
            });
        }
    });

    let cloned_state = state.clone();
    //{"width:auto;background-color:red;transform: translate3d(" + cloned_state.mx + "px, " +  cloned_state.my + "px, 0px);"}

    // let _mouse_x = cloned_state.mx;
    // let _mouse_y = cloned_state.my;
    let rect_top = cloned_state.rect_top;
    let rect_left = cloned_state.rect_top;

    let zoom = cloned_state.zoom;
    let pane_x = cloned_state.pane_x;
    let pane_y = cloned_state.pane_y;
    let mx = cloned_state.mx;
    let my = cloned_state.my;
    
    // Do I need this point?
    let pane_cursor_x = mx - pane_x; // + cloned_state.pane_x + (cloned_state.pane_origin_x * cloned_state.zoom);
    let pane_cursor_y = my - pane_y; // + cloned_state.pane_y + (cloned_state.pane_origin_y * cloned_state.zoom);
    
    // Good (without dynamic origin)
    let pane_cusor_x_relative = mx/zoom; // - pane_x;
    let pane_cusor_y_relative = my/zoom; // - pane_y;

    let zoom_cursor_x =  cloned_state.zoom_mouse_x;
    let zoom_cursor_y =  cloned_state.zoom_mouse_y;


    // let zoom_origin_x = pane_x / zoom;
    // let zoom_origin_y = pane_y / zoom;

    // let pane_origin_x = 0.0;
    // let pane_origin_y = 0.0;

    let zoom_translate_x = (cloned_state.pane_x - zoom_cursor_x * zoom) / zoom;
    let zoom_translate_y = (cloned_state.pane_y - zoom_cursor_y * zoom) / zoom;
    
    let zoom_origin_x = zoom_cursor_x;
    let zoom_origin_y = zoom_cursor_y;

    // let window = web_sys::window().expect("no global `window` exists");
    // let document = window.document().expect("should have a document on window");
    // let pan_rect: web_sys::HtmlElement = document
    //     .get_element_by_id("#pan-pane")
    //     .expect("should have #pan-pane on the page")
    //     .dyn_into::<web_sys::HtmlElement>()
    //     .expect("#pan-pane should be an `HtmlElement`");
    // let pan_rect_x = cloned_state.
    // let pan_rect_y = pan_rect.offset_top();


    html! {
        <div style="height:100%;">
            <div style="height:150px;">
                <span>
                    {format!(" Mouse: xy: {:.1},{:.1}", cloned_state.mx, cloned_state.my)}
                    {format!(" down: {}", cloned_state.mouse_down)}</span><br/>

                <span>
                    {format!(" Zoom Mouse: xy: {:.1},{:.1} origin: {:.1},{:.1} ", 
                    cloned_state.zoom_mouse_x, cloned_state.zoom_mouse_y,
                    zoom_origin_x, zoom_origin_y)}
                    </span><br/>

                <span>
                    {format!(" Pane: {:.1},{:.1}", cloned_state.pane_x, cloned_state.pane_y)}
                    {format!(" cursor: {:.1},{:.1}", pane_cursor_x, pane_cursor_y)}
                    {format!(" rcursor: {:.1},{:.1}", pane_cusor_x_relative, pane_cusor_y_relative)}
                    {format!(" zoom: {:.3}", cloned_state.zoom)}
                    {format!(" origin: {:.1},{:.1}", cloned_state.pane_origin_x, cloned_state.pane_origin_y)}</span><br/>

                <span>
                    {format!(" Touch: {:.1},{:.1}", cloned_state.touch_0_x, cloned_state.touch_0_y)}
                    {format!(" pinch start: {:.1}", cloned_state.pinch_width_start)}
                    {format!(" to {:.1} ", cloned_state.pinch_width)}
                    {format!(" ratio: {:.3}", cloned_state.pinch_ratio)}</span><br/>

                <span>
                    {format!(" Pan start: {:.1},{:.1}", cloned_state.start_pan_x, cloned_state.start_pan_y)}
                    {format!(" delta: {:.1},{:.1}", cloned_state.x_delta, cloned_state.y_delta)}
                    {format!(" wheel: {:.1}", cloned_state.wheel_delta)}
                    {format!(" Rect: top: {:.1} left: {:.1}", rect_top, rect_left)}
                </span>
            </div>

            <div id="mouse-pane" {onmousemove} {onmousedown} {onmouseup} {onmouseleave} {onwheel} {ontouchmove} {ontouchstart}
                style="touch-action: none;width:100%;height:calc(100% - 150px);background-color:gray;overflow:hidden;">
                                
                // <div style={format!("touch-action: none;pointer-events: none;position:relative;background-color:white;top:{}px;left:{}px;width:10px;height:0;", 
                //     cloned_state.my, cloned_state.mx)}>{"M"}</div>                // <div style={format!("touch-action: none;pointer-events: none;position:relative;background-color:white;top:{}px;left:{}px;width:10px;height:0;", 
                //     cloned_state.my, cloned_state.mx)}>{"M"}</div>

//"touch-action: none;transform-origin: {}px {}px;*/pointer-events: none;width:5000px;height:5000px;background-color:red;transform: translate({}px, {}px) scale({});",
                // Panning pane
                <div id="pan-pane" style={format!(
                    "transform-origin: {}px {}px;transform: translate({}px, {}px);touch-action: none;pointer-events: none;width:500px;height:500px;background-color:red;",
                    0.0, 0.0, 
                    zoom_translate_x, zoom_translate_y)} >
                    
                    <div style={format!("touch-action: none;position:absolute;touch-action: none;pointer-events: none;background-color:green;top:{}px;left:{}px;", 
                    //cloned_state.zoom_mouse_x, cloned_state.zoom_mouse_x)}>{"PR"}</div>
                    pane_cusor_y_relative, pane_cusor_x_relative)}>{"PR"}</div>
                
                    // Zooming pane
                    <div id="zoom-pane"
                        style={format!(
                        "transform-origin: {}px {}px;transform:  scale({});position:absolute;touch-action: none;pointer-events: none;width:500px;height:500px;background-color:blue;",
                        zoom_origin_x, zoom_origin_y,
                        //pane_origin_x, pane_origin_y, 
                        cloned_state.zoom)} >

                    <div style={format!("touch-action: none;pointer-events: none;position:absolute;background-color:blue;left:{}px;top:{}px;", 
                        zoom_cursor_x, zoom_cursor_y)}>{"O"}</div>

                    <div style={format!("touch-action: none;pointer-events: none;position:absolute;background-color:green;left:{}px;top:{}px;", 
                        zoom_cursor_x, zoom_cursor_y)}>{"ZR"}</div>
                    
                    // <div style={format!("touch-action: none;position:absolute;background-color:magenta;top:{}px;left:{}px;", 
                    //     pane_cursor_y, pane_cursor_x)}>{"C"}</div>

                    //<svg width="149" height="147" viewBox="0 0 149 147" fill="none" xmlns="http://www.w3.org/2000/svg">
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
                    </div>
                </div>
            </div>
        </div>
    }
}
