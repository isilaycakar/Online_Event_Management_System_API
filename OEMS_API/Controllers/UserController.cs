using Data_Access_Layer.Concrete;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace OEMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly Context _context;

        public UserController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllEvents()
        {
            var allEvents = _context.Events
               .Where(e => e.Status == true)
               .Include(e => e.AppUser)
               .Include(e => e.Category)
               .Include(e => e.City)
               .ToList();

            var eventList = allEvents.Select(e => new
            {
                EventID = e.EventID,
                NameSurname = e.AppUser.NameSurname,
                Title = e.Title,
                CategoryName = e.Category.CategoryName,
                Description = e.Description,
                Date = e.Date,
                CloseDate = e.CloseDate,
                CityName = e.City.CityName,
                Location = e.Location,
                Capacity = e.Capacity,
                Ticket = e.Ticket,
                Price = e.Price,
                Status = e.Status
            }).ToList();

            return Ok(eventList);
        }

        [HttpGet("filterEvents")]
        public IActionResult FilterEvents(string? city, string? category)
        {
           var filteredEvents = _context.Events
                .Where(e => e.Status == true);

            if (!string.IsNullOrEmpty(city))
            {
                filteredEvents = filteredEvents
                    .Where(e => e.City.CityName == city);
            }

            if (!string.IsNullOrEmpty(category))
            {
                filteredEvents = filteredEvents
                    .Where(e => e.Category.CategoryName == category);
            }

            var eventList = filteredEvents.Select(e => new
            {
                EventID = e.EventID,
                NameSurname = e.AppUser.NameSurname,
                Title = e.Title,
                CategoryName = e.Category.CategoryName,
                Description = e.Description,
                Date = e.Date,
                CloseDate = e.CloseDate,
                CityName = e.City.CityName,
                Location = e.Location,
                Capacity = e.Capacity,
                Ticket = e.Ticket,
                Price = e.Price,
                Status = e.Status
            }).ToList();

            if (eventList.Count == 0)
            {
                return NotFound("Filtrelemeye uygun sonuç bulunamadı.");
            }
            
            return Ok(eventList);
        }

        [HttpPost("{eventId}")]
        public IActionResult JoinEvent(int eventId)
        {
            var user = User.Identity.Name;
            var userId = _context.AppUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();

            int capacity = _context.Events.Where(x=>x.EventID == eventId).Select(x=>x.Capacity).FirstOrDefault();
            int currentCapacity = _context.EventParticipants.Where(x=>x.EventId == eventId).Count();
           
            if (currentCapacity < capacity)
            {
                var participation = new EventParticipant
                {
                    UserID = userId,
                    EventId = eventId
                };

                _context.EventParticipants.Add(participation);
                _context.SaveChanges();

                return Ok("Etkinliğe katıldınız.");
            }

            return BadRequest("Etkinlik kontenjanı dolu. Katılamazsınız.");


        }

        [HttpGet("participatedPastEvents")]
        public IActionResult GetParticipatedPastEvents()
        {
            var user = User.Identity.Name;
            var userId = _context.AppUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();

            var currentDate = DateTime.Now;

            var pastEvents = _context.EventParticipants
                .Where(ep => ep.UserID == userId && ep.Event.Date < currentDate)
                .Select(ep => ep.Event)
                .OrderByDescending(e => e.Date)
                .ToList();

            var result = pastEvents.Select(e => new
            {
                EventID = e.EventID,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date
            }).ToList();

            return Ok(result);
        }

        [HttpGet("participatedFutureEvents")]
        public IActionResult GetParticipatedFutureEvents()
        {
            var user = User.Identity.Name;
            var userId = _context.AppUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();

            var currentDate = DateTime.Now;

            var futureEvents = _context.EventParticipants
                .Where(ep => ep.UserID == userId && ep.Event.Date >= currentDate)
                .Select(ep => ep.Event)
                .OrderBy(e => e.Date)
                .ToList();

            var result = futureEvents.Select(e => new
            {
                EventID = e.EventID,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date
            }).ToList();

            return Ok(result);
        }
    }
}
