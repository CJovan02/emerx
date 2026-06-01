using EMerx.DTOs.Id;
using EMerx.Infrastructure.CloudinaryContext;
using EMerx.Infrastructure.MongoDb;
using EMerx.Repositories.AuthRepository;
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

// We use fake auth for api-testing
// dotnet run --launch-profile Api-Testing
if (builder.Environment.IsEnvironment("Api-Testing"))
{
    builder.Services.AddFakeAuthentication();
    builder.Services.AddScoped<IAuthRepository, TestAuthRepository>();
}
else
{
    builder.Services.AddFirebaseAuthentication(builder.Configuration);
    builder.Services.AddScoped<IAuthRepository, FirebaseAuthRepository>();
}

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

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Api-Testing"))
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

app.Run();