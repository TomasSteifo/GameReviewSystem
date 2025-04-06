// ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GameReviewSystem.Data;
using GameReviewSystem.Services;
using FluentValidation.AspNetCore;
// ... your other using statements

namespace GameReviewSystem.Installers
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers EF Core with SQL Server using the "DefaultConnection" from appsettings.
        /// </summary>
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }

        /// <summary>
        /// Registers domain/business logic services (e.g., IGameService, IUserService).
        /// </summary>
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReviewService, ReviewService>();
            // Add more domain services if needed

            return services;
        }

        /// <summary>
        /// Configures JWT authentication from "Jwt" section in appsettings.
        /// </summary>
        public static IServiceCollection AddJwtAuth(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            var secretKey = jwtSection["Key"] ?? "";
            var issuer = jwtSection["Issuer"] ?? "";
            var audience = jwtSection["Audience"] ?? "";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };
            });

            return services;
        }

        /// <summary>
        /// Registers FluentValidation and optionally other MVC configuration.
        /// </summary>
        public static IServiceCollection AddValidation(
            this IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation(fv =>
                {
                    // register your validators
                    fv.RegisterValidatorsFromAssemblyContaining<Program>();
                });

            return services;
        }

        /// <summary>
        /// Registers Swagger for endpoint documentation.
        /// </summary>
        public static IServiceCollection AddSwaggerDocs(
            this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        /// <summary>
        /// Example: ChatGPT HttpClient registration, if needed.
        /// </summary>
        public static IServiceCollection AddChatGPTClient(
            this IServiceCollection services)
        {
            services.AddHttpClient<ChatGPTService>((sp, httpClient) =>
            {
                httpClient.BaseAddress = new Uri("https://api.openai.com/");
            });
            services.AddTransient<ChatGPTService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient(typeof(ChatGPTService).FullName!);
                var config = sp.GetRequiredService<IConfiguration>();
                return new ChatGPTService(client, config);
            });
            return services;
        }
        public static IServiceCollection AddJwtService(this IServiceCollection services)
        {
            services.AddScoped<JwtService>();
            return services;
        }

    }
}
