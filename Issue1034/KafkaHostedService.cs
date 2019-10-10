using System;
using System.Threading;
using System.Threading.Tasks;
using com.company.sub.@event;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Issue1034
{
    public class KafkaHostedService : BackgroundService
    {
        private readonly ILogger<KafkaHostedService> logger;
        public KafkaHostedService(ILogger<KafkaHostedService> logger)
        {
            this.logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => 
            {
                var bootstrapServers = "localhost:9092";
                var schemaRegistryUrl = "http://localhost:8081/";
                var topicName = "topic2";

                NewConstructionAddressEvent addr = new NewConstructionAddressEvent
                {
                    eventId = "EventId",
                    eventType = "EventType",
                    constructionAddressId = "ConstructionAddressId",
                    constructionIndicator = "constructionIndicator"
                };
                Produce(bootstrapServers, schemaRegistryUrl, topicName, addr);
                Consume(bootstrapServers, schemaRegistryUrl, topicName);
            });
        }

        public static void Produce(string broker, 
            string schemaRegistryUrl,
            string topic,
            NewConstructionAddressEvent item)
        {
            using(var schemaRegistry = new CachedSchemaRegistryClient(
                new SchemaRegistryConfig {SchemaRegistryUrl = schemaRegistryUrl}
            ))
            {
                var config = new ProducerConfig{
                        BootstrapServers = broker,
                    };
                using(var producer = new ProducerBuilder<string, NewConstructionAddressEvent>(config)
                .SetValueSerializer(new SyncOverAsyncSerializer<NewConstructionAddressEvent>(new AvroSerializer<NewConstructionAddressEvent>(schemaRegistry)))
                .SetKeySerializer(new SyncOverAsyncSerializer<string>(new AvroSerializer<string>(schemaRegistry)))
                 .Build())
                {
                    producer
                            .Produce(topic, new Message<string, NewConstructionAddressEvent>{Value = item, Key = Guid.NewGuid().ToString()});                        

                    producer.Flush();
                }
            }

        }

        public void Consume(string bootstrapServers, string schemaRegistryUrl, string topicName)
        {
            var schemaRegistryRequestTimeoutMs = 30000;
            var schemaRegistryMaxCachedSchemas = 1000;
            var groupID = Guid.NewGuid().ToString();
            var bufferBytes = 1024;
            var autoRegisterSchema = true;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                SchemaRegistryUrl = schemaRegistryUrl,
                // optional schema registry client properties:
                SchemaRegistryRequestTimeoutMs = schemaRegistryRequestTimeoutMs,
                SchemaRegistryMaxCachedSchemas = schemaRegistryMaxCachedSchemas
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Latest,
                GroupId = groupID // "Test" //Guid.NewGuid().ToString()
            };

            var avroSerializerConfig = new AvroSerializerConfig
            {
                // optional Avro serializer properties:
                BufferBytes = bufferBytes,
                AutoRegisterSchemas = autoRegisterSchema
            };

            NewConstructionAddressEvent addr = new NewConstructionAddressEvent();

            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            using (var consumer =
                new ConsumerBuilder<string, NewConstructionAddressEvent>(consumerConfig)
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<NewConstructionAddressEvent>(schemaRegistry).AsSyncOverAsync())
                    .SetErrorHandler((_, e) => logger.LogError($"Error: {e.Reason}"))
                    .Build())
            {
                try
                {
                    logger.LogInformation($"Starting consumer.subscribe.");

                    consumer.Subscribe(topicName);

                    while (true)
                    {
                        try
                        {
                            logger.LogInformation($"Starting: consumer.Consume");
                            var consumeResult = consumer.Consume();

                            string k = consumeResult.Key;
                            logger.LogInformation($"BusMessage: {consumeResult.Message}, constructionAddressId: {consumeResult.Value.constructionAddressId}");
                        }
                        catch (OperationCanceledException)
                        {
                            logger.LogInformation($"OperationCancelled for consumer.Consume");
                            break;
                        }
                        catch (ConsumeException e)
                        {
                            logger.LogInformation(e, $"Consume error: {e.Error.Reason}");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Consume error: {ex.Message}");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}