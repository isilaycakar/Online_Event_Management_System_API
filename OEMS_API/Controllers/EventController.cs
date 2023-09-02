using Data_Access_Layer.Concrete;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OEMS_API.Models;
using System.Security.Claims;

namespace OEMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public EventController(Context context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Ticket == true && model.Price == null)
            {
                ModelState.AddModelError("Price", "Ücretli bir etkinlik için geçerli bir ücret girin.");
                return BadRequest(ModelState);
            }
            if (model.Ticket == false && model.Price != null)
            {
                ModelState.AddModelError("Price", "Biletli bir etkinlik oluşturmadınız. Fiyat bilgisi veremezsiniz.");
                return BadRequest(ModelState);
            }
            if (model.Ticket == false && model.TicketCompanyIds != null)
            {
                ModelState.AddModelError("Price", "Biletli bir etkinlik oluşturmadınız. Firma bilgisi veremezsiniz.");
                return BadRequest(ModelState);
            }

            if (!_context.Cities.Any(c => c.CityID == model.CityID))
            {
                ModelState.AddModelError("CityID", "Geçersiz şehir seçildi.");
                return BadRequest(ModelState);
            }

            if (!_context.Categories.Any(c => c.CategoryID == model.CategoryID))
            {
                ModelState.AddModelError("CategoryID", "Geçersiz kategori seçildi.");
                return BadRequest(ModelState);
            }

            var user = User.Identity.Name;

            var userId = _context.AppUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();

            if (userId == null)
            {
                return BadRequest("Geçerli bir kullanıcı kimliği bulunamadı.");
            }

            var newEvent = new Event
            {
                UserID = userId,
                Title = model.Title,
                Description = model.Description,
                Date = model.Date,
                CloseDate = model.CloseDate,
                Location = model.Location,
                Capacity = model.Capacity,
                Ticket = model.Ticket,
                Price = model.Price,
                Status = false,
                CategoryID = model.CategoryID,
                CityID = model.CityID
            };

            try
            {
                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                if (model.Ticket)
                {
                    if (model.TicketCompanyIds == null || !model.TicketCompanyIds.Any())
                    {
                        ModelState.AddModelError("TicketCompanyIds", "Biletli bir etkinlik için en az bir firma seçmelisiniz.");
                        return BadRequest(ModelState);
                    }

                    foreach (int companyId in model.TicketCompanyIds)
                    {
                        var ticketCompanyEvent = new TicketCompanyEvent
                        {
                            EventId = newEvent.EventID,
                            CompanyId = companyId
                        };
                        _context.TicketCompanyEvents.Add(ticketCompanyEvent);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Etkinlik başarıyla oluşturuldu.");
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = "Veritabanı hatası oluştu: " + ex.Message;

                if (ex.InnerException != null)
                {
                    errorMessage += "\nİç Hata: " + ex.InnerException.Message;
                }

                return StatusCode(500, errorMessage);
            }
        }

        [HttpPatch("{eventId}")]
        public async Task<IActionResult> EditEvent(int eventId, EditEventModel model)
        {
            var user = User.Identity.Name;
            var userId = _context.AppUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();

            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventID == eventId && e.UserID == userId);

            if (existingEvent == null)
            {
                return NotFound("Etkinlik bulunamadı veya düzenleme yetkiniz yok.");
            }

            var today = DateTime.Now;
            var fiveDaysBeforeCloseDate = existingEvent.CloseDate.AddDays(-5);

            if (today >= fiveDaysBeforeCloseDate)
            {
                return BadRequest("Etkinliği düzenlemek için süre doldu.");
            }

            existingEvent.Title = model.Title;
            existingEvent.Description = model.Description;
            existingEvent.Location = model.Location;
            existingEvent.Capacity = (int)model.Capacity;

            if (model.CityID.HasValue)
            {
               
                var cityExists = await _context.Cities.AnyAsync(c => c.CityID == model.CityID.Value);
                if (!cityExists)
                {
                    return BadRequest("Geçersiz CityID. Şehir bulunamadı.");
                }

                existingEvent.CityID = model.CityID.Value;
            }
            await _context.SaveChangesAsync();

            return Ok("Etkinlik başarıyla güncellendi.");
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> CancelEvent(int eventId)
        {
            var user = User.Identity.Name;
            var userId = _context.AppUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();

            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventID == eventId && e.UserID == userId);

            if (existingEvent == null)
            {
                return NotFound("Etkinlik bulunamadı veya iptal yetkiniz yok.");
            }

            var today = DateTime.Now;
            var fiveDaysBeforeEventDate = existingEvent.CloseDate.AddDays(-5);

            if (today >= fiveDaysBeforeEventDate)
            {
                return BadRequest("Etkinliği iptal etmek için süre doldu.");
            }

            
            _context.Events.Remove(existingEvent);

            await _context.SaveChangesAsync();

            return Ok("Etkinlik başarıyla iptal edildi.");
        }

        [HttpGet]
        public IActionResult GetMyEvents(MemberEventListModel memberEventListModel)
        {
            var user = User.Identity.Name;
            var userId = _context.AppUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();

            if (userId == null)
            {
                return BadRequest("Kullanıcı kimliği alınamadı.");
            }

            var userEvents = _context.Events
                .Where(e => e.UserID == userId)
                .ToList();


            var eventList = new List<MemberEventListModel>();

            foreach (var item in userEvents)
            {
                var statusText = item.Status ? "Onaylandı" : "Onay Bekliyor";


                eventList.Add(new MemberEventListModel
                {
                    EventID = item.EventID,
                    Title = item.Title,
                    Description = item.Description,
                    Date = item.Date,
                    CloseDate = item.CloseDate,
                    Location = item.Location,
                    Capacity = item.Capacity,
                    Ticket = item.Ticket,
                    Price = item.Price,
                    CategoryID = item.CategoryID,
                    CityID = item.CityID,
                    Status = statusText
                });
            }

            return Ok(eventList);
        }


       
    }
}


