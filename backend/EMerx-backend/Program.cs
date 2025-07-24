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
using EMerx_backend.Repositories.OrderRepository;
using EMerx_backend.Repositories.ProductRepository;
using EMerx_backend.Repositories.ReviewRepository;
using EMerx_backend.Repositories.UserRepository;

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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services
    .AddOpenApi()
    .AddSwaggerGen();

var app = builder.Build();

// Ping test to see if everything is connected
var mongoContext = app.Services.GetRequiredService<MongoDbContext>();
await mongoContext.PingAsync();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();