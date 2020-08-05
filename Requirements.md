# Mission
- Identify the service boundaries
- What data does each service own?
- What part of which UI does it own?
- What events does it publish / subscribe to?

# Scenarios
- single hotel
- 1 guest per reservation
- 1 room per reservation
- no loyalty program / sign-in

## Search for room availability
- search for room types and not actual rooms
- search for date period
- list all available room types with price per night (with picture and room description)
  - include information about rate changes
  - rate is a price per night in a given date period (like high/low season)
  
## Make booking

### Refine booking
- summary of the room booking
  - Room Selection
  - Summary of Charges
    - Dates and rates
    - Total
- additional requests (on top of previous requests) can be specified **free of cost (for now)**
    - e.g. accessibility, early check-in, extra towels...
- Confirmation details

### Payment
- Guest Info needs to be stored for later verifyication
- Payment Mehtods 
  - Limited Methods due to complexity

### Booking validation and confirmation
- Payment method verification 
- authorize the amount of money needed to cancel the booking
- verification of room availability
  - if no room available release authorization
  - tell user "no room available"
  - there are limited amount of rooms, this capacity must be respected
  - capacity is the amount of roomtypes available on a given period
- Booking Confirmation
  - email
  - confirmation page

### Assign Room
- After booking validation and confirmation rooms must be reserved but NOT ASSIGNED (a room number) 

## Check-in
- Search confirmed Bookings by lastname
- Verify Info
- authorize Payment for the full stay
- Allocation of a room (room number)

## Check-out
- bill has to be printed (day before checkout)
- free room type capacity
- charge credit card etc. with actual amount

