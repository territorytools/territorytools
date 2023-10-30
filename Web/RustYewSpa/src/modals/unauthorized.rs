use yew::prelude::*;

#[derive(Properties, Clone, PartialEq)]
pub struct Props {
    pub return_url: String,
}

#[function_component(UnauthorizedModal)]
pub fn unauthorized_modal(props: &Props) -> Html {
    let login_uri = format!("/Identity/Account/Login?ReturnUrl={}", props.return_url.clone());

    html!{
        <div class="modal show" id="unauthorized-modal" style="display:block;">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">{"Must Log In"}</h4>
                    </div>
                    <div class="modal-body">
                        <p>{"You are not logged in.  Please log in to continue."}</p>
                    </div>
                    <div class="modal-footer">
                        <a href={login_uri} class="btn btn-primary">{"Login"}</a>
                    </div>
                </div>
            </div>
        </div>
    }
}