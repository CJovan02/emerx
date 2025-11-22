using EMerx.Common;
using EMerx.Common.Exceptions;
using EMerx.Infrastructure.MongoDb;
using EMerx.Repositories.AuthRepository;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.ReviewRepository;
using EMerx.Repositories.UserRepository;
using EMerx.Services.Products;
using EMerx.Services.UserService;

namespace EMerx.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        return services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IReviewRepository, ReviewRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IAuthRepository, AuthRepository>();
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IProductService, ProductService>();
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.Configure<MongoDbSettings>(settings =>
        {
            settings.ConnectionString = Environment.GetEnvironmentVariable(Constants.EnvVariables.Database) ??
                                        throw new EnvVariableNotFoundException(Constants.EnvVariables.Database);
        });
        
        services.AddSingleton<MongoDbContext>();

        return services;
    }
}