# Yellow Service

## Responsibility
The service is responsible for managing payment for orders.

## Commands
- `artiso.AdsdHotel.Yellow.Contracts.Commands.AuthorizeCancellationFeeRequest`<br>
  Checks for the request having a FormOfPayment with a valid CreditCard. <br>
  Checks for the CreditCard having the required <br>
  amount of money to pay the `Rate` `CancellationFee`. <br> 
  Gets a Payment token to be used for in future transactions for this order.
- `artiso.AdsdHotel.Yellow.Contracts.Commands.ChargeCancellationFeeRequest`<br>
  Gets the Amount of money to pay the order `Rate` `CancellationFee`.
- `artiso.AdsdHotel.Yellow.Contracts.Commands.ChargeForFullAmountRequest`<br>
  Gets the Amount of money to pay the order `Rate` `Amount`.

## Events
- `artiso.AdsdHotel.Yellow.Events.RateCancellationFeeAuthorizationAcquired`<br>
  Triggered when the `AuthorizeCancellationFeeRequest` ends.
- `artiso.AdsdHotel.Yellow.Events.RateCancellationFeeCharged`<br>
  Triggered when the `ChargeCancellationFeeRequest` ends.
- `artiso.AdsdHotel.Yellow.Events.RateAmountCharged`<br>
  Triggered when the `ChargeForFullAmountRequest` ends.