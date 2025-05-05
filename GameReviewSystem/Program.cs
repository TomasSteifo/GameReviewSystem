
using FluentValidation; 
using GameReviewSystem.Data; 
using GameReviewSystem.Installers; 
using GameReviewSystem.Mapping; 
using GameReviewSystem.Services; 
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens; 
using System.Text; 

// Create the WebApplicationBuilder, which sets up configuration and services.
var builder = WebApplication.CreateBuilder(args);

// Register application services using our custom extension methods.
// These methods add our database context, domain services, JWT authentication, FluentValidation, Swagger, etc.
builder.Services
    .AddDatabase(builder.Configuration)   // Registers EF Core with SQL Server using the "DefaultConnection" from configuration.
    .AddDomainServices()                    // Registers domain/business logic services (e.g., IGameService, IUserService, IReviewService).
    .AddJwtAuth(builder.Configuration)      // Configures JWT authentication using settings from the "Jwt" section.
    .AddValidation()                        // Adds controllers and FluentValidation integration.
    .AddSwaggerDocs()                       // Registers Swagger services for API documentation.
    .AddJwtService()                        // Registers the custom JwtService for generating JWT tokens.
    .AddChatGPTClient();                    // Registers ChatGPT client and its dependencies (if needed).

// Additional registrations (if not already included in the extension methods)
// The following lines might be redundant if your extension methods already register these services.
builder.Services.AddDbContext<AppDbContext>(); // (Optional/Redundant) Registers the EF Core database context.
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // Registers all validators found in the current assembly.
builder.Services.AddAutoMapper(typeof(AppMappingProfile)); // Registers AutoMapper and scans for profiles in the assembly containing AppMappingProfile.

// Build the application, which finalizes service registrations and configuration.
var app = builder.Build();

// Configure global error handling.
// This sets up an endpoint (/error) that returns a ProblemDetails response if an unhandled exception occurs.
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

// (Optional) Migrate & Seed the Database.
// This creates a service scope, retrieves the AppDbContext, applies migrations, and can seed data if necessary.
using (var scope = app.Services.CreateScope())
{
    // Uncomment and customize the following code if you want to apply migrations and seed the DB:
    // var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // context.Database.Migrate();
    // if (!context.Games.Any())
    // {
    //     // Use Bogus to generate and add fake data to the database.
    // }
}

// Configure Swagger UI for development environments.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Enables detailed error pages during development.
    app.UseSwagger();                // Generates the Swagger JSON endpoint.
    app.UseSwaggerUI();              // Serves the Swagger UI, allowing interactive API exploration.
}

// Enforce HTTPS redirection so that HTTP requests are automatically redirected to HTTPS.
app.UseHttpsRedirection();

// Add Authentication and Authorization middleware to the request pipeline.
// Note: app.UseAuthentication() must be called before app.UseAuthorization().
app.UseAuthentication();   // Authenticates requests (e.g., validates JWT tokens).
app.UseAuthorization();    // Authorizes user access based on roles and policies.

// Map controller routes so that endpoints defined in your controllers are available.
app.MapControllers();

// Run the application. This starts the web server and listens for incoming HTTP requests.
app.Run();
