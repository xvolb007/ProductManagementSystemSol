using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductManagmentSystem.Application.Interfaces;

namespace ProductManagmentSystem.Infrastructure.Messaging
{
    public class StockUpdateConsumerService: BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly string _topic;
        private readonly IServiceScopeFactory _scopeFactory;

        public StockUpdateConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            var kafkaSection = configuration.GetSection("Kafka");
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = kafkaSection["BootstrapServers"]?? throw new InvalidOperationException("Kafka BootstrapServers is not configured."),
                GroupId = kafkaSection["ConsumerGroup"]?? throw new InvalidOperationException("Kafka ConsumerGroup is not configured."),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _topic = kafkaSection["Topic"] ?? throw new InvalidOperationException("Kafka Topic is not configured.");
            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _consumer.Subscribe(_topic);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("StockUpdateConsumerService started, waiting for messages...");
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(TimeSpan.FromSeconds(3));
                    if (result == null)
                    {
                        await Task.Yield();
                        continue;
                    }

                    if (int.TryParse(result.Message.Key, out int productId))
                    {
                        if (int.TryParse(result.Message.Value, out int newStock))
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                            await productService.UpdateProductStockAsync(productId, newStock);
                            Console.WriteLine($"Product {productId} stock updated to {newStock} via consumer.");
                        }
                        else
                        {
                            Console.WriteLine($"Invalid newStock value: {result.Message.Value}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid productId value: {result.Message.Key}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in StockUpdateConsumerService: {ex.Message}");
            }
            finally
            {
                _consumer.Close();
            }
        }

    }
}
