using EMerx.Common;
using EMerx.Common.Exceptions;
using EMerx.ExceptionHandlers;
using EMerx.Infrastructure.MongoDb;
using EMerx.Repositories.AuthRepository;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.ReviewRepository;
using EMerx.Repositories.UserRepository;
using EMerx.Services.Orders;
using EMerx.Services.Products;
using EMerx.Services.Reviews;
using EMerx.Services.Users;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
            .AddScoped<IProductService, ProductService>()
            .AddScoped<IReviewService, ReviewService>()
            .AddScoped<IOrderService, OrderService>();
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.Configure<MongoDbSettings>(settings =>
        {
            settings.ConnectionString = Environment.GetEnvironmentVariable(Constants.EnvVariables.Database) ??
                                        throw new EnvVariableNotFoundException(Constants.EnvVariables.Database);
        });

        services.AddSingleton<MongoContext>();

        return services;
    }

    public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
    {
        return services
            .AddSwaggerGen(option =>
            {
                option.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.HttpMethod}");
                option.SwaggerDoc("v1", new OpenApiInfo() { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        []
                    }
                });
            });
    }

    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        return services
            .AddExceptionHandler<GlobalExceptionHandler>();
    }

    public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var credentialsPath = configuration["Firebase:CredentialsPath"];

        if (string.IsNullOrEmpty(credentialsPath))
            throw new InvalidOperationException("Firebase:CredentialsPath not set.");

        if (!File.Exists(credentialsPath))
            throw new FileNotFoundException(
                $"Firebase credentials file not found at path: {credentialsPath}",
                credentialsPath);

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(credentialsPath)
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Firebase:ValidIssuer"];
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Firebase:ValidIssuer"],
                    ValidAudience = configuration["Firebase:Audience"]
                };

                // Firebase requires this
                options.IncludeErrorDetails = true;
            });

        return services;

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
    }
}