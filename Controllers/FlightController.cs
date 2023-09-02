using angular_asp.Dtos;
using angular_asp.Models;
using angular_asp.Data;
using Microsoft.AspNetCore.Mvc;
using angular_asp.Domain.Errors;
using Microsoft.EntityFrameworkCore;
using angular_asp.Domain.Entities;

namespace angular_asp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly ILogger<FlightController> _logger;

        private readonly Entities _entities;


        public FlightController(ILogger<FlightController> logger, Entities entities)
        {
            _logger = logger;
            _entities = entities;
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FlightRm>), 200)]
        public IEnumerable<FlightRm> Search([FromQuery] FlightSearchParameters @params)
            {
            IQueryable<Flight> flights = _entities.Flights;

            if (!string.IsNullOrWhiteSpace(@params.Destination))
                flights = flights.Where(f => f.Arrival.Place.Contains(@params.Destination));

            if (!string.IsNullOrWhiteSpace(@params.From))
                flights = flights.Where(f => f.Departure.Place.Contains(@params.From));

            if (@params.FromDate != null)
                flights = flights.Where(f => f.Departure.Time >= @params.FromDate.Value.Date);

            if (@params.ToDate != null)
                flights = flights.Where(f => f.Departure.Time >= @params.ToDate.Value.Date.AddDays(1).AddTicks(-1));

            if (@params.NumberOfPassengers != 0 && @params.NumberOfPassengers != null)
                flights = flights.Where(f => f.RemainingNumberOfSeats >= @params.NumberOfPassengers);
            else
                flights = flights.Where(f => f.RemainingNumberOfSeats >= 1);

            var flightRmList = flights
                .Select(flight => new FlightRm(
                    flight.Id,
                    flight.Airline,
                    flight.Price,
                    new TimePlaceRm(flight.Departure.Place.ToString(), flight.Departure.Time),
                    new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time),
                    flight.RemainingNumberOfSeats
                    
                    )).ToArray();
            
            return flightRmList;
            }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(typeof(FlightRm), 200)]
        [HttpGet("{id}")]
        public ActionResult<FlightRm> Find(Guid id) {
            var flight = _entities.Flights.SingleOrDefault(f => f.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            var readModel = new FlightRm(
                flight.Id,
                flight.Airline,
                flight.Price,
                new TimePlaceRm(flight.Departure.Place.ToString(), flight.Departure.Time),
                new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time),
                flight.RemainingNumberOfSeats
                );
            
            return Ok(readModel);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(200)]
        public IActionResult Book(BookDto dto)
        {
            System.Diagnostics.Debug.WriteLine($"Booking ${dto} and ${dto.FlightId}");
            System.Diagnostics.Debug.WriteLine($"{_entities.Flights}");

            var flight = _entities.Flights.SingleOrDefault(f => f.Id == dto.FlightId);

            if (flight == null) 
                return NotFound();

            var error = flight.MakeBooking(dto.PassengerEmail, dto.NumberOfSeats);

            if (error is OverbookError)
                return Conflict(new {message = "The number of requested seats exceeds the number of remaining seats." });

            try {
                _entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex) {
                return Conflict(new { message = "An error occurred while booking. Please try again." });
            }

            return CreatedAtAction(nameof(Find), new { id = dto.FlightId });

        }


        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(500)]
        public IActionResult Cancel(BookDto dto)
        {
            var flight = _entities.Flights.Find(dto.FlightId);

            var error = flight?.CancelBooking(dto.PassengerEmail, dto.NumberOfSeats);

            if(error == null)
            {
                _entities.SaveChanges();
                return NoContent();
            }

            if (error is NotFoundError) return NotFound();
               
            throw new Exception($"The error of type: {error.GetType().Name} occured while canceling the booking made by {dto.PassengerEmail}");

        }


    }
}