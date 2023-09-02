using angular_asp.Domain.Entities;
using angular_asp.Dtos;
using angular_asp.Models;
using Microsoft.AspNetCore.Mvc;
using angular_asp.Data;

namespace angular_asp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private readonly Entities _entities;

        public PassengerController(Entities entities)
        {
            _entities = entities;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Register(NewPassangerDto dto) 
        {
            _entities.Passengers.Add(new Passenger(
                dto.Email,
                dto.FirstName,
                dto.LastName,
                dto.Age
                ));

            _entities.SaveChanges();

            return CreatedAtAction(nameof(Find), new { email = dto.Email } );
        }

        [HttpGet("{email}")]
        public ActionResult<PassengerRm> Find(string email)
        {
            var passenger = _entities.Passengers.FirstOrDefault(x => x.Email == email);

            if (passenger == null)
            {
                return NotFound();
            }

            var rm = new PassengerRm(
                passenger.Email,
                passenger.FirstName,
                passenger.LastName,
                passenger.Age
                );

            return Ok(rm);
        }

    }
}
