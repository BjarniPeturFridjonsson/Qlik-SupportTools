# Qlik-SupportTools
Tools that are used by Qlik Support for gathering and visualizing customer environments.

There are 4 tools in this repository 

* QlikView Log Collector 
* Qlik Sense Log Collector
* Qlik Cockpit (Demo)
* Proactive Express

### About the code.
The code has leveraged the code from the project Qlik Proactive Support which explains all the different project names all based on Norse mythology.  
The Support Tools in this repository was created for Qlik Product Support as a side project in 2018 and the Qlik Sense Log collector is built into Qlik Sense but we thought it would be interesting to allow the community and others opportunity to improve the software or make it their own.

Also, there is allot of communication with the Qlik Sense and QlikView api's and the code here would be a perfect start for someone that needs finished code that communicates with either of these products. So, feel free to rip anything out and use for your own projects. But I would love to hear from you how and what you used it for. 

For more detailed information look at the Wiki for this project.

## Licensing
This software is licensed under CC0 (Creative Commons) which basically means that you are free to do anything with this software as you see fit. You should though be aware that any graphics which contain Qlik Logos or copyrighted iconography are owned by Qlik and you are NOT free to use them in your own software or your own builds of this software.


## QlikView Log Collector
The QlikView Log Collector collects information used by product support for troubleshooting your QlikView installation.

You should run the QlikView Log Collector tool on the machine that is causing you issues. It is also possible to run the log collector from other locations and any QlikView server. 

The QlikView Log Collector creates a zip file on the machine where it runs. This file is created in the same folder as the QlikView Log Collector, or in your local temp folder. The zip file is named “QlikViewCollector” and is appended with a unique transport ID. Once the tool has finished running, it opens Windows Explorer with the zip file selected.

For QlikView Management API calls the user needs to be a member of the QlikView Management API security group but this group is not created by the QlikView installer and so must be created, and the relevant user added. The tool can assist you in creating this group.

## Qlik Sense Log Collector
The Qlik Sense Log Collector collects information used by product support for troubleshooting your Qlik Sense installation.

You can run the Qlik Sense Log Collector tool on your Qlik Sense central node, or the machine that is causing you issues. It is also possible to run the log collector from other locations, but you will need to install a certificate to enable it to communicate with your Qlik Sense installation. 

The Qlik Sense Log Collector creates a zip file on the machine that it runs on. This file is created in the same folder as the Qlik Sense Log Collector, or in your local temp folder. The zip file is named “SenseCollector” and is appended with a unique transport ID. Once the tool has finished running, it opens Windows Explorer with the zip file selected.

If you have any difficulty connecting to Qlik Sense, then run the tool using the same user account that you use for running the Qlik Sense services.

To do this, hold down the “Shift” key, right click on the tool, and choose “Run as different user”.

The Qlik Sense Log Collector needs a certificate to communicate with your Qlik Sense installation. If you run it on an existing Qlik Sense server, the certificate is already installed.

For more some help in using this tool look at these videos.

Qlik Fix: Collecting QS Log Files: https://www.youtube.com/watch?v=QfeTxdDzZXU 

and  Resolving your Qlik Issues 101: https://youtu.be/Gs-sZR5OlFo


## Qlik Cockpit
Is a demo in Winforms on how to display the information from QlikView and Qlik Sense log collectors.

The Qlik Cockpit tool allows you to view information gathered by the Qlik Sense Log Collector. You can also use it for viewing any log file with the extension .txt or .log or for viewing Windows Event log files with the .evtx extension. 

Qlik Cockpit does not access your Qlik Sense Installation or make any changes to the log files you are viewing. Due to high memory consumption and CPU usage, the tool should only be run on non-production servers.

You can drag and drop files and folders from the Qlik Sense Log Collector directly into the Qlik Cockpit tool. For example, when you drag and drop a Qlik Sense Log Collector zip file into the tool it will automatically unzip these files to your Temp folder so you can view the logs and system information in the Qlik Cockpit. You can also highlight and search through open files, log files and Windows event logs.

The Qlik Cockpit can also open the following file types:

*	.zip
*	.log
*	.txt 
*	.evtx

For a brief introduction to Qlik Cockpit see:
 https://youtu.be/YcwT77qb4uE

Techspert Thursday session on the Qlik Sense Log Collector and Qlik Cockpit:
https://youtu.be/Gs-sZR5OlFo



## Proactive Express
Qlik Proactive Express is locally installed real time monitoring system for Qlik Sense and QlikView. It is based around the notion of automatically warn of any outages or anomalies detected in a running system

* Simple real-time warning system for QlikView and Qlik Sense. 
* Locally installed at your site.
* Notifies to email, Microsoft Teams or Slack directly to customer.
* Configurable by DSE or knowledgeable administrator at customer site.

The monitor looks at:
* Disk, Cpu, Memory or any performance monitor counters in Windows.
* Sense service or server failures.
* Sense Tasks failures. 
* Monitors external exports used as import data for Sense/Qv.
* Monitoring Qvd files.

The analytics engine:
* Will trigger failures with duration or immediately.
* Has filters that can be used for only analyze important aspects.
* Has individual configurable duration of each failure.
* Any trigger value is configurable.

Detected issues:
* Can be sent to Email, Slack, MsTeams or any Webhooks enabled application.
* Each rule can be configured to notify to different channels or multiple.
* Has filters making sure that the notification does not spam.
* Logs each issue in a clear and easy log.











