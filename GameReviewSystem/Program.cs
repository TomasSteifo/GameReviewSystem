using FluentValidation;
using GameReviewSystem.Data;
using GameReviewSystem.Installers; // The namespace where your extension methods live

var builder = WebApplication.CreateBuilder(args);

// 1) Add services via extension methods
builder.Services
    .AddDatabase(builder.Configuration)   // EF, domain DB
    .AddDomainServices()                 // IGameService, IUserService, etc.
    .AddJwtAuth(builder.Configuration)   // JWT config
    .AddValidation()                     // FluentValidation + controllers
    .AddSwaggerDocs()                       // Swagger
    .AddJwtService()   
    .AddChatGPTClient();                 // if needed

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();



// 2) Build the app
var app = builder.Build();

// 3) (Optional) Error handling 
//    If you want a global error page or simple handler
app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext context) =>
{
    var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var ex = feature?.Error;
    return Results.Problem(
        title: "An error occurred",
        detail: ex?.Message,
        statusCode: 500
    );
});

// 4) (Optional) Migrate & Seed 
//    If you want to ensure DB is created / migrated, then seed data
using (var scope = app.Services.CreateScope())
{
    // e.g. var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // context.Database.Migrate();
    // if (!context.Games.Any()) { ... fill with Bogus data ... }
}

// 5) Swagger in dev mode
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}


// 6) Typical pipeline steps
app.UseHttpsRedirection();

// If using JWT or other auth:
app.UseAuthentication();   // must come before UseAuthorization
app.UseAuthorization();

// 7) Map controllers
app.MapControllers();

// 8) Run
app.Run();
