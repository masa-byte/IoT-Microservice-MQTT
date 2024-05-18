using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace EventInfo.Services
{
    public class MQTTService : BackgroundService
    {
        private readonly IMqttClient mqttClient;
        private readonly MqttClientOptions mqttOptions;

        public MQTTService()
        {
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId("EventInfo")
                .WithTcpServer("mosquitto", 8883)
                .WithCleanSession()
                .Build();


            mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Connected successfully with MQTT broker.");

                // Subscribing to the topic
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic("analytics/pond-data/+")
                    .Build());

                Console.WriteLine("Subscribed to topic");
            };

            mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("Disconnected from MQTT broker.");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Reconnect failed: {ex.Message}");
                }
            };

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                Console.WriteLine($"Received message: {message}");

                EventInfoClass eventInfo = new EventInfoClass();

                var messageDict = new Dictionary<string, object>();

                using (JsonDocument doc = JsonDocument.Parse(message))
                {
                    foreach (JsonProperty element in doc.RootElement.EnumerateObject())
                    {
                        messageDict[element.Name] = element.Value.ToString();
                    }
                }
                
                eventInfo.Id = QueueClass.Instance.Count() + 1;

                try
                {
                    eventInfo.EventType = messageDict["EventType"].ToString()!;
                    eventInfo.LocationId = messageDict["PondId"].ToString()!;

                    if (messageDict.TryGetValue("Timestamp", out var timestampStr))
                    {
                        if (DateTime.TryParseExact(timestampStr.ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp))
                        {
                            messageDict["Timestamp"] = timestamp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing message: {ex.Message}");
                    return;
                }

                foreach (var key in messageDict.Keys)
                {
                    if (key != "EventType" && key != "PondId" && key != "Timestamp")
                    {
                        eventInfo.Values.Add(new KeyValuePair<string, string>(key, messageDict[key].ToString()!));
                    }
                }

                QueueClass.Instance.Enqueue(eventInfo);

                await Task.CompletedTask;
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!mqttClient.IsConnected)
                {
                    try
                    {
                        await mqttClient.ConnectAsync(mqttOptions, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"MQTT connection failed: {ex.Message}");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Retrying every 5 seconds if not connected
            }
        }
    }
}
