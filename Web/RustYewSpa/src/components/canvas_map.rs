use gloo_console::log;
use wasm_bindgen::JsCast;
use web_sys::HtmlElement;
use yew::prelude::*;

#[function_component(CanvasMap)]
pub fn canvas_map() -> Html {
    //console_error_panic_hook::set_once();
    let window = web_sys::window().unwrap();
    let document = window.document().unwrap();
    let canvas = document
        .get_element_by_id("canvas")
        .expect("An HTML element with the id 'canvas'")
        .dyn_into::<web_sys::HtmlCanvasElement>()
        .expect("An HtmlCanvasElement");
    let context = canvas
        .get_context("2d")
        .expect("2d context")
        .unwrap()
        .dyn_into::<web_sys::CanvasRenderingContext2d>()
        .unwrap();
    context.move_to(100.0, 0.0); // top of triangle
    context.begin_path();
    context.line_to(0.0, 200.0); // bottom left of triangle
    context.line_to(200.0, 300.0); // bottom right of triangle
    context.line_to(100.0, 0.0); // back to top of triangle
    context.close_path();
    context.stroke();
    context.fill();

    // canvas.addEventListener("click", |event| {
    //     const boundingRect = canvas.getBoundingClientRect();

    //     const scaleX = canvas.width / boundingRect.width;
    //     const scaleY = canvas.height / boundingRect.height;

    //     const canvasLeft = (event.clientX - boundingRect.left) * scaleX;
    //     const canvasTop = (event.clientY - boundingRect.top) * scaleY;

    //     const row = Math.min(Math.floor(canvasTop / (CELL_SIZE + 1)), height - 1);
    //     const col = Math.min(Math.floor(canvasLeft / (CELL_SIZE + 1)), width - 1);

    //     universe.toggle_cell(row, col);

    //     // drawGrid();
    //     // drawCells();
    //   });
    //Ok(())
    html! {
        <>
        <p>{"Canvas Map Here"}</p>

        </>
    }
}
