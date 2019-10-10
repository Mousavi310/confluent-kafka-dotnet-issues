using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Issue1077
{
    public class KafkaHostedService : BackgroundService
    {
        private readonly ILogger<KafkaHostedService> logger;
        public KafkaHostedService(ILogger<KafkaHostedService> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string topic = "mytopic";
            string broker = "localhost:9092";
            int consuemrDelay = 50;
            int producerDelay = 10;

            var producerTask = Task.Run(async () => 
            {      
                while(true)      
                {
                    await Produce(broker, topic, consuemrDelay);
                }
                
            });

            var consumerTask = Task.Run(async () => 
            {        
                await Consume(broker, topic, producerDelay);
            });

        }

        private async Task Produce(string broker, string topic, int delay)
        {
            var config = new ProducerConfig { BootstrapServers =  broker};

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync(topic, new Message<Null, string> { Value="test" });
                    //Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                    await Task.Delay(delay);
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }

        private async Task Consume(string broker, string topic, int delay)
        {
            var conf = new ConsumerConfig
            { 
                //Just for test
                GroupId = Guid.NewGuid().ToString(),
                BootstrapServers = "localhost:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest,
                StatisticsIntervalMs = 10000
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf)
            .SetStatisticsHandler((d, statisticsJson) => 
            {
                var statistics = JsonConvert.DeserializeObject<ConsumerStatistics>(statisticsJson);

                foreach(var topic in statistics.Topics)
                {
                    foreach(var partition in topic.Value.Partitions)
                    {
                        logger.LogInformation($"Consumer lag for topic: [{topic.Key}], partition: [{partition.Key}], consumer group: [{conf.GroupId}] is {partition.Value.ConsumerLag}");
                    }
                }
            })
            .Build())
            {
                c.Subscribe(topic);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            //Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                            await Task.Delay(delay);
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }
    }
}