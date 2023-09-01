using Data_Access_Layer.Concrete;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OEMS_API.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<TokenOption>(builder.Configuration.GetSection("TokenOption"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {

        TokenOption tokenOption = builder.Configuration.GetSection("TokenOption").Get<TokenOption>();

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = tokenOption.Issuer,
            ValidAudience = tokenOption.Audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecretKey))
        };
    });

builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, AppRole>(x =>
{
    x.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<Context>();



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
