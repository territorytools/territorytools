use gloo_console::log;
use yew::prelude::*;
use web_sys::HtmlElement;

#[derive(Properties, PartialEq, Clone, Default)]
pub struct SvgMapModel {
    pub mx: f64,
    pub my: f64,
    pub mouse_down: bool,
    pub pane_x: f64,
    pub pane_y: f64,
    pub start_pan_x: f64,
    pub start_pan_y: f64,
    pub pane_start_x: f64,
    pub pane_start_y: f64,
    pub x_delta: f64,
    pub y_delta: f64,
    pub wheel_delta: f64,
    pub pinch_width_start: f64,
    pub pinch_width: f64,
    pub pinch_ratio: f64,
    pub zoom: f64,
}

#[function_component(SvgMap)]
pub fn svg_map() -> Html { 
    let state: yew::UseStateHandle<SvgMapModel> = use_state(|| SvgMapModel { 
        mx: 0.0, 
        my: 0.0, 
        mouse_down: false,
        pane_x: 0.0, 
        pane_y: 0.0, 
        pane_start_x: 0.0, 
        pane_start_y: 0.0, 
        start_pan_x: 0.0, 
        start_pan_y: 0.0, 
        x_delta: 0.0, 
        y_delta: 0.0, 
        wheel_delta: 0.0,
        pinch_width_start: 0.0,
        pinch_width: 0.0,
        pinch_ratio: 1.0,
        zoom: 1.0, 
    });
    let cloned_state = state.clone();
    let onmousemove = Callback::from(move |e: MouseEvent| {
        let cloned_state = cloned_state.clone();
        if let Some(target) = e.target_dyn_into::<HtmlElement>() {
            let rect = target.get_bounding_client_rect();
            let x = (e.client_x() as f64) - rect.left();
            let y = (e.client_y() as f64) - rect.top();
            log!(format!("Left? : {} ; Top? : {}", x, y));
            if cloned_state.mouse_down {
                let x_delta = cloned_state.mx - x;
                let y_delta = cloned_state.my - y;
                cloned_state.set(SvgMapModel { 
                    mx: x, 
                    my: y,
                    mouse_down: cloned_state.mouse_down,
                    pane_x: cloned_state.pane_x - x_delta,
                    pane_y: cloned_state.pane_y - y_delta,                    
                    pane_start_x: cloned_state.pane_start_x,
                    pane_start_y: cloned_state.pane_start_y,
                    start_pan_x: cloned_state.start_pan_x,
                    start_pan_y: cloned_state.start_pan_y,
                    x_delta: x_delta,
                    y_delta: y_delta,
                    wheel_delta: cloned_state.wheel_delta,
                    pinch_width_start: 0.0,
                    pinch_width: 0.0,
                    pinch_ratio: 1.0,
                    zoom: cloned_state.zoom,
                });
            } else {
                cloned_state.set(SvgMapModel { 
                    mx: x, 
                    my: y,
                    mouse_down: cloned_state.mouse_down,
                    pane_x: cloned_state.pane_x,
                    pane_y: cloned_state.pane_y,
                    pane_start_x: cloned_state.pane_start_x,
                    pane_start_y: cloned_state.pane_start_y,
                    start_pan_x: 0.0,
                    start_pan_y: 0.0,
                    x_delta: 0.0,
                    y_delta: 0.0,
                    wheel_delta: cloned_state.wheel_delta,
                    pinch_width_start: 0.0,
                    pinch_width: 0.0,
                    pinch_ratio: 1.0,
                    zoom: cloned_state.zoom,
                });
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
            //log!(format!("Left? : {} ; Top? : {}", x, y));
            let wide = if e.touches().length() == 2 {
                let touch_1 = e.touches().item(1).expect("Second touch object");
                let w = (touch_0.client_x() - touch_1.client_x()).abs() as f64;
                let h = (touch_0.client_y() - touch_1.client_y()).abs() as f64;
                if w >= h { w } else { h }
            } else { 0.0 };

            cloned_state.set(SvgMapModel { 
                mx: x, 
                my: y,
                mouse_down: cloned_state.mouse_down,
                pane_x: cloned_state.pane_x,
                pane_y: cloned_state.pane_y,                    
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: cloned_state.start_pan_x,
                start_pan_y: cloned_state.start_pan_y,
                x_delta: cloned_state.x_delta,
                y_delta: cloned_state.y_delta,
                wheel_delta: cloned_state.wheel_delta,
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
            //log!(format!("Left? : {} ; Top? : {}", x, y));
            let wide = if e.touches().length() == 2 {
                let touch_1 = e.touches().item(1).expect("Second touch object");
                let w = (touch_0.client_x() - touch_1.client_x()).abs() as f64;
                let h = (touch_0.client_y() - touch_1.client_y()).abs() as f64;
                if w >= h { w } else { h }
            } else { 0.0 };

            let pinch_ratio: f64 = if cloned_state.pinch_width_start > 1.0 && wide > 1.0 {
                 let ratio = ((wide as f64)/cloned_state.pinch_width_start).abs();
                 if ratio >= 2.0 
                 { 
                    2.0 
                } else { 
                    if ratio < 0.5 { 0.5 } else { ratio }
                }
            } else { 1.0 };

            log!(format!("Left? : {} ; Top? : {}", x, y));
            let x_delta = cloned_state.mx - x;
            let y_delta = cloned_state.my - y;
            cloned_state.set(SvgMapModel { 
                mx: x, 
                my: y,
                mouse_down: cloned_state.mouse_down,
                pane_x: cloned_state.pane_x - x_delta,
                pane_y: cloned_state.pane_y - y_delta,                    
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: cloned_state.start_pan_x,
                start_pan_y: cloned_state.start_pan_y,
                x_delta: cloned_state.x_delta,
                y_delta: cloned_state.y_delta,
                wheel_delta: cloned_state.wheel_delta,
                pinch_width_start: wide,
                pinch_width: wide,
                pinch_ratio: pinch_ratio,
                zoom: (cloned_state.zoom * pinch_ratio),
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
            cloned_state.set(SvgMapModel { 
                mx: x,
                my: y,
                mouse_down: true,
                pane_x: cloned_state.pane_x,
                pane_y: cloned_state.pane_y,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,                
                start_pan_x: cloned_state.mx,
                start_pan_y: cloned_state.my,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
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
        if let Some(_target) = e.target_dyn_into::<HtmlElement>() {

            // let x_delta = cloned_state.start_pan_x - cloned_state.mx;
            // let y_delta = cloned_state.start_pan_y - cloned_state.my;

           cloned_state.set(SvgMapModel { 
                mx: cloned_state.mx,
                my: cloned_state.my, 
                mouse_down: false,
                pane_x: cloned_state.pane_x, // - x_delta,
                pane_y: cloned_state.pane_y, // - y_delta,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: 0.0,
                start_pan_y: 0.0,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
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
        if let Some(_target) = e.target_dyn_into::<HtmlElement>() {

            // let x_delta = cloned_state.start_pan_x - cloned_state.mx;
            // let y_delta = cloned_state.start_pan_y - cloned_state.my;

           cloned_state.set(SvgMapModel { 
                mx: cloned_state.mx,
                my: cloned_state.my, 
                mouse_down: false,
                pane_x: cloned_state.pane_x, // - x_delta,
                pane_y: cloned_state.pane_y, // - y_delta,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: 0.0,
                start_pan_y: 0.0,
                x_delta: 0.0,
                y_delta: 0.0,
                wheel_delta: cloned_state.wheel_delta,
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
        if let Some(_target) = e.target_dyn_into::<HtmlElement>() {

            let wheel_y = e.delta_y() as f64;
            let mut zoom = cloned_state.zoom - (wheel_y / 300.0);

            if zoom > 12.0 { zoom = 12.0 };
            if zoom < 0.1 { zoom = 0.1 };

            cloned_state.set(SvgMapModel { 
                mx: cloned_state.mx,
                my: cloned_state.my, 
                mouse_down: false,
                pane_x: cloned_state.pane_x, // - x_delta,
                pane_y: cloned_state.pane_y, // - y_delta,
                pane_start_x: cloned_state.pane_start_x,
                pane_start_y: cloned_state.pane_start_y,
                start_pan_x: cloned_state.start_pan_x,
                start_pan_y: cloned_state.start_pan_y,
                x_delta: cloned_state.x_delta,
                y_delta: cloned_state.y_delta,
                wheel_delta: cloned_state.wheel_delta,
                pinch_width_start: cloned_state.pinch_width_start,
                pinch_width: 0.0,
                pinch_ratio: 1.0,
                zoom: zoom,
            });
        }
    });

    let cloned_state = state.clone();
    //{"width:auto;background-color:red;transform: translate3d(" + cloned_state.mx + "px, " +  cloned_state.my + "px, 0px);"} 

    let _mouse_x = cloned_state.mx;
    let _mouse_y = cloned_state.my;
    let pane_x = cloned_state.pane_x;
    let pane_y = cloned_state.pane_y;

    html!{
        <div style="height:100%;">
            <div style="height:100px;">
                <span>{" Mouse: "}{cloned_state.mx}{", "}{cloned_state.my}{" Mouse Down:"}{cloned_state.mouse_down}</span><br/>
                <span>{" Pane: "}{cloned_state.pane_x}{", "}{cloned_state.pane_y}</span><br/>
                <span>{" Pinch: start: "}{cloned_state.pinch_width_start}{" to "}{cloned_state.pinch_width}{" ratio: "}{cloned_state.pinch_ratio}</span><br/>
                <span>{" Start: "}{cloned_state.start_pan_x}{", "}{cloned_state.start_pan_y}</span>
                <span>{" Delta: "}{cloned_state.x_delta}{", "}{cloned_state.y_delta}{" Wheel: "}{cloned_state.wheel_delta}</span>
            </div>

        <div {onmousemove} {onmousedown} {onmouseup} {onmouseleave} {onwheel} {ontouchmove} {ontouchstart}
            style="touch-action: pinch-zoom;width:100%;height:calc(100% - 100px);background-color:gray;overflow:hidden;">
        <div style={format!(" pointer-events: none;width:auto;background-color:red;transform: translate3d({}px, {}px, 0px) scale({}, {});", pane_x, pane_y, cloned_state.zoom, cloned_state.zoom)} >
        
        //<svg width="149" height="147" viewBox="0 0 149 147" fill="none" xmlns="http://www.w3.org/2000/svg">
        <svg width="700" height="1024" viewBox="0 0 700 512" fill="none" xmlns="http://www.w3.org/2000/svg">
            <image x="-56" y="-156"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/328/710.png"
                />
            <image x="200" y="100"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/329/711.png"
                />
            <image x="200" y="356"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/329/712.png"
                />
            <image x="200" y="612"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/329/713.png"
                />
            <image x="456" y="100"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/330/711.png"
                />
            <image x="456" y="356"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/330/712.png"
                />
            <image x="456" y="612"
                width="256" height="256"
                //xlink:href="data:image/png;base64,IMAGE_DATA"
                href="https://c.tile.openstreetmap.org/11/330/713.png"
                />
            <path d="M60.5776 13.8268L51.8673 42.6431L77.7475 37.331L60.5776 13.8268Z" fill="#DEB819"/>
            <path d="M108.361 94.9937L138.708 90.686L115.342 69.8642" stroke="black" stroke-width="4" stroke-linecap="round" stroke-linejoin="round"/>
            <g>
                <circle cx="75.3326" cy="73.4918" r="55" fill="#FDD630"/>
                <circle cx="75.3326" cy="73.4918" r="52.5" stroke="black" stroke-width="5"/>
            </g>
            <circle cx="71" cy="99" r="5" fill="white" fill-opacity="0.75" stroke="black" stroke-width="3"/>
        </svg>
        </div>
        </div>
        </div>
    }
} 