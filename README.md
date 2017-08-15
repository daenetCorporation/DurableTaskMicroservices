# DurableTaskMicroservices

Microservice Framework based on Durable Task Framework

## Hosts

This repository contains multiple hosts, you can decide which host fits best to your needs.
At the moment we have the following hosts are available.

### WindowsServiceHost

This is a windows service.

#### How to Install the Service

The installation process is quite simple. First you should compile the DtfService solution and copy the output into the deployment folder (you should create one).
Then you should compile your orchestrations project and copy the output dll/config (xml/json) to the deployment folder. For each interface (orchestration) there should be one configuration. For example Test.Orch1.config.xml or Test.Orch2.config.json.

Now you should install the service with the installservice.bat file.

*TODO: Add image*

`Installservice.bat Orchestration1 "WindowsServiceHost.exePath" "Description of your Windows Server"`

#### How to Uninstall the service

`uninstallservice.bat Orchestration1`
