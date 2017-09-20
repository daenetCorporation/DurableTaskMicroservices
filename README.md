# DurableTaskMicroservices

Microservice Framework based on Durable Task Framework.
A [nuget](https://www.nuget.org/packages/Daenet.DurableTask.Microservices/) package is available.

## Introduction to DurableTaskFramework

The Durable Task Framework is an open source project originally started by Microsoft. It enables you to write orchestrations in pure C# using the .Net framework. By using of this framework, you can implement many kinds of integration projects without of need to use any Enterprise Service Bus product.

Here are the key features of the durable task framework:

- Definition of code orchestrations in simple C# code
- Automatic persistence and check-pointing of program state
- Versioning of orchestrations and activities
- Async timers, orchestration composition, user aided checkpoints

The framework itself is very light weight and only requires an Azure Service Bus namespace and optionally an Azure Storage account. Running instances of the orchestration and worker nodes are completely hosted by the user. The framework uses a scheduler on top of Azure Service Bus (hosted in Cloud or OnPrem) and it is completely transparent to developer. Developers is not ever required to have any Service Bus know-how.

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

1. The *WindowsServiceHost* searches for all `*.config.xml` files in the working directory. These files must contain your XML serialized `Microservice`.
1. Now *WindowsServiceHost* gets all Types which are used in the `Microservices`. To do this, the host gets all *.dlls in the working folder and check for the existence of `IntegrationAssemblyAttribute`.
1. Then the host starts the `TaskHubWorker` and create an `OrchestrationInstance` (if none are running).

### WindowsServiceHost

A simple windows service used to host the DurableTaskFramework.

#### How to Install the Service

The installation process is quite simple. First you should compile the WindowsServiceHost solution and copy the output into the deployment folder (you should create one).

To install the windows service, just run the `installservice.ps1`.
The PowerShell script will guide you trough the install process.

#### How to Uninstall the service

To uninstall the windows service, just run the `uninstallservice.ps1`.

# Daenet.DurableTaskMicroservices.Common

This library extends the Durable Task Framework with base classes for Orchestrations, Tasks.
Furthermore it includes general reusable Tasks and Adapters for SQL and FileSystems.

A [nuget](https://www.nuget.org/packages/Daenet.DurableTaskMicroservices.Common/) package is available.

## BaseClasses

There are several BaseClasses, they extend the DurableTask Framework.

### OrchestrationBase

`OrchestrationBase` inherits `TaskOrchestration` and adds base Logging functionality to an Orchestration.

This includes:

- Logging Scope with ActivityId
- Logging Scope with OrchestrationInstanceId

### TaskBase

`TaskBase` inherits `TaskActivity` and extends it with following features:

- LogManager initialization (Logging Scopes)
- Exception logging in case of Task execution throws an exception.

### Adapters

*Adapters* are used to send/receive data from any source to a different source.

### ReceiveAdapterBase

`ReceiveAdapterBase` makes writing adapters for receiving data easier.

Features:

- build-in method `executeValidationRules` executes all given (implemented by user) `ValidationRuleDescriptor`

### SendAdapterBase

`ReceiveAdapterBase` makes writing adapters for sending data easier.
It is implemented as an abstract class with a `SendData` method the user has to implement.

## Tasks

The `Daenet.DurableTaskMicroservices.Common` package includes several ready-to-use Tasks.

### DelayTask

The `DelayTask` delays the execution by using `Thread.Sleep`.

### LoggingTask

The `LoggingTask` allows you to Log a messages.
`LoggingTask` uses [LogManager](https://github.com/daenetCorporation/Daenet.Common.Logging) internal.