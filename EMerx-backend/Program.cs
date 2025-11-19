//Product - id, price, name, category, image
//Review - id, userid, productid, rating, ...
//Order - id, quantity, productid, userid, address, ... 

//result
//error<list>
//isokay
//iserror
//ierror
//error - features/users/errors/
//exception - constructor/create/mapping
//automapper?

using EMerx_backend.Infrastructure;
using EMerx_backend.Infrastructure.MongoDb;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(settings =>
    {
        settings.ConnectionString =
            Environment.GetEnvironmentVariable("MONGO_EMERX_CONNECTION_STRING") ??
            throw new Exception("MONGO_EMERX_CONNECTION_STRING not found");
        settings.DatabaseName = "EMerx";
    }
);

builder.Services.AddSingleton<MongoDbContext>();

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase.json")
});

builder.Services.AddRepository();
builder.Services.AddOpenApi();

var app = builder.Build();

// Ping test to see if everything is connected
var mongoContext = app.Services.GetRequiredService<MongoDbContext>();
await mongoContext.PingAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();