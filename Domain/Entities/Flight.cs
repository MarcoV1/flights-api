using angular_asp.Domain.Errors;
using angular_asp.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace angular_asp.Domain.Entities
{
    public class Flight {

        public Guid Id { get; set; }
        public string Airline { get; set; }
        public string Price { get; set; }
        public TimePlace Departure { get; set; }
        public TimePlace Arrival { get; set; }
        public int RemainingNumberOfSeats { get; set; }

        public IList<Booking> Bookings = new List<Booking>();

        public Flight() {}

        public Flight(
            Guid id,
            string airline,
            string price,
            TimePlace departure,
            TimePlace arrival,
            int remainingNumberOfSeats)
        {
            Id = id;
            Airline = airline;
            Price = price;
            Departure = departure;
            Arrival = arrival;
            RemainingNumberOfSeats = remainingNumberOfSeats;
        }

        internal object? MakeBooking(string passengerEmail, byte numberOfSeats)
        {
            var flight = this;

            if (flight.RemainingNumberOfSeats < numberOfSeats)
                return new OverbookError();

            flight.Bookings.Add(
                new Booking(
                    passengerEmail,
                    numberOfSeats
                )
            );

            flight.RemainingNumberOfSeats -= numberOfSeats;
            return null;
        }

        public object? CancelBooking(string passengerEmail, byte numberOfSeats)
        {
            var booking = Bookings.FirstOrDefault(b => b.NumberOfSeats == numberOfSeats
                && b.PassengerEmail.ToLower() == passengerEmail.ToLower()
            );

            if ( booking == null )
                return new NotFoundError();

            Bookings.Remove( booking );
            RemainingNumberOfSeats += numberOfSeats;

            return null;
        }

  
    };
}
