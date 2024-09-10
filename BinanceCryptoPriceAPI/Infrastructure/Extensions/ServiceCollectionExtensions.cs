namespace BinanceCryptoPriceAPI.Infrastructure.Extensions
{
    #region Usings
    using Application.Interfaces;
    using Application.Services;
    using BinanceCryptoPriceAPI.Infrastructure.JWT;
    using Data.Context;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System.Text;
    #endregion

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext with PostgreSQL provider
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString);
            });

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddSingleton<BinanceWebSocketService>();
            services.AddHostedService(provider => provider.GetRequiredService<BinanceWebSocketService>());

            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<IJwtToken, JwtToken>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       var key = configuration.GetSection("JWT:Secret").Value;
                       var issuer = configuration["JWT:Issuer"];
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuerSigningKey = true,
                           ValidIssuer = issuer,
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? "")),
                           ValidateLifetime = true,
                           ValidateIssuer = false,
                           ValidateAudience = false,
                           ClockSkew = TimeSpan.Zero
                       };
                       options.Events = new JwtBearerEvents
                       {
                           OnAuthenticationFailed = context =>
                           {
                               context.Response.Headers.Add("FAULT-TOKEN", "true");
                               if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                               {
                                   context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                               }
                               return Task.CompletedTask;
                           }
                       };
                       options.Events = new JwtBearerEvents
                       {
                           OnTokenValidated = context =>
                           {
                               context.Response.Headers.Clear();

                               return Task.CompletedTask;
                           }
                       };
                   });

            return services;
        }

        public static IServiceCollection AddAppSwagger(this IServiceCollection services)
          => services.AddSwaggerGen(configure =>
          {
              configure.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
              {
                  Name = "Authorization",
                  Type = SecuritySchemeType.ApiKey,
                  Scheme = "Bearer",
                  BearerFormat = "JWT",
                  In = ParameterLocation.Header,
                  Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
              });

              configure.AddSecurityRequirement(new OpenApiSecurityRequirement
                  {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                  });

              configure.SwaggerDoc("v1", new OpenApiInfo
              {
                  Title = "Binance Crypto Price API",
                  Version = "v1",
              });
          });
    }
}
