using EMerx.DTOs.Id;
using EMerx.Infrastructure;
using EMerx.Infrastructure.MongoDb;
using FirebaseAdmin;
using FluentValidation;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssembly(typeof(IdRequest).Assembly, includeInternalTypes: true);
builder.Services.AddFluentValidationAutoValidation();


FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(builder.Configuration["Firebase:CredentialsPath"])
});

builder.Services
    .AddRepository()
    .AddServices();


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
    {
        jwtOptions.Audience = builder.Configuration["Firebase:Audience"];
        jwtOptions.TokenValidationParameters.ValidIssuer =
            builder.Configuration["Firebase:ValidIssuer"];
    });

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

app.UseAuthorization();
app.UseAuthorization();

app.Run();

// services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         // Ne mora ništa od Issuer/Audience – FirebaseAdmin radi validaciju umesto toga
//         options.Events = new JwtBearerEvents
//         {
//             OnMessageReceived = context =>
//             {
//                 // Setujemo token ručno (standardni kod)
//                 var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
//                 if (authHeader?.StartsWith("Bearer ") == true)
//                     context.Token = authHeader.Substring("Bearer ".Length).Trim();
//
//                 return Task.CompletedTask;
//             },
//
//             OnTokenValidated = async context =>
//             {
//                 var token = context.SecurityToken.RawData;
//
//                 try
//                 {
//                     // 🔥 Ovo je Firebase Admin SDK verifikacija
//                     var firebaseToken = await FirebaseAuth.DefaultInstance
//                         .VerifyIdTokenAsync(token);
//
//                     // 🔥 Mapiramo claims u .NET principal
//                     var claims = firebaseToken.Claims
//                         .Select(kv => new Claim(kv.Key, kv.Value.ToString()))
//                         .ToList();
//
//                     claims.Add(new Claim(ClaimTypes.NameIdentifier, firebaseToken.Uid));
//
//                     var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
//
//                     context.Principal = new ClaimsPrincipal(identity);
//                 }
//                 catch (Exception ex)
//                 {
//                     context.Fail("Invalid Firebase token: " + ex.Message);
//                 }
//             }
//         };
//     });