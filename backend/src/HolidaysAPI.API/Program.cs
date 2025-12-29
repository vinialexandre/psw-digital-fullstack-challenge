using System.Text;
using HealthChecks.UI.Client;
using HolidaysAPI.API.Middlewares;
using HolidaysAPI.Application.Interfaces;
using HolidaysAPI.Application.Services;
using HolidaysAPI.Domain.Configuration;
using HolidaysAPI.Domain.Interfaces;
using HolidaysAPI.Infrastructure.Cache;
using HolidaysAPI.Infrastructure.Configuration;
using HolidaysAPI.Infrastructure.ExternalServices;
using HolidaysAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Holidays API",
        Version = "v1",
        Description = "API para consulta de feriados brasileiros"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddOptions<JwtSettings>()
    .Configure(options =>
    {
        options.Key = Environment.GetEnvironmentVariable("JWT_KEY")
            ?? builder.Configuration["Jwt:Key"]
            ?? "DefaultDevKey12345678901234567890123";
        options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? builder.Configuration["Jwt:Issuer"]
            ?? "HolidaysAPI";
        options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? builder.Configuration["Jwt:Audience"]
            ?? "HolidaysAPIUsers";
        options.ExpirationHours = int.TryParse(
            Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS") ?? builder.Configuration["Jwt:ExpirationHours"],
            out var hours) ? hours : 24;
    })
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<AuthSettings>()
    .Configure(options =>
    {
        options.AdminUsername = Environment.GetEnvironmentVariable("AUTH_ADMIN_USERNAME")
            ?? builder.Configuration["Auth:AdminUsername"]
            ?? "admin";
        options.AdminPassword = Environment.GetEnvironmentVariable("AUTH_ADMIN_PASSWORD")
            ?? builder.Configuration["Auth:AdminPassword"]
            ?? "admin";
    })
    .ValidateDataAnnotations()
    .ValidateOnStart();

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? builder.Configuration["Jwt:Key"]
    ?? "DefaultDevKey12345678901234567890123";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? builder.Configuration["Jwt:Issuer"]
    ?? "HolidaysAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? builder.Configuration["Jwt:Audience"]
    ?? "HolidaysAPIUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) && context.Request.Cookies.ContainsKey("jwt"))
                {
                    token = context.Request.Cookies["jwt"];
                }

                context.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var redisSettings = builder.Configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>()
    ?? new RedisSettings();

builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(RedisSettings.SectionName));

if (redisSettings.Enabled)
{
    var redisConfiguration = ConfigurationOptions.Parse(redisSettings.ConnectionString);
    redisConfiguration.AbortOnConnectFail = redisSettings.AbortOnConnectFail;
    redisConfiguration.ConnectTimeout = redisSettings.ConnectTimeout;
    redisConfiguration.SyncTimeout = redisSettings.SyncTimeout;
    redisConfiguration.ConnectRetry = redisSettings.ConnectRetry;

    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        return ConnectionMultiplexer.Connect(redisConfiguration);
    });

    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
}
else
{
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ICacheService, CacheService>();
}

builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["ready"])
    .AddRedis(redisSettings.ConnectionString, name: "redis", tags: ["cache", "redis", "ready"]);

builder.Services.AddHttpClient<IBrasilApiService, BrasilApiService>();

builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Holidays API v1");
    });
}

app.UseCors("AllowFrontend");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();
