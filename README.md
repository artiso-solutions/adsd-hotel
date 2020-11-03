# Advanced Distributed Systems Design - Hotel Booking

Demo app designed with the SOA definition described in the Advanced Distributed Systems Design course by Udi Dahan.

[![Requirements](https://img.shields.io/badge/view-requirements-informational)](docs/Requirements.md)
[![Discussions](https://img.shields.io/badge/view-discussions-informational)](https://github.com/artiso-solutions/adsd-hotel/discussions)
[![How to Run](https://img.shields.io/badge/view-how%20to%20run-informational)](docs/how-to-run.md)
[![Project Structure](https://img.shields.io/badge/view-project%20structure-informational)](https://github.com/artiso-solutions/adsd-hotel/wiki/Project-structure)
[![Meeting Notes](https://img.shields.io/badge/view-meeting%20notes-informational)](https://github.com/artiso-solutions/adsd-hotel/wiki/Meeting-notes)

## Design

![System diagram](assets/images/system-diagram.png)

The system will be composed of seven business-specific services.<br/>
The diagram shows the [efferent coupling](https://en.wikipedia.org/wiki/Efferent_coupling) between the services, where the arrows mean "depends on / knows".

As shown in the image, the purple and indigo services have the highest levels of coupling: this is because they both operate as orchestrators (i.e. as a semaphonre) or aggregators (i.e. composing models based upon external data).

## Companion services

Every system designed with SOA will have two additional services: IT Ops and Branding.<br/>
These services are primarily technical in nature and tend to have no dependencies, and they're used (e.g. dll reference) by the other services.<br/>

![IT Ops and Branding diagram](assets/images/itops-branding-diagram.png)

IT Ops is responsible for things like integration and data transformations.<br/>
Branding is responsible for localization, wording and UI assets.

## UI composition

The UI will communicate with a backing [gateway](https://docs.microsoft.com/en-us/azure/architecture/patterns/gateway-aggregation) in order to interact with the services.

The gateway will use a series of [ambassadors](https://docs.microsoft.com/en-us/azure/architecture/patterns/ambassador) provided by each service in order to reduce the platform and schema coupling.<br/>
The results will be aggregated into a generic structure, sent back as a response and then interpreted by specific UI components, also owned by the services.

![UI composition diagram](assets/images/ui-diagram.png)
