using EMerx.DTOs.Id;
using EMerx.Infrastructure;
using EMerx.Infrastructure.MongoDb;
using FirebaseAdmin;
using FluentValidation;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase();
builder.Services.AddSwaggerWithAuth();

builder.Services.AddValidatorsFromAssembly(typeof(IdRequest).Assembly, includeInternalTypes: true);
builder.Services.AddFluentValidationAutoValidation();


FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(builder.Configuration["Firebase:CredentialsPath"])
});
builder.Services.AddFirebaseAuthentication(builder.Configuration);

builder.Services
    .AddRepository()
    .AddServices();


builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Ping test to see if everything is connected
var mongoContext = app.Services.GetRequiredService<MongoDbContext>();
await mongoContext.PingAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();