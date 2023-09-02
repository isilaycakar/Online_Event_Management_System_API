using Data_Access_Layer.Concrete;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OEMS_API.Models;
using System.Data;

namespace OEMS_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly Context _context;

        public AdminController(Context context)
        {
            this._context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                CategoryName = model.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok("Kategori başarıyla eklendi.");
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return NotFound("Kategori bulunamadı.");
            }

            category.CategoryName = model.CategoryName;
            await _context.SaveChangesAsync();

            return Ok("Kategori başarıyla güncellendi.");
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return NotFound("Kategori bulunamadı.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Kategori başarıyla silindi.");
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = new City
            {
                CityName = model.CityName
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return Ok("Şehir başarıyla eklendi.");
        }

        [HttpPut("{cityId}")]
        public async Task<IActionResult> UpdateCity(int cityId, CityModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = await _context.Cities.FindAsync(cityId);

            if (city == null)
            {
                return NotFound("Şehir bulunamadı.");
            }

            city.CityName = model.CityName;
            await _context.SaveChangesAsync();

            return Ok("Şehir başarıyla güncellendi.");
        }

        [HttpDelete("{cityId}")]
        public async Task<IActionResult> DeleteCity(int cityId)
        {
            var city = await _context.Cities.FindAsync(cityId);

            if (city == null)
            {
                return NotFound("Şehir bulunamadı.");
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Ok("Şehir başarıyla silindi.");
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var statusFalseEvents = _context.Events
                .Where(e => e.Status == false)
                .Select(item => new
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
                    Status = item.Status
                })
                .ToList();

            return Ok(statusFalseEvents);
        }

        [HttpPatch("{eventId}")]
        public async Task<IActionResult> ApproveEvent(int eventId)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);

            if (existingEvent == null)
            {
                return NotFound("Etkinlik bulunamadı.");
            }

            existingEvent.Status = true;

            await _context.SaveChangesAsync();

            return Ok("Etkinlik onaylandı.");
        }
    }
}
