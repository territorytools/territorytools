# Back Up Scripts
This project is deployed to **/root/TerritoryWeb/Scripts**

# Steps
Attach a volume, a disk, to the VM, this should automatically be done if you're using a Digital Ocean droplet

Remember to link a storage volume (Digital Ocean)
````
ln -s /mnt/volume_sfo2_17/ /root/backups
````
(The sfo2_17 part of that path may be different_)

Create and .env file based on the examle.env file in this folder.

````
   cp ./backup-territory-db.timer /etc/systemd/system
   cp ./backup-territory-db.service /etc/systemd/system
   systemctl daemon-reload
   systemctl enable backup-territory-db.timer
   systemctl start backup-territory-db.timer
````

# Useful Commands
systemctl daemon-reload
systemctl enable backup-territory-db
systemctl list-timers
systemctl enable backup-territory-db.timer
journalctl -u backup-territory-db

