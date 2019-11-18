FROM nginx
RUN apt-get update && apt-get install -y openssl
COPY proxy/nginx-entrypoint.sh /usr/local/bin/
COPY proxy/nginx.template.conf /app/nginx.template.conf
RUN chmod +x /usr/local/bin/nginx-entrypoint.sh
RUN ln -s /usr/local/bin/nginx-entrypoint.sh / # backwards compat
ENTRYPOINT ["/nginx-entrypoint.sh"]