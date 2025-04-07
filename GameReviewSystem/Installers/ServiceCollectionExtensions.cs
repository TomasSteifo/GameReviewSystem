using Microsoft.Extensions.DependencyInjection;    
using Microsoft.Extensions.Configuration;          
using Microsoft.EntityFrameworkCore;               
using Microsoft.AspNetCore.Authentication.JwtBearer;  
using Microsoft.IdentityModel.Tokens;              
using System.Text;                                  
using GameReviewSystem.Data;                        
using GameReviewSystem.Services;                    
using FluentValidation.AspNetCore;                 

namespace GameReviewSystem.Installers
{
    // This static class contains extension methods for IServiceCollection.
    // These methods help register different parts of your application's services (e.g., EF, domain services, JWT, Swagger, etc.)
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers EF Core with SQL Server using the "DefaultConnection" string from configuration.
        /// </summary>
        /// <param name="services">The service collection to add the DbContext to.</param>
        /// <param name="configuration">The configuration instance to read the connection string from.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add AppDbContext to the DI container and configure it to use SQL Server.
            services.AddDbContext<AppDbContext>(options =>
            {
                // Use the connection string named "DefaultConnection" from the configuration (appsettings.json).
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            return services; // Return the modified service collection for method chaining.
        }

        /// <summary>
        /// Registers domain/business logic services (e.g., IGameService, IUserService, IReviewService).
        /// </summary>
        /// <param name="services">The service collection to add domain services to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            // Register IGameService with its concrete implementation GameService as a scoped service.
            services.AddScoped<IGameService, GameService>();
            // Register IUserService with its concrete implementation UserService as a scoped service.
            services.AddScoped<IUserService, UserService>();
            // Register IReviewService with its concrete implementation ReviewService as a scoped service.
            services.AddScoped<IReviewService, ReviewService>();
            // Additional domain services can be registered here if needed.

            return services; // Return the modified service collection.
        }

        /// <summary>
        /// Configures JWT authentication using settings from the "Jwt" section in configuration.
        /// </summary>
        /// <param name="services">The service collection to add authentication to.</param>
        /// <param name="configuration">The configuration instance to read JWT settings from.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddJwtAuth(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Get the JWT section from the configuration.
            var jwtSection = configuration.GetSection("Jwt");
            // Retrieve the secret key, issuer, and audience values from configuration.
            var secretKey = jwtSection["Key"] ?? "";
            var issuer = jwtSection["Issuer"] ?? "";
            var audience = jwtSection["Audience"] ?? "";

            // Configure authentication to use JWT Bearer tokens.
            services.AddAuthentication(options =>
            {
                // Set the default authentication scheme to JWT Bearer.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Configure the JWT Bearer options.
            .AddJwtBearer(options =>
            {
                // For development, HTTPS metadata is not required. In production, this should be true.
                options.RequireHttpsMetadata = false;
                // Save the token once validated.
                options.SaveToken = true;
                // Set up token validation parameters.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,                 // Validate the issuer (set in configuration).
                    ValidateAudience = true,               // Validate the audience.
                    ValidateLifetime = true,               // Ensure the token has not expired.
                    ValidateIssuerSigningKey = true,       // Ensure the signing key is valid.
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    // Create a symmetric security key from the secret key string.
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };
            });

            return services; // Return the modified service collection.
        }

        /// <summary>
        /// Registers controllers and configures FluentValidation for model validation.
        /// </summary>
        /// <param name="services">The service collection to add controllers and validation to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddValidation(
            this IServiceCollection services)
        {
            // Add controllers and integrate FluentValidation for automatic model validation.
            services.AddControllers()
                .AddFluentValidation(fv =>
                {
                    // Automatically register all validators found in the assembly containing the Program class.
                    fv.RegisterValidatorsFromAssemblyContaining<Program>();
                });

            return services; // Return the modified service collection.
        }

        /// <summary>
        /// Registers Swagger for API documentation and interactive testing.
        /// </summary>
        /// <param name="services">The service collection to add Swagger to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddSwaggerDocs(
            this IServiceCollection services)
        {
            // Adds services required for generating an OpenAPI/Swagger document.
            services.AddEndpointsApiExplorer();
            // Registers the Swagger generator, which creates a Swagger document for your API.
            services.AddSwaggerGen();
            return services; // Return the modified service collection.
        }

        /// <summary>
        /// Registers the ChatGPT HttpClient and its associated service.
        /// </summary>
        /// <param name="services">The service collection to add the ChatGPT client to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddChatGPTClient(
            this IServiceCollection services)
        {
            // Add a typed HttpClient for ChatGPTService with a predefined base address.
            services.AddHttpClient<ChatGPTService>((sp, httpClient) =>
            {
                httpClient.BaseAddress = new Uri("https://api.openai.com/");
            });
            // Register ChatGPTService as a transient service.
            // This factory retrieves an HttpClient from the factory and the configuration to create an instance.
            services.AddTransient<ChatGPTService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient(typeof(ChatGPTService).FullName!);
                var config = sp.GetRequiredService<IConfiguration>();
                return new ChatGPTService(client, config);
            });
            return services; // Return the modified service collection.
        }

        /// <summary>
        /// Registers the custom JwtService which is used to generate JWT tokens.
        /// </summary>
        /// <param name="services">The service collection to add the JwtService to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddJwtService(this IServiceCollection services)
        {
            // Register JwtService as a scoped service so it can be injected into controllers.
            services.AddScoped<JwtService>();
            return services;
        }
    }
}
