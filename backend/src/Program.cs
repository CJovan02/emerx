using EMerx.DTOs.Users.Request;
using EMerx.Infrastructure;
using EMerx.Infrastructure.MongoDb;
using FirebaseAdmin;
using FluentValidation;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase();
builder.Services.AddSwaggerGen();

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase.json")
});

builder.Services
    .AddRepository()
    .AddServices();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Ping test to see if everything is connected
var mongoContext = app.Services.GetRequiredService<MongoDbContext>();
await mongoContext.PingAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
    app.MapControllers();
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();