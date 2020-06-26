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
- search for special rate
  - rate is a price per night in a given date period (like high/low season)
- list all available room types with price per night (with picture and room description)
  - include information about rate changes
  
## Make booking
