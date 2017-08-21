# DurableTaskMicroservices

Microservice Framework based on Durable Task Framework

## Introduction to DurableTaskMicroservices

The *DurableTaskMicroservices* allows you to serialize/deserialize (save/load) orchestrations and tasks.

### Why should I use DurableTaskMicroservices?

The idea of *DurableTaskMicroservices* is to split the host and orchestration/task code. The host loads the assemblies of the orchestrations/tasks and their configuration files. This allows to change running orchestrations without changing the host code, just by replacing the assemblies and the configuration.

- host and orchestrations decoupling
- client and host decoupling
- dynamic loading of orchestrations (with configuration)

## Hosts

This repository contains multiple hosts, you can decide which host fits best to your needs.
You are free to implement your own host and contribute it via pull request.

### Microservice (Orchestrations) loading

This topic explains how the host loads the orchestration assemblies and the configuration.

1. The *WindowsServiceHost* searches for all `*.config.xml` files in the working directory.
These files must contain your XML serialized `Microservice`.
1. Now *WindowsServiceHost* gets all Types which are used in the `Microservices`.
To do this, the host gets all *.dlls in the working folder and check for the existence of `IntegrationAssemblyAttribute`.
1. Then the host starts the `TaskHubWorker` and create an `OrchestrationInstance` (if none are running).

### WindowsServiceHost

A simple windows service used to host the DurableTaskFramework.

#### How to Install the Service

The installation process is quite simple. First you should compile the WindowsServiceHost solution and copy the output into the deployment folder (you should create one).

To install the windows service, just run the installservice.ps1.
The PowerShell script will guide you trough the install process.

#### How to Uninstall the service

To uninstall the windows service, just run the uninstallservice.ps1.
