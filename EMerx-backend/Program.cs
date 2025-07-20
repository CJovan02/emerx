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

using EMerx_backend.Infrastructure.MongoDb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(settings =>
    {
        settings.ConnectionString =
            Environment.GetEnvironmentVariable("MONGO_EMERX_CONNECTION_STRING") ??
            throw new Exception("MONGO_EMERX_CONNECTION_STRING not found");
        settings.DatabaseName = "EMerx";
        //settings.TestConnectionWithPing = builder.Environment.IsDevelopment();
    }
);

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();