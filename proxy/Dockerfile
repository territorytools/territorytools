FROM nginx
RUN apt-get update && apt-get install -y openssl
COPY nginx-entrypoint.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/nginx-entrypoint.sh
RUN ln -s /usr/local/bin/nginx-entrypoint.sh / # backwards compat
ENTRYPOINT ["/nginx-entrypoint.sh"]