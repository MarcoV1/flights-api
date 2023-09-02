using angular_asp.Data;
using angular_asp.Dtos;
using angular_asp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace angular_asp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly Entities _entities;

        public BookingController(Entities entities)
        {
             _entities = entities;
        }

        [HttpGet]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IEnumerable<BookingRm>), 200)]
        public ActionResult<IEnumerable<BookingRm>> Search()
        {
            var bookings = _entities.Flights.ToArray()
                .SelectMany(f => f.Bookings
                    .Select(b => new BookingRm(
                            f.Id,
                            f.Airline,
                            f.Price.ToString(),
                            new TimePlaceRm(f.Arrival.Place, f.Arrival.Time),
                            new TimePlaceRm(f.Departure.Place, f.Departure.Time),
                            b.NumberOfSeats,
                            b.PassengerEmail
                        ))
                    );


            return Ok(bookings);
        }


        [HttpGet("{email}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IEnumerable<BookingRm>), 200)]
        public ActionResult<IEnumerable<BookingRm>> List(string email)
        {

            var bookings = _entities.Flights.ToArray()
                .SelectMany(f => f.Bookings
                    .Where(b => b.PassengerEmail == email)
                    .Select(b => new BookingRm(
                            f.Id,
                            f.Airline,
                            f.Price.ToString(),
                            new TimePlaceRm(f.Arrival.Place, f.Arrival.Time),
                            new TimePlaceRm(f.Departure.Place, f.Departure.Time),
                            b.NumberOfSeats,
                            email
                        ))
                    );

            return Ok( bookings );
        }

    }
}
