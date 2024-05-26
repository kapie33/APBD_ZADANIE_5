using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourNamespace.Data;
using YourNamespace.Models;
using YourNamespace.DTOs;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly YourDbContext _context;

        public TripsController(YourDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips()
        {
            return await _context.Trips
                .OrderByDescending(t => t.StartDate)
                .Select(t => new TripDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                })
                .ToListAsync();
        }

        [HttpDelete("clients/{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var client = await _context.Clients
                .Include(c => c.ClientTrips)
                .FirstOrDefaultAsync(c => c.Id == idClient);

            if (client == null)
            {
                return NotFound();
            }

            if (client.ClientTrips.Any())
            {
                return BadRequest("Client has assigned trips.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientTripDto clientTripDto)
        {
            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null)
            {
                return NotFound("Trip not found.");
            }

            var client = await _context.Clients.SingleOrDefaultAsync(c => c.Pesel == clientTripDto.Pesel);
            if (client == null)
            {
                client = new Client
                {
                    Name = clientTripDto.Name,
                    Surname = clientTripDto.Surname,
                    Pesel = clientTripDto.Pesel
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            if (_context.ClientTrips.Any(ct => ct.ClientId == client.Id && ct.TripId == idTrip))
            {
                return BadRequest("Client is already assigned to this trip.");
            }

            var clientTrip = new ClientTrip
            {
                ClientId = client.Id,
                TripId = idTrip,
                RegisteredAt = DateTime.UtcNow,
                PaymentDate = clientTripDto.PaymentDate
            };

            _context.ClientTrips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
