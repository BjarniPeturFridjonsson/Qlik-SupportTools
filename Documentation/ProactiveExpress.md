# Proactive Express - Questions and answers.
#### Questions and answers


* **Is the statistics sent directly to Qlik or a 3rd party?**  
The data is sent directly to the network here at Qlik and not shared with any 3rd party.
* **Is the statistics secured in transport?**  
The statistics are encrypted end to end through HTTPS using Qlik certificate and can’t be read while in transport.
* **How frequently are the statistics sent?**  
A small payload of statistics is sent once an hour and a slightly larger one once every 24 hours.
* **How much volume is sent?**  
The hourly payload is less than 5kb of Json formatted text, and the daily one is somewhere around ~ 50 kb of Json formatted text but can vary based on the size of the installation.
* **Will this Service impact my Sense deployment (i.e. slow it down or interfere with existing port traffic flows)?**  
No, it will not affect the installation. It utilizes the Qlik Sense and QlikView API’s to collect the statistics and is running so rarely that the added load is neglectable. This software is based on the Qlik Proactive Support which has been running for 4 years and Qlik Log Collectors which have been running for 2 years without any issues. 
* **What if I have an issue with the Service, whom do I reach out to?**  
Your Customer Success Manager will assist you with any question that you might have.
* **If I no longer wish to be included in the program, how easily can I uninstall the software?**  
It’s a standard Windows service installation. You go to “Add remove programs” and uninstall as you would any other standard software.
* **What software is required to “run” this software?**  
There are no further requirements for this software that is not already covered by a standard QlikView or Qlik Sense installation.
* **What permission is required to install this software?**  
We recommend you run this software with the same user account that is running QlikView or Qlik Sense.
* **Does this software have System Requirements?**  
There are no further requirements for this software that is not already covered by a standard QlikView or Qlik Sense installation.
* **Does this software have browser requirements (if needed)?**  
This software does not use a browser.
* **Will virus scanner or firewalls potentially stop outgoing traffic flows?**  
A virus scanner should not stop this software and the firewall should be configured to allow access to proactive.qliktech.com on port 443 (HTTPS) if outgoing traffic is generally blocked.
* **Does the software need to be installed on DEV, TEST, and PROD nodes? Central nodes?**  
The software can be run from any server, but running it on a Qlik server node is recommended. What environments should be a part of this program is something that you the customer and your Customer Success Manager will agree on.
* **What testing has Qlik done to insure this is safe software (low/no vulnerabilities)?**  
This software is based on the Qlik Proactive Support which has been running for 4 years and Qlik Log Collectors which have been running for 2 years without any issues. This software has been vetted by our security department and is deemed safe. This software has no capabilities to receive any data or send sensitive information.

