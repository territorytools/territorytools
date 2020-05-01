# Clock Tick Service
ln -s /root/TerritoryWeb/territory-tools-web-clock.timer /etc/systemd/system  
ln -s /root/TerritoryWeb/territory-tools-web-clock.service /etc/systemd/system  
systemctl enable territory-tools-web-clock  
systemctl start territory-tools-web-clock
systemctl daemon-reload   

# Useful Commands

systemctl daemon-reload   
systemctl enable territory-tools-web-clock  
systemctl list-timers  
systemctl enable territory-tools-web-clock.timer  
journalctl -u territory-tools-web-clock  

