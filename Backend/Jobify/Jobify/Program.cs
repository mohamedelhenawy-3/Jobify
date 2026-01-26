using Application.Interfaces.Auth;
using Infrastructure.Reposoitory.AuthRepository.Implementation; // wherever your AuthService lives
using Infrastructure.Reposoitory.AuthRepository;
using Application.Interfaces.Auth;
using Infrastructure.Implementation.AuthService;
using Domain.Identity;
using Infrastructure.DBContext;
using Infrastructure.Reposoitory.UserRepository.Implementation;
using Infrastructure.Reposoitory.UserRepository.Interface;
using Jobify.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Interfaces.Token;
using Infrastructure.Implementation.TokenServices;
using Infrastructure.Reposoitory.RefreshTokenRepository.Interface;
using Infrastructure.Reposoitory.RefreshTokenRepository.Implementation;
using Infrastructure.Reposoitory.AuthRepository.Interface;
using Infrastructure.Security;
using Application.Interfaces.Company;
using Infrastructure.Implementation.CompanyService;
using Infrastructure.Implementation.CLoudinary;
using Application.Interfaces.Job;
using Infrastructure.Implementation.JobService;
using Application.Interfaces.JobSeeker;
using Domain.Entities;
using Infrastructure.Implementation;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
    policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();

    });

            });


// Add services to the container.
// 1. Add DbContext
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDBContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IJobSeeker, JobSeekerRepository>();
builder.Services.AddScoped<IEmployeer, EmployerRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<JwtGenerator>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobSeekerService, JobSeekerService>();


builder.Services.AddScoped<CloudinaryService>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
        ClockSkew = TimeSpan.Zero

    };
});


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    await SeedRoles.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp"); // Use CORS middleware here, AFTER Build

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
