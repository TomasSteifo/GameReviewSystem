using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using GameReviewSystem.Data;
using GameReviewSystem.Services;
using GameReviewSystem.Validators;
using AutoMapper.Extensions.Microsoft.DependencyInjection; // For AddAutoMapper
using Bogus;
using GameReviewSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 2) Services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// 3) AutoMapper
// Make sure your package versions match. Possibly "14.0.0" for both:
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// 4) Controllers + FluentValidation (NEW WAY)
builder.Services.AddControllers();

// Instead of .AddFluentValidation(...):
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// This automatically registers all validators in the given assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 4a) Register all validators from the current assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 5) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6) Optional Error Handler
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

// 7) Database + Bogus
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

// 8) Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
