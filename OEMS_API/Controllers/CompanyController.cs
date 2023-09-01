using Data_Access_Layer.Concrete;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OEMS_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Serialization;

namespace OEMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly Context _context;
        private readonly IConfiguration _configuration;

        public CompanyController(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CompanySignUp(TicketCompanyModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest("Şifreler uyuşmuyor!");
            }

            var ticketCompany = new TicketCompany
            {
                CompanyName = model.CompanyName,
                Url = model.WebsiteUrl,
                Mail = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Role = "Company"
            };

            _context.TicketCompanies.Add(ticketCompany);
            await _context.SaveChangesAsync();

            return Ok("Bilet şirketi başarıyla kaydedildi.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> CompanyLogin(SignInModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticketCompany = await _context.TicketCompanies.FirstOrDefaultAsync(c => c.Mail == model.Mail);

            if (ticketCompany == null || (model.Password != ticketCompany.Password))
            {
                return Unauthorized();
            }
            if (ticketCompany.Role != "Company")
            {
                return Forbid();
            }
            var token = GenerateJwtToken(ticketCompany);

            return Ok(new { token });
        }

        private string GenerateJwtToken(TicketCompany ticketCompany)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, ticketCompany.CompanyName),
        new Claim(ClaimTypes.Email, ticketCompany.Mail),
            };


            var tokenOption = _configuration.GetSection("TokenOption").Get<TokenOption>();
            var secretKey = tokenOption.SecretKey;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                 issuer: _configuration["JwtSettings:Issuer"],
                 audience: _configuration["JwtSettings:Audience"],
                 claims: claims,
                 expires: DateTime.UtcNow.AddHours(24),
                 signingCredentials: credentials
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet("info")]
        public IActionResult GetEventInfo(int eventId, string format)
        {
            var user = HttpContext.User;

            var eventInfo = _context.Events.FirstOrDefault(e => e.EventID == eventId);
            if (eventInfo == null)
            {
                return NotFound();
            }
            var companyRole = _context.TicketCompanies.Select(t => t.Role).FirstOrDefault();

            if (companyRole != "Company")
            {
                return Forbid();
            }

            if (format == "json")
            {
                return Ok(eventInfo);
            }
            else if (format == "xml")
            {
                // XML formatında dönüşüm yapın ve cevap olarak verin
                var xmlSerializer = new XmlSerializer(typeof(Event));
                var stringWriter = new StringWriter();
                xmlSerializer.Serialize(stringWriter, eventInfo);
                return Content(stringWriter.ToString(), "application/xml");
            }
            else
            {
                return BadRequest("Geçersiz format"); // Desteklenmeyen format isteği
            }
        }
    }
}
