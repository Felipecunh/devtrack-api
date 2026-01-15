using DevTrack.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ================================
// 🔹 Controllers (API tradicional)
// ================================
builder.Services.AddControllers();

// ================================
// 🔹 Swagger / OpenAPI
// ================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================================
// 🔹 Entity Framework + SQLite
// ================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=devtrack.db"));

// ================================
// 🔹 JWT Authentication
// ================================
var jwtKey = "SUPER_SECRET_KEY_123_456"; // depois vai para appsettings

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ================================
// 🔹 Pipeline HTTP
// ================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⚠️ ORDEM IMPORTANTE
app.UseAuthentication();
app.UseAuthorization();

// ================================
// 🔹 Mapeia Controllers
// ================================
app.MapControllers();

app.Run();
