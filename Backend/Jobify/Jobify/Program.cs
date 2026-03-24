using Application.Events;
using Application.Handlers;
using Application.Interfaces.Auth;
using Application.Interfaces.Auth;
using Application.Interfaces.Company;
using Application.Interfaces.Email;
using Application.Interfaces.Job;
using Application.Interfaces.JobApplication;
using Application.Interfaces.JobSeeker;
using Application.Interfaces.SavedJob;
using Application.Interfaces.Skills;
using Application.Interfaces.Token;
using Domain.Entities;
using Domain.Identity;
using Infrastructure.DBContext;
using Infrastructure.Implementation;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.Background;
using Infrastructure.Implementation.CLoudinary;
using Infrastructure.Implementation.CompanyService;
using Infrastructure.Implementation.Email;
using Infrastructure.Implementation.FileUploadService;
using Infrastructure.Implementation.JobApplicationSrevice;
using Infrastructure.Implementation.JobService;
using Infrastructure.Implementation.SavedJobService;
using Infrastructure.Implementation.TokenServices;
using Infrastructure.Reposoitory.AuthRepository;
using Infrastructure.Reposoitory.AuthRepository.Implementation; // wherever your AuthService lives
using Infrastructure.Reposoitory.AuthRepository.Interface;
using Infrastructure.Reposoitory.RefreshTokenRepository.Implementation;
using Infrastructure.Reposoitory.RefreshTokenRepository.Interface;
using Infrastructure.Reposoitory.UserRepository.Implementation;
using Infrastructure.Reposoitory.UserRepository.Interface;
using Infrastructure.Security;
using Jobify.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

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
builder.Services.AddScoped<ISkillsService, SkillsService>();
builder.Services.AddScoped<IJobApplication, JobApplicationService>();
builder.Services.AddScoped<ISavedJob, SavedJobService>();
builder.Services.AddSingleton<ApplicationStatusNotifier>();
builder.Services.AddScoped<JobEmailTemplate>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddScoped<SupabaseService>();
builder.Services.AddHostedService<JobAlertBackgroundService>();
builder.Services.AddScoped<ApplicationStatusHandler>();
builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase"));
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
builder.Services.AddExceptionHandler<GlobalExeptionHandler>();
builder.Services.AddProblemDetails(); // Required for standardized error formats

builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT Token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
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
var notifier = app.Services.GetRequiredService<ApplicationStatusNotifier>();

notifier.OnStatusChnaged += async (appData) =>
{
    using var scope = app.Services.CreateScope();

    var handler = scope.ServiceProvider.GetRequiredService<ApplicationStatusHandler>();

    await handler.Handler(appData);
};

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
app.UseCors("AllowAngularApp");
app.UseExceptionHandler();
app.UseAuthorization();

app.MapControllers();

app.Run();
