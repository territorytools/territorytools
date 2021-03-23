# Fire Wall
On an Ubuntu server you can use **ufw** as a firewall

## Docky Ignores ufw Rules! Fix it
Source Article: https://www.techrepublic.com/article/how-to-fix-the-docker-and-ufw-security-flaw/

File: /etc/default/docker
````
DOCKER_OPTS="--iptables=false"
````

Then restart Docker
````
sudo systemctl restart docker
````

## SQL IP Address Whitelist
Source Article: https://www.digitalocean.com/community/tutorials/how-to-set-up-a-firewall-with-ufw-on-ubuntu-14-04

Only allow IP 1.2.3.4 to access port 1433
````
ufw allow from 1.2.3.4 to any port 1433
````

ufw status verbos
ufw status numbered

Delete rule number 123, Use `ufw status numbered` to find the number
````
ufw delete 123
````

# Limit ssh connections
By default, UFW rate limits 6 connections per 30 seconds, and it’s intended to be used for SSH: 
````
sudo ufw limit ssh
````