FROM alpine:latest

ENV APP_USER appuser

RUN adduser -g "App User" -D $APP_USER
RUN apk add curl

COPY crontab /var/spool/cron/crontabs/$APP_USER
RUN chmod 0600 /var/spool/cron/crontabs/$APP_USER

ENTRYPOINT "crond"

CMD ["-f", "-d", "8"]