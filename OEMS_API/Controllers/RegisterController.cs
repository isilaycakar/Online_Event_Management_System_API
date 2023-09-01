using Data_Access_Layer.Concrete;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OEMS_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OEMS_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly Context context;

        public RegisterController(UserManager<AppUser> userManager, IConfiguration configuration, SignInManager<AppUser> signInManager, Context context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel signUpModel)
        {
            var existingUser = await _userManager.FindByEmailAsync(signUpModel.Mail);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresi zaten kullanılıyor.");
                return BadRequest(ModelState);
            }
            if (signUpModel.Password != signUpModel.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Şifreler uyuşmuyor.");
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    NameSurname = signUpModel.NameSurname,
                    UserName = signUpModel.Username,
                    Email = signUpModel.Mail
                };

                var result = await _userManager.CreateAsync(user, signUpModel.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Member"); 
                    return Ok("Kullanıcı başarıyla oluşturuldu ve 'Member' rolüne atandı!");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return BadRequest(ModelState);

        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Mail);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        var token = GenerateJwtToken(user);
                        return Ok(new { Token = token });
                    }
                }
            }

            return Unauthorized("Geçersiz giriş bilgileri.");
        }
        private string GenerateJwtToken(AppUser user)
        {
            var tokenOption = _configuration.GetSection("TokenOption").Get<TokenOption>();
            var secretKey = tokenOption.SecretKey;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));


            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
