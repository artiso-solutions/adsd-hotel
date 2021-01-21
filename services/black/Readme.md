# Black Service

## Responsability
The black service is responsible for managing the guest information linked to a specific order.

## Connectivity

The black service is reachable via a NServiceBus endpoint called `Black.Api` and supports callbacks.

## Data

- `artiso.AdsdHotel.Black.Contracts.GuestInformation`<br>
Contains information about the guest, i.e. name and contact information.

## Commands

- `artiso.AdsdHotel.Black.Commands.SetGuestInformation`<br>
Used to add or update guest information for a specific order.
- `artiso.AdsdHotel.Black.Commands.RequestGuestInformation`<br>
Used to get guest information for a specific order.
- `artiso.AdsdHotel.Black.Commands.GuestInformationReponse`<br>
The response containing the requested guest information.

## Events

- `artiso.AdsdHotel.Black.Events.GuestInformationSet`<br>
Is published when new guest information for a specific order was set.
