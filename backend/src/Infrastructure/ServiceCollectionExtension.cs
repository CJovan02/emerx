using EMerx.Repositories.AuthRepository;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.ReviewRepository;
using EMerx.Repositories.UserRepository;

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
}