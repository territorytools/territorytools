#!/bin/sh
cp ./backup-territory-db.timer /etc/systemd/system
cp ./backup-territory-db.service /etc/systemd/system
systemctl enable backup-territory-db.timer
systemctl start backup-territory-db.timer
systemctl daemon-reload
systemctl list-timers