using EMerx_backend.Features.Orders.Repositories;
using EMerx_backend.Features.Products.Repositories;
using EMerx_backend.Features.Reviews.Repositories;
using EMerx_backend.Features.Users.Repositories;
using EMerx_backend.Infrastructure.MongoDb;

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