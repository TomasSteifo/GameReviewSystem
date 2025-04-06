using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using GameReviewSystem.Data;
using GameReviewSystem.Services;
using GameReviewSystem.Validators;
using AutoMapper;
using Bogus;
using GameReviewSystem.Models;
using GameReviewSystem.Mapping;

// JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1) Read the JWT settings from configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["Key"];        // "this_should_be_long_and_random"
var issuer = jwtSection["Issuer"];        // "YourAppName"
var audience = jwtSection["Audience"];    // "YourAppName"

// 2) Add Authentication with JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // set true in production if using HTTPS
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,        // tokens expire
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

// 3) EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4) Domain Services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// 5) AutoMapper
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

// 6) Controllers + FluentValidation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
               .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 7) ChatGPT service registrations
builder.Services.AddHttpClient<ChatGPTService>((serviceProvider, httpClient) =>
{
    httpClient.BaseAddress = new Uri("https://api.openai.com/");
})
.ConfigureHttpMessageHandlerBuilder(builder =>
{
    // optional advanced config
});

builder.Services.AddTransient<ChatGPTService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = httpClientFactory.CreateClient(typeof(ChatGPTService).FullName!);
    var config = sp.GetRequiredService<IConfiguration>();
    return new ChatGPTService(client, config);
});

// 8) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 9) Build the app
var app = builder.Build();

// 10) Global Error Handler
app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext httpContext) =>
{
    var exceptionHandler = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var exception = exceptionHandler?.Error;
    return Results.Problem(
        title: "An error occurred while processing your request.",
        detail: exception?.Message,
        statusCode: 500
    );
});

// 11) Migrate & Seed (Bogus)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    if (!context.Games.Any())
    {
        var gameFaker = new Faker<Game>()
            .RuleFor(g => g.Title, f => f.Commerce.ProductName())
            .RuleFor(g => g.Platform, f => f.PickRandom(new[] { "PC", "Xbox", "PlayStation", "Switch" }))
            .RuleFor(g => g.Genre, f => f.PickRandom(new[] { "Action", "RPG", "Puzzle", "Strategy" }))
            .RuleFor(g => g.Status, f => f.PickRandom(new[] { "Backlog", "Pågående", "Klar" }));

        var fakeGames = gameFaker.Generate(10);
        context.Games.AddRange(fakeGames);
        context.SaveChanges();
    }
}

// 12) Configure the pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANT: Add authentication BEFORE authorization
app.UseAuthentication();  // required for JWT
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
