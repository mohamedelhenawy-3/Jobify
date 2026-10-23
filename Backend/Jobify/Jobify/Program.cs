using Application.Events;
using Application.Handlers;
using Application.Interfaces.Admin;
using Application.Interfaces.Auth;
using Application.Interfaces.Company;
using Application.Interfaces.CompanyMembers;
using Application.Interfaces.Email;
using Application.Interfaces.Employer;
using Application.Interfaces.Job;
using Application.Interfaces.JobApplication;
using Application.Interfaces.JobsAlert;
using Application.Interfaces.JobSeeker;
using Application.Interfaces.Logger;
using Application.Interfaces.OnBoarding;
using Application.Interfaces.Permisions;
using Application.Interfaces.PublicCompany;
using Application.Interfaces.PublicProfile;
using Application.Interfaces.SavedJob;
using Application.Interfaces.Stats;
using Application.Interfaces.Token;
using Application.Interfaces.User;
using Application.SearchFeature.Interface;
using Domain.Identity;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation;
using Infrastructure.Implementation.Admin;
using Infrastructure.Implementation.AlertJobs;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.Background;
using Infrastructure.Implementation.CLoudinary;
using Infrastructure.Implementation.CompanyMembersService;
using Infrastructure.Implementation.CompanyMembersService.CurrentUserService;
using Infrastructure.Implementation.CompanyService;
using Infrastructure.Implementation.Email;
using Infrastructure.Implementation.EmployerService;
using Infrastructure.Implementation.FileUploadService;
using Infrastructure.Implementation.JobApplicationSrevice;
using Infrastructure.Implementation.JobService;
using Infrastructure.Implementation.Logger;
using Infrastructure.Implementation.OnBoardingService;
using Infrastructure.Implementation.PermisionsService;
using Infrastructure.Implementation.PublicCompanyService;
using Infrastructure.Implementation.PublicProfiles;
using Infrastructure.Implementation.SavedJobService;
using Infrastructure.Implementation.SearchService;
using Infrastructure.Implementation.StatsService;
using Infrastructure.Implementation.TokenServices;
using Infrastructure.Implementation.UserServices;
using Infrastructure.Reposoitory.AuthRepository;
using Infrastructure.Reposoitory.AuthRepository.Implementation;
using Infrastructure.Reposoitory.AuthRepository.Interface;
using Infrastructure.Reposoitory.RefreshTokenRepository.Implementation;
using Infrastructure.Reposoitory.RefreshTokenRepository.Interface;
using Infrastructure.Reposoitory.UserRepository.Implementation;
using Infrastructure.Reposoitory.UserRepository.Interface;
using Infrastructure.Security;
using Jobify.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. UPDATED CORS POLICY: Allow both Angular App AND local Swagger origin
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Add DbContext
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Add Identity
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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// Form Options for IFormFile Image Uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB limit
});

// Register Dependencies
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddScoped<IPublicProfileService, PublicProfileService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<ISimilarJobsService, SimilarJobsService>();
builder.Services.AddScoped<IActivityLogger, ActivityLogger>();
builder.Services.AddScoped<ICurrentUser,CurrentUserService>();
builder.Services.AddScoped<IJobSeeker, JobSeekerRepository>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IEmployeer, EmployerRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<JwtGenerator>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobSeekerService, JobSeekerService>();
builder.Services.AddScoped<IJobApplication, JobApplicationService>();
builder.Services.AddScoped<ISavedJob, SavedJobService>();
builder.Services.AddSingleton<ApplicationStatusNotifier>();
builder.Services.AddScoped<JobEmailTemplate>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IAdminVerificationService, AdminVerificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IEmployer, EmployerServicee>();
builder.Services.AddScoped<SupabaseService>();
builder.Services.AddHostedService<JobAlertBackgroundService>();
builder.Services.AddScoped<ApplicationStatusHandler>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IEmployerPermissionService, EmployerPermissionService>();
builder.Services.AddScoped<ICompanyMemberService, CompanyMemberService>();
builder.Services.AddScoped<IPublicCompanyService, PublicCompanyService>();
builder.Services.AddScoped<IAlertJobs, AlertJobsService>();
builder.Services.AddMemoryCache();

builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase"));

builder.Services.AddExceptionHandler<GlobalExeptionHandler>();
builder.Services.AddProblemDetails();

// Swagger Configuration with JWT Bearer
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

// Authentication & JWT Config
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

// Event Notification Handler Setup
var notifier = app.Services.GetRequiredService<ApplicationStatusNotifier>();
notifier.OnStatusChnaged += async (appData) =>
{
    using var scope = app.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<ApplicationStatusHandler>();
    await handler.Handler(appData);
};

// Seed Roles
using (var scope = app.Services.CreateScope())
{
    await SeedRoles.SeedAsync(scope.ServiceProvider);
}

// HTTP Pipeline Setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// MUST BE AT THE VERY TOP OF PIPELINE
app.UseCors("AllowAll");

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();