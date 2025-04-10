using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductManagmentSystem.Infrastructure.Configuration;
using ProductManagmentSystem.Infrastructure.Persistence;


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

            return services;
        }
    }
}
