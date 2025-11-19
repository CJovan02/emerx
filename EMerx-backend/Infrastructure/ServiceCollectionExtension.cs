using EMerx_backend.Repositories.AuthRepository;
using EMerx_backend.Repositories.OrderRepository;
using EMerx_backend.Repositories.ProductRepository;
using EMerx_backend.Repositories.ReviewRepository;
using EMerx_backend.Repositories.UserRepository;

namespace EMerx_backend.Infrastructure;

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