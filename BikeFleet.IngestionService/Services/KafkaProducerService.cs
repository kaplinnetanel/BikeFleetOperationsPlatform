using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeFleet.IngestionService.Services;

public class KafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    public KafkaProducerService(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    public async Task SendMessageAsync(string topicName, string? key, string massegeValue)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = massegeValue
        };
        var result = await _producer.ProduceAsync(topicName, message);
        Console.WriteLine($"Message sent to {result.TopicPartitionOffset}");
    }
}
