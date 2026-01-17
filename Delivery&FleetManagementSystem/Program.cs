using Microsoft.EntityFrameworkCore;
using SystemContext.SystemDbContext;
using ServiceLayer.AuthenticationServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using ServiceLayer.Hubs;
using ServiceLayer.OrderServices;
using ServiceLayer.UserServices;
using ServiceLayer.RestaurantServices;
using ServiceLayer.AreaServices;
using SystemModel.Entities;
using ServiceLayer.DriverServices;
using ServiceLayer.VehicleServices;
using ServiceLayer.MenuItemServices;
using ServiceLayer.NotificationServices;
using ServiceLayer.LocationServices;
using Microsoft.Extensions.Options;
using ServiceLayer.OrderStatusHistoryServices;
using ServiceLayer.ReviewServices;
var builder = WebApplication.CreateBuilder(args);
// Add DbContext

builder.Services.AddDbContext<DelivryDB>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Bind JWT settings

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add AuthService

builder.Services.AddScoped<IAuthService, AuthService>();

// Add OrderService

builder.Services.AddScoped<OrderService>();

// Add UserService

builder.Services.AddScoped<UserService>();

// Add RestaurantService

builder.Services.AddScoped<RestaurantService>();

// Add AreaService

builder.Services.AddScoped<AreaService>();

// Add RestaurantUserService

builder.Services.AddScoped<RestaurantUsersService>();

// Add DriverService

builder.Services.AddScoped<DriverService>();

// Add VehicleService

builder.Services.AddScoped<VehicleService>();

// Add MenuItemService

builder.Services.AddScoped<MenuItemService>();

// Add NotificationService

builder.Services.AddScoped<NotificationService>();

// Add DriverLocationService

builder.Services.AddScoped<LocationService>();

// Add OrderStatusHistoryService

builder.Services.AddScoped<OrderStatusHistoryService>();

// Add ReviewService

builder.Services.AddScoped<ReviewService>();

// Add EmailVerificationService

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailVerificationService>();

// Add Cart

builder.Services.AddScoped<CartService>();

// JWT Authentication

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey))
    };

    options.Events = new JwtBearerEvents
    {
         OnMessageReceived = context =>
         {
             var accessToken = context.Request.Query["access_token"];
             var path = context.HttpContext.Request.Path;
             if (!string.IsNullOrEmpty(accessToken) &&
            (path.StartsWithSegments("/hubs/DriverTracking") || path.StartsWithSegments("/hubs/Notification")))
            {
                 context.Token = accessToken;
            }
             return Task.CompletedTask;
         }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddSignalR();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Enter JWT token like: Bearer {your_token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<Delivery_FleetManagementSystem.Middlewares.ApiExceptionMiddleware>();

app.UseStaticFiles();
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/orderHub");
app.MapHub<NotificationHub>("/hubs/Notification");
app.MapHub<DriverTrackingHub>("/hubs/DriverTracking");
app.Run();
