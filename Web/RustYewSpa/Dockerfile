FROM rust:latest as build

RUN rustup target add wasm32-unknown-unknown
RUN cargo install trunk wasm-bindgen-cli

WORKDIR /usr/app
COPY ./src ./src
COPY ./scss ./scss
COPY ./app ./app
COPY ./*.toml .
COPY ./*.locks .
COPY ./*.ico .
COPY ./*.png .
COPY ./*.html .

RUN mkdir ./data
#RUN mkdir ./app
RUN trunk build --release --public-url "/app/"

FROM nginx:latest

#COPY ./nginx/nginx.conf /etc/nginx/conf.d/default.conf
RUN mkdir /etc/nginx/sites-available/
RUN mkdir /etc/nginx/sites-enabled/
#RUN mkdir /var/www/html
COPY ./nginx/default /etc/nginx/sites-available/default
COPY ./nginx/nginx.conf /etc/nginx/nginx.conf
RUN ln -s -f /etc/nginx/sites-available/default /etc/nginx/sites-enabled/
COPY --from=build /usr/app/dist /usr/share/nginx/html
#COPY --from=build /usr/app/dist /var/www/html

EXPOSE 80

STOPSIGNAL SIGQUIT

CMD ["nginx", "-g", "daemon off;"]