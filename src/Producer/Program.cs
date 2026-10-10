using System.Text.Json;
using Confluent.Kafka;

const string broker = "localhost:9092";
const string topic  = "orders";

var orderEvent = new
{
    OrderId      = Guid.NewGuid().ToString(),
    CustomerName = "Alice Smith",
    Amount       = 149.99m,
    CreatedAtUtc = DateTime.UtcNow,
};

var json = JsonSerializer.Serialize(orderEvent);

var config = new ProducerConfig { BootstrapServers = broker };

using var producer = new ProducerBuilder<Null, string>(config).Build();

var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = json });

Console.WriteLine($"Produced to {result.TopicPartitionOffset}: {json}");
