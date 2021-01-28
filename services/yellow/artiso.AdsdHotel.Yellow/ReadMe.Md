# Yellow Service

## Responsibility
The service is responsible for managing payment for orders.

## Commands
- `artiso.AdsdHotel.Yellow.Contracts.Commands.AuthorizeOrderCancellationFeeRequest`<br>
  Checks for the request having a FormOfPayment with a valid CreditCard. <br>
  Checks for the CreditCard having the required amount of money to pay the `Order` `CancellationFee`. <br> 
  Gets a Payment token to be used for future order transactions.
- `artiso.AdsdHotel.Yellow.Contracts.Commands.ChargeForOrderCancellationFeeRequest`<br>
  Gets the Amount of money to pay the order `Order` `CancellationFee`.
- `artiso.AdsdHotel.Yellow.Contracts.Commands.ChargeForOrderFullAmountRequest`<br>
  Gets the Amount of money to pay the order `Order` `Amount`.

## Events
- `artiso.AdsdHotel.Yellow.Events.OrderCancellationFeeAuthorizationAcquired`<br>
  Triggered when the `AuthorizeOrderCancellationFeeRequest` ends.
- `artiso.AdsdHotel.Yellow.Events.OrderCancellationFeeCharged`<br>
  Triggered when the `ChargeForOrderCancellationFeeRequest` ends.
- `artiso.AdsdHotel.Yellow.Events.OrderFullAmountCharged`<br>
  Triggered when the `ChargeForOrderFullAmountRequest` ends.
  
