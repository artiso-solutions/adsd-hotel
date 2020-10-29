# Red Service

## Responsability
The red service is responsible for managing price information for rooms, rates and cancellation fees depending on the selected room type.

## Data

- Room Type Pricing: Describes the price of a room type
- Rates: Contains a description, a list of services included (with prices for each service)
- Confirmation details: A collection of policies (cancellation fees/dates etc)

## Commands

- InputRoomRates: saves given room rates for OrderID and date (OrderID, Rates, Date)
- GetRoomRatesByRoomType: Returns the rate for a given room type (Room Pricing, Rates, Confirmation Details)

## Events

- OrderRatesSelected: is triggered after storing input room rate informations (OrderID, Rates)