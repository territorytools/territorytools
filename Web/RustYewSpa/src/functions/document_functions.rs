pub fn set_document_title(title: &str) {
    web_sys::window()
    .expect("no global `window` exists")
    .document()
    .expect("should have a document on window")
    .set_title(title);
}