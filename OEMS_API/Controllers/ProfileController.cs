using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OEMS_API.Models;
using System.Security.Claims;

namespace OEMS_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı bilgileri alınamadı.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var profileModel = new ProfileModel
            {
                Email = user.Email,
                NameSurname = user.NameSurname
            };

            return Ok(profileModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı bilgileri alınamadı.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Mevcut şifre yanlış.");
                return BadRequest(ModelState);
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Şifre başarıyla değiştirildi.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}
