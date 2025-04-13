using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductManagmentSystem.Application.Interfaces;
using ProductManagmentSystem.Application.Mapping;
using ProductManagmentSystem.Application.Messaging;
using ProductManagmentSystem.Application.Services;
using ProductManagmentSystem.Domain.RepositoryInterfaces;
using ProductManagmentSystem.Infrastructure.Configuration;
using ProductManagmentSystem.Infrastructure.Messaging;
using ProductManagmentSystem.Infrastructure.Persistence;
using ProductManagmentSystem.Infrastructure.Repositories;


namespace ProductManagmentSystem.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProductInfrastructure(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            var connectionString = databaseOptions.DefaultConnection ?? throw new InvalidOperationException("Connection string not found in configuration.");

            services.AddDbContext<ProductDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IProductService, ProductService>();

            services.AddAutoMapper(typeof(ProductProfile).Assembly);
            //messaging
            services.AddSingleton<IStockUpdateProducer, KafkaStockUpdateProducer>();
            services.AddHostedService<StockUpdateConsumerService>();

            return services;
        }
    }
}
