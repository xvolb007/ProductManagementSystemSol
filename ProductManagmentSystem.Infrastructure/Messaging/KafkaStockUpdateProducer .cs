using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using ProductManagmentSystem.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductManagmentSystem.Infrastructure.Messaging
{
    public class KafkaStockUpdateProducer: IStockUpdateProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaStockUpdateProducer(IConfiguration configuration)
        {
            var kafkaSection = configuration.GetSection("Kafka");
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSection["BootstrapServers"] ?? throw new InvalidOperationException("Kafka BootstrapServers is not configured."),
                Acks = Acks.All
            };
            _topic = kafkaSection["Topic"] ?? throw new InvalidOperationException("Kafka Topic is not configured.");
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task PublishStockUpdateAsync(int id, int stockQuantity)
        {
            try
            {
                var msg = new Message<string, string>() 
                {
                    Key = id.ToString(),
                    Value = stockQuantity.ToString()
                };
                var deliveryResult = await _producer.ProduceAsync(_topic, msg);
                Console.WriteLine($"Succesfully send to {_topic} partion: {deliveryResult.Partition} with offset {deliveryResult.Offset}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to the Kafka: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
    }
}
