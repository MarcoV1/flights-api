namespace angular_asp.Models
{
    public record BookingRm(
        Guid FlightId,
        string Airline,
        string Price,
        TimePlaceRm Arrival,
        TimePlaceRm Departure,
        int NumberOfBookedSeats,
        string PassengerEmail
     );
}
