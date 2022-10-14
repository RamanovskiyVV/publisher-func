using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ServiceBusShared;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Publisher
{
    public class MultiSender : ISender
    {
        public async Task Send()
        {
            var serviceBusAdministrationClient = new ServiceBusAdministrationClient(Configuration.CONNECTION_STRING);
            string replyQueueName = Guid.NewGuid().ToString();
            try
            {
                //Temporary Queue for Receiver to send their replies into

                await serviceBusAdministrationClient.CreateQueueAsync(new CreateQueueOptions(replyQueueName)
                {
                    AutoDeleteOnIdle = TimeSpan.FromSeconds(300)
                });

                // Sending the message
                await using ServiceBusClient serviceBusClient = new ServiceBusClient(Configuration.CONNECTION_STRING);
                ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(Configuration.QUEUE_NAME);

                // Creating a receiver and waiting for the Receiver to reply
                ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(replyQueueName);

                var applicationMessage = new ApplicationMessage("Test");
                var serviceBusMessage = new ServiceBusMessage(JsonSerializer.SerializeToUtf8Bytes(applicationMessage))
                {
                    ContentType = "application/json",
                    ReplyTo = replyQueueName,
                };
                await serviceBusSender.SendMessageAsync(serviceBusMessage);
                Console.WriteLine($"Message Sent: {applicationMessage}.\n");


                ServiceBusReceivedMessage serviceBusReceivedMessage = await serviceBusReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(120));

                if (serviceBusReceivedMessage == null)
                {
                    Console.WriteLine("Error: Didn't receive a response.");
                    return;
                }

                applicationMessage = JsonSerializer.Deserialize<ApplicationMessage>(serviceBusReceivedMessage.Body.ToString());
                Console.WriteLine($"Reply Received: {applicationMessage.Output}.\n");
            }

            finally
            {
                await serviceBusAdministrationClient.DeleteQueueAsync(replyQueueName);
            }
        }
    }
}
