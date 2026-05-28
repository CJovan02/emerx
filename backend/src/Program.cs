using System.Security.Claims;
using EMerx.Auth;
using EMerx.DTOs.Id;
using EMerx.Infrastructure.CloudinaryContext;
using EMerx.Infrastructure.MongoDb;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load("./.env.local");

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandlers();

builder.Services.AddDatabase();
builder.Services.AddSwaggerWithAuth();

builder.Services.AddValidatorsFromAssembly(typeof(IdRequest).Assembly, includeInternalTypes: true);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddFirebaseAuthentication(builder.Configuration);

builder.Services.AddCloudinaryContext();
builder.Services
    .AddRepository()
    .AddServices();


builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Connect and ping the database
var mongoContext = app.Services.GetRequiredService<MongoContext>();
mongoContext.Connect();
await mongoContext.PingAsync();
await mongoContext.EnsureIndexesAsync();

// Ping Cloudinary
var cloudinary = app.Services.GetRequiredService<CloudinaryContext>();
cloudinary.Connect();
await cloudinary.PingAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

if (!app.Environment.IsEnvironment("Api-Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// dotnet run --launch-profile Api-Testing
// Used during api-testing. Since some endpoints require valid JWT in order to work and
// there is no way to acquire JWT using the Firebase SDK (auth service this server uses), for api testing purpuses
// we need to inject fake claims in order to test the API.
// This is not required during end-to-end tests, since we easily log on the frontend.
if (app.Environment.IsEnvironment("Api-Testing"))
{
    Console.WriteLine("Api-Testing environment is running.\n" +
                      "Injecting fake auth claims into context...");

    app.Use(async (context, next) =>
    {
        if (context.Request.Headers.ContainsKey("Test-Auth"))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, ((int)Roles.Admin).ToString()),
                new Claim(ClaimTypes.NameIdentifier, "test-user"),
                new Claim(ClaimTypes.Email, "testuser@test.com")
            };

            var identity = new ClaimsIdentity(claims, "Api-Test");
            context.User = new ClaimsPrincipal(identity);
        }

        await next();
    });
}

app.Run();